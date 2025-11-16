using AdvancedCsharpConcepts.Advanced.HighPerformance;
using FluentAssertions;

namespace AdvancedCsharpConcepts.Tests.HighPerformance;

public class SpanMemoryTests
{
    [Fact]
    public void ParseNumbersTraditional_ShouldParseCorrectly()
    {
        // Arrange
        const string input = "1, 2, 3, 4, 5";

        // Act
        var result = SpanMemoryExamples.ParseNumbersTraditional(input);

        // Assert
        result.Should().Equal(1, 2, 3, 4, 5);
    }

    [Fact]
    public void ParseNumbersModern_ShouldParseCorrectly()
    {
        // Arrange
        const string input = "1, 2, 3, 4, 5";

        // Act
        var result = SpanMemoryExamples.ParseNumbersModern(input);

        // Assert
        result.Should().Equal(1, 2, 3, 4, 5);
    }

    [Fact]
    public void ParseNumbersModern_ShouldHandleWhitespace()
    {
        // Arrange
        const string input = " 10 ,  20  , 30 ";

        // Act
        var result = SpanMemoryExamples.ParseNumbersModern(input);

        // Assert
        result.Should().Equal(10, 20, 30);
    }

    [Fact]
    public async Task AsyncMemoryExample_ShouldCalculateSum()
    {
        // Act
        var result = await SpanMemoryExamples.AsyncMemoryExample();

        // Assert
        // Sum of 0..999 = n*(n-1)/2 = 1000*999/2 = 499500
        result.Should().Be(499500);
    }

    [Fact]
    public void CustomTokenizer_ShouldTokenizeCorrectly()
    {
        // Arrange
        const string input = "apple,banana,cherry";
        var expectedTokens = new[] { "apple", "banana", "cherry" };
        var actualTokens = new List<string>();

        // Act
        var tokenizer = new SpanMemoryExamples.SpanTokenizer(input.AsSpan(), ',');
        while (tokenizer.MoveNext(out var token))
        {
            actualTokens.Add(token.ToString());
        }

        // Assert
        actualTokens.Should().Equal(expectedTokens);
    }

    [Fact]
    public void CustomTokenizer_ShouldHandleEmptyInput()
    {
        // Arrange
        var input = string.Empty;
        var tokenCount = 0;

        // Act
        var tokenizer = new SpanMemoryExamples.SpanTokenizer(input.AsSpan(), ',');
        while (tokenizer.MoveNext(out _))
        {
            tokenCount++;
        }

        // Assert
        tokenCount.Should().Be(0);
    }

    [Fact]
    public void CustomTokenizer_ShouldHandleSingleToken()
    {
        // Arrange
        const string input = "single";

        // Act
        var tokenizer = new SpanMemoryExamples.SpanTokenizer(input.AsSpan(), ',');
        var hasNext = tokenizer.MoveNext(out var token);

        // Assert
        hasNext.Should().BeTrue();
        token.ToString().Should().Be("single");
        tokenizer.MoveNext(out _).Should().BeFalse();
    }
}
