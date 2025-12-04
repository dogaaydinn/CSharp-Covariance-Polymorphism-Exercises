using Xunit;
using FluentAssertions;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;
using AdvancedCsharpConcepts.Advanced.Observability;
using System.Diagnostics;

namespace AdvancedConcepts.Tests;

/// <summary>
/// Tests for Observability patterns including Serilog, OpenTelemetry, and Health Checks
/// </summary>
public class ObservabilityTests
{
    #region Structured Logging Tests

    [Fact]
    public void StructuredLogging_ConfigureLogger_ShouldCreateLogger()
    {
        // Act
        var logger = StructuredLogging.ConfigureLogger();

        // Assert
        logger.Should().NotBeNull();
        logger.Should().BeAssignableTo<ILogger>();
    }

    [Fact]
    public void StructuredLogging_DataProcessor_ShouldLogInformation()
    {
        // Arrange
        using (TestCorrelator.CreateContext())
        {
            var logger = new LoggerConfiguration()
                .WriteTo.TestCorrelator()
                .CreateLogger();

            var processor = new StructuredLogging.DataProcessor(logger);
            var items = new[] { "item1", "item2", "item3" };

            // Act
            processor.ProcessData(items);

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            logEvents.Should().NotBeEmpty();
            logEvents.Should().Contain(e => e.Level == LogEventLevel.Information);
            logEvents.Should().Contain(e => e.MessageTemplate.Text.Contains("Starting data processing"));
        }
    }

    [Fact]
    public void StructuredLogging_DataProcessor_ShouldCaptureItemCount()
    {
        // Arrange
        using (TestCorrelator.CreateContext())
        {
            var logger = new LoggerConfiguration()
                .WriteTo.TestCorrelator()
                .CreateLogger();

            var processor = new StructuredLogging.DataProcessor(logger);
            var items = new[] { "test1", "test2", "test3", "test4", "test5" };

            // Act
            processor.ProcessData(items);

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            var startEvent = logEvents.FirstOrDefault(e =>
                e.MessageTemplate.Text.Contains("Starting data processing"));

            startEvent.Should().NotBeNull();
            startEvent!.Properties.Should().ContainKey("ItemCount");
            startEvent.Properties["ItemCount"].ToString().Should().Contain("5");
        }
    }

