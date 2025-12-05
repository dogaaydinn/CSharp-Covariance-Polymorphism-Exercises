using Microsoft.Extensions.DependencyInjection;

namespace FactoryPattern;

/// <summary>
/// Factory Pattern Demo - UI Theme Factory
///
/// This demo demonstrates three variations of the Factory Pattern:
/// 1. Simple Factory - Single factory class with creation logic
/// 2. Factory Method - Template method pattern with subclass factories
/// 3. Abstract Factory - Families of related objects
///
/// Scenario: UI Theme System with Dark, Light, and High Contrast themes
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Factory Pattern Demo - UI Theme Factory ===\n");

        DemonstrateSimpleFactory();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateFactoryMethod();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateAbstractFactory();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateDependencyInjection();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateFactoryWithConfiguration();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateFactoryComparison();

        Console.WriteLine("\n=== Demo Complete ===");
    }

    /// <summary>
    /// Demonstrates Simple Factory Pattern
    /// Single class responsible for creating theme instances
    /// </summary>
    static void DemonstrateSimpleFactory()
    {
        Console.WriteLine("1. SIMPLE FACTORY PATTERN");
        Console.WriteLine("Single factory class creates theme instances based on type\n");

        // Simple Factory usage
        var darkTheme = SimpleThemeFactory.CreateTheme(ThemeType.Dark);
        darkTheme.Apply();
        Console.WriteLine($"   Background: {darkTheme.BackgroundColor}");
        Console.WriteLine($"   Text: {darkTheme.TextColor}");
        Console.WriteLine($"   Accent: {darkTheme.AccentColor}\n");

        var lightTheme = SimpleThemeFactory.CreateTheme(ThemeType.Light);
        lightTheme.Apply();
        Console.WriteLine($"   Background: {lightTheme.BackgroundColor}");
        Console.WriteLine($"   Text: {lightTheme.TextColor}");
        Console.WriteLine($"   Accent: {lightTheme.AccentColor}\n");

        var highContrastTheme = SimpleThemeFactory.CreateTheme(ThemeType.HighContrast);
        highContrastTheme.Apply();
        Console.WriteLine($"   Background: {highContrastTheme.BackgroundColor}");
        Console.WriteLine($"   Text: {highContrastTheme.TextColor}");
        Console.WriteLine($"   Accent: {highContrastTheme.AccentColor}");

        Console.WriteLine("\n✓ Simple Factory: Easy to use, but violates Open/Closed Principle");
    }

    /// <summary>
    /// Demonstrates Factory Method Pattern
    /// Subclasses decide which theme to create
    /// </summary>
    static void DemonstrateFactoryMethod()
    {
        Console.WriteLine("2. FACTORY METHOD PATTERN");
        Console.WriteLine("Subclasses override factory method to create specific themes\n");

        // Factory Method usage - Each creator creates a specific theme
        ThemeCreator darkCreator = new DarkThemeCreator();
        ITheme darkTheme = darkCreator.CreateTheme();
        darkCreator.RenderUI();
        Console.WriteLine($"   Created via: {darkCreator.GetType().Name}\n");

        ThemeCreator lightCreator = new LightThemeCreator();
        ITheme lightTheme = lightCreator.CreateTheme();
        lightCreator.RenderUI();
        Console.WriteLine($"   Created via: {lightCreator.GetType().Name}\n");

        ThemeCreator highContrastCreator = new HighContrastThemeCreator();
        ITheme highContrastTheme = highContrastCreator.CreateTheme();
        highContrastCreator.RenderUI();
        Console.WriteLine($"   Created via: {highContrastCreator.GetType().Name}");

        Console.WriteLine("\n✓ Factory Method: Follows Open/Closed Principle, extensible");
    }

    /// <summary>
    /// Demonstrates Abstract Factory Pattern
    /// Creates families of related UI components
    /// </summary>
    static void DemonstrateAbstractFactory()
    {
        Console.WriteLine("3. ABSTRACT FACTORY PATTERN");
        Console.WriteLine("Creates families of related UI components (Theme + Button + Panel)\n");

        // Abstract Factory usage - Creates complete UI component families
        IUIFactory darkFactory = new DarkUIFactory();
        ITheme darkTheme = darkFactory.CreateTheme();
        IButton darkButton = darkFactory.CreateButton();
        IPanel darkPanel = darkFactory.CreatePanel();

        Console.WriteLine("Dark UI Family:");
        darkTheme.Apply();
        darkButton.Render();
        darkPanel.Render();
        Console.WriteLine($"   Components work together with consistent styling\n");

        IUIFactory lightFactory = new LightUIFactory();
        ITheme lightTheme = lightFactory.CreateTheme();
        IButton lightButton = lightFactory.CreateButton();
        IPanel lightPanel = lightFactory.CreatePanel();

        Console.WriteLine("Light UI Family:");
        lightTheme.Apply();
        lightButton.Render();
        lightPanel.Render();
        Console.WriteLine($"   Components work together with consistent styling\n");

        IUIFactory highContrastFactory = new HighContrastUIFactory();
        ITheme highContrastTheme = highContrastFactory.CreateTheme();
        IButton highContrastButton = highContrastFactory.CreateButton();
        IPanel highContrastPanel = highContrastFactory.CreatePanel();

        Console.WriteLine("High Contrast UI Family:");
        highContrastTheme.Apply();
        highContrastButton.Render();
        highContrastPanel.Render();
        Console.WriteLine($"   Components work together with consistent styling");

        Console.WriteLine("\n✓ Abstract Factory: Creates consistent families of objects");
    }

    /// <summary>
    /// Demonstrates Dependency Injection integration with Factory Pattern
    /// </summary>
    static void DemonstrateDependencyInjection()
    {
        Console.WriteLine("4. DEPENDENCY INJECTION INTEGRATION");
        Console.WriteLine("Factory Pattern with .NET DI container\n");

        // Setup DI container
        var services = new ServiceCollection();

        // Register factories and themes
        services.AddSingleton<IUIFactory, DarkUIFactory>(); // Default factory
        services.AddTransient<ITheme>(sp =>
        {
            var factory = sp.GetRequiredService<IUIFactory>();
            return factory.CreateTheme();
        });
        services.AddTransient<IButton>(sp =>
        {
            var factory = sp.GetRequiredService<IUIFactory>();
            return factory.CreateButton();
        });
        services.AddTransient<IPanel>(sp =>
        {
            var factory = sp.GetRequiredService<IUIFactory>();
            return factory.CreatePanel();
        });

        // Register theme factory selector
        services.AddSingleton<ThemeFactorySelector>();

        // Register application
        services.AddTransient<ThemeApplication>();

        var serviceProvider = services.BuildServiceProvider();

        // Use the application
        var app = serviceProvider.GetRequiredService<ThemeApplication>();
        app.Run();

        Console.WriteLine("\n✓ DI Integration: Factory pattern works seamlessly with DI");
    }

    /// <summary>
    /// Demonstrates Factory with Configuration-based theme selection
    /// </summary>
    static void DemonstrateFactoryWithConfiguration()
    {
        Console.WriteLine("5. FACTORY WITH CONFIGURATION");
        Console.WriteLine("Select theme based on configuration/environment\n");

        var config = new ThemeConfiguration
        {
            ThemeType = ThemeType.Dark,
            EnableAnimations = true,
            FontSize = 14,
            Accessibility = AccessibilityLevel.Standard
        };

        var factory = new ConfigurableThemeFactory(config);
        var theme = factory.CreateConfiguredTheme();

        Console.WriteLine($"Configuration:");
        Console.WriteLine($"   Theme Type: {config.ThemeType}");
        Console.WriteLine($"   Animations: {config.EnableAnimations}");
        Console.WriteLine($"   Font Size: {config.FontSize}");
        Console.WriteLine($"   Accessibility: {config.Accessibility}\n");

        theme.Apply();
        Console.WriteLine($"   Theme applied based on configuration");

        // Change configuration
        Console.WriteLine("\nChanging configuration to High Contrast for accessibility...\n");
        config.ThemeType = ThemeType.HighContrast;
        config.Accessibility = AccessibilityLevel.High;
        config.FontSize = 18;

        var newTheme = factory.CreateConfiguredTheme();
        newTheme.Apply();
        Console.WriteLine($"   Theme updated dynamically based on new configuration");

        Console.WriteLine("\n✓ Configuration-based: Themes adapt to user preferences");
    }

    /// <summary>
    /// Compares all three factory patterns
    /// </summary>
    static void DemonstrateFactoryComparison()
    {
        Console.WriteLine("6. FACTORY PATTERN COMPARISON");
        Console.WriteLine("Understanding when to use each pattern\n");

        Console.WriteLine("Simple Factory:");
        Console.WriteLine("   ✓ Pros: Simple, centralized creation logic");
        Console.WriteLine("   ✗ Cons: Violates Open/Closed, grows with new types");
        Console.WriteLine("   Use when: Limited types, infrequent changes\n");

        Console.WriteLine("Factory Method:");
        Console.WriteLine("   ✓ Pros: Open/Closed compliant, extensible");
        Console.WriteLine("   ✗ Cons: More classes, can be over-engineered");
        Console.WriteLine("   Use when: Expecting new types, need extensibility\n");

        Console.WriteLine("Abstract Factory:");
        Console.WriteLine("   ✓ Pros: Consistent families, enforces compatibility");
        Console.WriteLine("   ✗ Cons: Complex, many interfaces/classes");
        Console.WriteLine("   Use when: Related objects must work together\n");

        // Performance comparison
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 100000; i++)
        {
            var theme = SimpleThemeFactory.CreateTheme(ThemeType.Dark);
        }
        sw.Stop();
        Console.WriteLine($"Simple Factory (100k iterations): {sw.ElapsedMilliseconds}ms");

        sw.Restart();
        for (int i = 0; i < 100000; i++)
        {
            ThemeCreator creator = new DarkThemeCreator();
            var theme = creator.CreateTheme();
        }
        sw.Stop();
        Console.WriteLine($"Factory Method (100k iterations): {sw.ElapsedMilliseconds}ms");

        sw.Restart();
        for (int i = 0; i < 100000; i++)
        {
            IUIFactory factory = new DarkUIFactory();
            var theme = factory.CreateTheme();
        }
        sw.Stop();
        Console.WriteLine($"Abstract Factory (100k iterations): {sw.ElapsedMilliseconds}ms");

        Console.WriteLine("\n✓ Performance: All patterns are fast, choose based on design needs");
    }
}

