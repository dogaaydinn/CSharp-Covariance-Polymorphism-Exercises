using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AdvancedCsharpConcepts.Advanced.Observability;

/// <summary>
/// OpenTelemetry Examples - Production-grade metrics and tracing.
/// NVIDIA/Silicon Valley best practice: Standardized observability with OpenTelemetry.
/// </summary>
public static class OpenTelemetryExamples
{
    // Activity source for distributed tracing
    private static readonly ActivitySource ActivitySource = new("AdvancedConcepts.Application", "1.0.0");

    // Meter for custom metrics
    private static readonly Meter Meter = new("AdvancedConcepts.Metrics", "1.0.0");

    // Custom metrics
    private static readonly Counter<long> RequestCounter = Meter.CreateCounter<long>(
        "app.requests.total",
        description: "Total number of requests processed");

    private static readonly Histogram<double> RequestDuration = Meter.CreateHistogram<double>(
        "app.request.duration",
        unit: "ms",
        description: "Request processing duration");

    private static readonly ObservableGauge<int> ActiveConnections = Meter.CreateObservableGauge(
        "app.active_connections",
        () => GetActiveConnections(),
        description: "Number of active connections");

    private static readonly Counter<long> ErrorCounter = Meter.CreateCounter<long>(
        "app.errors.total",
        description: "Total number of errors");

    private static int _activeConnections;

