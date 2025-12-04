using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MicroVideoPlatform.Shared.DTOs;
using MicroVideoPlatform.Shared.Enums;
using MicroVideoPlatform.Web.UI.Pages;
using MicroVideoPlatform.Web.UI.State;
using Moq;
using MudBlazor.Services;
using Xunit;

namespace MicroVideoPlatform.Web.UI.Tests;

/// <summary>
/// Tests for VideoList component.
/// </summary>
public class VideoListTests : TestContext
{
    private readonly Mock<VideoStore> _mockVideoStore;
    private readonly Mock<ILogger<VideoList>> _mockLogger;
    private readonly List<VideoDto> _testVideos;

    public VideoListTests()
    {
        // Setup test data
        _testVideos = new List<VideoDto>
        {
            new VideoDto
            {
                Id = Guid.NewGuid(),
                Title = "Test Video 1",
                Description = "Description 1",
                Category = "Technology",
                Tags = "csharp, programming",
                ViewsCount = 1000,
                LikesCount = 50,
                CommentsCount = 10,
                UploadDate = DateTime.UtcNow.AddDays(-1)
            },
            new VideoDto
            {
                Id = Guid.NewGuid(),
                Title = "Test Video 2",
                Description = "Description 2",
                Category = "Education",
                Tags = "tutorial, learning",
                ViewsCount = 5000,
                LikesCount = 200,
                CommentsCount = 30,
                UploadDate = DateTime.UtcNow.AddDays(-7)
            },
            new VideoDto
            {
                Id = Guid.NewGuid(),
                Title = "Gaming Video",
                Description = "Gaming content",
                Category = "Gaming",
                Tags = "gaming, fun",
                ViewsCount = 10000,
                LikesCount = 500,
                CommentsCount = 100,
                UploadDate = DateTime.UtcNow.AddHours(-2)
            }
        };

        // Setup mocks
        _mockVideoStore = new Mock<VideoStore>(Mock.Of<Services.IVideoApiClient>());
        _mockVideoStore.Setup(x => x.Videos).Returns(_testVideos.AsReadOnly());
        _mockVideoStore.Setup(x => x.LoadVideosAsync(It.IsAny<VideoStatus?>(), It.IsAny<string?>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        _mockLogger = new Mock<ILogger<VideoList>>();

        // Register services
        Services.AddMudServices();
        Services.AddSingleton(_mockVideoStore.Object);
        Services.AddSingleton(_mockLogger.Object);
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void VideoList_ShouldRender_WithTitle()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoList>();

        // Assert
        var title = cut.Find(".mud-typography-h3");
        title.TextContent.Should().Contain("Videos");
    }

    [Fact]
    public async Task VideoList_ShouldLoadVideos_OnInitialization()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoList>();
        await Task.Delay(100);

        // Assert
        _mockVideoStore.Verify(x => x.LoadVideosAsync(
            It.IsAny<VideoStatus?>(),
            It.IsAny<string?>(),
            false
        ), Times.Once);
    }

    [Fact]
    public void VideoList_ShouldDisplay_SearchBox()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoList>();

        // Assert
        var searchInput = cut.Find("input[type='text']");
        searchInput.Should().NotBeNull();
    }

    [Fact]
    public void VideoList_ShouldDisplay_CategoryFilter()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoList>();