#region Theme Types and Configuration

/// <summary>
/// Available theme types
/// </summary>
public enum ThemeType
{
    Light,
    Dark,
    HighContrast
}

/// <summary>
/// Accessibility levels for themes
/// </summary>
public enum AccessibilityLevel
{
    Standard,
    Medium,
    High
}

/// <summary>
/// Theme configuration
/// </summary>
public class ThemeConfiguration
{
    public ThemeType ThemeType { get; set; } = ThemeType.Light;
    public bool EnableAnimations { get; set; } = true;
    public int FontSize { get; set; } = 12;
    public AccessibilityLevel Accessibility { get; set; } = AccessibilityLevel.Standard;
}

#endregion

#region 1. SIMPLE FACTORY PATTERN

/// <summary>
/// Theme interface - Contract for all themes
/// </summary>
public interface ITheme
{
    string Name { get; }
    string BackgroundColor { get; }
    string TextColor { get; }
    string AccentColor { get; }
    void Apply();
}

/// <summary>
/// Dark theme implementation
/// </summary>
public class DarkTheme : ITheme
{
    public string Name => "Dark Theme";
    public string BackgroundColor => "#1E1E1E";
    public string TextColor => "#FFFFFF";
    public string AccentColor => "#007ACC";

    public void Apply()
    {
        Console.WriteLine($"✓ Applied {Name}");
    }
}

