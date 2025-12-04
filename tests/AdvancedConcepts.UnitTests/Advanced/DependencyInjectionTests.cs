using AdvancedConcepts.Core.Advanced.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdvancedConcepts.UnitTests.Advanced;

/// <summary>
/// Tests for Dependency Injection patterns.
/// Validates DI configuration and service lifetimes.
/// </summary>
public class DependencyInjectionTests
{
    [Fact]
    public void ServiceCollection_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddLogging();
        services.AddSingleton<DIExample.IDataRepository, DIExample.InMemoryDataRepository>();
        services.AddTransient<DIExample.IDataProcessor, DIExample.DataProcessor>();

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var repository = serviceProvider.GetService<DIExample.IDataRepository>();
        var processor = serviceProvider.GetService<DIExample.IDataProcessor>();

        repository.Should().NotBeNull();
        processor.Should().NotBeNull();
        repository.Should().BeOfType<DIExample.InMemoryDataRepository>();
        processor.Should().BeOfType<DIExample.DataProcessor>();
    }

    [Fact]
    public void Singleton_ShouldReturnSameInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<DIExample.IDataRepository, DIExample.InMemoryDataRepository>();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var instance1 = serviceProvider.GetService<DIExample.IDataRepository>();
        var instance2 = serviceProvider.GetService<DIExample.IDataRepository>();

        // Assert
        instance1.Should().BeSameAs(instance2);
    }

    [Fact]
    public void Transient_ShouldReturnDifferentInstances()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddTransient<DIExample.IDataProcessor, DIExample.DataProcessor>();

        var mockRepo = new Mock<DIExample.IDataRepository>();
        services.AddSingleton(mockRepo.Object);

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var instance1 = serviceProvider.GetService<DIExample.IDataProcessor>();
        var instance2 = serviceProvider.GetService<DIExample.IDataProcessor>();

        // Assert
        instance1.Should().NotBeSameAs(instance2);
    }

    [Fact]
    public async Task DataProcessor_ShouldUseInjectedRepository()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<DIExample.DataProcessor>>();
        var mockRepository = new Mock<DIExample.IDataRepository>();
        var testData = new[] { "data1", "data2", "data3" };
        mockRepository.Setup(x => x.GetDataAsync()).ReturnsAsync(testData);

        var processor = new DIExample.DataProcessor(mockLogger.Object, mockRepository.Object);

        // Act
        var result = await processor.ProcessDataAsync(testData);

        // Assert
        result.Should().BeGreaterThan(0);
        mockRepository.Verify(x => x.SaveDataAsync(It.IsAny<string[]>()), Times.Once);
    }

    [Fact]
    public void InMemoryDataRepository_ShouldStoreAndRetrieveData()
    {
        // Arrange
        var repository = new DIExample.InMemoryDataRepository();
        var testData = new[] { "test1", "test2" };

        // Act
        var saveTask = repository.SaveDataAsync(testData);
        saveTask.Wait();

        var retrieveTask = repository.GetDataAsync();
        retrieveTask.Wait();
        var result = retrieveTask.Result;

        // Assert
        result.Should().BeEquivalentTo(testData);
    }

    [Fact]
    public void NotificationService_ShouldNotThrow()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<DIExample.ConsoleNotificationService>>();
        var service = new DIExample.ConsoleNotificationService(mockLogger.Object);

        // Act & Assert
        service.Invoking(s => s.Notify("Test message"))
            .Should().NotThrow();
    }

    [Fact]
    public void Constructor_ShouldThrowWhenLoggerIsNull()
    {
        // Arrange
        var mockRepo = new Mock<DIExample.IDataRepository>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new DIExample.DataProcessor(null!, mockRepo.Object));
    }

    [Fact]
    public void Constructor_ShouldThrowWhenRepositoryIsNull()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<DIExample.DataProcessor>>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new DIExample.DataProcessor(mockLogger.Object, null!));
    }
}
