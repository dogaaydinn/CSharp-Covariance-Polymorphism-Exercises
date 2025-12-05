# Factory Pattern - UI Theme Factory

**Category**: Creational Design Pattern
**Level**: Advanced
**Complexity**: ⭐⭐⭐⭐ (4/5)

## Overview

This project demonstrates **three variations of the Factory Pattern** using a real-world UI Theme system scenario. The Factory Pattern is a creational design pattern that provides an interface for creating objects in a superclass, but allows subclasses to alter the type of objects that will be created.

### What You'll Learn

1. **Simple Factory** - Centralized object creation with static methods
2. **Factory Method** - Template method pattern for extensible factories
3. **Abstract Factory** - Creating families of related objects
4. **Dependency Injection Integration** - Using factories with DI containers
5. **Configuration-Based Creation** - Runtime theme selection
6. **Performance Comparison** - Understanding the overhead of each pattern

## Project Statistics

- **Lines of Code**: 757
- **Factory Patterns**: 3 (Simple, Factory Method, Abstract)
- **Themes**: 3 (Dark, Light, High Contrast)
- **UI Components**: 3 per theme (Theme, Button, Panel)
- **Demonstrations**: 6 scenarios

## Key Features

### 1. Simple Factory Pattern ✅
- Single static class with creation logic
- Easy to use, minimal boilerplate
- Good for limited product types
- **Trade-off**: Violates Open/Closed Principle

### 2. Factory Method Pattern ✅
- Abstract creator class with factory method
- Subclasses decide which class to instantiate
- Follows Open/Closed Principle
- **Trade-off**: More classes to maintain

### 3. Abstract Factory Pattern ✅
- Creates families of related objects
- Ensures consistency across product families
- Perfect for theme systems
- **Trade-off**: Increased complexity

### 4. Dependency Injection Support ✅
- Integration with Microsoft.Extensions.DependencyInjection
- Factory pattern works seamlessly with DI
- Service registration examples

### 5. Configuration-Based Creation ✅
- Runtime theme selection based on config
- Accessibility overrides
- Dynamic theme switching

### 6. Performance Benchmarks ✅
- 100k iterations benchmark for each pattern
- Real performance metrics
- Informed decision making

## Quick Start

### Prerequisites

- .NET 8.0 SDK
- Basic understanding of interfaces and inheritance
- Familiarity with dependency injection (optional)

### Running the Demo

```bash
cd samples/03-Advanced/FactoryPattern
dotnet restore
dotnet build
dotnet run
```

### Expected Output

```
=== Factory Pattern Demo - UI Theme Factory ===

1. SIMPLE FACTORY PATTERN
Single factory class creates theme instances based on type

✓ Applied Dark Theme
   Background: #1E1E1E
   Text: #FFFFFF
   Accent: #007ACC

✓ Applied Light Theme
   Background: #FFFFFF
   Text: #000000
   Accent: #0078D4

✓ Simple Factory: Easy to use, but violates Open/Closed Principle

============================================================

2. FACTORY METHOD PATTERN
Subclasses override factory method to create specific themes

✓ Applied Dark Theme
   UI rendered with Dark Theme
   Created via: DarkThemeCreator

✓ Factory Method: Follows Open/Closed Principle, extensible

============================================================

3. ABSTRACT FACTORY PATTERN
Creates families of related UI components (Theme + Button + Panel)

Dark UI Family:
✓ Applied Dark Theme
   [Dark Button] Background: #2D2D30, Border: #3F3F46
   [Dark Panel] Background: #252526, Border: #3F3F46
   Components work together with consistent styling

✓ Abstract Factory: Creates consistent families of objects

============================================================

... (and more demonstrations)
```

## Core Concepts

### 1. Simple Factory

**Definition**: A single class responsible for creating instances based on parameters.

```csharp
// Simple Factory - Static method creates objects
public static class SimpleThemeFactory
{
    public static ITheme CreateTheme(ThemeType type)
    {
        return type switch
        {
            ThemeType.Dark => new DarkTheme(),
            ThemeType.Light => new LightTheme(),
            ThemeType.HighContrast => new HighContrastTheme(),
            _ => throw new ArgumentException($"Unknown theme type: {type}")
        };
    }
}

// Usage
var theme = SimpleThemeFactory.CreateTheme(ThemeType.Dark);
theme.Apply();
```

**When to use**:
- Limited number of product types
- Creation logic is simple
- Infrequent changes to product types

### 2. Factory Method

**Definition**: An abstract class defines a factory method, subclasses implement it to create specific objects.

