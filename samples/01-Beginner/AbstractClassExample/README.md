# Abstract Class Example - Abstract Class vs Interface

## 📚 Konu
Abstract class ve interface arasındaki farklar, ne zaman hangisini kullanmalı.

## 🎯 Amaç
Abstract class ve interface'in farklı kullanım senaryolarını, güçlü ve zayıf yönlerini öğrenmek.

## 🔑 Anahtar Kavramlar
- **Abstract Class**: State (fields) + behavior (methods) içerir, single inheritance
- **Interface**: Sadece contract tanımlar, multiple inheritance
- **Single Inheritance**: Bir class sadece 1 base class'tan türeyebilir
- **Multiple Inheritance**: Bir class birden fazla interface implement edebilir
- **IS-A Relationship**: Abstract class için (Circle IS-A Shape)
- **CAN-DO Relationship**: Interface için (Shape CAN-BE drawn)

## 💻 Kullanım

```bash
cd samples/01-Beginner/AbstractClassExample
dotnet run
```

## 🎓 Öğrenilen Kavramlar

### 1. Abstract Class
```csharp
public abstract class Shape
{
    // ✅ Fields (state)
    public string Color { get; set; }

    // ✅ Constructor
    protected Shape(string color) { Color = color; }

    // ✅ Abstract method
    public abstract double CalculateArea();

    // ✅ Concrete method
    public void DisplayInfo() { }
}

public class Circle : Shape
{
    public override double CalculateArea() { }
}
```

### 2. Interface
```csharp
public interface IDrawable
{
    // ❌ Fields yok
    // ❌ Constructor yok

    // ✅ Method signatures
    void Draw();
    void Erase();
}

public class Point : IDrawable
{
    public void Draw() { }
    public void Erase() { }
}
```

### 3. Multiple Inheritance
```csharp
// ✅ Multiple interface implementation
public class ColoredCircle : Shape, IDrawable, IMeasurable
{
    // Shape'ten gelen abstract metodlar
    public override double CalculateArea() { }

    // IDrawable interface
    public void Draw() { }
    public void Erase() { }
}
```

## 📊 Abstract Class vs Interface

| Özellik | Abstract Class | Interface |
|---------|---------------|-----------|
| **Fields** | ✅ Var | ❌ Yok |
| **Constructor** | ✅ Var | ❌ Yok |
| **Concrete Methods** | ✅ Var | ⚠️ C# 8+ |
| **Abstract Methods** | ✅ Var | ✅ Var |
| **Access Modifiers** | ✅ Var | ❌ Public only |
| **Multiple Inheritance** | ❌ Single | ✅ Multiple |
| **Static Members** | ✅ Var | ⚠️ C# 8+ |

## 🎯 Ne Zaman Kullanmalı?

### Abstract Class Kullan:
- ✅ IS-A ilişkisi (Circle IS-A Shape)
- ✅ Ortak state paylaşımı
- ✅ Ortak behavior (concrete methods)
- ✅ Constructor logic
- ✅ Access modifiers gerekli

### Interface Kullan:
- ✅ CAN-DO ilişkisi (CAN-BE drawn)
- ✅ Multiple inheritance
- ✅ Sadece contract
- ✅ Farklı hierarchy'ler
- ✅ Dependency Injection

### İkisini Birlikte Kullan:
- ✅ En esnek yaklaşım
- ✅ Abstract class: state/behavior
- ✅ Interface: contract/capability

## 💡 Best Practices

1. **Abstract class için IS-A test et**: "Circle IS-A Shape" mantıklı mı?
2. **Interface için CAN-DO test et**: "Shape CAN-BE drawn" mantıklı mı?
3. **Ortak state varsa abstract class kullan**
4. **Multiple inheritance gerekiyorsa interface kullan**
5. **İkisini birlikte kullan** (en güçlü yaklaşım)
