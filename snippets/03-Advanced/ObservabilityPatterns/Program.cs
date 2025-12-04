using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Serilog;
using Serilog.Context;

namespace ObservabilityPatterns;

/// <summary>
/// Observability Patterns Tutorial - Production Monitoring
/// Demonstrates logging, tracing, metrics, and health checks
/// </summary>
class Program
{
    private static readonly ActivitySource ActivitySource = new("ObservabilityDemo", "1.0.0");
    private static readonly Meter Meter = new("ObservabilityDemo", "1.0.0");
    private static readonly Counter<long> RequestCounter = Meter.CreateCounter<long>("requests_total");
    private static readonly Histogram<double> RequestDuration = Meter.CreateHistogram<double>("request_duration_ms");

    static void Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .WriteTo.Console(outputTemplate: 
                "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .CreateLogger();

        try
        {
            Console.WriteLine("═══════════════════════════════════════════════════════════════════");
            Console.WriteLine("OBSERVABILITY PATTERNS TUTORIAL");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════\n");

            RunStructuredLoggingExamples();
            RunDistributedTracingExamples();
            RunMetricsExamples();
            RunCorrelationIdExamples();
            RunHealthCheckExamples();
            RunRealWorldExample();

            Console.WriteLine("\n\n═══════════════════════════════════════════════════════════════════");
            Console.WriteLine("Tutorial Complete!");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    #region 1. Structured Logging with Serilog

    static void RunStructuredLoggingExamples()
    {
        Console.WriteLine("\n1. STRUCTURED LOGGING");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        // ❌ Bad: String interpolation - not structured
        var userId = 123;
        var action = "Login";
        Log.Information($"User {userId} performed {action}");  // Loses structure

        // ✅ Good: Structured logging with named parameters
        Log.Information("User {UserId} performed {Action}", userId, action);

        // ✅ Excellent: Destructuring objects
        var user = new { Id = 123, Name = "Alice", Email = "alice@example.com" };
        Log.Information("User logged in: {@User}", user);  // @ = destructure

        // Log levels demonstration
        Log.Verbose("Verbose: Noisy information for debugging");
        Log.Debug("Debug: Internal system events");
        Log.Information("Information: General flow of application");
        Log.Warning("Warning: Something unusual happened");
        Log.Error(new Exception("Simulated error"), "Error: Something failed");

        // Log context - adds properties to all logs in scope
        using (LogContext.PushProperty("CorrelationId", Guid.NewGuid()))
        using (LogContext.PushProperty("Environment", "Production"))
        {
            Log.Information("This log has correlation ID automatically");
            ProcessOrder(new Order { Id = 1001, Total = 99.99m });
        }

        Console.WriteLine("\nKey Takeaways:");
        Console.WriteLine("  ✓ Use structured logging with named parameters");
        Console.WriteLine("  ✓ Use @ to destructure objects");
        Console.WriteLine("  ✓ Use LogContext for ambient properties");
        Console.WriteLine("  ✗ Avoid string interpolation in logs");
    }

    static void ProcessOrder(Order order)
    {
        Log.Information("Processing order {OrderId} for amount {Amount:C}", order.Id, order.Total);
        // Log automatically includes CorrelationId from context
    }

    #endregion

    #region 2. Distributed Tracing with Activity

    static void RunDistributedTracingExamples()
    {
        Console.WriteLine("\n\n2. DISTRIBUTED TRACING");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        // Create a trace for end-to-end request
        using var rootActivity = ActivitySource.StartActivity("ProcessOrder", ActivityKind.Server);
        rootActivity?.SetTag("order.id", 12345);
        rootActivity?.SetTag("customer.id", 67890);

        Console.WriteLine($"Trace ID: {Activity.Current?.TraceId}");
        Console.WriteLine($"Span ID: {Activity.Current?.SpanId}");

        // Child span - database call
        using (var dbActivity = ActivitySource.StartActivity("Database.Query", ActivityKind.Client))
        {
            dbActivity?.SetTag("db.operation", "SELECT");
            dbActivity?.SetTag("db.table", "Orders");
            System.Threading.Thread.Sleep(50);  // Simulate DB query
            dbActivity?.SetStatus(ActivityStatusCode.Ok);
            Console.WriteLine($"  DB Span ID: {dbActivity?.SpanId} (Parent: {dbActivity?.ParentSpanId})");
        }

        // Child span - external API call
        using (var apiActivity = ActivitySource.StartActivity("ExternalAPI.Call", ActivityKind.Client))
        {
            apiActivity?.SetTag("http.method", "POST");
            apiActivity?.SetTag("http.url", "https://api.example.com/payment");
            System.Threading.Thread.Sleep(100);  // Simulate API call
            apiActivity?.SetStatus(ActivityStatusCode.Ok);
            Console.WriteLine($"  API Span ID: {apiActivity?.SpanId} (Parent: {apiActivity?.ParentSpanId})");
        }

        rootActivity?.SetStatus(ActivityStatusCode.Ok);
        
        Console.WriteLine("\nKey Takeaways:");
        Console.WriteLine("  ✓ Use Activity for distributed tracing");
        Console.WriteLine("  ✓ Each operation gets a unique span ID");
        Console.WriteLine("  ✓ Spans are hierarchical (parent-child)");
        Console.WriteLine("  ✓ Trace ID stays same across all spans");
    }

    #endregion

    #region 3. Metrics Collection

    static void RunMetricsExamples()
    {
        Console.WriteLine("\n\n3. METRICS COLLECTION");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        // Counter - things that only increase
        RequestCounter.Add(1, new KeyValuePair<string, object?>("endpoint", "/api/orders"));
        RequestCounter.Add(1, new KeyValuePair<string, object?>("endpoint", "/api/orders"));
        RequestCounter.Add(1, new KeyValuePair<string, object?>("endpoint", "/api/users"));
        Console.WriteLine("✓ Recorded 3 requests (2x /orders, 1x /users)");

        // Histogram - distribution of values
        RequestDuration.Record(45.2, new KeyValuePair<string, object?>("endpoint", "/api/orders"));
        RequestDuration.Record(123.8, new KeyValuePair<string, object?>("endpoint", "/api/orders"));
        RequestDuration.Record(12.5, new KeyValuePair<string, object?>("endpoint", "/api/users"));
        Console.WriteLine("✓ Recorded request durations for analysis");

        // Gauge example (via observable)
        var observableGauge = Meter.CreateObservableGauge("memory_usage_mb", () => 
        {
            return GC.GetTotalMemory(false) / 1024.0 / 1024.0;
        });
        Console.WriteLine($"✓ Current memory usage: {GC.GetTotalMemory(false) / 1024.0 / 1024.0:F2} MB");

        Console.WriteLine("\nMetric Types:");
        Console.WriteLine("  Counter: Monotonically increasing (requests, errors)");
        Console.WriteLine("  Histogram: Distribution of values (latency, size)");
        Console.WriteLine("  Gauge: Current value (memory, CPU, queue size)");

        Console.WriteLine("\nKey Takeaways:");
        Console.WriteLine("  ✓ Use counters for totals (requests, errors)");
        Console.WriteLine("  ✓ Use histograms for distributions (latency)");
        Console.WriteLine("  ✓ Use gauges for current state (memory, connections)");
        Console.WriteLine("  ✓ Always add labels/tags for dimensions");
    }

    #endregion

    #region 4. Correlation IDs

    static void RunCorrelationIdExamples()
    {
        Console.WriteLine("\n\n4. CORRELATION IDs");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        var correlationId = Guid.NewGuid().ToString();
        Console.WriteLine($"Request started with Correlation ID: {correlationId}\n");

        // All logs in this scope will have the correlation ID
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            Log.Information("Received request");
            
            ServiceA();
            ServiceB();
            
            Log.Information("Request completed");
        }

        Console.WriteLine("\nKey Takeaways:");
        Console.WriteLine("  ✓ Generate correlation ID at entry point");
        Console.WriteLine("  ✓ Pass through all service calls");
        Console.WriteLine("  ✓ Include in all logs for traceability");
        Console.WriteLine("  ✓ Use LogContext for automatic propagation");
    }

    static void ServiceA()
    {
        Log.Information("ServiceA processing");  // Automatically has CorrelationId
        System.Threading.Thread.Sleep(10);
    }

    static void ServiceB()
    {
        Log.Information("ServiceB processing");  // Automatically has CorrelationId
        System.Threading.Thread.Sleep(10);
    }

    #endregion

    #region 5. Health Checks

    static void RunHealthCheckExamples()
    {
        Console.WriteLine("\n\n5. HEALTH CHECKS");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        // Simulate health check endpoints
        var healthStatus = PerformHealthChecks();

        Console.WriteLine("Health Check Results:");
        Console.WriteLine($"  Overall: {healthStatus.Status}");
        Console.WriteLine($"  Database: {healthStatus.Checks["Database"]}");
        Console.WriteLine($"  Cache: {healthStatus.Checks["Cache"]}");
        Console.WriteLine($"  External API: {healthStatus.Checks["ExternalAPI"]}");

        Console.WriteLine("\nHealth Check Best Practices:");
        Console.WriteLine("  ✓ Liveness: Is the app running?");
        Console.WriteLine("  ✓ Readiness: Can the app serve traffic?");
        Console.WriteLine("  ✓ Check dependencies (DB, cache, APIs)");
        Console.WriteLine("  ✓ Fast responses (< 1 second)");
        Console.WriteLine("  ✓ Different endpoints for liveness/readiness");
    }

    static HealthStatus PerformHealthChecks()
    {
        var checks = new Dictionary<string, string>();

        // Database check
        try
        {
            // Simulate DB ping
            System.Threading.Thread.Sleep(20);
            checks["Database"] = "Healthy";
        }
        catch
        {
            checks["Database"] = "Unhealthy";
        }

        // Cache check
        try
        {
            // Simulate cache ping
            System.Threading.Thread.Sleep(5);
            checks["Cache"] = "Healthy";
        }
        catch
        {
            checks["Cache"] = "Degraded";
        }

        // External API check
        checks["ExternalAPI"] = "Healthy";

        var status = checks.Values.All(v => v == "Healthy") ? "Healthy" : "Degraded";
        return new HealthStatus { Status = status, Checks = checks };
    }

    #endregion

    #region 6. Real-World Example

    static void RunRealWorldExample()
    {
        Console.WriteLine("\n\n6. REAL-WORLD EXAMPLE - E-Commerce Checkout");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        var correlationId = Guid.NewGuid().ToString();
        var orderId = 12345;
        var customerId = 67890;

        // Start root trace
        using var activity = ActivitySource.StartActivity("Checkout", ActivityKind.Server);
        activity?.SetTag("order.id", orderId);
        activity?.SetTag("customer.id", customerId);

        // Add correlation context
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("OrderId", orderId))
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Log.Information("Starting checkout for customer {CustomerId}", customerId);

                // 1. Validate inventory
                using (var span = ActivitySource.StartActivity("ValidateInventory"))
                {
                    Log.Information("Validating inventory");
                    System.Threading.Thread.Sleep(50);
                    span?.SetStatus(ActivityStatusCode.Ok);
                }

                // 2. Process payment
                using (var span = ActivitySource.StartActivity("ProcessPayment"))
                {
                    span?.SetTag("amount", 99.99);
                    span?.SetTag("currency", "USD");
                    Log.Information("Processing payment for amount {Amount:C}", 99.99m);
                    System.Threading.Thread.Sleep(100);
                    span?.SetStatus(ActivityStatusCode.Ok);
                }

                // 3. Update order status
                using (var span = ActivitySource.StartActivity("UpdateOrderStatus"))
                {
                    Log.Information("Updating order status to Confirmed");
                    System.Threading.Thread.Sleep(30);
                    span?.SetStatus(ActivityStatusCode.Ok);
                }

                stopwatch.Stop();

                // Record metrics
                RequestCounter.Add(1, 
                    new KeyValuePair<string, object?>("operation", "checkout"),
                    new KeyValuePair<string, object?>("status", "success"));
                
                RequestDuration.Record(stopwatch.Elapsed.TotalMilliseconds,
                    new KeyValuePair<string, object?>("operation", "checkout"));

                Log.Information("Checkout completed successfully in {Duration}ms", stopwatch.ElapsedMilliseconds);
                activity?.SetStatus(ActivityStatusCode.Ok);

                Console.WriteLine($"\n✓ Checkout completed in {stopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine($"  Trace ID: {activity?.TraceId}");
                Console.WriteLine($"  Correlation ID: {correlationId}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Checkout failed");
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                
                RequestCounter.Add(1, 
                    new KeyValuePair<string, object?>("operation", "checkout"),
                    new KeyValuePair<string, object?>("status", "error"));
            }
        }

        Console.WriteLine("\nObservability Stack:");
        Console.WriteLine("  📊 Metrics → Prometheus/Grafana");
        Console.WriteLine("  📝 Logs → ELK Stack/Splunk");
        Console.WriteLine("  🔍 Traces → Jaeger/Zipkin");
        Console.WriteLine("  ❤️  Health → Kubernetes probes");
    }

    #endregion

    // Helper classes
    record Order
    {
        public int Id { get; init; }
        public decimal Total { get; init; }
    }

    record HealthStatus
    {
        public string Status { get; init; } = "";
        public Dictionary<string, string> Checks { get; init; } = new();
    }
}