```csharp
// Factory Method - Abstract creator
public abstract class ThemeCreator
{
    // Factory method - subclasses override
    public abstract ITheme CreateTheme();

    // Template method using factory method
    public void RenderUI()
    {
        var theme = CreateTheme();
        theme.Apply();
    }
}

// Concrete creator
public class DarkThemeCreator : ThemeCreator
{
    public override ITheme CreateTheme() => new DarkTheme();
}

// Usage
ThemeCreator creator = new DarkThemeCreator();
creator.RenderUI(); // Creates and applies Dark Theme
```

**When to use**:
- Expecting new product types
- Need extensibility without modifying existing code
- Single product hierarchy

### 3. Abstract Factory

**Definition**: Creates families of related objects without specifying their concrete classes.

```csharp
// Abstract Factory - Creates families of objects
public interface IUIFactory
{
    ITheme CreateTheme();
    IButton CreateButton();
    IPanel CreatePanel();
}

// Concrete factory for Dark theme family
public class DarkUIFactory : IUIFactory
{
    public ITheme CreateTheme() => new DarkTheme();
    public IButton CreateButton() => new DarkButton();
    public IPanel CreatePanel() => new DarkPanel();
}

// Usage
IUIFactory factory = new DarkUIFactory();
var theme = factory.CreateTheme();
var button = factory.CreateButton();
var panel = factory.CreatePanel();

// All components are guaranteed to work together
theme.Apply();
button.Render();
panel.Render();
```

**When to use**:
- Multiple related products must work together
- Need to enforce consistency across product families
- Complex UI systems with themes

## Architecture

### Class Diagram (Simplified)

```
┌─────────────────────────────────────────────────────────┐
│                  SIMPLE FACTORY                         │
│                                                         │
│  ┌──────────────────────┐                              │
│  │ SimpleThemeFactory   │                              │
│  │   (static class)     │                              │
│  ├──────────────────────┤                              │
│  │ + CreateTheme(type)  │──────► ITheme                │
│  └──────────────────────┘            ▲                 │
│                                       │                 │
│                         ┌─────────────┼─────────────┐   │
│                         │             │             │   │
│                   DarkTheme    LightTheme   HighContrastTheme
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│                 FACTORY METHOD                          │
│                                                         │
│  ┌──────────────────────┐                              │
│  │   ThemeCreator       │                              │
│  │   (abstract)         │                              │
│  ├──────────────────────┤                              │
│  │ + CreateTheme() ◄──┐ │                              │
│  │ + RenderUI()       │ │                              │
│  └──────────────────────┘                              │
│           ▲            │                                │
│           │    ┌───────┴────────┬──────────────┐       │
│           │    │                │              │       │
│    DarkThemeCreator  LightThemeCreator  HighContrastThemeCreator
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│               ABSTRACT FACTORY                          │
│                                                         │
│  ┌──────────────────────┐                              │
│  │    IUIFactory        │                              │
│  ├──────────────────────┤                              │
│  │ + CreateTheme()      │                              │
│  │ + CreateButton()     │                              │
│  │ + CreatePanel()      │                              │
│  └──────────────────────┘                              │
│           ▲                                             │
│           │                                             │
│    ┌──────┴──────┬─────────────┐                       │
│    │             │             │                       │
│ DarkUIFactory LightUIFactory HighContrastUIFactory     │
│    │             │             │                       │
│    Creates:      Creates:      Creates:                │
│    - DarkTheme   - LightTheme  - HighContrastTheme     │
│    - DarkButton  - LightButton - HighContrastButton    │
│    - DarkPanel   - LightPanel  - HighContrastPanel     │
└─────────────────────────────────────────────────────────┘
```

### Theme Component Hierarchy

```
ITheme
├── DarkTheme
├── LightTheme
└── HighContrastTheme

IButton
├── DarkButton
├── LightButton
└── HighContrastButton

IPanel
├── DarkPanel
├── LightPanel
└── HighContrastPanel
```

## Usage Examples

### Example 1: Simple Factory for Quick Theme Creation

```csharp
// Create themes based on user preference
bool isDarkMode = true;
bool needsAccessibility = false;

var theme = SimpleThemeFactory.CreateThemeFromUserPreference(
    isDarkMode,
    needsAccessibility
);

theme.Apply();
Console.WriteLine($"Applied: {theme.Name}");
// Output: Applied: Dark Theme
```

### Example 2: Factory Method for Extensibility

