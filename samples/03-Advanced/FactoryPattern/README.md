# Factory Pattern

**Konu**: Creational pattern - Object creation abstraction

## Öğrenilen Kavramlar

- ✅ Simple Factory (static factory)
- ✅ Factory Method (virtual constructor)
- ✅ Abstract Factory (families of objects)
- ✅ Dependency Inversion via factories

## Kullanım

```bash
dotnet run
```

## Pattern Türleri

### 1. Simple Factory
En basit - static method ile object creation
```csharp
Theme theme = ThemeFactory.CreateTheme("dark");
```

### 2. Factory Method
Interface ile - subclass'lar factory metodunu implement eder
```csharp
IThemeFactory factory = new DarkThemeFactory();
IButton button = factory.CreateButton();
```

### 3. Abstract Factory
Aileler - related objects'i birlikte üretir
```csharp
IUIFactory factory = new ModernDarkUIFactory();
Application app = new Application(factory);
```

## Ne Zaman Kullanılır?

- Object creation logic karmaşık
- Runtime'da tip seçimi gerekli
- İlgili object'leri birlikte üretmek istiyorsun
- Concrete class'lardan bağımsız olmak istiyorsun

## Örnekler

```
=== Factory Pattern Demo ===

❌ BAD APPROACH:
Dark Button
Dark Textbox

✅ SIMPLE FACTORY:
✅ Dark themed button
✅ Light themed button

✅ ABSTRACT FACTORY:
✅ [Modern] Dark button with shadows
✅ [Classic] Light button simple
```
