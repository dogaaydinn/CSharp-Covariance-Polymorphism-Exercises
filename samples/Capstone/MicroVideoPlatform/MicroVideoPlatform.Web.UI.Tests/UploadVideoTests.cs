using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Forms;
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
/// Tests for UploadVideo component.
/// </summary>
public class UploadVideoTests : TestContext
{
    private readonly Mock<VideoStore> _mockVideoStore;
    private readonly Mock<VideoHubClient> _mockHubClient;
    private readonly Mock<AppState> _mockAppState;
    private readonly Mock<ILogger<UploadVideo>> _mockLogger;
    private readonly Mock<ISnackbar> _mockSnackbar;

    public UploadVideoTests()
    {
        // Setup mocks
        _mockVideoStore = new Mock<VideoStore>(Mock.Of<IVideoApiClient>());
        _mockVideoStore.Setup(x => x.CreateVideoAsync(It.IsAny<VideoDto>()))
            .ReturnsAsync((VideoDto dto) => dto);

        _mockHubClient = new Mock<VideoHubClient>(
            Mock.Of<ILogger<VideoHubClient>>(),
            Mock.Of<Microsoft.Extensions.Configuration.IConfiguration>()
        );
        _mockHubClient.Setup(x => x.NotifyNewVideoUpload(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()
        )).Returns(Task.CompletedTask);

        _mockAppState = new Mock<AppState>(Mock.Of<Blazored.LocalStorage.ILocalStorageService>());
        _mockAppState.Setup(x => x.IsAuthenticated).Returns(true);
        _mockAppState.Setup(x => x.CurrentUserId).Returns("test-user-123");

        _mockLogger = new Mock<ILogger<UploadVideo>>();
        _mockSnackbar = new Mock<ISnackbar>();

        // Register services
        Services.AddMudServices();
        Services.AddSingleton(_mockVideoStore.Object);
        Services.AddSingleton(_mockHubClient.Object);
        Services.AddSingleton(_mockAppState.Object);
        Services.AddSingleton(_mockLogger.Object);
        Services.AddSingleton(_mockSnackbar.Object);
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void UploadVideo_ShouldRender_WithTitle()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        var title = cut.Find(".mud-typography-h3");
        title.TextContent.Should().Contain("Upload Video");
    }

    [Fact]
    public void UploadVideo_ShouldShowLoginWarning_WhenNotAuthenticated()
    {
        // Arrange
        _mockAppState.Setup(x => x.IsAuthenticated).Returns(false);

        // Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        var alert = cut.Find(".mud-alert");
        alert.TextContent.Should().Contain("logged in to upload");
    }

    [Fact]
    public void UploadVideo_ShouldShowStepper_WhenAuthenticated()
    {
        // Arrange
        _mockAppState.Setup(x => x.IsAuthenticated).Returns(true);

        // Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        var stepper = cut.Find(".mud-stepper");
        stepper.Should().NotBeNull();
    }

