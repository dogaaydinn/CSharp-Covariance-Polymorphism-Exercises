using FluentAssertions;
using Microsoft.Extensions.Logging;
using MicroVideoPlatform.Analytics.Function.Models;
using MicroVideoPlatform.Analytics.Function.Services;
using Moq;
using Xunit;

namespace MicroVideoPlatform.Analytics.Tests;

public class VideoCommentAnalyzerTests
{
    private readonly VideoCommentAnalyzer _analyzer;
    private readonly List<CommentData> _trainingData;

    public VideoCommentAnalyzerTests()
    {
        var loggerMock = new Mock<ILogger<VideoCommentAnalyzer>>();
        _analyzer = new VideoCommentAnalyzer(loggerMock.Object);

        // Sample training data with clear positive/negative examples
        _trainingData = new List<CommentData>
        {
            // Positive examples
            new() { CommentId = "1", Text = "This video is amazing! Great content!", Label = true },
            new() { CommentId = "2", Text = "Love it! Very helpful tutorial.", Label = true },
            new() { CommentId = "3", Text = "Excellent work, keep it up!", Label = true },
            new() { CommentId = "4", Text = "Best video I've seen on this topic", Label = true },
            new() { CommentId = "5", Text = "Thank you for sharing this!", Label = true },
            new() { CommentId = "6", Text = "Very informative and well explained", Label = true },
            new() { CommentId = "7", Text = "Great quality content, subscribed!", Label = true },
            new() { CommentId = "8", Text = "This helped me a lot, thanks!", Label = true },
            new() { CommentId = "9", Text = "Perfect! Exactly what I needed", Label = true },
            new() { CommentId = "10", Text = "Outstanding video, loved it", Label = true },

            // Negative examples
            new() { CommentId = "11", Text = "This is terrible, waste of time", Label = false },
            new() { CommentId = "12", Text = "Boring content, don't recommend", Label = false },
            new() { CommentId = "13", Text = "Not helpful at all", Label = false },
            new() { CommentId = "14", Text = "Disliked, very poor quality", Label = false },
            new() { CommentId = "15", Text = "This video is misleading", Label = false },
            new() { CommentId = "16", Text = "Awful content, unsubscribed", Label = false },
            new() { CommentId = "17", Text = "Worst tutorial ever", Label = false },
            new() { CommentId = "18", Text = "Complete waste of my time", Label = false },
            new() { CommentId = "19", Text = "Disappointed with this video", Label = false },
            new() { CommentId = "20", Text = "Horrible, do not watch", Label = false }
        };
    }

