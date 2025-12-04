using Serilog;
using Serilog.Context;
using Serilog.Events;

namespace AdvancedCsharpConcepts.Advanced.Observability;

/// <summary>
/// Structured Logging with Serilog - Production-ready logging patterns.
/// Silicon Valley best practice: Always use structured logging in production.
/// NVIDIA-style: Capture context and performance metrics in logs.
/// </summary>
public static class StructuredLogging
{
    /// <summary>
    /// Configures Serilog with production-ready settings.
    /// </summary>
    public static ILogger ConfigureLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/app-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .CreateLogger();
    }

    /// <summary>
    /// Example class demonstrating structured logging.
    /// </summary>
    public class DataProcessor
    {
        private readonly ILogger _logger;

        public DataProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public void ProcessData(string[] items)
        {
            _logger.Information("Starting data processing for {ItemCount} items", items.Length);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                for (int i = 0; i < items.Length; i++)
                {
                    ProcessItem(items[i], i);
                }

                stopwatch.Stop();

                _logger.Information(
                    "Data processing completed successfully. " +
                    "Items: {ItemCount}, Duration: {DurationMs}ms, " +
                    "Throughput: {ItemsPerSecond} items/sec",
                    items.Length,
                    stopwatch.ElapsedMilliseconds,
                    (items.Length * 1000.0) / stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Data processing failed at item {CurrentIndex}", items.Length);
                throw;
            }
        }

        private void ProcessItem(string item, int index)
        {
            _logger.Debug("Processing item {Index}: {Item}", index, item);

            // Simulate processing
            if (item.Contains("error", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warning("Suspicious item detected at {Index}: {Item}", index, item);
            }
        }

        public async Task<int> ProcessDataAsync(IEnumerable<string> items)
        {
            var itemList = items.ToList();
            _logger.Information("Starting async data processing for {ItemCount} items", itemList.Count);

            var tasks = itemList.Select((item, index) => Task.Run(() =>
            {
                using (LogContext.PushProperty("TaskId", Task.CurrentId))
                using (LogContext.PushProperty("ItemIndex", index))
                {
                    _logger.Debug("Processing item {Index} on thread {ThreadId}", index, Environment.CurrentManagedThreadId);
                    return item.Length;
                }
            }));

            var results = await Task.WhenAll(tasks);
            var totalLength = results.Sum();

            _logger.Information(
                "Async processing completed. Items: {ItemCount}, Total length: {TotalLength}",
                itemList.Count,
                totalLength);

            return totalLength;
        }
    }

    /// <summary>
    /// Demonstrates error handling with structured logging.
    /// </summary>
    public class ErrorHandlingExample
    {
        private readonly ILogger _logger;

        public ErrorHandlingExample(ILogger logger)
        {
            _logger = logger;
        }

        public void HandleApiRequest(string endpoint, Dictionary<string, string> parameters)
        {
            using (LogContext.PushProperty("Endpoint", endpoint))
            using (LogContext.PushProperty("Parameters", parameters))
            {
                _logger.Information("Processing API request to {Endpoint}", endpoint);

                try
                {
                    ValidateParameters(parameters);
                    ExecuteRequest(endpoint, parameters);
                    _logger.Information("API request completed successfully");
                }
                catch (ArgumentException ex)
                {
                    _logger.Warning(ex, "Invalid parameters for {Endpoint}", endpoint);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "API request failed for {Endpoint}", endpoint);
                    throw;
                }
            }
        }

        private void ValidateParameters(Dictionary<string, string> parameters)
        {
            if (parameters.Count == 0)
            {
                throw new ArgumentException("Parameters cannot be empty");
            }
        }

        private void ExecuteRequest(string endpoint, Dictionary<string, string> parameters)
        {
            _logger.Debug("Executing request with {ParameterCount} parameters", parameters.Count);
            // Simulate request execution
        }
    }

    /// <summary>
    /// Performance logging for benchmarks.
    /// </summary>
    public class PerformanceLogger
    {
        private readonly ILogger _logger;

        public PerformanceLogger(ILogger logger)
        {
            _logger = logger;
        }

        public T MeasureOperation<T>(string operationName, Func<T> operation)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Information("Starting operation: {OperationName}", operationName);

            try
            {
                var result = operation();
                stopwatch.Stop();

                _logger.Information(
                    "Operation completed: {OperationName}, Duration: {DurationMs}ms",
                    operationName,
                    stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error(
                    ex,
                    "Operation failed: {OperationName}, Duration: {DurationMs}ms",
                    operationName,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }

    /// <summary>
    /// Demonstrates structured logging examples.
    /// </summary>
    public static void RunExample()
    {
        Console.WriteLine("=== Structured Logging with Serilog ===\n");

        var logger = ConfigureLogger();

        try
        {
            // Basic logging
            logger.Information("Application started");

            // Data processing with metrics
            var processor = new DataProcessor(logger);
            processor.ProcessData(new[] { "item1", "item2", "error_item", "item4" });

            // Async processing
            Console.WriteLine("\nAsync Processing:");
            var asyncTask = processor.ProcessDataAsync(new[] { "async1", "async2", "async3" });
            asyncTask.Wait();
            Console.WriteLine($"Total length: {asyncTask.Result}");

            // Error handling
            Console.WriteLine("\nError Handling:");
            var errorHandler = new ErrorHandlingExample(logger);
            var validParams = new Dictionary<string, string>
            {
                ["key1"] = "value1",
                ["key2"] = "value2"
            };
            errorHandler.HandleApiRequest("/api/test", validParams);

            // Performance logging
            Console.WriteLine("\nPerformance Logging:");
            var perfLogger = new PerformanceLogger(logger);
            var result = perfLogger.MeasureOperation(
                "HeavyComputation",
                () =>
                {
                    System.Threading.Thread.Sleep(100);
                    return 42;
                });
            Console.WriteLine($"Result: {result}");

            logger.Information("Application completed successfully");
        }
        catch (Exception ex)
        {
            logger.Fatal(ex, "Application crashed");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
