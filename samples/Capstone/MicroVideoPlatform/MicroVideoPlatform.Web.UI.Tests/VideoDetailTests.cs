using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MicroVideoPlatform.Shared.DTOs;
using MicroVideoPlatform.Web.UI.Pages;
using MicroVideoPlatform.Web.UI.Services;
using MicroVideoPlatform.Web.UI.State;
using Moq;
using MudBlazor;
using MudBlazor.Services;
using Xunit;

namespace MicroVideoPlatform.Web.UI.Tests;

/// <summary>
/// Tests for VideoDetail component.
/// </summary>
public class VideoDetailTests : TestContext
{
    private readonly Mock<VideoStore> _mockVideoStore;
    private readonly Mock<VideoHubClient> _mockHubClient;
    private readonly Mock<IAnalyticsApiClient> _mockAnalyticsClient;
    private readonly Mock<AppState> _mockAppState;
    private readonly Mock<ILogger<VideoDetail>> _mockLogger;
    private readonly Mock<ISnackbar> _mockSnackbar;
    private readonly VideoDto _testVideo;
    private readonly Guid _testVideoId;

    public VideoDetailTests()
    {
        _testVideoId = Guid.NewGuid();

        // Setup test video
        _testVideo = new VideoDto
        {
            Id = _testVideoId,
            Title = "Test Video Title",
            Description = "Test video description with details",
            Category = "Technology",
            Tags = "csharp, dotnet, tutorial",
            VideoUrl = "https://example.com/video.mp4",
            ThumbnailUrl = "https://example.com/thumb.jpg",
            ViewsCount = 1000,
            LikesCount = 50,
            DislikesCount = 5,
            CommentsCount = 10,
            UploadDate = DateTime.UtcNow.AddDays(-7)
        };

        var recommendations = new List<VideoRecommendation>
        {
            new VideoRecommendation
            {
                VideoId = Guid.NewGuid().ToString(),
                Title = "Related Video 1",
                Category = "Technology",
                Score = 0.95f,
                ReasonCode = "similar_category"
            },
            new VideoRecommendation
            {
                VideoId = Guid.NewGuid().ToString(),
                Title = "Related Video 2",
                Category = "Education",
                Score = 0.85f,
                ReasonCode = "similar_tags"
            }
        };

        // Setup mocks
        _mockVideoStore = new Mock<VideoStore>(Mock.Of<IVideoApiClient>());
        _mockVideoStore.Setup(x => x.GetVideoAsync(_testVideoId)).ReturnsAsync(_testVideo);
        _mockVideoStore.Setup(x => x.IncrementViewCountAsync(_testVideoId)).Returns(Task.CompletedTask);

        _mockHubClient = new Mock<VideoHubClient>(
            Mock.Of<ILogger<VideoHubClient>>(),
            Mock.Of<Microsoft.Extensions.Configuration.IConfiguration>()
        );
        _mockHubClient.Setup(x => x.JoinVideoRoomAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        _mockHubClient.Setup(x => x.UpdateViewCountAsync(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.CompletedTask);
        _mockHubClient.Setup(x => x.SendCommentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        _mockAnalyticsClient = new Mock<IAnalyticsApiClient>();
        _mockAnalyticsClient.Setup(x => x.GetRecommendationsAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(recommendations);

        _mockAppState = new Mock<AppState>(Mock.Of<Blazored.LocalStorage.ILocalStorageService>());
        _mockAppState.Setup(x => x.IsAuthenticated).Returns(true);
        _mockAppState.Setup(x => x.CurrentUserId).Returns("test-user-123");

        _mockLogger = new Mock<ILogger<VideoDetail>>();
        _mockSnackbar = new Mock<ISnackbar>();

        // Register services
        Services.AddMudServices();
        Services.AddSingleton(_mockVideoStore.Object);
        Services.AddSingleton(_mockHubClient.Object);
        Services.AddSingleton(_mockAnalyticsClient.Object);
        Services.AddSingleton(_mockAppState.Object);
        Services.AddSingleton(_mockLogger.Object);
        Services.AddSingleton(_mockSnackbar.Object);
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public async Task VideoDetail_ShouldLoadVideo_OnInitialization()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        _mockVideoStore.Verify(x => x.GetVideoAsync(_testVideoId), Times.Once);
    }

    [Fact]
    public async Task VideoDetail_ShouldDisplayVideoTitle()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        var title = cut.Find(".mud-typography-h4");
        title.TextContent.Should().Contain("Test Video Title");
    }

    [Fact]
    public async Task VideoDetail_ShouldDisplayVideoDescription()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        cut.Markup.Should().Contain("Test video description with details");
    }

    [Fact]
    public async Task VideoDetail_ShouldDisplayVideoPlayer()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        var videoElement = cut.Find("video");
        videoElement.Should().NotBeNull();
        videoElement.GetAttribute("src").Should().Contain("video.mp4");
    }

    [Fact]
    public async Task VideoDetail_ShouldDisplayLikeButton()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        var likeButtons = cut.FindAll("button").Where(x =>
            x.OuterHtml.Contains("thumb_up")
        );
        likeButtons.Should().NotBeEmpty();
    }

