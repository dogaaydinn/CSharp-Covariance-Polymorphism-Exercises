using Microsoft.Extensions.Logging;

namespace AdvancedConcepts.Core.Advanced.DependencyInjection;

/// <summary>
/// Dependency Injection - Modern .NET dependency management.
/// Silicon Valley best practice: Inversion of Control (IoC) for testability.
/// </summary>
public static class DIExample
{
    // Interfaces (Abstractions)
    public interface IDataRepository
    {
        Task<string[]> GetDataAsync();
        Task SaveDataAsync(string[] data);
    }

    public interface IDataProcessor
    {
        Task<int> ProcessDataAsync(string[] data);
    }

    public interface INotificationService
    {
        void Notify(string message);
    }

    // Implementations (Concrete classes)
    public class InMemoryDataRepository : IDataRepository
    {
        private readonly List<string> _data = new();

        public Task<string[]> GetDataAsync()
        {
            return Task.FromResult(_data.ToArray());
        }

        public Task SaveDataAsync(string[] data)
        {
            _data.Clear();
            _data.AddRange(data);
            return Task.CompletedTask;
        }
    }

    public class DataProcessor : IDataProcessor
    {
        private readonly ILogger<DataProcessor> _logger;
        private readonly IDataRepository _repository;

        public DataProcessor(ILogger<DataProcessor> logger, IDataRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<int> ProcessDataAsync(string[] data)
        {
            _logger.LogInformation("Processing {Count} items", data.Length);

            await _repository.SaveDataAsync(data);

            var totalLength = data.Sum(x => x.Length);
            _logger.LogInformation("Processed {TotalLength} characters", totalLength);

            return totalLength;
        }
    }

    public class ConsoleNotificationService : INotificationService
    {
        private readonly ILogger<ConsoleNotificationService> _logger;

        public ConsoleNotificationService(ILogger<ConsoleNotificationService> logger)
        {
            _logger = logger;
        }

        public void Notify(string message)
        {
            _logger.LogInformation("Notification: {Message}", message);
            Console.WriteLine($"[NOTIFICATION] {message}");
        }
    }

    // Application Service (orchestrates dependencies)
    public class ApplicationService
    {
        private readonly IDataProcessor _processor;
        private readonly IDataRepository _repository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ApplicationService> _logger;

        public ApplicationService(
            IDataProcessor processor,
            IDataRepository repository,
            INotificationService notificationService,
            ILogger<ApplicationService> logger)
        {
            _processor = processor;
            _repository = repository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("Application started");

            try
            {
                // Process new data
                var newData = new[] { "Item1", "Item2", "Item3", "Item4", "Item5" };
                var result = await _processor.ProcessDataAsync(newData);

                _notificationService.Notify($"Processed {newData.Length} items, total {result} characters");

                // Retrieve and display data
                var storedData = await _repository.GetDataAsync();
                _logger.LogInformation("Retrieved {Count} items from repository", storedData.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Application error occurred");
                _notificationService.Notify("An error occurred during processing");
                throw;
            }
        }
    }

    /// <summary>
    /// Service lifetimes demonstration.
    /// </summary>
    public interface ICounterService
    {
        int GetNextId();
    }

    public class CounterService : ICounterService
    {
        private int _counter;

        public int GetNextId() => ++_counter;
    }

    /// <summary>
    /// Factory pattern with DI.
    /// </summary>
    public interface IProcessorFactory
    {
        IDataProcessor CreateProcessor(string type);
    }

    public class ProcessorFactory : IProcessorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ProcessorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDataProcessor CreateProcessor(string type)
        {
            // Use service provider to resolve with all dependencies
            return type.ToLower() switch
            {
                "default" => _serviceProvider.GetRequiredService<IDataProcessor>(),
                _ => throw new ArgumentException($"Unknown processor type: {type}")
            };
        }
    }

    /// <summary>
    /// Configures and demonstrates dependency injection.
    /// </summary>
    public static void RunExample()
    {
        Console.WriteLine("=== Dependency Injection Examples ===\n");

        // 1. Configure services
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Register services with different lifetimes
        services.AddSingleton<IDataRepository, InMemoryDataRepository>();    // Singleton
        services.AddTransient<IDataProcessor, DataProcessor>();               // Transient
        services.AddScoped<INotificationService, ConsoleNotificationService>(); // Scoped

        // Register application service
        services.AddTransient<ApplicationService>();

        // Register factory
        services.AddSingleton<IProcessorFactory, ProcessorFactory>();

        // Register counter to demonstrate lifetimes
        services.AddSingleton<ICounterService, CounterService>();

        // 2. Build service provider
        var serviceProvider = services.BuildServiceProvider();

        try
        {
            // 3. Resolve and run application
            Console.WriteLine("1. Running Application Service:");
            var app = serviceProvider.GetRequiredService<ApplicationService>();
            app.RunAsync().Wait();

            // 4. Demonstrate service lifetimes
            Console.WriteLine("\n2. Demonstrating Service Lifetimes:");
            DemonstrateLifetimes(serviceProvider);

            // 5. Demonstrate factory pattern
            Console.WriteLine("\n3. Demonstrating Factory Pattern:");
            var factory = serviceProvider.GetRequiredService<IProcessorFactory>();
            var processor = factory.CreateProcessor("default");
            var result = processor.ProcessDataAsync(new[] { "Test1", "Test2" }).Result;
            Console.WriteLine($"Factory-created processor result: {result}");
        }
        finally
        {
            // 6. Dispose service provider
            if (serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    private static void DemonstrateLifetimes(IServiceProvider serviceProvider)
    {
        // Singleton - same instance every time
        var counter1 = serviceProvider.GetRequiredService<ICounterService>();
        var counter2 = serviceProvider.GetRequiredService<ICounterService>();

        Console.WriteLine($"Singleton Counter 1: {counter1.GetNextId()}");
        Console.WriteLine($"Singleton Counter 2: {counter2.GetNextId()}"); // Continues from 1
        Console.WriteLine($"Same instance? {ReferenceEquals(counter1, counter2)}");

        // Transient - new instance every time
        var proc1 = serviceProvider.GetRequiredService<IDataProcessor>();
        var proc2 = serviceProvider.GetRequiredService<IDataProcessor>();
        Console.WriteLine($"Transient: Same instance? {ReferenceEquals(proc1, proc2)}"); // False
    }
}
