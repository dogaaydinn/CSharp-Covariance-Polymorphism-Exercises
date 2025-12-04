using AdvancedCsharpConcepts.Advanced.ModernCSharp;
using FluentAssertions;

namespace AdvancedCsharpConcepts.Tests.ModernCSharp;

public class PatternMatchingTests
{
    [Theory]
    [InlineData(10.0, 314.16)] // Small circle
    [InlineData(15.0, 706.86)] // Large circle
    public void CalculateArea_Circle_ShouldReturnCorrectArea(double radius, double expectedArea)
    {
        // Arrange
        var circle = new AdvancedPatternMatching.Circle(radius);

        // Act
        var area = AdvancedPatternMatching.CalculateArea(circle);

        // Assert
        area.Should().BeApproximately(expectedArea, 0.01);
    }

    [Theory]
    [InlineData(5.0, 5.0, 25.0)]
    [InlineData(10.0, 20.0, 200.0)]
    public void CalculateArea_Rectangle_ShouldReturnCorrectArea(double width, double height, double expectedArea)
    {
        // Arrange
        var rectangle = new AdvancedPatternMatching.Rectangle(width, height);

        // Act
        var area = AdvancedPatternMatching.CalculateArea(rectangle);

        // Assert
        area.Should().Be(expectedArea);
    }

    [Fact]
    public void ClassifyShape_Square_ShouldReturnSquare()
    {
        // Arrange
        var square = new AdvancedPatternMatching.Rectangle(10, 10);

        // Act
        var classification = AdvancedPatternMatching.ClassifyShape(square);

        // Assert
        classification.Should().Be("Square");
    }

    [Theory]
    [InlineData(-1, "Invalid age")]
    [InlineData(5, "Child")]
    [InlineData(15, "Teenager")]
    [InlineData(30, "Adult")]
    [InlineData(70, "Senior")]
    public void ClassifyAge_ShouldReturnCorrectClassification(int age, string expected)
    {
        // Act
        var result = AdvancedPatternMatching.ClassifyAge(age);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(50, true)]
    [InlineData(100, true)]
    [InlineData(-1, false)]
    [InlineData(101, false)]
    public void IsValidScore_ShouldValidateCorrectly(int score, bool expected)
    {
        // Act
        var result = AdvancedPatternMatching.IsValidScore(score);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(95, "A")]
    [InlineData(85, "B")]
    [InlineData(75, "C")]
    [InlineData(65, "D")]
    [InlineData(55, "F")]
    [InlineData(-1, "Invalid score")]
    [InlineData(101, "Invalid score")]
    public void EvaluateScore_ShouldReturnCorrectGrade(int score, string expected)
    {
        // Act
        var result = AdvancedPatternMatching.EvaluateScore(score);

        // Assert
        result.Should().Be(expected);
    }
}
