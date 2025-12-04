using Serilog;
using Serilog.Context;
using Serilog.Events;

namespace AdvancedCsharpConcepts.Advanced.Observability;

/// <summary>
/// Enhanced Serilog Examples - Production-grade structured logging.
/// NVIDIA/Silicon Valley best practice: Enriched logging with contextual data.
/// </summary>
public static class EnhancedSerilogExamples
{
    /// <summary>
    /// Configures Serilog with enrichers for production environments.
    /// </summary>
    public static void ConfigureEnhancedSerilog()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)

            // Environment enrichers
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithEnvironmentUserName()

            // Process enrichers
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName()

            // Custom enrichers
            .Enrich.WithProperty("Application", "AdvancedConcepts")
            .Enrich.WithProperty("Version", "1.0.0")

            // Output sinks
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] [{MachineName}] [{ProcessId}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                "logs/app-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] [{MachineName}] [{ProcessId}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 30)
            .CreateLogger();
    }

    /// <summary>
    /// Demonstrates structured logging with context enrichment.
    /// </summary>
    public static async Task StructuredLoggingExample()
    {
        Console.WriteLine("=== Structured Logging with Enrichers ===\n");

        // Simple structured log
        Log.Information("Application started");

        // Structured log with properties
        var userId = Guid.NewGuid();
        var userName = "john.doe@example.com";

        Log.Information("User {UserId} logged in as {UserName}", userId, userName);

        // Using LogContext for scoped properties
        using (LogContext.PushProperty("CorrelationId", Guid.NewGuid()))
        using (LogContext.PushProperty("RequestPath", "/api/users"))
        {
            Log.Information("Processing API request");

            await Task.Delay(100);

            Log.Information("API request completed successfully");
        }

        // Logging complex objects
        var order = new
        {
            OrderId = "ORD-12345",
            CustomerId = userId,
            Amount = 299.99m,
            Items = new[] { "Product A", "Product B" }
        };

        Log.Information("Order created: {@Order}", order); // @ for structured logging

        // Different log levels
        Log.Debug("Debug message - detailed diagnostics");
        Log.Information("Information message - normal operation");
        Log.Warning("Warning message - potential issue detected");

        try
        {
            throw new InvalidOperationException("Simulated error for logging demo");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while processing order {OrderId}", order.OrderId);
        }
    }

    /// <summary>
    /// Demonstrates performance logging with metrics.
    /// </summary>
    public static async Task PerformanceLoggingExample()
    {
        Console.WriteLine("\n=== Performance Logging ===\n");

        var operationId = Guid.NewGuid();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        using (LogContext.PushProperty("OperationId", operationId))
        {
            Log.Information("Starting expensive operation");

            // Simulate work
            await Task.Delay(Random.Shared.Next(100, 500));

            stopwatch.Stop();

            Log.Information(
                "Operation completed in {ElapsedMs}ms",
                stopwatch.ElapsedMilliseconds);

            // Log performance metrics
            if (stopwatch.ElapsedMilliseconds > 300)
            {
                Log.Warning(
                    "Slow operation detected: {ElapsedMs}ms exceeds threshold of 300ms",
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }

    /// <summary>
    /// Demonstrates distributed tracing with correlation IDs.
    /// </summary>
    public static async Task DistributedTracingExample()
    {
        Console.WriteLine("\n=== Distributed Tracing with Correlation IDs ===\n");

        var correlationId = Guid.NewGuid();
        var traceId = Guid.NewGuid();

        // Simulate distributed request across multiple services
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("TraceId", traceId))
        {
            Log.Information("Incoming request to API Gateway");

            await SimulateServiceCall("AuthService", correlationId, traceId);
            await SimulateServiceCall("UserService", correlationId, traceId);
            await SimulateServiceCall("NotificationService", correlationId, traceId);

            Log.Information("Request completed successfully");
        }
    }

    private static async Task SimulateServiceCall(string serviceName, Guid correlationId, Guid traceId)
    {
        using (LogContext.PushProperty("ServiceName", serviceName))
        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("TraceId", traceId))
        {
            var spanId = Guid.NewGuid();
            using (LogContext.PushProperty("SpanId", spanId))
            {
                Log.Information("Calling {ServiceName}", serviceName);

                await Task.Delay(Random.Shared.Next(50, 150));

                Log.Information("{ServiceName} responded successfully", serviceName);
            }
        }
    }

    /// <summary>
    /// Demonstrates security event logging.
    /// </summary>
    public static void SecurityLoggingExample()
    {
        Console.WriteLine("\n=== Security Event Logging ===\n");

        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.100";

        // Successful authentication
        using (LogContext.PushProperty("EventType", "Authentication"))
        using (LogContext.PushProperty("IPAddress", ipAddress))
        {
            Log.Information("User {UserId} authenticated successfully", userId);
        }

        // Failed authentication attempt
        using (LogContext.PushProperty("EventType", "AuthenticationFailure"))
        using (LogContext.PushProperty("IPAddress", ipAddress))
        {
            Log.Warning("Failed authentication attempt for user {UserId}", userId);
        }

        // Authorization check
        using (LogContext.PushProperty("EventType", "Authorization"))
        using (LogContext.PushProperty("Resource", "/admin/users"))
        {
            Log.Information("User {UserId} authorized for admin access", userId);
        }

        // Suspicious activity
        using (LogContext.PushProperty("EventType", "SecurityThreat"))
        using (LogContext.PushProperty("IPAddress", ipAddress))
        using (LogContext.PushProperty("Severity", "High"))
        {
            Log.Error("Suspicious activity detected: Multiple failed login attempts from {IPAddress}", ipAddress);
        }
    }

    /// <summary>
    /// Demonstrates business event logging.
    /// </summary>
    public static void BusinessEventLoggingExample()
    {
        Console.WriteLine("\n=== Business Event Logging ===\n");

        var orderId = "ORD-12345";
        var customerId = Guid.NewGuid();
        var amount = 1299.99m;

        // Order lifecycle events
        using (LogContext.PushProperty("EventType", "OrderCreated"))
        using (LogContext.PushProperty("OrderId", orderId))
        using (LogContext.PushProperty("CustomerId", customerId))
        {
            Log.Information(
                "Order {OrderId} created by customer {CustomerId} for amount {Amount:C}",
                orderId, customerId, amount);
        }

        using (LogContext.PushProperty("EventType", "PaymentProcessed"))
        using (LogContext.PushProperty("OrderId", orderId))
        {
            Log.Information("Payment processed for order {OrderId}: {Amount:C}", orderId, amount);
        }

        using (LogContext.PushProperty("EventType", "OrderShipped"))
        using (LogContext.PushProperty("OrderId", orderId))
        using (LogContext.PushProperty("TrackingNumber", "TRACK-98765"))
        {
            Log.Information("Order {OrderId} shipped with tracking {TrackingNumber}", orderId, "TRACK-98765");
        }
    }

    /// <summary>
    /// Runs all enhanced Serilog examples.
    /// </summary>
    public static async Task RunExamples()
    {
        // Configure Serilog with enrichers
        ConfigureEnhancedSerilog();

        try
        {
            await StructuredLoggingExample();
            await PerformanceLoggingExample();
            await DistributedTracingExample();
            SecurityLoggingExample();
            BusinessEventLoggingExample();

            Console.WriteLine("\n✓ All enhanced Serilog patterns demonstrated!");
        }
        finally
        {
            // Flush and close
            await Log.CloseAndFlushAsync();
        }
    }
}