/// <summary>
/// Light theme implementation
/// </summary>
public class LightTheme : ITheme
{
    public string Name => "Light Theme";
    public string BackgroundColor => "#FFFFFF";
    public string TextColor => "#000000";
    public string AccentColor => "#0078D4";

    public void Apply()
    {
        Console.WriteLine($"✓ Applied {Name}");
    }
}

/// <summary>
/// High contrast theme for accessibility
/// </summary>
public class HighContrastTheme : ITheme
{
    public string Name => "High Contrast Theme";
    public string BackgroundColor => "#000000";
    public string TextColor => "#FFFF00";
    public string AccentColor => "#00FF00";

    public void Apply()
    {
        Console.WriteLine($"✓ Applied {Name} (Accessibility Enhanced)");
    }
}

/// <summary>
/// Simple Factory - Creates themes based on type parameter
/// </summary>
public static class SimpleThemeFactory
{
    /// <summary>
    /// Creates a theme instance based on the specified type
    /// </summary>
    /// <param name="type">The type of theme to create</param>
    /// <returns>A theme instance</returns>
    /// <exception cref="ArgumentException">Thrown when type is not supported</exception>
    public static ITheme CreateTheme(ThemeType type)
    {
        return type switch
        {
            ThemeType.Dark => new DarkTheme(),
            ThemeType.Light => new LightTheme(),
            ThemeType.HighContrast => new HighContrastTheme(),
            _ => throw new ArgumentException($"Unknown theme type: {type}", nameof(type))
        };
    }