    [Fact]
    public void StructuredLogging_DataProcessor_ShouldLogWarningForSuspiciousItems()
    {
        // Arrange
        using (TestCorrelator.CreateContext())
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.TestCorrelator()
                .CreateLogger();

            var processor = new StructuredLogging.DataProcessor(logger);
            var items = new[] { "normal", "error-item", "another" };

            // Act
            processor.ProcessData(items);

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            var warningEvents = logEvents.Where(e => e.Level == LogEventLevel.Warning).ToList();

            warningEvents.Should().NotBeEmpty();
            warningEvents.Should().Contain(e => e.MessageTemplate.Text.Contains("Suspicious item detected"));
        }
    }

    [Fact]
    public async Task StructuredLogging_DataProcessor_ShouldProcessDataAsync()
    {
        // Arrange
        using (TestCorrelator.CreateContext())
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.TestCorrelator()
                .CreateLogger();

            var processor = new StructuredLogging.DataProcessor(logger);
            var items = new[] { "async1", "async2", "async3" };

            // Act
            var result = await processor.ProcessDataAsync(items);

            // Assert
            result.Should().Be(items.Sum(i => i.Length));

            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            logEvents.Should().Contain(e => e.MessageTemplate.Text.Contains("Starting async data processing"));
        }
    }

    [Fact]
    public void StructuredLogging_DataProcessor_ShouldCapturePerformanceMetrics()
    {
        // Arrange
        using (TestCorrelator.CreateContext())
        {
            var logger = new LoggerConfiguration()
                .WriteTo.TestCorrelator()
                .CreateLogger();

            var processor = new StructuredLogging.DataProcessor(logger);
            var items = Enumerable.Range(1, 10).Select(i => $"item{i}").ToArray();

            // Act
            processor.ProcessData(items);

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            var completionEvent = logEvents.FirstOrDefault(e =>
                e.MessageTemplate.Text.Contains("Data processing completed"));

            completionEvent.Should().NotBeNull();
            completionEvent!.Properties.Should().ContainKey("DurationMs");
            completionEvent.Properties.Should().ContainKey("ItemsPerSecond");
        }
    }

    #endregion

    #region LogContext and Enrichment Tests

    [Fact]
    public void StructuredLogging_ShouldEnrichLogsWithContext()
    {
        // Arrange
        using (TestCorrelator.CreateContext())
        {
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.TestCorrelator()
                .CreateLogger();

            // Act
            using (Serilog.Context.LogContext.PushProperty("UserId", "user123"))
            using (Serilog.Context.LogContext.PushProperty("RequestId", "req456"))
            {
                logger.Information("Processing user request");
            }

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            var requestEvent = logEvents.FirstOrDefault();

            requestEvent.Should().NotBeNull();
            requestEvent!.Properties.Should().ContainKey("UserId");
            requestEvent.Properties.Should().ContainKey("RequestId");
            requestEvent.Properties["UserId"].ToString().Should().Contain("user123");
        }
    }

    [Fact]
    public void StructuredLogging_ShouldLogAtDifferentLevels()
    {
        // Arrange
        using (TestCorrelator.CreateContext())
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestCorrelator()
                .CreateLogger();

            // Act
            logger.Verbose("Verbose message");
            logger.Debug("Debug message");
            logger.Information("Information message");
            logger.Warning("Warning message");
            logger.Error("Error message");

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            logEvents.Should().HaveCount(5);
            logEvents.Should().Contain(e => e.Level == LogEventLevel.Verbose);
            logEvents.Should().Contain(e => e.Level == LogEventLevel.Debug);
            logEvents.Should().Contain(e => e.Level == LogEventLevel.Information);
            logEvents.Should().Contain(e => e.Level == LogEventLevel.Warning);
            logEvents.Should().Contain(e => e.Level == LogEventLevel.Error);
        }
    }

    [Fact]
    public void StructuredLogging_ShouldCaptureExceptionDetails()
    {
        // Arrange
        using (TestCorrelator.CreateContext())
        {
            var logger = new LoggerConfiguration()
                .WriteTo.TestCorrelator()
                .CreateLogger();

            var exception = new InvalidOperationException("Test exception");

            // Act
            logger.Error(exception, "An error occurred while processing {Operation}", "TestOperation");

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            var errorEvent = logEvents.FirstOrDefault();

            errorEvent.Should().NotBeNull();
            errorEvent!.Level.Should().Be(LogEventLevel.Error);
            errorEvent.Exception.Should().NotBeNull();
            errorEvent.Exception!.Message.Should().Be("Test exception");
            errorEvent.Properties.Should().ContainKey("Operation");
        }
    }

    #endregion

    #region Telemetry and Metrics Tests

    [Fact]
    public void Activity_ShouldTrackOperation()
    {
        // Arrange
        using var activitySource = new ActivitySource("TestSource");
        using var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded
        };
        ActivitySource.AddActivityListener(listener);

        // Act
        using var activity = activitySource.StartActivity("TestOperation");
        activity?.SetTag("UserId", "123");
        activity?.SetTag("Operation", "DataProcessing");

        // Assert
        activity.Should().NotBeNull();
        activity!.Tags.Should().Contain(t => t.Key == "UserId" && t.Value == "123");
        activity.Tags.Should().Contain(t => t.Key == "Operation" && t.Value == "DataProcessing");
    }

    [Fact]
    public void Activity_ShouldNestActivities()
    {
        // Arrange
        using var activitySource = new ActivitySource("TestSource");
        using var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded
        };
        ActivitySource.AddActivityListener(listener);

        // Act
        using var parentActivity = activitySource.StartActivity("ParentOperation");
        var parentId = parentActivity?.Id;

        using var childActivity = activitySource.StartActivity("ChildOperation");
        var childParentId = childActivity?.ParentId;

        // Assert
        parentActivity.Should().NotBeNull();
        childActivity.Should().NotBeNull();
        childParentId.Should().Be(parentId);
    }

    [Fact]
    public void Activity_ShouldMeasureDuration()
    {
        // Arrange
        using var activitySource = new ActivitySource("TestSource");
        using var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded
        };
        ActivitySource.AddActivityListener(listener);

        // Act
        TimeSpan? duration;
        using (var activity = activitySource.StartActivity("TestOperation"))
        {
            Thread.Sleep(50); // Simulate work
            activity!.Stop();
            duration = activity.Duration;
        }

        // Assert
        duration.Should().NotBeNull();
        duration.Value.TotalMilliseconds.Should().BeGreaterThanOrEqualTo(40); // Allow for timing variance
    }

    #endregion

    #region Performance and Correlation Tests

    [Fact]
    public void StructuredLogging_ShouldHandleHighVolumeLogging()
    {
        // Arrange
        using (TestCorrelator.CreateContext())
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.TestCorrelator()
                .CreateLogger();

            var itemCount = 1000;
            var items = Enumerable.Range(1, itemCount).Select(i => $"item{i}").ToArray();

            var processor = new StructuredLogging.DataProcessor(logger);

            // Act
            var stopwatch = Stopwatch.StartNew();
            processor.ProcessData(items);
            stopwatch.Stop();

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            logEvents.Should().NotBeEmpty();

            // Performance check: should complete in reasonable time
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000);
        }
    }

    [Fact]
    public async Task StructuredLogging_ShouldCorrelateAsyncOperations()
    {
        // Arrange
        using (TestCorrelator.CreateContext())
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.TestCorrelator()
                .CreateLogger();

            var processor = new StructuredLogging.DataProcessor(logger);
            var items = new[] { "async1", "async2", "async3", "async4", "async5" };

            // Act
            await processor.ProcessDataAsync(items);

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            var itemProcessingEvents = logEvents.Where(e =>
                e.MessageTemplate.Text.Contains("Processing item") &&
                e.Properties.ContainsKey("ItemIndex")).ToList();

            itemProcessingEvents.Should().NotBeEmpty();
            // Verify that all items were logged
            itemProcessingEvents.Should().HaveCountGreaterThanOrEqualTo(items.Length);
        }
    }

    #endregion

    #region Helper Methods

    [Theory]
    [InlineData(LogEventLevel.Verbose)]
    [InlineData(LogEventLevel.Debug)]
    [InlineData(LogEventLevel.Information)]
    [InlineData(LogEventLevel.Warning)]
    [InlineData(LogEventLevel.Error)]
    [InlineData(LogEventLevel.Fatal)]
    public void StructuredLogging_ShouldRespectMinimumLevel(LogEventLevel minimumLevel)
    {
        // Arrange
        using (TestCorrelator.CreateContext())
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Is(minimumLevel)
                .WriteTo.TestCorrelator()
                .CreateLogger();

            // Act
            logger.Verbose("Verbose");
            logger.Debug("Debug");
            logger.Information("Information");
            logger.Warning("Warning");
            logger.Error("Error");
            logger.Fatal("Fatal");

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            logEvents.Should().OnlyContain(e => e.Level >= minimumLevel);
        }
    }

    [Fact]
    public void StructuredLogging_ShouldFormatStructuredProperties()
    {
        // Arrange
        using (TestCorrelator.CreateContext())
        {
            var logger = new LoggerConfiguration()
                .WriteTo.TestCorrelator()
                .CreateLogger();

            var userData = new
            {
                UserId = 123,
                UserName = "TestUser",
                Email = "test@example.com"
            };

            // Act
            logger.Information("User logged in: {@User}", userData);

            // Assert
            var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
            var loginEvent = logEvents.FirstOrDefault();

            loginEvent.Should().NotBeNull();
            loginEvent!.Properties.Should().ContainKey("User");
        }
    }

    #endregion
}
