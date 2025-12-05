using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// Background Service Demo - Email Queue Processor
///
/// Demonstrates:
/// - BackgroundService for long-running tasks
/// - Channel T for thread-safe queue
/// - Error handling with retry logic
/// - Graceful shutdown
/// - Multiple background workers
/// </summary>

// Configure host with background services
var builder = Host.CreateApplicationBuilder(args);

// Register email queue (singleton - shared across services)
builder.Services.AddSingleton<EmailQueue>();

// Register background services
builder.Services.AddHostedService<EmailQueueWorker>();
builder.Services.AddHostedService<EmailProducer>();
builder.Services.AddHostedService<MetricsReporter>();

// Build and run
var host = builder.Build();

Console.WriteLine("=== Background Service Demo - Email Queue Processor ===\n");
Console.WriteLine("Starting workers...");
Console.WriteLine("Press Ctrl+C to stop gracefully\n");

await host.RunAsync();

Console.WriteLine("\n=== Application stopped gracefully ===");

// ============================================================================
// Email Models
// ============================================================================

/// <summary>
/// Represents an email to be sent
/// </summary>
public record EmailMessage(
    string To,
    string Subject,
    string Body,
    int Priority = 5,
    DateTime QueuedAt = default)
{
    public DateTime QueuedAt { get; init; } = QueuedAt == default ? DateTime.UtcNow : QueuedAt;
    public int RetryCount { get; init; }
}

/// <summary>
/// Result of sending an email
/// </summary>
public record EmailResult(
    bool Success,
    string? ErrorMessage = null);

// ============================================================================
// Email Queue (Thread-Safe with Channel<T>)
// ============================================================================

/// <summary>
/// Thread-safe email queue using System.Threading.Channels
/// Provides bounded capacity and backpressure handling
/// </summary>
public class EmailQueue
{
    private readonly Channel<EmailMessage> _channel;
    private int _totalQueued;
    private int _totalProcessed;
    private int _totalFailed;

    public EmailQueue()
    {
        // Bounded channel with capacity of 100
        // When full, Wait for space to be available
        var options = new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        _channel = Channel.CreateBounded<EmailMessage>(options);
    }

    /// <summary>
    /// Enqueue an email for processing
    /// </summary>
    public async Task<bool> EnqueueAsync(EmailMessage email, CancellationToken cancellationToken = default)
    {
        try
        {
            await _channel.Writer.WriteAsync(email, cancellationToken);
            Interlocked.Increment(ref _totalQueued);
            return true;
        }
        catch (ChannelClosedException)
        {
            return false;
        }
    }

    /// <summary>
    /// Dequeue an email for processing
    /// </summary>
    public async ValueTask<EmailMessage?> DequeueAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _channel.Reader.ReadAsync(cancellationToken);
        }
        catch (ChannelClosedException)
        {
            return null;
        }
    }

    /// <summary>
    /// Mark email as processed successfully
    /// </summary>
    public void MarkProcessed()
    {
        Interlocked.Increment(ref _totalProcessed);
    }

    /// <summary>
    /// Mark email as failed
    /// </summary>
    public void MarkFailed()
    {
        Interlocked.Increment(ref _totalFailed);
    }

    /// <summary>
    /// Get queue metrics
    /// </summary>
    public (int Queued, int Processed, int Failed, int Pending) GetMetrics()
    {
        var queued = Interlocked.CompareExchange(ref _totalQueued, 0, 0);
        var processed = Interlocked.CompareExchange(ref _totalProcessed, 0, 0);
        var failed = Interlocked.CompareExchange(ref _totalFailed, 0, 0);
        var pending = _channel.Reader.Count;

        return (queued, processed, failed, pending);
    }

    /// <summary>
    /// Signal queue completion (no more items will be added)
    /// </summary>
    public void CompleteAdding()
    {
        _channel.Writer.Complete();
    }
}

// ============================================================================
// Email Service (Simulates SMTP)
// ============================================================================

/// <summary>
/// Email service that simulates sending emails via SMTP
/// </summary>
public class EmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly Random _random = new();

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Send email (simulated with 10% failure rate)
    /// </summary>
    public async Task<EmailResult> SendAsync(EmailMessage email, CancellationToken cancellationToken)
    {
        try
        {
            // Simulate network latency (100-500ms)
            var delay = _random.Next(100, 500);
            await Task.Delay(delay, cancellationToken);

            // Simulate 10% failure rate
            if (_random.Next(100) < 10)
            {
                return new EmailResult(false, "SMTP connection timeout");
            }

            _logger.LogInformation(
                "✅ Email sent to {To} | Subject: {Subject} | Priority: {Priority} | Latency: {Latency}ms",
                email.To,
                email.Subject,
                email.Priority,
                delay);

            return new EmailResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", email.To);
            return new EmailResult(false, ex.Message);
        }
    }
}

// ============================================================================
// Background Workers
// ============================================================================

/// <summary>
/// Background service that processes emails from the queue
/// Implements retry logic and error handling
/// </summary>
public class EmailQueueWorker : BackgroundService
{
    private readonly EmailQueue _queue;
    private readonly ILogger<EmailQueueWorker> _logger;
    private const int MaxRetries = 3;

