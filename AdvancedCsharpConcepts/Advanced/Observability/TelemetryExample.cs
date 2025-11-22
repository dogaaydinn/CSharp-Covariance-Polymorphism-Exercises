using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace AdvancedCsharpConcepts.Advanced.Observability;

/// <summary>
/// OpenTelemetry-compatible metrics and tracing examples.
/// Silicon Valley best practice: Instrument code for production observability.
/// </summary>
/// <remarks>
/// Uses System.Diagnostics.Metrics (OpenTelemetry standard) for metrics.
/// Uses System.Diagnostics.Activity for distributed tracing.
/// Compatible with Prometheus, Grafana, Jaeger, Azure Monitor, AWS X-Ray.
/// </remarks>
public static class TelemetryExample
{
    private static readonly Meter ApplicationMeter = new("AdvancedCsharpConcepts", "1.0.0");
    private static readonly ActivitySource ActivitySource = new("AdvancedCsharpConcepts");

    // Counters
    private static readonly Counter<long> RequestCounter = ApplicationMeter.CreateCounter<long>(
        "app.requests.total",
        description: "Total number of requests processed");

    private static readonly Counter<long> ErrorCounter = ApplicationMeter.CreateCounter<long>(
        "app.errors.total",
        description: "Total number of errors");

    // Histograms
    private static readonly Histogram<double> RequestDuration = ApplicationMeter.CreateHistogram<double>(
        "app.request.duration",
        unit: "ms",
        description: "Request processing duration in milliseconds");

    private static readonly Histogram<long> PayloadSize = ApplicationMeter.CreateHistogram<long>(
        "app.payload.size",
        unit: "bytes",
        description: "Request payload size in bytes");

    // Gauges (ObservableGauge)
    private static long _activeConnections = 0;
    private static readonly ObservableGauge<long> ActiveConnections = ApplicationMeter.CreateObservableGauge(
        "app.connections.active",
        () => _activeConnections,
        description: "Current number of active connections");

    private static readonly ObservableGauge<long> MemoryUsage = ApplicationMeter.CreateObservableGauge(
        "app.memory.usage",
        () => GC.GetTotalMemory(false) / 1024 / 1024,
        unit: "MB",
        description: "Current memory usage in megabytes");

    /// <summary>
    /// Simulates processing a request with telemetry.
    /// </summary>
    public static async Task<string> ProcessRequestAsync(
        string endpoint,
        int payloadSizeBytes,
        CancellationToken cancellationToken = default)
    {
        // Start distributed trace span
        using var activity = ActivitySource.StartActivity("ProcessRequest", ActivityKind.Server);
        activity?.SetTag("http.endpoint", endpoint);
        activity?.SetTag("http.payload_size", payloadSizeBytes);

        var sw = Stopwatch.StartNew();

        try
        {
            // Increment active connections
            Interlocked.Increment(ref _activeConnections);

            // Record request
            RequestCounter.Add(1, new KeyValuePair<string, object?>("endpoint", endpoint));

            // Record payload size
            PayloadSize.Record(payloadSizeBytes, new KeyValuePair<string, object?>("endpoint", endpoint));

            // Simulate processing
            var processingTime = Random.Shared.Next(50, 500);
            await Task.Delay(processingTime, cancellationToken);

            // Simulate occasional errors
            if (Random.Shared.Next(0, 10) == 0)
            {
                throw new InvalidOperationException("Simulated error");
            }

            sw.Stop();

            // Record successful request duration
            RequestDuration.Record(sw.Elapsed.TotalMilliseconds,
                new KeyValuePair<string, object?>("endpoint", endpoint),
                new KeyValuePair<string, object?>("status", "success"));

            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("http.status_code", 200);

            return $"Success: Processed {payloadSizeBytes} bytes in {sw.ElapsedMilliseconds}ms";
        }
        catch (Exception ex)
        {
            sw.Stop();

            // Record error
            ErrorCounter.Add(1,
                new KeyValuePair<string, object?>("endpoint", endpoint),
                new KeyValuePair<string, object?>("error_type", ex.GetType().Name));

            // Record failed request duration
            RequestDuration.Record(sw.Elapsed.TotalMilliseconds,
                new KeyValuePair<string, object?>("endpoint", endpoint),
                new KeyValuePair<string, object?>("status", "error"));

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("http.status_code", 500);
            activity?.SetTag("exception.type", ex.GetType().FullName);
            activity?.SetTag("exception.message", ex.Message);

            return $"Error: {ex.Message}";
        }
        finally
        {
            // Decrement active connections
            Interlocked.Decrement(ref _activeConnections);
        }
    }

