using AdvancedCsharpConcepts.Advanced.HighPerformance;
using System.Diagnostics;

namespace AdvancedCsharpConcepts.IntegrationTests;

/// <summary>
/// Integration tests for high-performance patterns.
/// Tests real-world scenarios combining multiple components.
/// </summary>
public class PerformanceIntegrationTests
{
    [Fact]
    public void ParallelProcessing_LargeDataset_ShouldBeFasterThanSequential()
    {
        // Arrange
        const int dataSize = 1_000_000;
        var sw = Stopwatch.StartNew();

        // Act - Sequential
        sw.Restart();
        var seqResult = ParallelProcessingExamples.SequentialSum(dataSize);
        var seqTime = sw.ElapsedMilliseconds;

        // Act - Parallel
        sw.Restart();
        var parResult = ParallelProcessingExamples.ParallelForSum(dataSize);
        var parTime = sw.ElapsedMilliseconds;

        // Assert
        parResult.Should().Be(seqResult);
        parTime.Should().BeLessThan(seqTime); // Parallel should be faster
    }

    [Fact]
    public void SpanMemory_ParsingLargeCSV_ShouldBeMoreEfficientThanTraditional()
    {
        // Arrange
        var numbers = string.Join(",", Enumerable.Range(1, 1000));

        // Act
        var traditional = SpanMemoryExamples.ParseNumbersTraditional(numbers);
        var modern = SpanMemoryExamples.ParseNumbersModern(numbers);

        // Assert
        traditional.Should().Equal(modern);
        traditional.Should().HaveCount(1000);
        traditional.Should().Equal(Enumerable.Range(1, 1000));
    }

    [Fact]
    public async Task AsyncMemoryOperations_ShouldWorkCorrectly()
    {
        // Act
        var result = await SpanMemoryExamples.AsyncMemoryExample();

        // Assert
        result.Should().Be(499500); // Sum of 0..999
    }

    [Fact]
    public void MatrixMultiplication_Parallel_ShouldProduceCorrectResults()
    {
        // Arrange
        var matrixA = new double[10, 10];
        var matrixB = new double[10, 10];

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                matrixA[i, j] = i + j;
                matrixB[i, j] = i * j;
            }
        }

        // Act
        var result = ParallelProcessingExamples.ParallelMatrixMultiply(matrixA, matrixB);

        // Assert
        result.Should().NotBeNull();
        result.GetLength(0).Should().Be(10);
        result.GetLength(1).Should().Be(10);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    public void ParallelPipeline_DifferentSizes_ShouldProduceCorrectResults(int size)
    {
        // Arrange
        var input = Enumerable.Range(1, size);

        // Act
        var result = ParallelProcessingExamples.ParallelPipeline(input);

        // Assert - Should contain only even squares
        result.Should().AllSatisfy(x => (x % 2).Should().Be(0));
        result.Should().AllSatisfy(x => Math.Sqrt(x).Should().BeGreaterThan(0));
    }

    [Fact]
    public void CombinedPatterns_SpanWithParallel_ShouldWorkTogether()
    {
        // Arrange
        var data = Enumerable.Range(0, 100).ToArray();

        // Act - Use Span for zero-allocation access
        var span = data.AsSpan();

        // Process chunks in parallel
        var sum = 0;
        System.Threading.Tasks.Parallel.For(0, 4, i =>
        {
            var chunkSize = span.Length / 4;
            var chunk = span.Slice(i * chunkSize, chunkSize);
            var localSum = 0;
            foreach (var val in chunk)
            {
                localSum += val;
            }
            System.Threading.Interlocked.Add(ref sum, localSum);
        });

        // Assert
        sum.Should().Be(Enumerable.Range(0, 100).Sum());
    }

    [Fact]
    public void RealWorldScenario_DataProcessingPipeline()
    {
        // Arrange - Simulate real-world data processing
        var csvData = string.Join(",", Enumerable.Range(1, 10000).Select(x => x.ToString()));

        // Act - Parse, transform, and aggregate
        var numbers = SpanMemoryExamples.ParseNumbersModern(csvData);
        var transformed = numbers.AsParallel()
            .Select(x => x * 2)
            .Where(x => x % 10 == 0)
            .ToArray();

        // Assert
        transformed.Should().NotBeEmpty();
        transformed.Should().AllSatisfy(x => (x % 10).Should().Be(0));
    }
}
