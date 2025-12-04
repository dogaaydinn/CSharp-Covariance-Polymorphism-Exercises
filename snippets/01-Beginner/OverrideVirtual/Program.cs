using System;

namespace OverrideVirtual;

/// <summary>
/// Comprehensive demonstration of virtual, override, new, abstract, and sealed keywords.
/// Shows how method hiding and overriding work in C# inheritance hierarchies.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        PrintHeader();
        
        DemonstrateVirtualOverride();
        DemonstrateMethodHiding();
        DemonstrateAbstractMethods();
        DemonstrateSealedMethods();
        DemonstrateRealWorldScenario();
        
        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine("Tutorial Complete! 🎉");
        Console.WriteLine(new string('=', 70) + "\n");
    }
    
    static void PrintHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║          VIRTUAL, OVERRIDE, NEW, ABSTRACT & SEALED           ║");
        Console.WriteLine("║            Master Method Overriding in C#                    ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }
    
    static void DemonstrateVirtualOverride()
    {
        Console.WriteLine(new string('=', 70));
        Console.WriteLine("1. VIRTUAL & OVERRIDE - Polymorphic Behavior");
        Console.WriteLine(new string('=', 70) + "\n");
        
        // Virtual methods can be overridden in derived classes
        Shape shape1 = new Circle { Radius = 5 };
        Shape shape2 = new Rectangle { Width = 4, Height = 6 };
        
        Console.WriteLine("✅ Polymorphism in action:");
        Console.WriteLine($"Circle area: {shape1.CalculateArea():F2}");     // Calls Circle.CalculateArea()
        Console.WriteLine($"Rectangle area: {shape2.CalculateArea():F2}");  // Calls Rectangle.CalculateArea()
        
        // Base implementation still accessible via base keyword
        var circle = new Circle { Radius = 10 };
        circle.ShowInfo();  // Calls overridden method, which uses base.ShowInfo()
        
        Console.WriteLine("\n💡 Key Points:");
        Console.WriteLine("  - 'virtual' allows method to be overridden");
        Console.WriteLine("  - 'override' replaces base implementation");
        Console.WriteLine("  - Runtime determines which method to call (dynamic dispatch)");
        Console.WriteLine("  - Use 'base.Method()' to call parent implementation\n");
    }
    
    static void DemonstrateMethodHiding()
    {
        Console.WriteLine(new string('=', 70));
        Console.WriteLine("2. METHOD HIDING with 'new' - Shadowing Base Methods");
        Console.WriteLine(new string('=', 70) + "\n");
        
        // Method hiding vs overriding
        BaseClass baseRef = new DerivedWithNew();
        DerivedWithNew derivedRef = new DerivedWithNew();
        
        Console.WriteLine("❌ Method HIDING with 'new' (not polymorphic):");
        baseRef.Display();      // Calls BaseClass.Display() - NOT polymorphic!
        derivedRef.Display();    // Calls DerivedWithNew.Display()
        
        Console.WriteLine("\n✅ Method OVERRIDING with 'override' (polymorphic):");
        BaseClass baseRef2 = new DerivedWithOverride();
        baseRef2.Display2();     // Calls DerivedWithOverride.Display2() - polymorphic!
        
        Console.WriteLine("\n💡 Key Difference:");
        Console.WriteLine("  - 'new' HIDES the base method (compile-time binding)");
        Console.WriteLine("  - 'override' REPLACES the base method (runtime binding)");
        Console.WriteLine("  - ALWAYS prefer 'override' for polymorphic behavior");
        Console.WriteLine("  - 'new' is rarely what you want!\n");
    }
    
    static void DemonstrateAbstractMethods()
    {
        Console.WriteLine(new string('=', 70));
        Console.WriteLine("3. ABSTRACT METHODS - Enforcing Implementation");
        Console.WriteLine(new string('=', 70) + "\n");
        
        // Abstract classes cannot be instantiated
        // Animal animal = new Animal();  // ❌ Won't compile!
        
        Animal dog = new Dog { Name = "Buddy" };
        Animal cat = new Cat { Name = "Whiskers" };
        
        Console.WriteLine("✅ Abstract methods MUST be overridden:");
        dog.MakeSound();  // Dog MUST implement MakeSound()
        cat.MakeSound();  // Cat MUST implement MakeSound()
        
        Console.WriteLine("\n💡 Key Points:");
        Console.WriteLine("  - Abstract methods have NO implementation in base class");
        Console.WriteLine("  - Derived classes MUST override abstract methods");
        Console.WriteLine("  - Abstract classes cannot be instantiated");
        Console.WriteLine("  - Use when base class cannot provide meaningful implementation\n");
    }
    
    static void DemonstrateSealedMethods()
    {
        Console.WriteLine(new string('=', 70));
        Console.WriteLine("4. SEALED METHODS - Preventing Further Overrides");
        Console.WriteLine(new string('=', 70) + "\n");
        
        MiddleClass middle = new MiddleClass();
        FinalClass final = new FinalClass();
        
        Console.WriteLine("✅ Sealed method prevents further overriding:");
        middle.Process();  // MiddleClass overrides and seals
        final.Process();   // FinalClass CANNOT override (sealed in MiddleClass)
        
        Console.WriteLine("\n💡 Key Points:");
        Console.WriteLine("  - 'sealed override' stops override chain");
        Console.WriteLine("  - Useful when you want to prevent further customization");
        Console.WriteLine("  - Can also seal entire classes to prevent inheritance\n");
    }
    
    static void DemonstrateRealWorldScenario()
    {
        Console.WriteLine(new string('=', 70));
        Console.WriteLine("5. REAL-WORLD SCENARIO - Payment Processing");
        Console.WriteLine(new string('=', 70) + "\n");
        
        PaymentProcessor[] processors = 
        {
            new CreditCardProcessor(),
            new PayPalProcessor(),
            new CryptoProcessor()
        };
        
        decimal amount = 100.00m;
        
        Console.WriteLine($"Processing ${amount} through different payment methods:\n");
        foreach (var processor in processors)
        {
            Console.WriteLine($"Method: {processor.GetType().Name}");
            processor.ProcessPayment(amount);  // Polymorphic call
            Console.WriteLine();
        }
        
        Console.WriteLine("💡 This shows real polymorphism:");
        Console.WriteLine("  - Single interface (PaymentProcessor)");
        Console.WriteLine("  - Multiple implementations (Credit, PayPal, Crypto)");
        Console.WriteLine("  - Runtime dispatch to correct method");
        Console.WriteLine("  - New payment types can be added without changing existing code\n");
    }
}

