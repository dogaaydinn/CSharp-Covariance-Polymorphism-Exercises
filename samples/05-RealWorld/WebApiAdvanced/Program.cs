using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;
using WebApiAdvanced.Services;

namespace WebApiAdvanced;

/// <summary>
/// Production-ready Web API with:
/// - JWT Authentication
/// - Rate Limiting
/// - Response Caching
/// - Structured Logging (Serilog)
/// - Swagger/OpenAPI
/// - Health Checks
/// - Global Error Handling
/// - CORS
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        try
        {
            Log.Information("Starting WebApiAdvanced");

            var builder = WebApplication.CreateBuilder(args);

            // Replace default logging with Serilog
            builder.Host.UseSerilog();

            // ====================
            // CONFIGURE SERVICES
            // ====================

            // 1. Controllers
            builder.Services.AddControllers();

            // 2. Memory Cache (for response caching)
            builder.Services.AddMemoryCache();

            // 3. Business Logic Services
            builder.Services.AddScoped<IProductService, ProductService>();

            // 4. JWT Authentication
            var jwtKey = builder.Configuration["Jwt:Key"]!;
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
                    ClockSkew = TimeSpan.Zero // Remove 5 min default tolerance
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

            // 5. Rate Limiting (ASP.NET Core 8.0 built-in)
            builder.Services.AddRateLimiter(options =>
            {
                // Fixed window rate limiter
                options.AddFixedWindowLimiter("fixed", opt =>
                {
                    opt.PermitLimit = builder.Configuration.GetValue<int>("RateLimiting:PermitLimit");
                    opt.Window = builder.Configuration.GetValue<TimeSpan>("RateLimiting:Window");
                    opt.QueueLimit = builder.Configuration.GetValue<int>("RateLimiting:QueueLimit");
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                // Rejection response
                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsync(
                        "Too many requests. Please try again later.",
                        cancellationToken
                    );

                    Log.Warning("Rate limit exceeded for {Path}", context.HttpContext.Request.Path);
                };
            });

            // 6. Response Caching
            builder.Services.AddResponseCaching();

            // 7. CORS (for frontend applications)
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // 8. Health Checks
            builder.Services.AddHealthChecks();

            // 9. Swagger/OpenAPI Documentation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebApiAdvanced",
                    Version = "v1",
                    Description = "Production-ready Web API with JWT, Rate Limiting, Caching, and more",
                    Contact = new OpenApiContact
                    {
                        Name = "API Support",
                        Email = "support@example.com"
                    }
                });

                // JWT Bearer authentication in Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // ====================
            // BUILD APP
            // ====================
            var app = builder.Build();

            // ====================
            // CONFIGURE MIDDLEWARE PIPELINE
            // ====================

            // 1. Exception Handling (should be first)
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            // 2. Swagger (development only)
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAdvanced v1");
                    options.RoutePrefix = string.Empty; // Serve Swagger at root
                });
            }

            // 3. HTTPS Redirection
            app.UseHttpsRedirection();

            // 4. Routing
            app.UseRouting();

            // 5. CORS (must be after UseRouting, before UseAuthentication)
            app.UseCors();

            // 6. Rate Limiting (before authentication)
            app.UseRateLimiter();

            // 7. Authentication (must be before Authorization)
            app.UseAuthentication();

            // 8. Authorization
            app.UseAuthorization();

            // 9. Response Caching
            app.UseResponseCaching();

            // 10. Request Logging
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

            // 11. Map Controllers
            app.MapControllers();

            // 12. Health Checks
            app.MapHealthChecks("/health");

            // 13. Root endpoint
            app.MapGet("/", () => new
            {
                Message = "WebApiAdvanced - Production-ready API",
                Version = "1.0.0",
                Features = new[]
                {
                    "JWT Authentication",
                    "Rate Limiting (10 requests per 10 seconds)",
                    "Response Caching",
                    "Swagger Documentation",
                    "Health Checks",
                    "Structured Logging"
                },
                Endpoints = new
                {
                    Swagger = "/",
                    Health = "/health",
                    Login = "/api/auth/login",
                    Products = "/api/products"
                },
                TestCredentials = new
                {
                    Admin = new { Username = "admin", Password = "admin123" },
                    User = new { Username = "user", Password = "user123" }
                }
            })
            .WithName("Root");

            // ====================
            // RUN APPLICATION
            // ====================
            Log.Information("Application started successfully");
            Log.Information("Navigate to https://localhost:{Port} for Swagger UI",
                app.Environment.IsDevelopment() ? "5001" : "443");

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