    [Fact]
    public async Task VideoDetail_ShouldDisplayDislikeButton()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        var dislikeButtons = cut.FindAll("button").Where(x =>
            x.OuterHtml.Contains("thumb_down")
        );
        dislikeButtons.Should().NotBeEmpty();
    }

    [Fact]
    public async Task VideoDetail_ShouldDisplayShareButton()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        var shareButtons = cut.FindAll("button").Where(x =>
            x.OuterHtml.Contains("share")
        );
        shareButtons.Should().NotBeEmpty();
    }

    [Fact]
    public async Task VideoDetail_ShouldDisplayViewCount()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        cut.Markup.Should().Contain("1.0K views");
    }

    [Fact]
    public async Task VideoDetail_ShouldDisplayCategory()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        cut.Markup.Should().Contain("Technology");
    }

    [Fact]
    public async Task VideoDetail_ShouldDisplayTags()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        cut.Markup.Should().Contain("csharp");
        cut.Markup.Should().Contain("dotnet");
        cut.Markup.Should().Contain("tutorial");
    }

    [Fact]
    public async Task VideoDetail_ShouldDisplayCommentsSection()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        cut.Markup.Should().Contain("Comments");
    }

    [Fact]
    public async Task VideoDetail_ShouldShowCommentInput_WhenAuthenticated()
    {
        // Arrange
        _mockAppState.Setup(x => x.IsAuthenticated).Returns(true);

        // Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        var commentInput = cut.FindAll("textarea").Should().ContainSingle(x =>
            x.OuterHtml.Contains("Add a comment")
        );
    }

    [Fact]
    public async Task VideoDetail_ShouldShowLoginPrompt_WhenNotAuthenticated()
    {
        // Arrange
        _mockAppState.Setup(x => x.IsAuthenticated).Returns(false);

        // Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        cut.Markup.Should().Contain("login to comment");
    }

    [Fact]
    public async Task VideoDetail_ShouldLoadRecommendations()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        _mockAnalyticsClient.Verify(x => x.GetRecommendationsAsync(
            _testVideoId.ToString(),
            10
        ), Times.Once);
    }

    [Fact]
    public async Task VideoDetail_ShouldDisplayRelatedVideos()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        cut.Markup.Should().Contain("Related Videos");
        cut.Markup.Should().Contain("Related Video 1");
        cut.Markup.Should().Contain("Related Video 2");
    }

    [Fact]
    public async Task VideoDetail_ShouldIncrementViewCount_OnLoad()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        _mockVideoStore.Verify(x => x.IncrementViewCountAsync(_testVideoId), Times.Once);
        _mockHubClient.Verify(x => x.UpdateViewCountAsync(
            _testVideoId.ToString(),
            It.IsAny<int>()
        ), Times.Once);
    }

    [Fact]
    public async Task VideoDetail_ShouldJoinVideoRoom_WhenAuthenticated()
    {
        // Arrange
        _mockAppState.Setup(x => x.IsAuthenticated).Returns(true);
        _mockAppState.Setup(x => x.CurrentUserId).Returns("test-user");

        // Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        _mockHubClient.Verify(x => x.JoinVideoRoomAsync(
            _testVideoId.ToString(),
            "test-user"
        ), Times.Once);
    }

    [Fact]
    public async Task VideoDetail_ShouldShowNotFound_WhenVideoDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _mockVideoStore.Setup(x => x.GetVideoAsync(nonExistentId)).ReturnsAsync((VideoDto?)null);

        // Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, nonExistentId)
        );
        await Task.Delay(100);

        // Assert
        cut.Markup.Should().Contain("Video not found");
    }

    [Fact]
    public async Task VideoDetail_ShouldFormatRelativeTime_Correctly()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoDetail>(parameters => parameters
            .Add(p => p.VideoId, _testVideoId)
        );
        await Task.Delay(100);

        // Assert
        cut.Markup.Should().Contain("7 days ago");
    }
}
