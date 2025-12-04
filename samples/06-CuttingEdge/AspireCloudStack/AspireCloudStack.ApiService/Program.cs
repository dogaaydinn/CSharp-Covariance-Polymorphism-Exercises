using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;
using AspireCloudStack.ApiService.Data;
using AspireCloudStack.ApiService.Services;

var builder = WebApplication.CreateBuilder(args);

// ===================================================================
// .NET ASPIRE SERVICE DEFAULTS
// ===================================================================
// Adds OpenTelemetry, Health Checks, Service Discovery, Resilience
builder.AddServiceDefaults();

// ===================================================================
// SERILOG STRUCTURED LOGGING
// ===================================================================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// ===================================================================
// DATABASE - POSTGRESQL (via Aspire)
// ===================================================================
builder.AddNpgsqlDbContext<ApplicationDbContext>("postgresdb");

// ===================================================================
// REDIS DISTRIBUTED CACHE (via Aspire)
// ===================================================================
builder.AddRedis("redis");

// ===================================================================
// CONTROLLERS & API EXPLORER
// ===================================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ===================================================================
// BUSINESS LOGIC SERVICES
// ===================================================================
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

// ===================================================================
// JWT AUTHENTICATION
// ===================================================================
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Warning("JWT Authentication failed: {Error}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var username = context.Principal?.Identity?.Name;
            Log.Information("JWT validated for user: {Username}", username);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ===================================================================
// RATE LIMITING
// ===================================================================
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 10;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync(
            "Rate limit exceeded. Please try again later.",
            cancellationToken
        );

        Log.Warning("Rate limit exceeded for {Path} from {IP}",
            context.HttpContext.Request.Path,
            context.HttpContext.Connection.RemoteIpAddress);
    };
});

// ===================================================================
// RESPONSE CACHING
// ===================================================================
builder.Services.AddResponseCaching();

// ===================================================================
// CORS
// ===================================================================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7001", "http://localhost:5001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ===================================================================
// SWAGGER/OPENAPI
// ===================================================================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "AspireCloudStack API",
        Version = "v1",
        Description = ".NET Aspire Cloud-Native API with JWT, Rate Limiting, PostgreSQL, Redis, and OpenTelemetry"
    });

    options.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ===================================================================
// BUILD APPLICATION
// ===================================================================
var app = builder.Build();

// ===================================================================
// MIDDLEWARE PIPELINE
// ===================================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AspireCloudStack API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();

app.UseCors();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCaching();

// Request logging middleware
app.Use(async (context, next) =>
{
    Log.Information("HTTP {Method} {Path} from {IP}",
        context.Request.Method,
        context.Request.Path,
        context.Connection.RemoteIpAddress);

    await next.Invoke();

    Log.Information("HTTP {Method} {Path} responded {StatusCode}",
        context.Request.Method,
        context.Request.Path,
        context.Response.StatusCode);
});

app.MapControllers();

// Map default Aspire health checks
app.MapDefaultEndpoints();

// ===================================================================
// DATABASE MIGRATION
// ===================================================================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
    Log.Information("Database migrations applied successfully");
}

// ===================================================================
// RUN APPLICATION
// ===================================================================
Log.Information("AspireCloudStack API started successfully");
Log.Information("Swagger UI available at: {Url}", app.Environment.IsDevelopment() ? "http://localhost:5000" : "https://localhost:7000");

try
{
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
