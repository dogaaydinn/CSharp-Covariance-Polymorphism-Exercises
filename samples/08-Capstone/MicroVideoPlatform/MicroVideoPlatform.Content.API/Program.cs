using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using MicroVideoPlatform.Content.API.Data;
using MicroVideoPlatform.Content.API.Services;
using MicroVideoPlatform.Content.API.Middleware;
using MicroVideoPlatform.Content.API.Configuration;
using MicroVideoPlatform.Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Serilog with Sensitive Data Masking
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .MaskSensitiveData() // Mask passwords, tokens, credit cards, etc.
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration["Seq:Url"] ?? "http://localhost:5341")
    .CreateLogger();

builder.Host.UseSerilog();

// Database
builder.Services.AddDbContext<VideoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

// Redis Caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "MicroVideo:";
});

// JWT Authentication
var jwtSecret = builder.Configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT:Secret not configured");
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? "MicroVideoPlatform";
var jwtAudience = builder.Configuration["JWT:Audience"] ?? "MicroVideoPlatform";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// Application Services
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Content API", Version = "v1", Description = "Video metadata management API" });
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("PostgreSQL")!, name: "postgres")
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!, name: "redis")
    .AddRabbitMQ(rabbitConnectionString: $"amqp://{builder.Configuration["RabbitMQ:Username"]}:{builder.Configuration["RabbitMQ:Password"]}@{builder.Configuration["RabbitMQ:Host"]}:{builder.Configuration["RabbitMQ:Port"]}", name: "rabbitmq");

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Database Migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<VideoDbContext>();
    try
    {
        dbContext.Database.Migrate();
        Log.Information("Database migration completed successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Database migration failed");
    }
}

// Middleware Pipeline (order matters!)
// 1. Security Headers - Applied to every response
app.UseSecurityHeaders();

// 2. HTTPS Redirection - Force HTTPS in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts(); // HTTP Strict Transport Security
}

// 3. Request Rate Limiting - DDoS protection
app.UseRequestRateLimiting();

// 4. Serilog Request Logging - Track all requests
app.UseSerilogRequestLogging();

// 5. CORS - Control cross-origin requests
app.UseCors();

// 6. Authentication - Verify JWT tokens
app.UseAuthentication();

// 7. Authorization - Check permissions
app.UseAuthorization();

// 8. Swagger (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 9. Map endpoints
app.MapControllers();
app.MapHealthChecks("/health");

Log.Information("Content.API starting on {Urls}", string.Join(", ", builder.Configuration["ASPNETCORE_URLS"]?.Split(';') ?? ["http://+:8080"]));

try
{
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
