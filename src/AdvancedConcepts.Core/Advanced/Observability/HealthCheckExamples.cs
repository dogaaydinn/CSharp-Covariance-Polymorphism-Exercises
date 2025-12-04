using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AdvancedCsharpConcepts.Advanced.Observability;

/// <summary>
/// Health Check Examples - Production-grade application health monitoring.
/// NVIDIA/Silicon Valley best practice: Comprehensive health checks for microservices.
/// </summary>
public static class HealthCheckExamples
{
    /// <summary>
    /// Database health check - Verifies database connectivity.
    /// </summary>
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;

        public DatabaseHealthCheck(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Simulate database connection check
                await Task.Delay(Random.Shared.Next(10, 50), cancellationToken);

                // Simulate occasional connection issues
                if (Random.Shared.Next(0, 20) == 0)
                {
                    return HealthCheckResult.Unhealthy(
                        "Database connection failed",
                        exception: new InvalidOperationException("Connection timeout"));
                }

                var data = new Dictionary<string, object>
                {
                    { "connection_string", _connectionString },
                    { "response_time_ms", Random.Shared.Next(10, 50) },
                    { "active_connections", Random.Shared.Next(1, 10) }
                };

                return HealthCheckResult.Healthy("Database is responsive", data);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database check failed", ex);
            }
        }
    }

    /// <summary>
    /// Redis cache health check - Verifies cache availability.
    /// </summary>
    public class CacheHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.Delay(Random.Shared.Next(5, 30), cancellationToken);

                var responseTime = Random.Shared.Next(5, 30);

                // Degraded if slow
                if (responseTime > 20)
                {
                    return HealthCheckResult.Degraded(
                        "Cache is slow",
                        data: new Dictionary<string, object>
                        {
                            { "response_time_ms", responseTime },
                            { "threshold_ms", 20 }
                        });
                }

                return HealthCheckResult.Healthy(
                    "Cache is responsive",
                    new Dictionary<string, object>
                    {
                        { "response_time_ms", responseTime },
                        { "hit_rate", 0.87 }
                    });
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Cache check failed", ex);
            }
        }
    }

    /// <summary>
    /// External API health check - Verifies external service availability.
    /// </summary>
    public class ExternalApiHealthCheck : IHealthCheck
    {
        private readonly string _apiUrl;

        public ExternalApiHealthCheck(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Simulate API call
                await Task.Delay(Random.Shared.Next(50, 200), cancellationToken);

                // Simulate occasional API failures
                if (Random.Shared.Next(0, 15) == 0)
                {
                    return HealthCheckResult.Unhealthy(
                        $"External API {_apiUrl} is unreachable",
                        data: new Dictionary<string, object>
                        {
                            { "api_url", _apiUrl },
                            { "status_code", 503 }
                        });
                }

                return HealthCheckResult.Healthy(
                    "External API is available",
                    new Dictionary<string, object>
                    {
                        { "api_url", _apiUrl },
                        { "status_code", 200 },
                        { "response_time_ms", Random.Shared.Next(50, 200) }
                    });
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Failed to reach {_apiUrl}", ex);
            }
        }
    }

    /// <summary>
    /// Memory health check - Monitors application memory usage.
    /// </summary>
    public class MemoryHealthCheck : IHealthCheck
    {
        private readonly long _threshold;

        public MemoryHealthCheck(long threshold = 1024L * 1024 * 1024) // 1GB default
        {
            _threshold = threshold;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var allocated = GC.GetTotalMemory(forceFullCollection: false);
            var data = new Dictionary<string, object>
            {
                { "allocated_bytes", allocated },
                { "threshold_bytes", _threshold },
                { "gen0_collections", GC.CollectionCount(0) },
                { "gen1_collections", GC.CollectionCount(1) },
                { "gen2_collections", GC.CollectionCount(2) }
            };

            if (allocated >= _threshold)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "Memory usage exceeds threshold",
                    data: data));
            }

            if (allocated >= _threshold * 0.8)
            {
                return Task.FromResult(HealthCheckResult.Degraded(
                    "Memory usage is high",
                    data: data));
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                "Memory usage is normal",
                data));
        }
    }

    /// <summary>
    /// Disk space health check - Monitors available disk space.
    /// </summary>
    public class DiskSpaceHealthCheck : IHealthCheck
    {
        private readonly string _driveName;
        private readonly long _minimumFreeMegabytes;

        public DiskSpaceHealthCheck(string driveName = "/", long minimumFreeMegabytes = 1024)
        {
            _driveName = driveName;
            _minimumFreeMegabytes = minimumFreeMegabytes;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Simulate disk space check
                var availableSpaceMb = Random.Shared.Next(500, 5000);
                var totalSpaceMb = 10000L;

                var data = new Dictionary<string, object>
                {
                    { "drive", _driveName },
                    { "available_mb", availableSpaceMb },
                    { "total_mb", totalSpaceMb },
                    { "usage_percent", (1.0 - (double)availableSpaceMb / totalSpaceMb) * 100 }
                };

                if (availableSpaceMb < _minimumFreeMegabytes)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy(
                        $"Disk space critically low on {_driveName}",
                        data: data));
                }

                if (availableSpaceMb < _minimumFreeMegabytes * 2)
                {
                    return Task.FromResult(HealthCheckResult.Degraded(
                        $"Disk space running low on {_driveName}",
                        data: data));
                }

                return Task.FromResult(HealthCheckResult.Healthy(
                    "Sufficient disk space available",
                    data));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "Failed to check disk space", ex));
            }
        }
    }

    /// <summary>
    /// Startup health check - Ensures application is fully initialized.
    /// </summary>
    public class StartupHealthCheck : IHealthCheck
    {
        private volatile bool _startupCompleted;

        public void MarkAsCompleted() => _startupCompleted = true;

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            if (_startupCompleted)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("Application startup completed"));
            }

            return Task.FromResult(
                HealthCheckResult.Unhealthy("Application is still starting up"));
        }
    }

    /// <summary>
    /// Configures and runs health checks.
    /// </summary>
    public static async Task RunExamples()
    {
        Console.WriteLine("=== Health Check Examples ===\n");

        var serviceCollection = new ServiceCollection();

        // Register health checks
        serviceCollection.AddHealthChecks()
            .AddCheck("database", new DatabaseHealthCheck("Server=localhost;Database=myapp"))
            .AddCheck("cache", new CacheHealthCheck())
            .AddCheck("external_api", new ExternalApiHealthCheck("https://api.example.com"))
            .AddCheck("memory", new MemoryHealthCheck())
            .AddCheck("disk_space", new DiskSpaceHealthCheck())
            .AddCheck<StartupHealthCheck>("startup", tags: new[] { "ready" });

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var healthCheckService = serviceProvider.GetRequiredService<HealthCheckService>();

        // Run all health checks
        Console.WriteLine("Running all health checks...\n");
        var result = await healthCheckService.CheckHealthAsync();

        PrintHealthReport(result);

        // Run specific tags
        Console.WriteLine("\n\nRunning 'ready' health checks...\n");
        var readyResult = await healthCheckService.CheckHealthAsync(
            check => check.Tags.Contains("ready"));

        PrintHealthReport(readyResult);

        // Simulate startup completion
        var startupCheck = serviceProvider.GetRequiredService<StartupHealthCheck>();
        startupCheck.MarkAsCompleted();

        Console.WriteLine("\n\nAfter startup completion...\n");
        var finalResult = await healthCheckService.CheckHealthAsync();
        PrintHealthReport(finalResult);

        Console.WriteLine("\n✓ All health check patterns demonstrated!");
    }

    private static void PrintHealthReport(HealthReport report)
    {
        Console.WriteLine($"Overall Status: {report.Status}");
        Console.WriteLine($"Total Duration: {report.TotalDuration.TotalMilliseconds:F2}ms");
        Console.WriteLine($"Checks: {report.Entries.Count}");
        Console.WriteLine();

        foreach (var (name, entry) in report.Entries)
        {
            var icon = entry.Status switch
            {
                HealthStatus.Healthy => "✓",
                HealthStatus.Degraded => "⚠",
                HealthStatus.Unhealthy => "✗",
                _ => "?"
            };

            Console.WriteLine($"{icon} {name}: {entry.Status}");
            if (!string.IsNullOrEmpty(entry.Description))
            {
                Console.WriteLine($"  Description: {entry.Description}");
            }

            if (entry.Duration > TimeSpan.Zero)
            {
                Console.WriteLine($"  Duration: {entry.Duration.TotalMilliseconds:F2}ms");
            }

            if (entry.Data.Count > 0)
            {
                Console.WriteLine("  Data:");
                foreach (var (key, value) in entry.Data)
                {
                    Console.WriteLine($"    - {key}: {value}");
                }
            }

            if (entry.Exception != null)
            {
                Console.WriteLine($"  Exception: {entry.Exception.Message}");
            }

            Console.WriteLine();
        }
    }
}
