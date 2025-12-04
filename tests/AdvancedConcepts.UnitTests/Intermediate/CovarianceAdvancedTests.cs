using Xunit;
using FluentAssertions;
using AdvancedCsharpConcepts.Intermediate.CovarianceContravariance;

namespace AdvancedConcepts.UnitTests.Intermediate;

/// <summary>
/// Advanced tests for covariance and contravariance
/// </summary>
public class CovarianceAdvancedTests
{
    [Fact]
    public void Covariance_IEnumerableOfDerivedType_CanBeAssignedToBase()
    {
        // Arrange
        IEnumerable<string> strings = new List<string> { "a", "b" };

        // Act - Covariance allows this
        IEnumerable<object> objects = strings;

        // Assert
        objects.Should().HaveCount(2);
    }

    [Fact]
    public void Covariance_ArrayOfDerivedType_CanBeAssignedToBase()
    {
        // Arrange
        string[] strings = { "hello", "world" };

        // Act
        object[] objects = strings;

        // Assert
        objects.Should().HaveCount(2);
        objects[0].Should().Be("hello");
    }

    [Fact]
    public void Covariance_FuncReturnType_AllowsCovariantReturn()
    {
        // Arrange
        Func<string> getString = () => "test";

        // Act - Covariance on return type
        Func<object> getObject = getString;
        object result = getObject();

        // Assert
        result.Should().Be("test");
    }

    [Fact]
    public void Contravariance_ActionParameter_AllowsContravariantParameter()
    {
        // Arrange
        Action<object> processObject = obj => { };

        // Act - Contravariance on parameter
        Action<string> processString = processObject;

        // Assert
        processString.Should().NotBeNull();
    }

    [Fact]
    public async Task Covariance_TaskReturnType_WorksWithAsyncAwait()
    {
        // Arrange
        Task<string> taskString = Task.FromResult("test");

        // Act - Use async/await instead of direct assignment
        object result = await taskString;

        // Assert
        result.Should().Be("test");
    }

    [Fact]
    public void Covariance_IQueryable_IsCovariant()
    {
        // Arrange
        IQueryable<string> strings = new List<string> { "a" }.AsQueryable();

        // Act
        IQueryable<object> objects = strings;

        // Assert
        objects.Should().HaveCount(1);
    }

    [Fact]
    public void Contravariance_IComparer_IsContravariant()
    {
        // Arrange
        IComparer<object> objectComparer = Comparer<object>.Default;

        // Act
        IComparer<string> stringComparer = objectComparer;

        // Assert
        stringComparer.Should().NotBeNull();
    }

    [Fact]
    public void Covariance_OutKeyword_EnablesCovariance()
    {
        // Arrange
        ICovariantInterface<string> stringInterface = new CovariantImpl<string>();

        // Act
        ICovariantInterface<object> objectInterface = stringInterface;

        // Assert
        objectInterface.Should().NotBeNull();
    }

    [Fact]
    public void Contravariance_InKeyword_EnablesContravariance()
    {
        // Arrange
        IContravariantInterface<object> objectInterface = new ContravariantImpl<object>();

        // Act
        IContravariantInterface<string> stringInterface = objectInterface;

        // Assert
        stringInterface.Should().NotBeNull();
    }

    [Fact]
    public void Covariance_Delegates_FuncIsCovariant()
    {
        // Arrange
        Func<List<string>> getList = () => new List<string>();

        // Act
        Func<IEnumerable<string>> getEnumerable = getList;
        Func<IEnumerable<object>> getObjects = getEnumerable;

        // Assert
        getObjects().Should().NotBeNull();
    }

    // Helper interfaces and classes for testing
    private interface ICovariantInterface<out T>
    {
        T Get();
    }

    private class CovariantImpl<T> : ICovariantInterface<T>
    {
        public T Get() => default!;
    }

    private interface IContravariantInterface<in T>
    {
        void Set(T value);
    }

    private class ContravariantImpl<T> : IContravariantInterface<T>
    {
        public void Set(T value) { }
    }
}