    /// <summary>
    /// Simulates a database operation with tracing.
    /// </summary>
    public static async Task<int> QueryDatabaseAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity("DatabaseQuery", ActivityKind.Client);
        activity?.SetTag("db.system", "postgresql");
        activity?.SetTag("db.statement", query);
        activity?.SetTag("db.name", "application_db");

        var sw = Stopwatch.StartNew();

        try
        {
            // Simulate database query
            await Task.Delay(Random.Shared.Next(10, 100), cancellationToken);

            var rowCount = Random.Shared.Next(1, 1000);

            activity?.SetTag("db.rows_affected", rowCount);
            activity?.SetStatus(ActivityStatusCode.Ok);

            return rowCount;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        finally
        {
            sw.Stop();
            activity?.SetTag("db.duration_ms", sw.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Demonstrates complex operation with nested spans.
    /// </summary>
    public static async Task<string> ComplexOperationAsync(CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity("ComplexOperation");
        activity?.SetTag("operation.type", "complex");

        try
        {
            // Step 1: Database query
            using (var dbActivity = ActivitySource.StartActivity("Step1_FetchData"))
            {
                var rowCount = await QueryDatabaseAsync("SELECT * FROM users", cancellationToken);
                dbActivity?.SetTag("result.row_count", rowCount);
            }

            // Step 2: External API call
            using (var apiActivity = ActivitySource.StartActivity("Step2_CallExternalAPI"))
            {
                apiActivity?.SetTag("api.endpoint", "https://api.example.com/data");
                await Task.Delay(100, cancellationToken);
            }

            // Step 3: Data processing
            using (var processActivity = ActivitySource.StartActivity("Step3_ProcessData"))
            {
                processActivity?.SetTag("processing.algorithm", "parallel");
                await Task.Delay(50, cancellationToken);
            }

            activity?.SetStatus(ActivityStatusCode.Ok);
            return "Complex operation completed successfully";
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Demonstrates telemetry collection.
    /// </summary>
    public static async Task RunExample(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== OpenTelemetry-Compatible Telemetry Examples ===\n");

        Console.WriteLine("Metrics and Traces are being collected...\n");
        Console.WriteLine("In production, export to:");
        Console.WriteLine("  - Prometheus (metrics)");
        Console.WriteLine("  - Grafana (visualization)");
        Console.WriteLine("  - Jaeger/Zipkin (distributed tracing)");
        Console.WriteLine("  - Azure Monitor / AWS X-Ray (cloud observability)\n");

        // Simulate multiple requests
        Console.WriteLine("1. Simulating API Requests:");
        for (var i = 0; i < 5; i++)
        {
            var endpoint = i % 3 switch
            {
                0 => "/api/users",
                1 => "/api/products",
                _ => "/api/orders"
            };

            var result = await ProcessRequestAsync(endpoint, Random.Shared.Next(100, 10000), cancellationToken);
            Console.WriteLine($"  Request {i + 1}: {result}");
        }

        Console.WriteLine($"\n  Current active connections: {_activeConnections}");
        Console.WriteLine($"  Current memory usage: {GC.GetTotalMemory(false) / 1024 / 1024} MB");

        // Complex operation with nested spans
        Console.WriteLine("\n2. Complex Operation with Distributed Tracing:");
        try
        {
            var result = await ComplexOperationAsync(cancellationToken);
            Console.WriteLine($"  {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Error: {ex.Message}");
        }

        // Display current metrics summary
        Console.WriteLine("\n3. Metrics Summary:");
        Console.WriteLine("  Metrics are being collected in the background.");
        Console.WriteLine("  In production, these would be exported to your observability platform.");
        Console.WriteLine("\n  Example Prometheus metrics:");
        Console.WriteLine("    app_requests_total{endpoint=\"/api/users\"} 15");
        Console.WriteLine("    app_errors_total{endpoint=\"/api/users\",error_type=\"InvalidOperationException\"} 2");
        Console.WriteLine("    app_request_duration_bucket{endpoint=\"/api/users\",le=\"100\"} 8");
        Console.WriteLine("    app_connections_active 12");
        Console.WriteLine($"    app_memory_usage_mb {GC.GetTotalMemory(false) / 1024 / 1024}");

        Console.WriteLine("\n4. Distributed Tracing:");
        Console.WriteLine("  Trace spans have been created with the following structure:");
        Console.WriteLine("    - ComplexOperation (parent)");
        Console.WriteLine("      ├─ Step1_FetchData");
        Console.WriteLine("      │  └─ DatabaseQuery");
        Console.WriteLine("      ├─ Step2_CallExternalAPI");
        Console.WriteLine("      └─ Step3_ProcessData");
        Console.WriteLine("\n  In production, view these traces in Jaeger or your APM tool.");
    }
}
