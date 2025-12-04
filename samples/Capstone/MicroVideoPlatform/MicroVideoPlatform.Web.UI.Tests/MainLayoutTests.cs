using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MicroVideoPlatform.Web.UI.Services;
using MicroVideoPlatform.Web.UI.Shared;
using MicroVideoPlatform.Web.UI.State;
using Moq;
using MudBlazor.Services;
using Xunit;

namespace MicroVideoPlatform.Web.UI.Tests;

/// <summary>
/// Tests for MainLayout component.
/// </summary>
public class MainLayoutTests : TestContext
{
    private readonly Mock<AppState> _mockAppState;
    private readonly Mock<VideoHubClient> _mockHubClient;
    private readonly Mock<ILogger<MainLayout>> _mockLogger;

    public MainLayoutTests()
    {
        // Setup mocks
        _mockAppState = new Mock<AppState>(Mock.Of<Blazored.LocalStorage.ILocalStorageService>());
        _mockHubClient = new Mock<VideoHubClient>(
            Mock.Of<ILogger<VideoHubClient>>(),
            Mock.Of<Microsoft.Extensions.Configuration.IConfiguration>()
        );
        _mockLogger = new Mock<ILogger<MainLayout>>();

        // Register services
        Services.AddMudServices();
        Services.AddSingleton(_mockAppState.Object);
        Services.AddSingleton(_mockHubClient.Object);
        Services.AddSingleton(_mockLogger.Object);
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void MainLayout_ShouldRender_WithDefaultState()
    {
        // Arrange
        _mockAppState.Setup(x => x.IsDarkMode).Returns(false);
        _mockAppState.Setup(x => x.IsAuthenticated).Returns(false);

        // Act
        var cut = RenderComponent<MainLayout>(parameters => parameters
            .AddChildContent("<div>Test Content</div>")
        );

        // Assert
        cut.Should().NotBeNull();
        cut.Find("div").TextContent.Should().Contain("Test Content");
    }

    [Fact]
    public void MainLayout_ShouldDisplay_AppTitle()
    {
        // Arrange & Act
        var cut = RenderComponent<MainLayout>();

        // Assert
        var title = cut.Find(".mud-typography-h5");
        title.TextContent.Should().Contain("MicroVideo Platform");
    }

    [Fact]
    public void MainLayout_ShouldShowLoginButton_WhenNotAuthenticated()
    {
        // Arrange
        _mockAppState.Setup(x => x.IsAuthenticated).Returns(false);

        // Act
        var cut = RenderComponent<MainLayout>();

        // Assert
        var loginButton = cut.Find("button:contains('Login')");
        loginButton.Should().NotBeNull();
    }

    [Fact]
    public void MainLayout_ShouldShowUserMenu_WhenAuthenticated()
    {
        // Arrange
        _mockAppState.Setup(x => x.IsAuthenticated).Returns(true);
        _mockAppState.Setup(x => x.CurrentUserId).Returns("test-user-123");

        // Act
        var cut = RenderComponent<MainLayout>();

        // Assert
        var accountIcon = cut.FindAll(".mud-icon-root").Should().Contain(x =>
            x.OuterHtml.Contains("account_circle")
        );
    }

    [Fact]
    public void MainLayout_ShouldToggleTheme_WhenThemeButtonClicked()
    {
        // Arrange
        var themeChanged = false;
        _mockAppState.Setup(x => x.IsDarkMode).Returns(false);
        _mockAppState.Setup(x => x.ToggleTheme()).Callback(() => themeChanged = true);

        var cut = RenderComponent<MainLayout>();

        // Act
        var themeButton = cut.FindAll("button").First(x =>
            x.OuterHtml.Contains("dark_mode") || x.OuterHtml.Contains("light_mode")
        );
        themeButton.Click();

        // Assert
        _mockAppState.Verify(x => x.ToggleTheme(), Times.Once);
    }

    [Fact]
    public void MainLayout_ShouldShowConnectionStatus_Connected()
    {
        // Arrange
        _mockHubClient.Setup(x => x.IsConnected).Returns(true);

        // Act
        var cut = RenderComponent<MainLayout>();

        // Assert
        var connectionChip = cut.FindAll(".mud-chip").Should().Contain(x =>
            x.TextContent.Contains("Connected")
        );
    }

    [Fact]
    public void MainLayout_ShouldShowConnectionStatus_Disconnected()
    {
        // Arrange
        _mockHubClient.Setup(x => x.IsConnected).Returns(false);

        // Act
        var cut = RenderComponent<MainLayout>();

        // Assert
        var connectionChip = cut.FindAll(".mud-chip").Should().Contain(x =>
            x.TextContent.Contains("Disconnected")
        );
    }

    [Fact]
    public void MainLayout_ShouldRenderNavigation_WithAllLinks()
    {
        // Arrange & Act
        var cut = RenderComponent<MainLayout>();

        // Assert
        var navLinks = cut.FindAll(".mud-nav-link");
        navLinks.Should().NotBeEmpty();

        var linkTexts = navLinks.Select(x => x.TextContent).ToList();
        linkTexts.Should().Contain(text => text.Contains("Home"));
        linkTexts.Should().Contain(text => text.Contains("Videos"));
        linkTexts.Should().Contain(text => text.Contains("Upload Video"));
    }

    [Fact]
    public void MainLayout_ShouldShowCategories_InNavigation()
    {
        // Arrange & Act
        var cut = RenderComponent<MainLayout>();

        // Assert
        var navContent = cut.Markup;
        navContent.Should().Contain("Categories");
        navContent.Should().Contain("Education");
        navContent.Should().Contain("Entertainment");
        navContent.Should().Contain("Technology");
    }

    [Fact]
    public void MainLayout_ShouldShowAuthenticatedOnlyLinks_WhenLoggedIn()
    {
        // Arrange
        _mockAppState.Setup(x => x.IsAuthenticated).Returns(true);

        // Act
        var cut = RenderComponent<MainLayout>();

        // Assert
        var navContent = cut.Markup;
        navContent.Should().Contain("My Videos");
        navContent.Should().Contain("Favorites");
        navContent.Should().Contain("Watch History");
    }

    [Fact]
    public void MainLayout_ShouldNotShowAuthenticatedOnlyLinks_WhenLoggedOut()
    {
        // Arrange
        _mockAppState.Setup(x => x.IsAuthenticated).Returns(false);

        // Act
        var cut = RenderComponent<MainLayout>();

        // Assert
        var navContent = cut.Markup;
        navContent.Should().NotContain("My Videos");
        navContent.Should().NotContain("Favorites");
        navContent.Should().NotContain("Watch History");
    }

    [Fact]
    public async Task MainLayout_ShouldInitializeSignalRConnection_OnLoad()
    {
        // Arrange
        _mockHubClient.Setup(x => x.IsConnected).Returns(false);
        _mockHubClient.Setup(x => x.StartAsync()).Returns(Task.CompletedTask);

        // Act
        var cut = RenderComponent<MainLayout>();
        await Task.Delay(100); // Wait for initialization

        // Assert
        _mockHubClient.Verify(x => x.StartAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task MainLayout_ShouldRegisterUser_WhenAuthenticatedOnLoad()
    {
        // Arrange
        _mockAppState.Setup(x => x.IsAuthenticated).Returns(true);
        _mockAppState.Setup(x => x.CurrentUserId).Returns("test-user");
        _mockHubClient.Setup(x => x.IsConnected).Returns(true);
        _mockHubClient.Setup(x => x.RegisterUserAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        var cut = RenderComponent<MainLayout>();
        await Task.Delay(100); // Wait for initialization

        // Assert
        _mockHubClient.Verify(x => x.RegisterUserAsync("test-user"), Times.Once);
    }
}