    /// <summary>
    /// Creates a theme based on user preference
    /// </summary>
    public static ITheme CreateThemeFromUserPreference(bool isDarkMode, bool needsAccessibility)
    {
        if (needsAccessibility)
        {
            return new HighContrastTheme();
        }

        return isDarkMode ? new DarkTheme() : new LightTheme();
    }
}

#endregion

#region 2. FACTORY METHOD PATTERN

/// <summary>
/// Abstract Theme Creator - Defines factory method
/// </summary>
public abstract class ThemeCreator
{
    /// <summary>
    /// Factory Method - Subclasses override to create specific themes
    /// </summary>
    public abstract ITheme CreateTheme();

    /// <summary>
    /// Template method that uses the factory method
    /// </summary>
    public void RenderUI()
    {
        var theme = CreateTheme();
        theme.Apply();
        Console.WriteLine($"   UI rendered with {theme.Name}");
    }
}

/// <summary>
/// Concrete Creator for Dark Theme
/// </summary>
public class DarkThemeCreator : ThemeCreator
{
    public override ITheme CreateTheme()
    {
        return new DarkTheme();
    }
}

/// <summary>
/// Concrete Creator for Light Theme
/// </summary>
public class LightThemeCreator : ThemeCreator
{
    public override ITheme CreateTheme()
    {
        return new LightTheme();
    }
}

/// <summary>
/// Concrete Creator for High Contrast Theme
/// </summary>
public class HighContrastThemeCreator : ThemeCreator
{
    public override ITheme CreateTheme()
    {
        return new HighContrastTheme();
    }
}

#endregion

#region 3. ABSTRACT FACTORY PATTERN

/// <summary>
/// Button interface for UI components
/// </summary>
public interface IButton
{
    void Render();
    void Click();
}

/// <summary>
/// Panel interface for UI components
/// </summary>
public interface IPanel
{
    void Render();
    void AddContent(string content);
}

/// <summary>
/// Abstract UI Factory - Creates families of related UI objects
/// </summary>
public interface IUIFactory
{
    ITheme CreateTheme();
    IButton CreateButton();
    IPanel CreatePanel();
}

/// <summary>
/// Dark UI component implementations
/// </summary>
public class DarkButton : IButton
{
    public void Render()
    {
        Console.WriteLine("   [Dark Button] Background: #2D2D30, Border: #3F3F46");
    }

    public void Click()
    {
        Console.WriteLine("   Dark button clicked!");
    }
}

public class DarkPanel : IPanel
{
    private readonly List<string> _content = new();

    public void Render()
    {
        Console.WriteLine("   [Dark Panel] Background: #252526, Border: #3F3F46");
    }

    public void AddContent(string content)
    {
        _content.Add(content);
    }
}

/// <summary>
/// Light UI component implementations
/// </summary>
public class LightButton : IButton
{
    public void Render()
    {
        Console.WriteLine("   [Light Button] Background: #F3F3F3, Border: #CCCCCC");
    }

    public void Click()
    {
        Console.WriteLine("   Light button clicked!");
    }
}

public class LightPanel : IPanel
{
    private readonly List<string> _content = new();

    public void Render()
    {
        Console.WriteLine("   [Light Panel] Background: #FFFFFF, Border: #E0E0E0");
    }

    public void AddContent(string content)
    {
        _content.Add(content);
    }
}

/// <summary>
/// High Contrast UI component implementations
/// </summary>
public class HighContrastButton : IButton
{
    public void Render()
    {
        Console.WriteLine("   [High Contrast Button] Background: #000000, Border: #FFFF00");
    }

    public void Click()
    {
        Console.WriteLine("   High contrast button clicked!");
    }
}

public class HighContrastPanel : IPanel
{
    private readonly List<string> _content = new();