// Example 1: Virtual and Override
abstract class Shape
{
    public virtual double CalculateArea() => 0;
    public virtual void ShowInfo() => Console.WriteLine("  Base: I'm a generic shape");
}

class Circle : Shape
{
    public double Radius { get; set; }
    public override double CalculateArea() => Math.PI * Radius * Radius;
    public override void ShowInfo()
    {
        base.ShowInfo();  // Call base implementation
        Console.WriteLine($"  Override: I'm a circle with radius {Radius}");
    }
}

class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }
    public override double CalculateArea() => Width * Height;
}

// Example 2: Method Hiding
class BaseClass
{
    public virtual void Display() => Console.WriteLine("  BaseClass.Display()");
    public virtual void Display2() => Console.WriteLine("  BaseClass.Display2()");
}

class DerivedWithNew : BaseClass
{
    public new void Display() => Console.WriteLine("  DerivedWithNew.Display() - using 'new'");
}

class DerivedWithOverride : BaseClass
{
    public override void Display2() => Console.WriteLine("  DerivedWithOverride.Display2() - using 'override'");
}

// Example 3: Abstract Methods
abstract class Animal
{
    public string Name { get; set; } = "";
    public abstract void MakeSound();  // MUST be overridden
    public virtual void Sleep() => Console.WriteLine($"  {Name} is sleeping...");
}

class Dog : Animal
{
    public override void MakeSound() => Console.WriteLine($"  {Name} says: Woof!");
}

class Cat : Animal
{
    public override void MakeSound() => Console.WriteLine($"  {Name} says: Meow!");
}

// Example 4: Sealed Methods
abstract class BaseProcessor
{
    public abstract void Process();
}

class MiddleClass : BaseProcessor
{
    public sealed override void Process() => Console.WriteLine("  MiddleClass.Process() - SEALED!");
}

class FinalClass : MiddleClass
{
    // Cannot override Process() here - it's sealed!
    // public override void Process() { }  // ❌ Won't compile
}

// Example 5: Real-World Scenario
abstract class PaymentProcessor
{
    public abstract string GetPaymentMethod();
    public abstract bool ValidatePayment(decimal amount);
    
    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine($"  Payment Method: {GetPaymentMethod()}");
        
        if (ValidatePayment(amount))
        {
            Console.WriteLine($"  ✅ Processing ${amount:F2}...");
            PerformTransaction(amount);
            Console.WriteLine($"  ✅ Payment successful!");
        }
        else
        {
            Console.WriteLine($"  ❌ Payment validation failed!");
        }
    }
    
    protected virtual void PerformTransaction(decimal amount) 
        => Console.WriteLine($"  → Transferring ${amount:F2}...");
}

class CreditCardProcessor : PaymentProcessor
{
    public override string GetPaymentMethod() => "Credit Card";
    public override bool ValidatePayment(decimal amount) => amount <= 10000;  // $10k limit
    protected override void PerformTransaction(decimal amount)
    {
        Console.WriteLine($"  → Charging credit card...");
        base.PerformTransaction(amount);
    }
}

class PayPalProcessor : PaymentProcessor
{
    public override string GetPaymentMethod() => "PayPal";
    public override bool ValidatePayment(decimal amount) => amount <= 5000;  // $5k limit
    protected override void PerformTransaction(decimal amount)
    {
        Console.WriteLine($"  → Connecting to PayPal API...");
        base.PerformTransaction(amount);
    }
}

class CryptoProcessor : PaymentProcessor
{
    public override string GetPaymentMethod() => "Cryptocurrency";
    public override bool ValidatePayment(decimal amount) => amount <= 50000;  // $50k limit
    protected override void PerformTransaction(decimal amount)
    {
        Console.WriteLine($"  → Broadcasting blockchain transaction...");
        base.PerformTransaction(amount);
    }
}
