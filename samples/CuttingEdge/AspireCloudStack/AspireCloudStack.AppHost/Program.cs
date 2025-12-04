var builder = DistributedApplication.CreateBuilder(args);

// ===================================================================
// .NET ASPIRE APP HOST
// ===================================================================
// This is the orchestration layer for the entire cloud-native application
// It manages:
// - Service discovery
// - Container orchestration (PostgreSQL, Redis)
// - Configuration management
// - OpenTelemetry collection
// - Dashboard for observability
// ===================================================================

Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║          .NET ASPIRE CLOUD-NATIVE STACK                        ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine("🚀 Starting AspireCloudStack orchestration...");
Console.WriteLine();

// ===================================================================
// INFRASTRUCTURE RESOURCES
// ===================================================================

// PostgreSQL Database
// Automatically starts a PostgreSQL container
// Connection string is injected into dependent services
var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres")
    .WithImageTag("16-alpine")
    .WithEnvironment("POSTGRES_USER", "aspire")
    .WithEnvironment("POSTGRES_PASSWORD", "aspire123")
    .WithDataVolume("postgres-data") // Persistent storage
    .WithPgAdmin(); // Adds PgAdmin web UI

var postgresdb = postgres.AddDatabase("postgresdb");

Console.WriteLine("✅ PostgreSQL configured (port: auto-assigned)");
Console.WriteLine("   - Database: postgresdb");
Console.WriteLine("   - User: aspire");
Console.WriteLine("   - PgAdmin: http://localhost:5050");
Console.WriteLine();

// Redis Cache
// Automatically starts a Redis container
// Connection string is injected into dependent services
var redis = builder.AddRedis("redis")
    .WithImage("redis")
    .WithImageTag("7-alpine")
    .WithDataVolume("redis-data") // Persistent storage
    .WithRedisCommander(); // Adds Redis Commander web UI

Console.WriteLine("✅ Redis configured (port: auto-assigned)");
Console.WriteLine("   - Redis Commander: http://localhost:8081");
Console.WriteLine();

// ===================================================================
// APPLICATION SERVICES
// ===================================================================

// API Service
// References the ApiService project
// Automatically gets:
// - PostgreSQL connection string (via postgresdb reference)
// - Redis connection string (via redis reference)
// - OpenTelemetry configuration
// - Service discovery
var apiService = builder.AddProject<Projects.AspireCloudStack_ApiService>("apiservice")
    .WithReference(postgresdb)  // Injects PostgreSQL connection
    .WithReference(redis)        // Injects Redis connection
    .WithReplicas(2);            // Run 2 instances for load balancing

Console.WriteLine("✅ API Service configured");
Console.WriteLine("   - Instances: 2 (load balanced)");
Console.WriteLine("   - Database: postgresdb");
Console.WriteLine("   - Cache: redis");
Console.WriteLine("   - Swagger: Will be available after startup");
Console.WriteLine();

// ===================================================================
// OBSERVABILITY
// ===================================================================
// .NET Aspire automatically configures:
// - OpenTelemetry traces (distributed tracing)
// - OpenTelemetry metrics (performance metrics)
// - OpenTelemetry logs (structured logging)
// - Dashboard at http://localhost:18888

Console.WriteLine("✅ Observability configured");
Console.WriteLine("   - OpenTelemetry: Enabled");
Console.WriteLine("   - Dashboard: http://localhost:18888");
Console.WriteLine("   - Traces: Enabled");
Console.WriteLine("   - Metrics: Enabled");
Console.WriteLine("   - Logs: Enabled");
Console.WriteLine();

// ===================================================================
// BUILD AND RUN
// ===================================================================

Console.WriteLine("📊 Resource Summary:");
Console.WriteLine("   - PostgreSQL (container)");
Console.WriteLine("   - Redis (container)");
Console.WriteLine("   - PgAdmin (web UI)");
Console.WriteLine("   - Redis Commander (web UI)");
Console.WriteLine("   - API Service (2 replicas)");
Console.WriteLine("   - Aspire Dashboard (observability)");
Console.WriteLine();
Console.WriteLine("⏳ Starting all resources...");
Console.WriteLine();

var app = builder.Build();

await app.RunAsync();

// ===================================================================
// ACCESS POINTS (after startup)
// ===================================================================
// Dashboard:        http://localhost:18888
// API Swagger:      http://localhost:<auto-port>/swagger
// PgAdmin:          http://localhost:5050 (admin@admin.com / admin)
// Redis Commander:  http://localhost:8081
// ===================================================================
