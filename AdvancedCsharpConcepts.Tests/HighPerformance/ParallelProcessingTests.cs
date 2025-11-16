using AdvancedCsharpConcepts.Advanced.HighPerformance;
using FluentAssertions;

namespace AdvancedCsharpConcepts.Tests.HighPerformance;

public class ParallelProcessingTests
{
    private const int TestCount = 10000;

    [Fact]
    public void SequentialSum_ShouldCalculateCorrectly()
    {
        // Arrange
        const long expected = (long)TestCount * (TestCount - 1) / 2;

        // Act
        var result = ParallelProcessingExamples.SequentialSum(TestCount);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ParallelForSum_ShouldCalculateCorrectly()
    {
        // Arrange
        const long expected = (long)TestCount * (TestCount - 1) / 2;

        // Act
        var result = ParallelProcessingExamples.ParallelForSum(TestCount);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void PlinqSum_ShouldCalculateCorrectly()
    {
        // Arrange
        const long expected = (long)TestCount * (TestCount - 1) / 2;

        // Act
        var result = ParallelProcessingExamples.PlinqSum(TestCount);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void OptimizedParallelSum_ShouldCalculateCorrectly()
    {
        // Arrange
        const long expected = (long)TestCount * (TestCount - 1) / 2;

        // Act
        var result = ParallelProcessingExamples.OptimizedParallelSum(TestCount);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ParallelMatrixMultiply_ShouldCalculateCorrectly()
    {
        // Arrange
        var a = new double[,] { { 1, 2 }, { 3, 4 } };
        var b = new double[,] { { 5, 6 }, { 7, 8 } };

        // Expected: [1*5+2*7, 1*6+2*8]   [19, 22]
        //           [3*5+4*7, 3*6+4*8] = [43, 50]

        // Act
        var result = ParallelProcessingExamples.ParallelMatrixMultiply(a, b);

        // Assert
        result[0, 0].Should().Be(19);
        result[0, 1].Should().Be(22);
        result[1, 0].Should().Be(43);
        result[1, 1].Should().Be(50);
    }

    [Fact]
    public void ParallelMatrixMultiply_ShouldThrow_WhenDimensionsIncompatible()
    {
        // Arrange
        var a = new double[,] { { 1, 2, 3 } }; // 1x3
        var b = new double[,] { { 4, 5 } };     // 1x2 (incompatible)

        // Act
        Action act = () => ParallelProcessingExamples.ParallelMatrixMultiply(a, b);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*incompatible*");
    }

    [Fact]
    public void ParallelPipeline_ShouldFilterAndTransform()
    {
        // Arrange
        var input = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        // Squared: 1,4,9,16,25,36,49,64,81,100
        // Evens only: 4,16,36,64,100

        // Act
        var result = ParallelProcessingExamples.ParallelPipeline(input);

        // Assert
        result.Should().BeEquivalentTo(new[] { 4, 16, 36, 64, 100 });
    }

    [Fact]
    public void AllParallelMethods_ShouldProduceSameResult()
    {
        // Arrange
        const int count = 1000;

        // Act
        var sequential = ParallelProcessingExamples.SequentialSum(count);
        var parallelFor = ParallelProcessingExamples.ParallelForSum(count);
        var plinq = ParallelProcessingExamples.PlinqSum(count);
        var optimized = ParallelProcessingExamples.OptimizedParallelSum(count);

        // Assert
        parallelFor.Should().Be(sequential);
        plinq.Should().Be(sequential);
        optimized.Should().Be(sequential);
    }
}