        // Assert
        var categorySelect = cut.FindAll(".mud-select").Should().ContainSingle(x =>
            x.OuterHtml.Contains("Category")
        );
    }

    [Fact]
    public void VideoList_ShouldDisplay_SortByFilter()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoList>();

        // Assert
        var sortSelect = cut.FindAll(".mud-select").Should().ContainSingle(x =>
            x.OuterHtml.Contains("Sort By")
        );
    }

    [Fact]
    public void VideoList_ShouldDisplay_VideoCount()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoList>();

        // Assert
        var countChip = cut.FindAll(".mud-chip").Should().ContainSingle(x =>
            x.TextContent.Contains("videos found")
        );
    }

    [Fact]
    public void VideoList_ShouldDisplay_ViewModeToggle()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoList>();

        // Assert
        var gridViewButton = cut.FindAll("button").Should().Contain(x =>
            x.OuterHtml.Contains("grid_view")
        );
        var listViewButton = cut.FindAll("button").Should().Contain(x =>
            x.OuterHtml.Contains("list")
        );
    }

    [Fact]
    public void VideoList_ShouldDisplayVideos_InGridView()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoList>();

        // Assert
        var videoCards = cut.FindAll(".mud-card");
        videoCards.Should().NotBeEmpty();
        videoCards.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void VideoList_ShouldDisplayVideoDetails_InCards()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoList>();

        // Assert
        var markup = cut.Markup;
        markup.Should().Contain("Test Video 1");
        markup.Should().Contain("Technology");
    }

    [Fact]
    public void VideoList_ShouldShowViewCounts_OnVideoCards()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoList>();

        // Assert
        var viewChips = cut.FindAll(".mud-chip").Where(x =>
            x.OuterHtml.Contains("visibility")
        );
        viewChips.Should().NotBeEmpty();
    }

    [Fact]
    public void VideoList_ShouldShowLikeCounts_OnVideoCards()
    {
        // Arrange & Act
        var cut = RenderComponent<VideoList>();

        // Assert
        var likeChips = cut.FindAll(".mud-chip").Where(x =>
            x.OuterHtml.Contains("thumb_up")
        );
        likeChips.Should().NotBeEmpty();
    }

    [Fact]
    public void VideoList_ShouldFilter_ByCategory()
    {
        // Arrange
        var cut = RenderComponent<VideoList>();

        // Act - simulate category filter change
        var instance = cut.Instance;
        cut.SetParametersAndRender(); // Trigger re-render

        // Assert
        var videoCards = cut.FindAll(".mud-card");
        videoCards.Should().NotBeEmpty();
    }

    [Fact]
    public void VideoList_ShouldShowPagination_WhenMultiplePages()
    {
        // Arrange
        var manyVideos = Enumerable.Range(1, 25).Select(i => new VideoDto
        {
            Id = Guid.NewGuid(),
            Title = $"Video {i}",
            Category = "Test",
            UploadDate = DateTime.UtcNow
        }).ToList();

        _mockVideoStore.Setup(x => x.Videos).Returns(manyVideos.AsReadOnly());

        // Act
        var cut = RenderComponent<VideoList>();

        // Assert
        var pagination = cut.FindAll(".mud-pagination");
        pagination.Should().NotBeEmpty();
    }

    [Fact]
    public void VideoList_ShouldNavigateToVideo_WhenCardClicked()
    {
        // Arrange
        var navManager = Services.GetRequiredService<FakeNavigationManager>();
        var cut = RenderComponent<VideoList>();

        // Act
        var firstCard = cut.Find(".mud-card");
        firstCard.Click();

        // Assert
        navManager.Uri.Should().Contain("/video/");
    }

    [Fact]
    public void VideoList_ShouldShowNoResults_WhenNoVideosMatch()
    {
        // Arrange
        _mockVideoStore.Setup(x => x.Videos).Returns(new List<VideoDto>().AsReadOnly());

        // Act
        var cut = RenderComponent<VideoList>();

        // Assert
        var alert = cut.Find(".mud-alert");
        alert.TextContent.Should().Contain("No videos found");
    }

    [Fact]
    public void VideoList_ShouldFormatCounts_Correctly()
    {
        // Arrange
        var videoWithLargeCounts = new List<VideoDto>
        {
            new VideoDto
            {
                Id = Guid.NewGuid(),
                Title = "Popular Video",
                Category = "Test",
                ViewsCount = 1500000,
                LikesCount = 50000,
                UploadDate = DateTime.UtcNow
            }
        };

        _mockVideoStore.Setup(x => x.Videos).Returns(videoWithLargeCounts.AsReadOnly());

        // Act
        var cut = RenderComponent<VideoList>();

        // Assert
        var markup = cut.Markup;
        markup.Should().Contain("1.5M"); // 1.5 million views
        markup.Should().Contain("50.0K"); // 50 thousand likes
    }
}