    public EmailQueueWorker(EmailQueue queue, ILogger<EmailQueueWorker> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("📧 Email Queue Worker started");

        // Create EmailService with appropriate logger type
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var emailLogger = loggerFactory.CreateLogger<EmailService>();
        var emailService = new EmailService(emailLogger);

        try
        {
            // Process emails until cancellation requested
            while (!stoppingToken.IsCancellationRequested)
            {
                var email = await _queue.DequeueAsync(stoppingToken);

                if (email is null)
                {
                    _logger.LogInformation("Queue closed, worker exiting...");
                    break;
                }

                await ProcessEmailAsync(email, emailService, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Worker cancellation requested");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Worker encountered fatal error");
            throw;
        }

        _logger.LogInformation("📧 Email Queue Worker stopped");
    }

    /// <summary>
    /// Process a single email with retry logic
    /// </summary>
    private async Task ProcessEmailAsync(
        EmailMessage email,
        EmailService emailService,
        CancellationToken cancellationToken)
    {
        var result = await emailService.SendAsync(email, cancellationToken);

        if (result.Success)
        {
            _queue.MarkProcessed();
        }
        else
        {
            _logger.LogWarning(
                "⚠️  Email failed to {To} | Error: {Error} | Retry: {Retry}/{Max}",
                email.To,
                result.ErrorMessage,
                email.RetryCount,
                MaxRetries);

            // Retry logic
            if (email.RetryCount < MaxRetries)
            {
                // Exponential backoff: 1s, 2s, 4s
                var backoffDelay = TimeSpan.FromSeconds(Math.Pow(2, email.RetryCount));
                await Task.Delay(backoffDelay, cancellationToken);

                // Re-queue with incremented retry count
                var retryEmail = email with { RetryCount = email.RetryCount + 1 };
                await _queue.EnqueueAsync(retryEmail, cancellationToken);
            }
            else
            {
                _logger.LogError(
                    "❌ Email permanently failed to {To} after {MaxRetries} retries",
                    email.To,
                    MaxRetries);

                _queue.MarkFailed();
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping worker gracefully...");
        await base.StopAsync(cancellationToken);
    }
}

/// <summary>
/// Background service that produces test emails
/// Simulates incoming email requests
/// </summary>
public class EmailProducer : BackgroundService
{
    private readonly EmailQueue _queue;
    private readonly ILogger<EmailProducer> _logger;
    private readonly Random _random = new();
    private int _emailCounter;

    private static readonly string[] Recipients =
    {
        "alice@example.com",
        "bob@example.com",
        "charlie@example.com",
        "diana@example.com",
        "eve@example.com"
    };

    private static readonly string[] Subjects =
    {
        "Weekly Newsletter",
        "Password Reset Request",
        "Order Confirmation",
        "Account Notification",
        "System Alert"
    };

    public EmailProducer(EmailQueue queue, ILogger<EmailProducer> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("📨 Email Producer started");

        // Wait 2 seconds before starting production
        await Task.Delay(2000, stoppingToken);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Produce emails at varying rates (1-3 emails every 1-3 seconds)
                var count = _random.Next(1, 4);
                for (int i = 0; i < count; i++)
                {
                    var email = GenerateTestEmail();
                    var queued = await _queue.EnqueueAsync(email, stoppingToken);

                    if (queued)
                    {
                        _logger.LogInformation(
                            "📨 Queued email #{Number} to {To} | Priority: {Priority}",
                            _emailCounter,
                            email.To,
                            email.Priority);
                    }
                }

                // Wait before next batch
                var delay = _random.Next(1000, 3000);
                await Task.Delay(delay, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Producer cancellation requested");
        }

        // Signal queue that no more emails will be added
        _queue.CompleteAdding();
        _logger.LogInformation("📨 Email Producer stopped");
    }

    private EmailMessage GenerateTestEmail()
    {
        var to = Recipients[_random.Next(Recipients.Length)];
        var subject = Subjects[_random.Next(Subjects.Length)];
        var priority = _random.Next(1, 11); // 1-10
        var number = Interlocked.Increment(ref _emailCounter);

        return new EmailMessage(
            To: to,
            Subject: $"{subject} #{number}",
            Body: $"This is test email #{number}",
            Priority: priority);
    }
}

/// <summary>
/// Background service that reports queue metrics periodically
/// </summary>
public class MetricsReporter : BackgroundService
{
    private readonly EmailQueue _queue;
    private readonly ILogger<MetricsReporter> _logger;

    public MetricsReporter(EmailQueue queue, ILogger<MetricsReporter> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("📊 Metrics Reporter started");

        // Wait 5 seconds before first report
        await Task.Delay(5000, stoppingToken);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var (queued, processed, failed, pending) = _queue.GetMetrics();

                var successRate = queued > 0
                    ? (processed * 100.0 / queued)
                    : 0;

                _logger.LogInformation(
                    "📊 METRICS | Queued: {Queued} | Processed: {Processed} | Failed: {Failed} | Pending: {Pending} | Success Rate: {SuccessRate:F1}%",
                    queued,
                    processed,
                    failed,
                    pending,
                    successRate);

                // Report every 10 seconds
                await Task.Delay(10000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Final metrics report
            var (queued, processed, failed, pending) = _queue.GetMetrics();
            _logger.LogInformation(
                "📊 FINAL METRICS | Queued: {Queued} | Processed: {Processed} | Failed: {Failed} | Pending: {Pending}",
                queued,
                processed,
                failed,
                pending);
        }

        _logger.LogInformation("📊 Metrics Reporter stopped");
    }
}
