# Why Factory Pattern?

## Problem

❌ Direct instantiation problems:
```csharp
public class Application
{
    public void Initialize()
    {
        var button = new DarkButton(); // Tight coupling!
        var textbox = new DarkTextbox();

        // Theme değiştirmek için tüm kodu değiştir
    }
}
```

## Çözüm: Factory Pattern

### Simple Factory
✅ Basit object creation centralization
```csharp
var theme = ThemeFactory.CreateTheme("dark");
// Tek yerden kontrol
```

### Factory Method
✅ Subclass'lar creation logic'i override eder
```csharp
public abstract class ThemeFactory
{
    public abstract IButton CreateButton();
}
```

### Abstract Factory
✅ İlgili object families
```csharp
IUIFactory factory = GetFactory(); // Runtime decision
Application app = new Application(factory);
```

## Faydalar

✅ **Loose Coupling**: Client, concrete class bilmiyor
✅ **Single Responsibility**: Creation logic ayrı
✅ **Open/Closed**: Yeni tip eklemek kolay
✅ **Dependency Inversion**: Interface'e bağımlı

## Gerçek Dünya Örnekleri

- **ASP.NET Core**: `IServiceProvider` (DI container is a factory)
- **Entity Framework**: `DbContext` factory
- **HttpClientFactory**: HTTP client creation
- **LoggerFactory**: Logger instances

## Anti-Patterns

❌ **Over-engineering**: Her nesne için factory yaratma
❌ **God Factory**: Tek factory her şeyi üretiyor
❌ **Leaky Abstraction**: Factory, implementation detayı sızdırıyor

## Seçim Kılavuzu

| Durum | Pattern |
|-------|---------|
| 2-3 tip var | Simple Factory |
| Her alt sınıf kendi tipini üretir | Factory Method |
| İlgili object families | Abstract Factory |
| Complex creation logic | Builder Pattern |