    public void Render()
    {
        Console.WriteLine("   [High Contrast Panel] Background: #000000, Border: #00FF00");
    }

    public void AddContent(string content)
    {
        _content.Add(content);
    }
}

/// <summary>
/// Dark UI Factory - Creates dark-themed component family
/// </summary>
public class DarkUIFactory : IUIFactory
{
    public ITheme CreateTheme() => new DarkTheme();
    public IButton CreateButton() => new DarkButton();
    public IPanel CreatePanel() => new DarkPanel();
}

/// <summary>
/// Light UI Factory - Creates light-themed component family
/// </summary>
public class LightUIFactory : IUIFactory
{
    public ITheme CreateTheme() => new LightTheme();
    public IButton CreateButton() => new LightButton();
    public IPanel CreatePanel() => new LightPanel();
}

/// <summary>
/// High Contrast UI Factory - Creates high contrast component family
/// </summary>
public class HighContrastUIFactory : IUIFactory
{
    public ITheme CreateTheme() => new HighContrastTheme();
    public IButton CreateButton() => new HighContrastButton();
    public IPanel CreatePanel() => new HighContrastPanel();
}

#endregion

#region 4. DEPENDENCY INJECTION INTEGRATION

/// <summary>
/// Theme factory selector for DI scenarios
/// </summary>
public class ThemeFactorySelector
{
    private readonly Dictionary<ThemeType, Func<IUIFactory>> _factories;

    public ThemeFactorySelector()
    {
        _factories = new Dictionary<ThemeType, Func<IUIFactory>>
        {
            [ThemeType.Dark] = () => new DarkUIFactory(),
            [ThemeType.Light] = () => new LightUIFactory(),
            [ThemeType.HighContrast] = () => new HighContrastUIFactory()
        };
    }

    public IUIFactory GetFactory(ThemeType type)
    {
        if (_factories.TryGetValue(type, out var factoryFunc))
        {
            return factoryFunc();
        }

        throw new ArgumentException($"No factory registered for {type}", nameof(type));
    }
}

/// <summary>
/// Sample application using DI with factory pattern
/// </summary>
public class ThemeApplication
{
    private readonly ITheme _theme;
    private readonly IButton _button;
    private readonly IPanel _panel;

    public ThemeApplication(ITheme theme, IButton button, IPanel panel)
    {
        _theme = theme ?? throw new ArgumentNullException(nameof(theme));
        _button = button ?? throw new ArgumentNullException(nameof(button));
        _panel = panel ?? throw new ArgumentNullException(nameof(panel));
    }

    public void Run()
    {
        Console.WriteLine("Theme Application Started:");
        _theme.Apply();
        _button.Render();
        _panel.Render();
        Console.WriteLine("   All components injected via DI and working together!");
    }
}

#endregion

#region 5. CONFIGURABLE FACTORY

/// <summary>
/// Configurable theme factory that adapts to configuration
/// </summary>
public class ConfigurableThemeFactory
{
    private readonly ThemeConfiguration _configuration;

    public ConfigurableThemeFactory(ThemeConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Creates a theme based on current configuration
    /// </summary>
    public ITheme CreateConfiguredTheme()
    {
        // Accessibility override
        if (_configuration.Accessibility == AccessibilityLevel.High)
        {
            return new HighContrastTheme();
        }

        return _configuration.ThemeType switch
        {
            ThemeType.Dark => new DarkTheme(),
            ThemeType.Light => new LightTheme(),
            ThemeType.HighContrast => new HighContrastTheme(),
            _ => new LightTheme() // Default
        };
    }

    /// <summary>
    /// Creates a complete UI factory based on configuration
    /// </summary>
    public IUIFactory CreateConfiguredUIFactory()
    {
        // Accessibility override
        if (_configuration.Accessibility == AccessibilityLevel.High)
        {
            return new HighContrastUIFactory();
        }

        return _configuration.ThemeType switch
        {
            ThemeType.Dark => new DarkUIFactory(),
            ThemeType.Light => new LightUIFactory(),
            ThemeType.HighContrast => new HighContrastUIFactory(),
            _ => new LightUIFactory() // Default
        };
    }

    /// <summary>
    /// Updates configuration and returns new theme
    /// </summary>
    public ITheme UpdateAndCreateTheme(ThemeType newType)
    {
        _configuration.ThemeType = newType;
        return CreateConfiguredTheme();
    }
}

#endregion