```csharp
// Add new theme creator without modifying existing code
public class CustomThemeCreator : ThemeCreator
{
    public override ITheme CreateTheme() => new CustomTheme();
}

// Use it
ThemeCreator creator = new CustomThemeCreator();
creator.RenderUI(); // Creates and applies Custom Theme
```

### Example 3: Abstract Factory for Complete UI

```csharp
// Create entire UI with consistent theming
IUIFactory factory = new DarkUIFactory();

// All components work together
var theme = factory.CreateTheme();
var button = factory.CreateButton();
var panel = factory.CreatePanel();

theme.Apply();
button.Render();
panel.Render();

// Output:
// ✓ Applied Dark Theme
// [Dark Button] Background: #2D2D30, Border: #3F3F46
// [Dark Panel] Background: #252526, Border: #3F3F46
```

### Example 4: Dependency Injection Integration

```csharp
// Setup DI container
var services = new ServiceCollection();

// Register factory as singleton
services.AddSingleton<IUIFactory, DarkUIFactory>();

// Register products as transient
services.AddTransient<ITheme>(sp =>
    sp.GetRequiredService<IUIFactory>().CreateTheme());
services.AddTransient<IButton>(sp =>
    sp.GetRequiredService<IUIFactory>().CreateButton());

// Build and use
var serviceProvider = services.BuildServiceProvider();
var theme = serviceProvider.GetRequiredService<ITheme>();
theme.Apply();
```

### Example 5: Configuration-Based Theme Selection

```csharp
// Configuration object
var config = new ThemeConfiguration
{
    ThemeType = ThemeType.Dark,
    EnableAnimations = true,
    FontSize = 14,
    Accessibility = AccessibilityLevel.Standard
};

// Factory adapts to configuration
var factory = new ConfigurableThemeFactory(config);
var theme = factory.CreateConfiguredTheme();
theme.Apply();

// Change configuration at runtime
config.ThemeType = ThemeType.HighContrast;
config.Accessibility = AccessibilityLevel.High;

var newTheme = factory.CreateConfiguredTheme();
newTheme.Apply(); // Applies High Contrast Theme
```

## Best Practices

### DO ✅

1. **Use Simple Factory when**:
   - You have a limited number of product types
   - Creation logic is straightforward
   - Changes to product types are infrequent

2. **Use Factory Method when**:
   - You expect new product types to be added
   - You want to follow the Open/Closed Principle
   - You have a single product hierarchy

3. **Use Abstract Factory when**:
   - Products must work together as a family
   - You need to enforce consistency
   - You have multiple related product hierarchies

4. **Return interfaces, not concrete classes**:
   ```csharp
   // Good
   public ITheme CreateTheme() => new DarkTheme();

   // Bad
   public DarkTheme CreateTheme() => new DarkTheme();
   ```

5. **Use dependency injection with factories**:
   ```csharp
   services.AddSingleton<IUIFactory, DarkUIFactory>();
   ```

### DON'T ❌

1. **Don't use Simple Factory if**:
   - You frequently add new product types
   - Creation logic becomes complex
   - You need strict adherence to SOLID principles

2. **Don't over-engineer with Factory Method**:
   ```csharp
   // Bad - unnecessary for 2 types
   public abstract class StringFormatterCreator
   {
       public abstract IFormatter CreateFormatter();
   }
   ```

3. **Don't use Abstract Factory for unrelated products**:
   ```csharp
   // Bad - unrelated products
   public interface IBadFactory
   {
       ITheme CreateTheme();
       ILogger CreateLogger();  // Unrelated!
       ICache CreateCache();    // Unrelated!
   }
   ```

4. **Don't expose factory internals**:
   ```csharp
   // Bad
   public class ThemeFactory
   {
       public DarkTheme darkThemeInstance; // Exposed!
   }
   ```

## Common Patterns

### Pattern 1: Factory with Registration

```csharp
public class ThemeFactoryRegistry
{
    private readonly Dictionary<ThemeType, Func<ITheme>> _factories = new();

    public void Register(ThemeType type, Func<ITheme> factory)
    {
        _factories[type] = factory;
    }

    public ITheme Create(ThemeType type)
    {
        if (_factories.TryGetValue(type, out var factory))
        {
            return factory();
        }
        throw new ArgumentException($"No factory for {type}");
    }
}

// Usage
var registry = new ThemeFactoryRegistry();
registry.Register(ThemeType.Dark, () => new DarkTheme());
registry.Register(ThemeType.Light, () => new LightTheme());

var theme = registry.Create(ThemeType.Dark);
```

### Pattern 2: Lazy Factory