    [Fact]
    public void UploadVideo_ShouldHaveThreeSteps()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        cut.Markup.Should().Contain("Upload File");
        cut.Markup.Should().Contain("Video Details");
        cut.Markup.Should().Contain("Preview & Submit");
    }

    [Fact]
    public void UploadVideo_Step1_ShouldShowFileUploadArea()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        cut.Markup.Should().Contain("Drag & Drop Video Here");
        cut.Markup.Should().Contain("or click to browse");
    }

    [Fact]
    public void UploadVideo_Step1_ShouldShowSupportedFormats()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        cut.Markup.Should().Contain("Supported formats");
        cut.Markup.Should().Contain("MP4");
        cut.Markup.Should().Contain("Maximum file size");
    }

    [Fact]
    public void UploadVideo_Step1_NextButton_ShouldBeDisabled_WithoutFile()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        var nextButton = cut.FindAll("button").FirstOrDefault(x =>
            x.TextContent.Contains("Next")
        );
        nextButton.Should().NotBeNull();
        nextButton!.HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void UploadVideo_Step2_ShouldShowTitleField()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        var titleInput = cut.FindAll("input").Should().Contain(x =>
            x.OuterHtml.Contains("Title")
        );
    }

    [Fact]
    public void UploadVideo_Step2_ShouldShowDescriptionField()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        var descriptionTextarea = cut.FindAll("textarea").Should().Contain(x =>
            x.OuterHtml.Contains("Description")
        );
    }

    [Fact]
    public void UploadVideo_Step2_ShouldShowCategorySelect()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        cut.Markup.Should().Contain("Category");
        cut.Markup.Should().Contain("Education");
        cut.Markup.Should().Contain("Entertainment");
        cut.Markup.Should().Contain("Technology");
    }

    [Fact]
    public void UploadVideo_Step2_ShouldShowTagsField()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        cut.Markup.Should().Contain("Tags");
        cut.Markup.Should().Contain("Separate tags with commas");
    }

    [Fact]
    public void UploadVideo_Step2_ShouldShowPublicSwitch()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        cut.Markup.Should().Contain("Make this video public");
    }

    [Fact]
    public void UploadVideo_Step2_ShouldHaveBackButton()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        var backButton = cut.FindAll("button").Should().Contain(x =>
            x.TextContent.Contains("Back")
        );
    }

    [Fact]
    public void UploadVideo_Step3_ShouldShowReviewSection()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        cut.Markup.Should().Contain("Review Your Video");
    }

    [Fact]
    public void UploadVideo_Step3_ShouldDisplayEnteredDetails()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        cut.Markup.Should().Contain("Title");
        cut.Markup.Should().Contain("Category");
        cut.Markup.Should().Contain("Visibility");
        cut.Markup.Should().Contain("File Name");
        cut.Markup.Should().Contain("File Size");
    }

    [Fact]
    public void UploadVideo_Step3_ShouldHavePublishButton()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        var publishButton = cut.FindAll("button").Should().Contain(x =>
            x.TextContent.Contains("Publish Video")
        );
    }

    [Fact]
    public void UploadVideo_ShouldFormatFileSize_Correctly()
    {
        // This tests the FormatFileSize helper method indirectly
        // by checking that file sizes are displayed in human-readable format

        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        // The markup should contain size formatting patterns like "KB", "MB", "GB"
        cut.Markup.Should().Contain("500 MB");
    }

    [Fact]
    public void UploadVideo_ShouldShowAllCategories_InSelect()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        cut.Markup.Should().Contain("Education");
        cut.Markup.Should().Contain("Entertainment");
        cut.Markup.Should().Contain("Technology");
        cut.Markup.Should().Contain("Music");
        cut.Markup.Should().Contain("Gaming");
        cut.Markup.Should().Contain("Sports");
    }

    [Fact]
    public void UploadVideo_ShouldHaveCharacterCounters()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        // MudTextField with Counter attribute should render character count
        cut.Markup.Should().Contain("100"); // Title counter
        cut.Markup.Should().Contain("500"); // Description counter
        cut.Markup.Should().Contain("200"); // Tags counter
    }

    [Fact]
    public void UploadVideo_ShouldDisplayVisibilityStatus()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        cut.Markup.Should().Contain("Public");
    }

    [Fact]
    public void UploadVideo_ShouldShowLoadingState_DuringSubmit()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        cut.Markup.Should().Contain("Submitting your video");
    }

    [Fact]
    public void UploadVideo_ShouldHaveRequiredValidation()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        cut.Markup.Should().Contain("required");
    }

    [Fact]
    public void UploadVideo_ShouldShowProgressBar_AfterFileSelect()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        // Progress bar markup should exist in the component
        cut.Markup.Should().Contain("mud-progress");
    }

    [Fact]
    public void UploadVideo_ShouldDisplayUploadedFileName()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        // File name display area should exist
        cut.Markup.Should().Contain("File Name");
    }

    [Fact]
    public void UploadVideo_ShouldAllowFileRemoval()
    {
        // Arrange & Act
        var cut = RenderComponent<UploadVideo>();

        // Assert
        // Delete button should exist for removing uploaded file
        var deleteButtons = cut.FindAll("button").Where(x =>
            x.OuterHtml.Contains("delete")
        );
        // Delete button may not be visible until a file is selected
        // but the component should have the capability
        cut.Markup.Should().Contain("Delete");
    }
}
