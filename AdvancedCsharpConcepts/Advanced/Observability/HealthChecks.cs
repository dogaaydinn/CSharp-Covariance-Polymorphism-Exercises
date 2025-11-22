using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace AdvancedCsharpConcepts.Advanced.Observability;

/// <summary>
/// Production-ready health check implementations.
/// Silicon Valley best practice: Monitor application health for observability.
/// </summary>
public static class HealthCheckExamples
{
    /// <summary>
    /// Health check for database connectivity.
    /// </summary>
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;

        public DatabaseHealthCheck(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Simulate database ping
                await Task.Delay(10, cancellationToken);

                var data = new Dictionary<string, object>
                {
                    { "connection_string", MaskConnectionString(_connectionString) },
                    { "response_time_ms", 10 }
                };

                return HealthCheckResult.Healthy("Database is responsive", data);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Database connection failed",
                    ex,
                    new Dictionary<string, object> { { "connection_string", MaskConnectionString(_connectionString) } });
            }
        }

        private static string MaskConnectionString(string connString)
        {
            // Mask sensitive information
            return connString.Length > 10 ? $"{connString[..10]}***" : "***";
        }
    }

    /// <summary>
    /// Health check for external API dependencies.
    /// </summary>
    public class ApiHealthCheck : IHealthCheck
    {
        private readonly string _apiEndpoint;
        private readonly TimeSpan _timeout;

        public ApiHealthCheck(string apiEndpoint, TimeSpan? timeout = null)
        {
            _apiEndpoint = apiEndpoint ?? throw new ArgumentNullException(nameof(apiEndpoint));
            _timeout = timeout ?? TimeSpan.FromSeconds(5);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                // Simulate API call
                await Task.Delay(50, cancellationToken);

                sw.Stop();

                var data = new Dictionary<string, object>
                {
                    { "endpoint", _apiEndpoint },
                    { "response_time_ms", sw.ElapsedMilliseconds },
                    { "timeout_ms", _timeout.TotalMilliseconds }
                };

                if (sw.Elapsed > _timeout)
                {
                    return HealthCheckResult.Degraded(
                        $"API response time ({sw.ElapsedMilliseconds}ms) exceeded timeout ({_timeout.TotalMilliseconds}ms)",
                        data: data);
                }

                return HealthCheckResult.Healthy("API is responsive", data);
            }
            catch (OperationCanceledException)
            {
                return HealthCheckResult.Unhealthy("API health check was cancelled");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"API health check failed: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// Health check for memory usage.
    /// </summary>
    public class MemoryHealthCheck : IHealthCheck
    {
        private readonly long _thresholdBytes;

        public MemoryHealthCheck(long thresholdBytes = 1024 * 1024 * 1024) // 1GB default
        {
            _thresholdBytes = thresholdBytes;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var allocated = GC.GetTotalMemory(forceFullCollection: false);
            var allocatedMB = allocated / 1024 / 1024;
            var thresholdMB = _thresholdBytes / 1024 / 1024;

            var data = new Dictionary<string, object>
            {
                { "allocated_mb", allocatedMB },
                { "threshold_mb", thresholdMB },
                { "gen0_collections", GC.CollectionCount(0) },
                { "gen1_collections", GC.CollectionCount(1) },
                { "gen2_collections", GC.CollectionCount(2) }
            };

            if (allocated > _thresholdBytes)
            {
                return Task.FromResult(HealthCheckResult.Degraded(
                    $"Memory usage ({allocatedMB}MB) exceeds threshold ({thresholdMB}MB)",
                    data: data));
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                $"Memory usage is normal ({allocatedMB}MB)",
                data));
        }
    }

    /// <summary>
    /// Health check for disk space.
    /// </summary>
    public class DiskSpaceHealthCheck : IHealthCheck
    {
        private readonly string _driveName;
        private readonly long _minimumFreeBytesThreshold;

        public DiskSpaceHealthCheck(string driveName = "C:\\", long minimumFreeGB = 10)
        {
            _driveName = driveName ?? throw new ArgumentNullException(nameof(driveName));
            _minimumFreeBytesThreshold = minimumFreeGB * 1024 * 1024 * 1024;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Simulate disk check (in real implementation, use DriveInfo)
                var simulatedFreeSpace = 50L * 1024 * 1024 * 1024; // 50GB
                var freeSpaceGB = simulatedFreeSpace / 1024 / 1024 / 1024;
                var thresholdGB = _minimumFreeBytesThreshold / 1024 / 1024 / 1024;

                var data = new Dictionary<string, object>
                {
                    { "drive", _driveName },
                    { "free_space_gb", freeSpaceGB },
                    { "threshold_gb", thresholdGB }
                };

                if (simulatedFreeSpace < _minimumFreeBytesThreshold)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy(
                        $"Disk space on {_driveName} ({freeSpaceGB}GB) below threshold ({thresholdGB}GB)",
                        data: data));
                }

                return Task.FromResult(HealthCheckResult.Healthy(
                    $"Sufficient disk space on {_driveName} ({freeSpaceGB}GB)",
                    data));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"Unable to check disk space: {ex.Message}",
                    ex));
            }
        }
    }

    /// <summary>
    /// Composite health check that combines multiple checks.
    /// </summary>
    public class CompositeHealthCheck : IHealthCheck
    {
        private readonly IEnumerable<IHealthCheck> _healthChecks;

        public CompositeHealthCheck(params IHealthCheck[] healthChecks)
        {
            _healthChecks = healthChecks ?? throw new ArgumentNullException(nameof(healthChecks));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var results = new List<HealthCheckResult>();

            foreach (var check in _healthChecks)
            {
                var result = await check.CheckHealthAsync(context, cancellationToken);
                results.Add(result);
            }

            var hasUnhealthy = results.Any(r => r.Status == HealthStatus.Unhealthy);
            var hasDegraded = results.Any(r => r.Status == HealthStatus.Degraded);

            var status = hasUnhealthy ? HealthStatus.Unhealthy :
                        hasDegraded ? HealthStatus.Degraded :
                        HealthStatus.Healthy;

            var description = $"{results.Count(r => r.Status == HealthStatus.Healthy)} healthy, " +
                            $"{results.Count(r => r.Status == HealthStatus.Degraded)} degraded, " +
                            $"{results.Count(r => r.Status == HealthStatus.Unhealthy)} unhealthy";

            return new HealthCheckResult(status, description);
        }
    }

    /// <summary>
    /// Demonstrates health check implementations.
    /// </summary>
    public static async Task RunExample(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Health Check Examples ===\n");

        // Create health checks
        var dbCheck = new DatabaseHealthCheck("Server=localhost;Database=MyApp");
        var apiCheck = new ApiHealthCheck("https://api.example.com/health");
        var memCheck = new MemoryHealthCheck(thresholdBytes: 500 * 1024 * 1024); // 500MB
        var diskCheck = new DiskSpaceHealthCheck(minimumFreeGB: 10);

        var context = new HealthCheckContext();

        // Run individual health checks
        Console.WriteLine("1. Database Health Check:");
        var dbResult = await dbCheck.CheckHealthAsync(context, cancellationToken);
        PrintHealthCheckResult("Database", dbResult);

        Console.WriteLine("\n2. API Health Check:");
        var apiResult = await apiCheck.CheckHealthAsync(context, cancellationToken);
        PrintHealthCheckResult("API", apiResult);

        Console.WriteLine("\n3. Memory Health Check:");
        var memResult = await memCheck.CheckHealthAsync(context, cancellationToken);
        PrintHealthCheckResult("Memory", memResult);

        Console.WriteLine("\n4. Disk Space Health Check:");
        var diskResult = await diskCheck.CheckHealthAsync(context, cancellationToken);
        PrintHealthCheckResult("Disk", diskResult);

        // Composite health check
        Console.WriteLine("\n5. Composite Health Check:");
        var compositeCheck = new CompositeHealthCheck(dbCheck, apiCheck, memCheck, diskCheck);
        var compositeResult = await compositeCheck.CheckHealthAsync(context, cancellationToken);
        PrintHealthCheckResult("Overall System", compositeResult);
    }

    private static void PrintHealthCheckResult(string name, HealthCheckResult result)
    {
        var statusSymbol = result.Status switch
        {
            HealthStatus.Healthy => "✓",
            HealthStatus.Degraded => "⚠",
            HealthStatus.Unhealthy => "✗",
            _ => "?"
        };

        Console.WriteLine($"  {statusSymbol} {name}: {result.Status}");
        Console.WriteLine($"     Description: {result.Description}");

        if (result.Data?.Any() == true)
        {
            Console.WriteLine("     Data:");
            foreach (var (key, value) in result.Data)
            {
                Console.WriteLine($"       - {key}: {value}");
            }
        }

        if (result.Exception != null)
        {
            Console.WriteLine($"     Exception: {result.Exception.Message}");
        }
    }
}