```csharp
public class LazyThemeFactory
{
    private readonly Lazy<ITheme> _darkTheme = new(() => new DarkTheme());
    private readonly Lazy<ITheme> _lightTheme = new(() => new LightTheme());

    public ITheme CreateTheme(ThemeType type)
    {
        return type switch
        {
            ThemeType.Dark => _darkTheme.Value,
            ThemeType.Light => _lightTheme.Value,
            _ => throw new ArgumentException()
        };
    }
}
```

### Pattern 3: Factory with Caching

```csharp
public class CachedThemeFactory
{
    private readonly Dictionary<ThemeType, ITheme> _cache = new();

    public ITheme CreateTheme(ThemeType type)
    {
        if (_cache.TryGetValue(type, out var theme))
        {
            return theme;
        }

        theme = type switch
        {
            ThemeType.Dark => new DarkTheme(),
            ThemeType.Light => new LightTheme(),
            _ => throw new ArgumentException()
        };

        _cache[type] = theme;
        return theme;
    }
}
```

## Use Cases

### 1. UI Theme Systems
**Scenario**: Application with multiple themes (Dark, Light, High Contrast)

**Why Factory Pattern**:
- Users can switch themes at runtime
- All UI components must match the selected theme
- Abstract Factory ensures consistency across components

**Implementation**: See Abstract Factory demonstration in Program.cs:111-144

### 2. Database Connection Factories
**Scenario**: Multi-database application (SQL Server, PostgreSQL, MySQL)

```csharp
public interface IDatabaseFactory
{
    IConnection CreateConnection();
    ICommand CreateCommand();
    ITransaction CreateTransaction();
}

public class SqlServerFactory : IDatabaseFactory
{
    public IConnection CreateConnection() => new SqlConnection();
    public ICommand CreateCommand() => new SqlCommand();
    public ITransaction CreateTransaction() => new SqlTransaction();
}
```

### 3. Document Export Systems
**Scenario**: Export data to multiple formats (PDF, Excel, Word)

```csharp
public abstract class DocumentExporter
{
    public abstract IDocument CreateDocument();

    public void Export(Data data)
    {
        var document = CreateDocument();
        document.Write(data);
        document.Save();
    }
}

public class PdfExporter : DocumentExporter
{
    public override IDocument CreateDocument() => new PdfDocument();
}
```

### 4. Logging Frameworks
**Scenario**: Different logging destinations (Console, File, Database)

```csharp
public static class LoggerFactory
{
    public static ILogger CreateLogger(LoggerType type)
    {
        return type switch
        {
            LoggerType.Console => new ConsoleLogger(),
            LoggerType.File => new FileLogger(),
            LoggerType.Database => new DatabaseLogger(),
            _ => throw new ArgumentException()
        };
    }
}
```

## Performance Considerations

### Benchmark Results (100,000 iterations)

```
Simple Factory:      ~15ms
Factory Method:      ~18ms
Abstract Factory:    ~20ms
```

**Key Insights**:
1. **Performance difference is negligible** for most applications
2. **Choose based on design needs**, not performance
3. **Simple Factory is fastest** but least flexible
4. **Abstract Factory overhead** is minimal (~5ms per 100k calls)

### Memory Considerations

```csharp
// Bad - Creates new instance every time
public ITheme GetTheme()
{
    return new DarkTheme(); // New allocation
}

// Good - Reuse singleton
private static readonly ITheme _darkTheme = new DarkTheme();
public ITheme GetTheme() => _darkTheme;

// Best - Lazy singleton
private static readonly Lazy<ITheme> _darkTheme = new(() => new DarkTheme());
public ITheme GetTheme() => _darkTheme.Value;
```

## Common Pitfalls

### Pitfall 1: Tight Coupling in Simple Factory

**Problem**:
```csharp
// Bad - Violates Open/Closed Principle
public static class ThemeFactory
{
    public static ITheme CreateTheme(string type)
    {
        if (type == "dark") return new DarkTheme();
        if (type == "light") return new LightTheme();
        // Must modify to add new theme ❌
        throw new ArgumentException();
    }
}
```

**Solution**: Use Factory Method or Abstract Factory for extensibility.

### Pitfall 2: Over-Engineering

**Problem**:
```csharp
// Bad - Over-engineered for simple case
public abstract class StringFormatterFactory
{
    public abstract IStringFormatter CreateFormatter();
}

public class UpperCaseFormatterFactory : StringFormatterFactory
{
    public override IStringFormatter CreateFormatter()
        => new UpperCaseFormatter();
}

// Just use: string.ToUpper() ❌
```

**Solution**: Use factories only when creation logic is complex or types vary.

