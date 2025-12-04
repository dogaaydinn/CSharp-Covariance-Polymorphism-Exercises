using FluentAssertions;
using Microsoft.Extensions.Logging;
using MicroVideoPlatform.Analytics.Function.Models;
using MicroVideoPlatform.Analytics.Function.Services;
using Moq;
using Xunit;

namespace MicroVideoPlatform.Analytics.Tests;

public class VideoRecommendationServiceTests
{
    private readonly VideoRecommendationService _service;
    private readonly List<VideoData> _sampleVideos;

    public VideoRecommendationServiceTests()
    {
        var loggerMock = new Mock<ILogger<VideoRecommendationService>>();
        _service = new VideoRecommendationService(loggerMock.Object);

        // Sample training data
        _sampleVideos = new List<VideoData>
        {
            new() { VideoId = "1", Title = "C# Tutorial for Beginners", Description = "Learn C# programming basics", Category = "Programming", Tags = "csharp,tutorial,programming", ViewsCount = 1000, LikesCount = 100, DurationSeconds = 3600 },
            new() { VideoId = "2", Title = "Advanced C# Patterns", Description = "Deep dive into design patterns in C#", Category = "Programming", Tags = "csharp,advanced,patterns", ViewsCount = 500, LikesCount = 80, DurationSeconds = 5400 },
            new() { VideoId = "3", Title = "Python Machine Learning", Description = "Introduction to ML with Python", Category = "AI", Tags = "python,machine-learning,ai", ViewsCount = 2000, LikesCount = 250, DurationSeconds = 4200 },
            new() { VideoId = "4", Title = "JavaScript Basics", Description = "Learn JavaScript fundamentals", Category = "Programming", Tags = "javascript,web,tutorial", ViewsCount = 1500, LikesCount = 150, DurationSeconds = 3000 },
            new() { VideoId = "5", Title = "Docker and Kubernetes", Description = "Container orchestration guide", Category = "DevOps", Tags = "docker,kubernetes,devops", ViewsCount = 800, LikesCount = 120, DurationSeconds = 4800 },
        };
    }

    [Fact]
    public void TrainModel_WithValidData_ShouldComplete()
    {
        // Act
        Action act = () => _service.TrainModel(_sampleVideos);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void GetRecommendations_BeforeTraining_ShouldThrowException()
    {
        // Act
        Action act = () => _service.GetRecommendations("1");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*not been trained*");
    }

    [Fact]
    public void GetRecommendations_WithValidVideoId_ShouldReturnSimilarVideos()
    {
        // Arrange
        _service.TrainModel(_sampleVideos);

        // Act
        var recommendations = _service.GetRecommendations("1", topN: 3);

        // Assert
        recommendations.Should().NotBeNull();
        recommendations.Should().HaveCountLessOrEqualTo(3);
        recommendations.Should().NotContain(r => r.VideoId == "1"); // Should not recommend itself
    }

    [Fact]
    public void GetRecommendations_ForCSharpVideo_ShouldReturnCSharpRelatedVideos()
    {
        // Arrange
        _service.TrainModel(_sampleVideos);

        // Act
        var recommendations = _service.GetRecommendations("1", topN: 5);

        // Assert
        // Video #1 is "C# Tutorial", should recommend video #2 "Advanced C# Patterns" with high score
        var topRecommendation = recommendations.FirstOrDefault();
        topRecommendation.Should().NotBeNull();
        topRecommendation!.VideoId.Should().Be("2"); // Most similar should be the other C# video
        topRecommendation.Score.Should().BeGreaterThan(0.3f); // High similarity
        topRecommendation.ReasonCode.Should().Be("content_based");
    }

    [Fact]
    public void GetRecommendations_WithNonExistentVideoId_ShouldReturnEmptyList()
    {
        // Arrange
        _service.TrainModel(_sampleVideos);

        // Act
        var recommendations = _service.GetRecommendations("non-existent-id");

        // Assert
        recommendations.Should().BeEmpty();
    }

    [Fact]
    public void GetPersonalizedRecommendations_WithWatchHistory_ShouldReturnRelevantVideos()
    {
        // Arrange
        _service.TrainModel(_sampleVideos);
        var watchedVideos = new List<string> { "1", "2" }; // Watched C# videos

        // Act
        var recommendations = _service.GetPersonalizedRecommendations("user123", watchedVideos, topN: 3);

        // Assert
        recommendations.Should().NotBeNull();
        recommendations.Should().HaveCountLessOrEqualTo(3);
        recommendations.Should().NotContain(r => watchedVideos.Contains(r.VideoId)); // Should exclude watched videos
        recommendations.All(r => r.ReasonCode == "personalized").Should().BeTrue();
    }

    [Fact]
    public void GetPersonalizedRecommendations_WithEmptyWatchHistory_ShouldReturnEmptyList()
    {
        // Arrange
        _service.TrainModel(_sampleVideos);
        var emptyWatchHistory = new List<string>();

        // Act
        var recommendations = _service.GetPersonalizedRecommendations("user123", emptyWatchHistory, topN: 3);

        // Assert
        recommendations.Should().BeEmpty();
    }

    [Fact]
    public void GetRecommendations_OrderedByScore_ShouldReturnDescendingOrder()
    {
        // Arrange
        _service.TrainModel(_sampleVideos);

        // Act
        var recommendations = _service.GetRecommendations("1", topN: 5);

        // Assert
        recommendations.Should().BeInDescendingOrder(r => r.Score);
    }
}