    [Fact]
    public void TrainModel_WithValidData_ShouldComplete()
    {
        // Act
        Action act = () => _analyzer.TrainModel(_trainingData);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void AnalyzeComment_BeforeTraining_ShouldThrowException()
    {
        // Act
        Action act = () => _analyzer.AnalyzeComment("This is a test comment");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*not been trained*");
    }

    [Fact]
    public void AnalyzeComment_WithPositiveText_ShouldReturnPositiveSentiment()
    {
        // Arrange
        _analyzer.TrainModel(_trainingData);

        // Act
        var result = _analyzer.AnalyzeComment("This is great! Love it!");

        // Assert
        result.Should().NotBeNull();
        result.Sentiment.Should().Be("Positive");
        result.Confidence.Should().BeGreaterThan(50); // At least 50% confident
    }

    [Fact]
    public void AnalyzeComment_WithNegativeText_ShouldReturnNegativeSentiment()
    {
        // Arrange
        _analyzer.TrainModel(_trainingData);

        // Act
        var result = _analyzer.AnalyzeComment("This is terrible! Waste of time!");

        // Assert
        result.Should().NotBeNull();
        result.Sentiment.Should().Be("Negative");
        result.Confidence.Should().BeGreaterThan(50);
    }

    [Fact]
    public void AnalyzeBatch_WithMultipleComments_ShouldReturnAnalysisForAll()
    {
        // Arrange
        _analyzer.TrainModel(_trainingData);
        var request = new CommentAnalysisRequest
        {
            VideoId = "vid123",
            Comments = new List<CommentItem>
            {
                new() { CommentId = "c1", Text = "Amazing video!", AuthorId = "user1", CreatedAt = DateTime.UtcNow },
                new() { CommentId = "c2", Text = "Terrible content", AuthorId = "user2", CreatedAt = DateTime.UtcNow },
                new() { CommentId = "c3", Text = "Very helpful, thanks", AuthorId = "user3", CreatedAt = DateTime.UtcNow }
            }
        };

        // Act
        var response = _analyzer.AnalyzeBatch(request);

        // Assert
        response.Should().NotBeNull();
        response.VideoId.Should().Be("vid123");
        response.TotalComments.Should().Be(3);
        response.CommentSentiments.Should().HaveCount(3);
        response.PositiveCount.Should().BeGreaterThan(0);
        response.NegativeCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public void AnalyzeBatch_WithAllPositiveComments_ShouldHavePositiveOverallScore()
    {
        // Arrange
        _analyzer.TrainModel(_trainingData);
        var request = new CommentAnalysisRequest
        {
            VideoId = "vid123",
            Comments = new List<CommentItem>
            {
                new() { CommentId = "c1", Text = "Great video!", AuthorId = "user1", CreatedAt = DateTime.UtcNow },
                new() { CommentId = "c2", Text = "Love it!", AuthorId = "user2", CreatedAt = DateTime.UtcNow },
                new() { CommentId = "c3", Text = "Excellent work!", AuthorId = "user3", CreatedAt = DateTime.UtcNow }
            }
        };

        // Act
        var response = _analyzer.AnalyzeBatch(request);

        // Assert
        response.PositiveCount.Should().Be(3);
        response.NegativeCount.Should().Be(0);
        response.OverallSentimentScore.Should().BeGreaterThan(0); // Positive overall score
    }

    [Fact]
    public void AnalyzeBatch_WithAllNegativeComments_ShouldHaveNegativeOverallScore()
    {
        // Arrange
        _analyzer.TrainModel(_trainingData);
        var request = new CommentAnalysisRequest
        {
            VideoId = "vid123",
            Comments = new List<CommentItem>
            {
                new() { CommentId = "c1", Text = "Terrible video!", AuthorId = "user1", CreatedAt = DateTime.UtcNow },
                new() { CommentId = "c2", Text = "Waste of time", AuthorId = "user2", CreatedAt = DateTime.UtcNow },
                new() { CommentId = "c3", Text = "Very bad content", AuthorId = "user3", CreatedAt = DateTime.UtcNow }
            }
        };

        // Act
        var response = _analyzer.AnalyzeBatch(request);

        // Assert
        response.PositiveCount.Should().Be(0);
        response.NegativeCount.Should().Be(3);
        response.OverallSentimentScore.Should().BeLessThan(0); // Negative overall score
    }

    [Theory]
    [InlineData("Fantastic work!", "Positive")]
    [InlineData("Horrible video", "Negative")]
    [InlineData("Great tutorial, very useful", "Positive")]
    [InlineData("Not helpful, very disappointing", "Negative")]
    public void AnalyzeComment_WithVariousTexts_ShouldPredictCorrectSentiment(string commentText, string expectedSentiment)
    {
        // Arrange
        _analyzer.TrainModel(_trainingData);

        // Act
        var result = _analyzer.AnalyzeComment(commentText);

        // Assert
        result.Sentiment.Should().Be(expectedSentiment);
    }

    [Fact]
    public void SaveModel_AfterTraining_ShouldNotThrow()
    {
        // Arrange
        _analyzer.TrainModel(_trainingData);
        var tempPath = Path.Combine(Path.GetTempPath(), "test_sentiment_model.zip");

        try
        {
            // Act
            Action act = () => _analyzer.SaveModel(tempPath);

            // Assert
            act.Should().NotThrow();
            File.Exists(tempPath).Should().BeTrue();
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    [Fact]
    public void LoadModel_WithValidPath_ShouldAllowPredictions()
    {
        // Arrange
        _analyzer.TrainModel(_trainingData);
        var tempPath = Path.Combine(Path.GetTempPath(), "test_sentiment_model.zip");

        try
        {
            _analyzer.SaveModel(tempPath);

            // Create new analyzer and load model
            var loggerMock = new Mock<ILogger<VideoCommentAnalyzer>>();
            var newAnalyzer = new VideoCommentAnalyzer(loggerMock.Object);

            // Act
            newAnalyzer.LoadModel(tempPath);
            var result = newAnalyzer.AnalyzeComment("This is great!");

            // Assert
            result.Should().NotBeNull();
            result.Sentiment.Should().Be("Positive");
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
}
