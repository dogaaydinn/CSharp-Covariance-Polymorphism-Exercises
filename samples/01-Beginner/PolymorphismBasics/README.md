# Polymorphism Basics

## 🎯 Learning Objectives

By completing this tutorial, you will:
- ✅ Understand what polymorphism means in C#
- ✅ Learn the difference between compile-time and runtime polymorphism
- ✅ Master method overriding with virtual and override keywords
- ✅ Understand when to use polymorphism in real applications

## 📚 What is Polymorphism?

**Polymorphism** (Greek: "many forms") allows objects to take multiple forms. In C#, this means:
- A derived class object can be treated as a base class object
- Method calls are resolved based on the actual object type (not the reference type)

### Real-World Analogy

Think of a **payment system**:
- You can pay by credit card, PayPal, or crypto
- All are "payments" but behave differently
- The cashier treats them all as "Payment" objects
- Each payment method knows how to process itself

## 🔍 Key Concepts

### 1. Runtime Polymorphism (Method Overriding)

```csharp
public class Animal
{
    public virtual void MakeSound() => Console.WriteLine("Some generic sound");
}

public class Dog : Animal
{
    public override void MakeSound() => Console.WriteLine("Woof!");
}

// Usage:
Animal myPet = new Dog();  // Dog object, Animal reference
myPet.MakeSound();         // Output: "Woof!" (runtime polymorphism)
```

**Key Points:**
- Base class method must be `virtual`
- Derived class uses `override` keyword
- Method resolution happens at **runtime**

### 2. Compile-Time Polymorphism (Method Overloading)

```csharp
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public double Add(double a, double b) => a + b;
    public string Add(string a, string b) => a + b;
}

// Usage:
var calc = new Calculator();
calc.Add(1, 2);           // Calls int version
calc.Add(1.5, 2.5);       // Calls double version
calc.Add("Hello", "!");   // Calls string version
```

**Key Points:**
- Same method name, different parameters
- Method resolution happens at **compile-time**

## 💻 This Sample Demonstrates

1. **Payment Processing System** - Real-world polymorphism example
2. **Shape Calculations** - Classic polymorphism tutorial
3. **Notification System** - Practical business example
4. **Abstract Classes vs Interfaces** - When to use which

## 🚀 Running the Sample

```bash
cd samples/01-Beginner/PolymorphismBasics
dotnet run
```

## 📖 Code Walkthrough

### Example 1: Payment System

```csharp
// Base class
public abstract class Payment
{
    public decimal Amount { get; set; }
    public abstract void ProcessPayment();

    // Common behavior (not overridden)
    public void ValidateAmount()
    {
        if (Amount <= 0)
            throw new ArgumentException("Amount must be positive");
    }
}

// Derived classes
public class CreditCardPayment : Payment
{
    public string CardNumber { get; set; }

    public override void ProcessPayment()
    {
        ValidateAmount();
        Console.WriteLine($"Processing credit card payment: ${Amount}");
        // Credit card specific logic
    }
}

public class PayPalPayment : Payment
{
    public string Email { get; set; }

    public override void ProcessPayment()
    {
        ValidateAmount();
        Console.WriteLine($"Processing PayPal payment: ${Amount} to {Email}");
        // PayPal specific logic
    }
}

// Usage - this is the power of polymorphism!
public void ProcessPayments(List<Payment> payments)
{
    foreach (var payment in payments)
    {
        payment.ProcessPayment();  // Calls correct method based on actual type
    }
}
```

### Example 2: Shape Calculations

```csharp
public abstract class Shape
{
    public abstract double CalculateArea();
    public abstract double CalculatePerimeter();
}

public class Circle : Shape
{
    public double Radius { get; set; }

    public override double CalculateArea()
        => Math.PI * Radius * Radius;

    public override double CalculatePerimeter()
        => 2 * Math.PI * Radius;
}

public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public override double CalculateArea()
        => Width * Height;

    public override double CalculatePerimeter()
        => 2 * (Width + Height);
}

// Usage
List<Shape> shapes = new()
{
    new Circle { Radius = 5 },
    new Rectangle { Width = 10, Height = 20 }
};

foreach (var shape in shapes)
{
    Console.WriteLine($"Area: {shape.CalculateArea()}");
    Console.WriteLine($"Perimeter: {shape.CalculatePerimeter()}");
}
```

## 🎓 Key Takeaways

### When to Use Polymorphism

✅ **Use polymorphism when:**
- You have multiple classes that share common behavior but differ in implementation
- You want to treat different types uniformly
- You need extensibility (easy to add new types without changing existing code)
- You're implementing design patterns (Strategy, Factory, etc.)

❌ **Don't use polymorphism when:**
- Classes don't share meaningful common behavior
- Performance is absolutely critical (virtual calls have tiny overhead)
- You're dealing with simple data structures

### Virtual vs Abstract

| Feature | Virtual | Abstract |
|---------|---------|----------|
| Implementation | Has default implementation | No implementation |
| Must override? | Optional | Required |
| Can be in non-abstract class? | Yes | No |
| Use when | Providing default behavior | Forcing derived classes to implement |

## 🔬 Experiment!

Try modifying the code:

1. **Add a new payment method** (Cryptocurrency?)
2. **Add a new shape** (Triangle?)
3. **What happens if you remove `virtual`?**
4. **What happens if you remove `override`?**
5. **Can you use `new` keyword instead of `override`?** (Hint: Yes, but it's different!)

## 🐛 Common Mistakes

### Mistake 1: Forgetting `virtual` in base class
```csharp
// ❌ Wrong
public class Animal
{
    public void MakeSound() => Console.WriteLine("Sound");  // Not virtual!
}

// ✅ Correct
public class Animal
{
    public virtual void MakeSound() => Console.WriteLine("Sound");
}
```

### Mistake 2: Using `new` instead of `override`
```csharp
// ⚠️ Hiding, not overriding
public class Dog : Animal
{
    public new void MakeSound() => Console.WriteLine("Woof");
}

Animal myDog = new Dog();
myDog.MakeSound();  // Output: "Sound" (not "Woof"!) - calls base implementation

// ✅ Correct - use override
public class Dog : Animal
{
    public override void MakeSound() => Console.WriteLine("Woof");
}
```

### Mistake 3: Calling base method by accident
```csharp
// ❌ Infinite recursion!
public override void ProcessPayment()
{
    ProcessPayment();  // Calls itself infinitely!
}

// ✅ Correct - call base if needed
public override void ProcessPayment()
{
    base.ProcessPayment();  // Explicit base call
    // Additional logic
}
```

## 📚 Further Reading

- [Polymorphism in C# (Microsoft Docs)](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/polymorphism)
- [Virtual Methods (C# Reference)](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/virtual)
- [Override Keyword (C# Reference)](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/override)

## ⏭️ Next Steps

After completing this tutorial, move on to:
- [02-CastingExamples](../CastingExamples/) - Learn upcasting and downcasting
- [03-OverrideVirtual](../OverrideVirtual/) - Deep dive into virtual methods

---

**Completed?** ✅ Mark this in your learning tracker!

**Questions?** Open an [issue](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/issues)
