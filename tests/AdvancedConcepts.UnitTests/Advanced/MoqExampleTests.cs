using AutoFixture;
using AutoFixture.Xunit2;
using Bogus;
using Moq;

namespace AdvancedConcepts.UnitTests.Advanced;

/// <summary>
/// Demonstrates advanced testing techniques using Moq, AutoFixture, and Bogus.
/// Silicon Valley best practice: Comprehensive mocking and test data generation.
/// </summary>
public class MoqExampleTests
{
    private readonly Fixture _fixture;
    private readonly Faker _faker;

    public MoqExampleTests()
    {
        _fixture = new Fixture();
        _faker = new Faker();
    }

    public interface IDataService
    {
        Task<string> GetDataAsync(int id);
        bool SaveData(string data);
        void LogOperation(string operation);
    }

    public class DataProcessor
    {
        private readonly IDataService _dataService;

        public DataProcessor(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<string> ProcessAsync(int id)
        {
            var data = await _dataService.GetDataAsync(id);
            var processed = data.ToUpper();
            _dataService.SaveData(processed);
            _dataService.LogOperation($"Processed: {id}");
            return processed;
        }
    }

    [Fact]
    public async Task ProcessAsync_ShouldCallGetDataAsync_WithCorrectId()
    {
        // Arrange
        var mockService = new Mock<IDataService>();
        mockService.Setup(x => x.GetDataAsync(It.IsAny<int>()))
            .ReturnsAsync("test data");
        mockService.Setup(x => x.SaveData(It.IsAny<string>()))
            .Returns(true);

        var processor = new DataProcessor(mockService.Object);

        // Act
        await processor.ProcessAsync(42);

        // Assert
        mockService.Verify(x => x.GetDataAsync(42), Times.Once);
        mockService.Verify(x => x.SaveData("TEST DATA"), Times.Once);
        mockService.Verify(x => x.LogOperation(It.Is<string>(s => s.Contains("42"))), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    public async Task ProcessAsync_ShouldHandleMultipleIds(int id)
    {
        // Arrange
        var mockService = new Mock<IDataService>();
        var testData = _faker.Lorem.Sentence();
        mockService.Setup(x => x.GetDataAsync(id)).ReturnsAsync(testData);

        var processor = new DataProcessor(mockService.Object);

        // Act
        var result = await processor.ProcessAsync(id);

        // Assert
        result.Should().Be(testData.ToUpper());
    }

    [Theory]
    [AutoData]
    public async Task ProcessAsync_WithAutoFixture_ShouldWork(int id)
    {
        // Arrange - AutoFixture generates test data automatically
        var mockService = new Mock<IDataService>();
        var testData = _fixture.Create<string>();
        mockService.Setup(x => x.GetDataAsync(id)).ReturnsAsync(testData);

        var processor = new DataProcessor(mockService.Object);

        // Act
        var result = await processor.ProcessAsync(id);

        // Assert
        result.Should().Be(testData.ToUpper());
    }

    [Fact]
    public void AutoFixture_ShouldGenerateComplexObjects()
    {
        // Arrange & Act
        var person = _fixture.Create<Person>();

        // Assert
        person.Name.Should().NotBeNullOrEmpty();
        person.Age.Should().BeGreaterThan(0);
        person.Email.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Bogus_ShouldGenerateRealisticData()
    {
        // Arrange
        var personFaker = new Faker<Person>()
            .RuleFor(p => p.Name, f => f.Name.FullName())
            .RuleFor(p => p.Age, f => f.Random.Int(18, 65))
            .RuleFor(p => p.Email, f => f.Internet.Email());

        // Act
        var people = personFaker.Generate(10);

        // Assert
        people.Should().HaveCount(10);
        people.Should().AllSatisfy(p =>
        {
            p.Name.Should().NotBeNullOrEmpty();
            p.Age.Should().BeInRange(18, 65);
            p.Email.Should().Contain("@");
        });
    }

    [Fact]
    public void Mock_ShouldVerifyMethodCallSequence()
    {
        // Arrange
        var mockService = new Mock<IDataService>(MockBehavior.Strict);
        var sequence = new MockSequence();

        mockService.InSequence(sequence).Setup(x => x.LogOperation("Start"));
        mockService.InSequence(sequence).Setup(x => x.SaveData(It.IsAny<string>())).Returns(true);
        mockService.InSequence(sequence).Setup(x => x.LogOperation("End"));

        // Act
        mockService.Object.LogOperation("Start");
        mockService.Object.SaveData("test");
        mockService.Object.LogOperation("End");

        // Assert - Verify all calls were made in sequence
        mockService.VerifyAll();
    }

    private class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