### Pitfall 3: Mixing Concerns

**Problem**:
```csharp
// Bad - Factory doing too much
public class ThemeFactory
{
    public ITheme CreateTheme(ThemeType type)
    {
        var theme = new DarkTheme();
        theme.Apply();          // ❌ Should not apply
        SaveToDatabase(theme);  // ❌ Should not persist
        return theme;
    }
}
```

**Solution**: Factories should only create objects, not use them.

## Related Patterns

### vs. Builder Pattern

**Factory Pattern**:
- Creates complete objects in one step
- Different types of objects
- Example: `CreateTheme(ThemeType.Dark)`

**Builder Pattern**:
- Creates complex objects step-by-step
- Same type, different configurations
- Example: `new ThemeBuilder().SetBackground("#000").SetText("#FFF").Build()`

### vs. Prototype Pattern

**Factory Pattern**:
- Creates new instances from scratch
- Uses constructors

**Prototype Pattern**:
- Clones existing instances
- Uses `Clone()` method

### vs. Singleton Pattern

**Factory Pattern**:
- Creates multiple instances
- Can return different types

**Singleton Pattern**:
- Ensures single instance
- Always returns same object

## Testing

### Unit Testing Factories

```csharp
[Fact]
public void SimpleFactory_CreateDarkTheme_ReturnsDarkTheme()
{
    // Act
    var theme = SimpleThemeFactory.CreateTheme(ThemeType.Dark);

    // Assert
    Assert.NotNull(theme);
    Assert.IsType<DarkTheme>(theme);
    Assert.Equal("Dark Theme", theme.Name);
}

[Fact]
public void AbstractFactory_DarkUIFactory_CreatesConsistentFamily()
{
    // Arrange
    IUIFactory factory = new DarkUIFactory();

    // Act
    var theme = factory.CreateTheme();
    var button = factory.CreateButton();
    var panel = factory.CreatePanel();

    // Assert
    Assert.IsType<DarkTheme>(theme);
    Assert.IsType<DarkButton>(button);
    Assert.IsType<DarkPanel>(panel);
}
```

## Learning Path

### Beginner (Week 1-2)
1. Understand the problem: Why not use `new` everywhere?
2. Implement Simple Factory
3. Practice with different product types

### Intermediate (Week 3-4)
1. Learn Factory Method pattern
2. Understand Open/Closed Principle
3. Refactor Simple Factory to Factory Method

### Advanced (Week 5-6)
1. Master Abstract Factory pattern
2. Integrate with Dependency Injection
3. Build production-ready theme systems

### Expert (Week 7+)
1. Combine with other patterns (Strategy, Builder)
2. Implement plugin architectures
3. Design extensible frameworks

## Summary

### When to Use Each Pattern

| Pattern | Use When | Avoid When |
|---------|----------|------------|
| **Simple Factory** | Limited types, simple logic | Frequent changes, need SOLID compliance |
| **Factory Method** | Extensibility needed, single product | Over-engineering simple cases |
| **Abstract Factory** | Related products must work together | Products are unrelated |

### Key Takeaways

1. ✅ **Factory Pattern decouples object creation from usage**
2. ✅ **Simple Factory is easiest but least flexible**
3. ✅ **Factory Method follows Open/Closed Principle**
4. ✅ **Abstract Factory ensures product family consistency**
5. ✅ **Choose based on design needs, not performance**
6. ✅ **Works great with Dependency Injection**
7. ✅ **Avoid over-engineering for simple cases**

## Next Steps

1. Run the demo: `dotnet run`
2. Read WHY_THIS_PATTERN.md for deeper understanding
3. Modify the code: Add a new theme (e.g., Solarized)
4. Implement your own factory for a different domain
5. Explore related patterns: Builder, Prototype, Singleton

## References

- **Program.cs:46-71** - Simple Factory demonstration
- **Program.cs:77-99** - Factory Method demonstration
- **Program.cs:105-144** - Abstract Factory demonstration
- **Program.cs:150-188** - Dependency Injection integration
- **Program.cs:194-229** - Configuration-based factory

## Additional Resources

- Design Patterns: Elements of Reusable Object-Oriented Software (Gang of Four)
- Refactoring: Improving the Design of Existing Code (Martin Fowler)
- Clean Code: A Handbook of Agile Software Craftsmanship (Robert C. Martin)

---

**Project**: Advanced C# Learning Platform
**Pattern**: Factory Pattern (Simple, Factory Method, Abstract)
**Scenario**: UI Theme Factory with Dark, Light, and High Contrast themes
