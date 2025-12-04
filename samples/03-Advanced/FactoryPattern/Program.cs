// Factory Pattern: Simple Factory, Factory Method, Abstract Factory

namespace FactoryPattern;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Factory Pattern Demo ===\n");

        // ❌ BAD: Direct instantiation everywhere
        Console.WriteLine("❌ BAD APPROACH:");
        var darkButton1 = new DarkButton();
        var darkTextbox1 = new DarkTextbox();
        // Hard to switch themes, code duplication
        darkButton1.Render();
        darkTextbox1.Render();

        // ✅ GOOD: Simple Factory
        Console.WriteLine("\n✅ SIMPLE FACTORY:");
        var darkTheme = ThemeFactory.CreateTheme("dark");
        darkTheme.Button.Render();
        darkTheme.Textbox.Render();

        var lightTheme = ThemeFactory.CreateTheme("light");
        lightTheme.Button.Render();
        lightTheme.Textbox.Render();

        // ✅ BETTER: Factory Method
        Console.WriteLine("\n✅ FACTORY METHOD:");
        IThemeFactory darkFactory = new DarkThemeFactory();
        IThemeFactory lightFactory = new LightThemeFactory();

        var btn1 = darkFactory.CreateButton();
        var btn2 = lightFactory.CreateButton();
        btn1.Render();
        btn2.Render();

        // ✅ BEST: Abstract Factory
        Console.WriteLine("\n✅ ABSTRACT FACTORY:");
        IUIFactory modernDark = new ModernDarkUIFactory();
        IUIFactory classicLight = new ClassicLightUIFactory();

        Application app1 = new(modernDark);
        Application app2 = new(classicLight);

        app1.RenderUI();
        app2.RenderUI();

        Console.WriteLine("\n=== Factory Patterns Compared ===");
    }
}

// BEFORE REFACTORING (Bad)
public class DarkButton
{
    public void Render() => Console.WriteLine("Dark Button");
}

public class DarkTextbox
{
    public void Render() => Console.WriteLine("Dark Textbox");
}

public class LightButton
{
    public void Render() => Console.WriteLine("Light Button");
}

public class LightTextbox
{
    public void Render() => Console.WriteLine("Light Textbox");
}

// AFTER REFACTORING 1: Simple Factory
public interface IButton
{
    void Render();
}

public interface ITextbox
{
    void Render();
}

public class DarkThemedButton : IButton
{
    public void Render() => Console.WriteLine("✅ Dark themed button");
}

public class DarkThemedTextbox : ITextbox
{
    public void Render() => Console.WriteLine("✅ Dark themed textbox");
}

public class LightThemedButton : IButton
{
    public void Render() => Console.WriteLine("✅ Light themed button");
}

public class LightThemedTextbox : ITextbox
{
    public void Render() => Console.WriteLine("✅ Light themed textbox");
}

public record Theme(IButton Button, ITextbox Textbox);

// Simple Factory
public static class ThemeFactory
{
    public static Theme CreateTheme(string type)
    {
        return type.ToLower() switch
        {
            "dark" => new Theme(new DarkThemedButton(), new DarkThemedTextbox()),
            "light" => new Theme(new LightThemedButton(), new LightThemedTextbox()),
            _ => throw new ArgumentException("Unknown theme")
        };
    }
}

// AFTER REFACTORING 2: Factory Method
public interface IThemeFactory
{
    IButton CreateButton();
    ITextbox CreateTextbox();
}

public class DarkThemeFactory : IThemeFactory
{
    public IButton CreateButton() => new DarkThemedButton();
    public ITextbox CreateTextbox() => new DarkThemedTextbox();
}

public class LightThemeFactory : IThemeFactory
{
    public IButton CreateButton() => new LightThemedButton();
    public ITextbox CreateTextbox() => new LightThemedTextbox();
}

// AFTER REFACTORING 3: Abstract Factory
public interface IUIFactory
{
    IButton CreateButton();
    ITextbox CreateTextbox();
    ICheckbox CreateCheckbox();
}

public interface ICheckbox
{
    void Render();
}

public class ModernDarkUIFactory : IUIFactory
{
    public IButton CreateButton() => new ModernDarkButton();
    public ITextbox CreateTextbox() => new ModernDarkTextbox();
    public ICheckbox CreateCheckbox() => new ModernDarkCheckbox();
}

public class ClassicLightUIFactory : IUIFactory
{
    public IButton CreateButton() => new ClassicLightButton();
    public ITextbox CreateTextbox() => new ClassicLightTextbox();
    public ICheckbox CreateCheckbox() => new ClassicLightCheckbox();
}

public class ModernDarkButton : IButton
{
    public void Render() => Console.WriteLine("✅ [Modern] Dark button with shadows");
}

public class ModernDarkTextbox : ITextbox
{
    public void Render() => Console.WriteLine("✅ [Modern] Dark textbox with rounded corners");
}

public class ModernDarkCheckbox : ICheckbox
{
    public void Render() => Console.WriteLine("✅ [Modern] Dark checkbox with animation");
}

public class ClassicLightButton : IButton
{
    public void Render() => Console.WriteLine("✅ [Classic] Light button simple");
}

public class ClassicLightTextbox : ITextbox
{
    public void Render() => Console.WriteLine("✅ [Classic] Light textbox bordered");
}

public class ClassicLightCheckbox : ICheckbox
{
    public void Render() => Console.WriteLine("✅ [Classic] Light checkbox square");
}

// Application using Abstract Factory
public class Application
{
    private readonly IUIFactory _factory;

    public Application(IUIFactory factory)
    {
        _factory = factory;
    }

    public void RenderUI()
    {
        var button = _factory.CreateButton();
        var textbox = _factory.CreateTextbox();
        var checkbox = _factory.CreateCheckbox();

        button.Render();
        textbox.Render();
        checkbox.Render();
    }
}

// BENCHMARK COMPARISON
// Pattern          | Flexibility | Complexity | Use Case
// -----------------|-------------|------------|---------------------------
// Direct           | Low         | Low        | Simple, no variation
// Simple Factory   | Medium      | Low        | Few product families
// Factory Method   | High        | Medium     | Single product, many types
// Abstract Factory | Highest     | High       | Product families, consistency