    /// <summary>
    /// Configures OpenTelemetry with metrics and tracing.
    /// </summary>
    public static MeterProvider ConfigureMetrics()
    {
        return Sdk.CreateMeterProviderBuilder()
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("AdvancedConcepts", serviceVersion: "1.0.0")
                .AddAttributes(new Dictionary<string, object>
                {
                    ["environment"] = "production",
                    ["deployment"] = "azure-westus2"
                }))
            .AddMeter("AdvancedConcepts.Metrics")
            .AddConsoleExporter()
            .Build();
    }

    /// <summary>
    /// Configures OpenTelemetry tracing.
    /// </summary>
    public static TracerProvider ConfigureTracing()
    {
        return Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("AdvancedConcepts", serviceVersion: "1.0.0"))
            .AddSource("AdvancedConcepts.Application")
            .AddConsoleExporter()
            .Build();
    }

    /// <summary>
    /// Demonstrates counter metrics for tracking events.
    /// </summary>
    public static async Task CounterMetricsExample()
    {
        Console.WriteLine("=== Counter Metrics Example ===\n");

        // Simulate requests
        for (var i = 0; i < 10; i++)
        {
            var tags = new TagList
            {
                { "endpoint", i % 2 == 0 ? "/api/users" : "/api/orders" },
                { "method", "GET" },
                { "status", i % 5 == 0 ? "500" : "200" }
            };

            RequestCounter.Add(1, tags);

            if (i % 5 == 0)
            {
                var errorTags = new TagList
                {
                    { "error_type", "TimeoutException" },
                    { "endpoint", "/api/users" }
                };
                ErrorCounter.Add(1, errorTags);
            }

            Console.WriteLine($"Request {i + 1} recorded");
            await Task.Delay(100);
        }

        Console.WriteLine("\n✓ Counter metrics recorded");
    }

    /// <summary>
    /// Demonstrates histogram metrics for measuring distributions.
    /// </summary>
    public static async Task HistogramMetricsExample()
    {
        Console.WriteLine("\n=== Histogram Metrics Example ===\n");

        var random = new Random();

        for (var i = 0; i < 20; i++)
        {
            var duration = random.Next(10, 500);
            var tags = new TagList
            {
                { "endpoint", i % 3 == 0 ? "/api/users" : "/api/orders" },
                { "cache_hit", random.Next(0, 2) == 0 ? "true" : "false" }
            };

            RequestDuration.Record(duration, tags);

            Console.WriteLine($"Request duration: {duration}ms");
            await Task.Delay(50);
        }

        Console.WriteLine("\n✓ Histogram metrics recorded");
    }

    /// <summary>
    /// Demonstrates observable gauge metrics for current state.
    /// </summary>
    public static async Task GaugeMetricsExample()
    {
        Console.WriteLine("\n=== Observable Gauge Metrics Example ===\n");

        // Simulate connections opening and closing
        for (var i = 0; i < 5; i++)
        {
            _activeConnections += Random.Shared.Next(1, 5);
            Console.WriteLine($"Active connections: {_activeConnections}");
            await Task.Delay(200);
        }

        for (var i = 0; i < 3; i++)
        {
            _activeConnections -= Random.Shared.Next(1, 3);
            if (_activeConnections < 0) _activeConnections = 0;
            Console.WriteLine($"Active connections: {_activeConnections}");
            await Task.Delay(200);
        }

        Console.WriteLine("\n✓ Gauge metrics demonstrated");
    }

    /// <summary>
    /// Demonstrates distributed tracing with activities.
    /// </summary>
    public static async Task DistributedTracingExample()
    {
        Console.WriteLine("\n=== Distributed Tracing Example ===\n");

        // Create root activity (incoming request)
        using var activity = ActivitySource.StartActivity("ProcessOrder", ActivityKind.Server);
        activity?.SetTag("order.id", "ORD-12345");
        activity?.SetTag("customer.id", Guid.NewGuid().ToString());

        Console.WriteLine($"Started root activity: {activity?.TraceId}");

        try
        {
            // Child activity 1: Validate order
            await ValidateOrderAsync(activity);

            // Child activity 2: Process payment
            await ProcessPaymentAsync(activity);

            // Child activity 3: Update inventory
            await UpdateInventoryAsync(activity);

            activity?.SetStatus(ActivityStatusCode.Ok);
            Console.WriteLine("\n✓ Order processed successfully");
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            Console.WriteLine($"\n✗ Order processing failed: {ex.Message}");
        }
    }

    private static async Task ValidateOrderAsync(Activity? parent)
    {
        using var activity = ActivitySource.StartActivity("ValidateOrder", ActivityKind.Internal, parent?.Context ?? default);
        activity?.SetTag("validation.type", "schema");

        Console.WriteLine("  Validating order...");
        await Task.Delay(Random.Shared.Next(50, 150));

        activity?.AddEvent(new ActivityEvent("OrderValidated"));
        Console.WriteLine("  ✓ Order validated");
    }

    private static async Task ProcessPaymentAsync(Activity? parent)
    {
        using var activity = ActivitySource.StartActivity("ProcessPayment", ActivityKind.Client, parent?.Context ?? default);
        activity?.SetTag("payment.method", "CreditCard");
        activity?.SetTag("payment.amount", 299.99m);

        Console.WriteLine("  Processing payment...");
        await Task.Delay(Random.Shared.Next(100, 300));

        // Simulate occasional payment failures
        if (Random.Shared.Next(0, 10) == 0)
        {
            var ex = new InvalidOperationException("Payment gateway timeout");
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            throw ex;
        }

        activity?.AddEvent(new ActivityEvent("PaymentProcessed"));
        Console.WriteLine("  ✓ Payment processed");
    }

    private static async Task UpdateInventoryAsync(Activity? parent)
    {
        using var activity = ActivitySource.StartActivity("UpdateInventory", ActivityKind.Internal, parent?.Context ?? default);
        activity?.SetTag("warehouse.location", "US-WEST");

        Console.WriteLine("  Updating inventory...");
        await Task.Delay(Random.Shared.Next(50, 100));

        activity?.AddEvent(new ActivityEvent("InventoryUpdated"));
        Console.WriteLine("  ✓ Inventory updated");
    }

    /// <summary>
    /// Demonstrates complex distributed trace with external calls.
    /// </summary>
    public static async Task ComplexDistributedTraceExample()
    {
        Console.WriteLine("\n=== Complex Distributed Trace Example ===\n");

        using var rootActivity = ActivitySource.StartActivity("HandleUserRequest", ActivityKind.Server);
        rootActivity?.SetTag("http.method", "POST");
        rootActivity?.SetTag("http.url", "/api/users/register");
        rootActivity?.SetTag("http.client_ip", "192.168.1.100");

        Console.WriteLine($"Trace ID: {rootActivity?.TraceId}");
        Console.WriteLine($"Span ID: {rootActivity?.SpanId}");

        try
        {
            await CallAuthServiceAsync(rootActivity);
            await CallDatabaseAsync(rootActivity);
            await CallNotificationServiceAsync(rootActivity);

            rootActivity?.SetTag("http.status_code", 200);
            rootActivity?.SetStatus(ActivityStatusCode.Ok);
            Console.WriteLine("\n✓ Request completed successfully");
        }
        catch (Exception ex)
        {
            rootActivity?.SetTag("http.status_code", 500);
            rootActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            rootActivity?.AddException(ex);
            Console.WriteLine($"\n✗ Request failed: {ex.Message}");
        }
    }

    private static async Task CallAuthServiceAsync(Activity? parent)
    {
        using var activity = ActivitySource.StartActivity("CallAuthService", ActivityKind.Client, parent?.Context ?? default);
        activity?.SetTag("peer.service", "auth-service");
        activity?.SetTag("rpc.method", "ValidateToken");

        Console.WriteLine("  → Calling Auth Service...");
        await Task.Delay(Random.Shared.Next(50, 150));
        Console.WriteLine("  ✓ Auth Service responded");
    }

    private static async Task CallDatabaseAsync(Activity? parent)
    {
        using var activity = ActivitySource.StartActivity("DatabaseQuery", ActivityKind.Client, parent?.Context ?? default);
        activity?.SetTag("db.system", "postgresql");
        activity?.SetTag("db.name", "users");
        activity?.SetTag("db.statement", "INSERT INTO users (id, email) VALUES ($1, $2)");

        Console.WriteLine("  → Executing database query...");
        await Task.Delay(Random.Shared.Next(30, 100));
        Console.WriteLine("  ✓ Database query completed");
    }

    private static async Task CallNotificationServiceAsync(Activity? parent)
    {
        using var activity = ActivitySource.StartActivity("SendNotification", ActivityKind.Producer, parent?.Context ?? default);
        activity?.SetTag("messaging.system", "kafka");
        activity?.SetTag("messaging.destination", "user-notifications");

        Console.WriteLine("  → Sending notification...");
        await Task.Delay(Random.Shared.Next(20, 80));
        Console.WriteLine("  ✓ Notification sent");
    }

    /// <summary>
    /// Runs all OpenTelemetry examples.
    /// </summary>
    public static async Task RunExamples()
    {
        Console.WriteLine("=== OpenTelemetry Metrics & Tracing ===\n");

        using var meterProvider = ConfigureMetrics();
        using var tracerProvider = ConfigureTracing();

        await CounterMetricsExample();
        await HistogramMetricsExample();
        await GaugeMetricsExample();
        await DistributedTracingExample();
        await ComplexDistributedTraceExample();

        Console.WriteLine("\n✓ All OpenTelemetry patterns demonstrated!");
    }

    private static int GetActiveConnections()
    {
        return _activeConnections;
    }
}
