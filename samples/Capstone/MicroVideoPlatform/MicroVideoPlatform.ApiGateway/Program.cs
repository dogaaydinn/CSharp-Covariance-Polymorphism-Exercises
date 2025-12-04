using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using MicroVideoPlatform.ApiGateway.Services;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Serilog.Events;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Yarp", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "ApiGateway")
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "/logs/apigateway-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting ApiGateway");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog
    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new()
        {
            Title = "MicroVideo Platform API Gateway",
            Version = "v1",
            Description = "Unified API Gateway for MicroVideo Platform microservices"
        });
    });

    // Configure YARP Reverse Proxy
    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    // Configure HTTP Clients with Polly Resilience Policies
    var retryPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: builder.Configuration.GetValue<int>("Retry:MaxRetryAttempts", 3),
            sleepDurationProvider: retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(
                    builder.Configuration.GetValue<double>("Retry:BackoffMultiplier", 2),
                    retryAttempt)),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                Log.Warning("Retry {RetryAttempt} after {Delay}ms for {PolicyKey}",
                    retryAttempt, timespan.TotalMilliseconds, context.PolicyKey);
            });

    var circuitBreakerPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: builder.Configuration.GetValue<int>("CircuitBreaker:FailureThreshold", 5),
            durationOfBreak: TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("CircuitBreaker:DurationOfBreakSeconds", 30)),
            onBreak: (outcome, breakDelay) =>
            {
                Log.Error("Circuit breaker opened. Break for {BreakDelay}s", breakDelay.TotalSeconds);
            },
            onReset: () =>
            {
                Log.Information("Circuit breaker reset");
            });

    var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));

    var combinedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);

    // Register HTTP Clients
    builder.Services.AddHttpClient("ContentApi", client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ContentApi"]!);
        client.Timeout = TimeSpan.FromSeconds(30);
    }).AddPolicyHandler(combinedPolicy);

    builder.Services.AddHttpClient("AnalyticsFunction", client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:AnalyticsFunction"]!);
        client.Timeout = TimeSpan.FromSeconds(30);
    }).AddPolicyHandler(combinedPolicy);

    builder.Services.AddHttpClient("ProcessingWorker", client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProcessingWorker"]!);
        client.Timeout = TimeSpan.FromSeconds(30);
    }).AddPolicyHandler(combinedPolicy);

    // Register Services
    builder.Services.AddScoped<VideoAggregationService>();

    // Configure JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings["Secret"]!))
            };
        });

    // Configure Authorization Policies
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("admin-only", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("premium-access", policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("Premium") || context.User.IsInRole("Admin")));
    });

    // Configure Rate Limiting
    builder.Services.AddRateLimiter(options =>
    {
        options.AddFixedWindowLimiter("standard", limiterOptions =>
        {
            limiterOptions.PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:Standard:PermitLimit", 100);
            limiterOptions.Window = TimeSpan.Parse(builder.Configuration["RateLimiting:Standard:Window"]!);
            limiterOptions.QueueLimit = builder.Configuration.GetValue<int>("RateLimiting:Standard:QueueLimit", 10);
        });

        options.AddFixedWindowLimiter("strict", limiterOptions =>
        {
            limiterOptions.PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:Strict:PermitLimit", 20);
            limiterOptions.Window = TimeSpan.Parse(builder.Configuration["RateLimiting:Strict:Window"]!);
            limiterOptions.QueueLimit = builder.Configuration.GetValue<int>("RateLimiting:Strict:QueueLimit", 5);
        });

        options.OnRejected = (context, cancellationToken) =>
        {
            Log.Warning("Rate limit exceeded for {Path}", context.HttpContext.Request.Path);
            context.HttpContext.Response.StatusCode = 429;
            return ValueTask.CompletedTask;
        };
    });

    // Configure Response Caching
    builder.Services.AddMemoryCache(options =>
    {
        options.SizeLimit = 1024; // 1024 entries
    });

    // Configure Response Compression
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
    });

    // Configure Redis for distributed caching (if available)
    if (!string.IsNullOrEmpty(builder.Configuration["Redis:ConnectionString"]))
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration["Redis:ConnectionString"];
            options.InstanceName = builder.Configuration["Redis:InstanceName"];
        });
    }

    // Configure OpenTelemetry
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource
            .AddService(
                serviceName: builder.Configuration["OpenTelemetry:ServiceName"]!,
                serviceVersion: builder.Configuration["OpenTelemetry:ServiceVersion"]!))
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddMeter("Yarp.ReverseProxy")
            .AddPrometheusExporter()
            .AddConsoleExporter())
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.EnrichWithHttpRequest = (activity, request) =>
                {
                    activity.SetTag("request.protocol", request.Protocol);
                    activity.SetTag("user.id", request.Headers["X-User-Id"].ToString());
                };
            })
            .AddHttpClientInstrumentation()
            .AddSource("ApiGateway")
            .AddConsoleExporter()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(builder.Configuration["OpenTelemetry:OtlpExporterEndpoint"]!);
            }));

    // Configure Health Checks
    builder.Services.AddHealthChecks()
        .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy())
        .AddUrlGroup(new Uri(builder.Configuration["ServiceUrls:ContentApi"]! + "/health"),
            name: "content-api",
            timeout: TimeSpan.FromSeconds(5))
        .AddUrlGroup(new Uri(builder.Configuration["ServiceUrls:ProcessingWorker"]! + "/health"),
            name: "processing-worker",
            timeout: TimeSpan.FromSeconds(5))
        .AddUrlGroup(new Uri(builder.Configuration["ServiceUrls:AnalyticsFunction"]! + "/api/health"),
            name: "analytics-function",
            timeout: TimeSpan.FromSeconds(5));

    // Add Redis health check if configured
    if (!string.IsNullOrEmpty(builder.Configuration["Redis:ConnectionString"]))
    {
        builder.Services.AddHealthChecks()
            .AddRedis(builder.Configuration["Redis:ConnectionString"]!, name: "redis");
    }

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "MicroVideo Platform API Gateway v1");
            options.RoutePrefix = "swagger";
        });
    }

    // Middleware pipeline
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            diagnosticContext.Set("UserId", httpContext.Request.Headers["X-User-Id"].ToString());
        };
    });

    app.UseResponseCompression();
    app.UseRateLimiter();

    app.UseAuthentication();
    app.UseAuthorization();

    // Map health checks
    app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    exception = e.Value.Exception?.Message,
                    duration = e.Value.Duration.TotalMilliseconds
                }),
                totalDuration = report.TotalDuration.TotalMilliseconds
            });
            await context.Response.WriteAsync(result);
        }
    });

    app.MapHealthChecks("/health/ready");
    app.MapHealthChecks("/health/live");

    // Map controllers
    app.MapControllers();

    // Map YARP Reverse Proxy
    app.MapReverseProxy();

    // Prometheus metrics endpoint
    app.MapPrometheusScrapingEndpoint();

    Log.Information("ApiGateway started successfully on {Urls}", string.Join(", ", app.Urls));

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ApiGateway terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;
