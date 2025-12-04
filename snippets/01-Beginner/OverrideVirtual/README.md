# Override Virtual - Method Overriding Mastery

**Level:** Beginner  
**Topics:** virtual, override, new, abstract, sealed  
**Estimated Time:** 30 minutes

## 🎯 Learning Objectives

Master the five key keywords for method customization in C#:
- ✅ **virtual/override** - Polymorphic behavior (the right way)
- ✅ **new** - Method hiding (usually avoid this)
- ✅ **abstract** - Enforcing implementation
- ✅ **sealed** - Preventing further overrides
- ✅ When to use each keyword

## 🚀 Quick Start

```bash
cd samples/01-Beginner/OverrideVirtual
dotnet run
```

## 📚 Core Concepts

### 1. Virtual & Override (Polymorphism)

**virtual** marks a method as overridable:
```csharp
class Shape
{
    public virtual double CalculateArea() => 0;
}
```

**override** replaces the base implementation:
```csharp
class Circle : Shape
{
    public double Radius { get; set; }
    public override double CalculateArea() => Math.PI * Radius * Radius;
}
```

**Result:** Polymorphic behavior - correct method called at runtime!

### 2. Method Hiding with 'new' (⚠️ Rarely Desired)

```csharp
class Base
{
    public virtual void Display() => Console.WriteLine("Base");
}

class Derived : Base
{
    public new void Display() => Console.WriteLine("Derived");  // HIDES, not overrides
}

// Problem:
Base baseRef = new Derived();
baseRef.Display();  // Prints "Base" ❌ NOT polymorphic!
```

**❌ 'new' gives compile-time binding (static dispatch)**  
**✅ 'override' gives runtime binding (dynamic dispatch)**

### 3. Abstract Methods (Must Override)

```csharp
abstract class Animal
{
    public abstract void MakeSound();  // No implementation!
}

class Dog : Animal
{
    public override void MakeSound() => Console.WriteLine("Woof!");  // MUST implement
}
```

**When to use:**
- Base class cannot provide meaningful implementation
- You want to enforce that derived classes implement the method
- Creating frameworks/templates

### 4. Sealed Methods (No Further Overrides)

```csharp
class Middle : Base
{
    public sealed override void Process() { }  // Seals the override chain
}

class Final : Middle
{
    // public override void Process() { }  // ❌ Won't compile - sealed!
}
```

**When to use:**
- Prevent further customization in inheritance chain
- Security/correctness reasons
- Performance optimization

## 💡 Decision Tree

```
Need to customize base class method?
├─ Base has no implementation?
│  └─ Use: abstract (forces override)
│
├─ Base has default implementation?
│  ├─ Want polymorphism?
│  │  └─ Use: virtual/override ✅
│  │
│  └─ Don't want polymorphism?
│     └─ Use: new (⚠️ rarely needed)
│
└─ Want to stop override chain?
   └─ Use: sealed override
```

## 📊 Comparison Table

| Keyword | Purpose | Polymorphic? | Must Override? |
|---------|---------|--------------|----------------|
| **virtual** | Allow override | ✅ Yes | ❌ No |
| **override** | Replace implementation | ✅ Yes | N/A |
| **new** | Hide base method | ❌ No | ❌ No |
| **abstract** | Force override | ✅ Yes | ✅ Yes |
| **sealed override** | Prevent further override | ✅ Yes (current) | N/A |

## ⚠️ Common Pitfalls

### Pitfall 1: Forgetting 'virtual'
```csharp
class Base
{
    public void Method() { }  // ❌ Not virtual!
}

class Derived : Base
{
    public override void Method() { }  // ❌ Won't compile - base not virtual!
}
```

**Fix:** Add `virtual` to base method.

### Pitfall 2: Using 'new' When You Meant 'override'
```csharp
Base baseRef = new Derived();
baseRef.Method();  // ❌ Calls Base.Method(), not Derived!
```

**Fix:** Use `override` instead of `new`.

### Pitfall 3: Abstract Without Abstract Class
```csharp
class Regular  // ❌ Not abstract!
{
    public abstract void Method();  // ❌ Won't compile!
}
```

**Fix:** Make class abstract: `abstract class Regular`.

## 🎓 Best Practices

1. **Default to virtual/override** for polymorphism
2. **Avoid 'new'** unless you specifically need method hiding (rare)
3. **Use abstract** for template methods that must be implemented
4. **Use sealed** sparingly - only when you have good reason
5. **Document why** you're using 'new' or 'sealed' (unusual choices)

## 🔬 Real-World Example

The sample includes a payment processing system demonstrating:
- Abstract base class (`PaymentProcessor`)
- Virtual methods with default implementation
- Override in multiple derived classes
- Polymorphic collection processing

```csharp
PaymentProcessor[] processors = {
    new CreditCardProcessor(),
    new PayPalProcessor(),
    new CryptoProcessor()
};

foreach (var processor in processors)
{
    processor.ProcessPayment(100);  // Polymorphic!
}
```

## 📈 Code Statistics

- **Examples:** 5 comprehensive scenarios
- **Classes:** 15+ demonstrating different patterns
- **Lines:** ~250 of educational code

## 🔗 Related Topics

- **Polymorphism:** See `PolymorphismExamples` sample
- **Interfaces:** More flexible polymorphism
- **Casting:** See `CastingExamples` sample

## 🎉 Summary

You've mastered:
- ✅ **virtual/override** for polymorphism
- ✅ **new** for method hiding (and why to avoid it)
- ✅ **abstract** for enforcing implementation
- ✅ **sealed** for preventing overrides
- ✅ When to use each approach

**Next:** Explore `GenericConstraints` to see how generics work with inheritance!

---

**Created:** 2025-12-01  
**C# Version:** 12.0 (.NET 8.0)  
**Difficulty:** ⭐⭐☆☆☆
