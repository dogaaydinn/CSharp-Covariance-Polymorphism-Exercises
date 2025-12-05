// Decorator Pattern - Coffee Ordering System
// Demonstrates how to add responsibilities to objects dynamically

namespace DecoratorPattern;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Decorator Pattern Demo - Coffee Ordering System ===\n");

        DemonstrateBasicDecoratorPattern();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateDynamicDecoration();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateCoffeeShopMenu();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateDecoratorChaining();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateMultipleDecorations();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateDecoratorOrdering();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateProblemWithoutDecorator();

        Console.WriteLine("\n=== Demo Complete ===");
    }

    // 1. Basic Decorator Pattern
    static void DemonstrateBasicDecoratorPattern()
    {
        Console.WriteLine("1. BASIC DECORATOR PATTERN");
        Console.WriteLine("Add responsibilities to objects dynamically\n");

        // Start with base component
        ICoffee espresso = new Espresso();
        Console.WriteLine($"Base Coffee:");
        Console.WriteLine($"   {espresso.GetDescription()}");
        Console.WriteLine($"   Cost: ${espresso.GetCost():F2}\n");

        // Decorate with milk
        ICoffee espressoWithMilk = new MilkDecorator(new Espresso());
        Console.WriteLine($"Decorated Coffee (Milk):");
        Console.WriteLine($"   {espressoWithMilk.GetDescription()}");
        Console.WriteLine($"   Cost: ${espressoWithMilk.GetCost():F2}\n");

        // Decorate with multiple decorators
        ICoffee fancyEspresso = new WhippedCreamDecorator(
            new SugarDecorator(
                new MilkDecorator(
                    new Espresso()
                )
            )
        );
        Console.WriteLine($"Multi-Decorated Coffee:");
        Console.WriteLine($"   {fancyEspresso.GetDescription()}");
        Console.WriteLine($"   Cost: ${fancyEspresso.GetCost():F2}");

        Console.WriteLine("\n✓ Each decorator adds functionality without modifying the component");
    }

    // 2. Dynamic Decoration at Runtime
    static void DemonstrateDynamicDecoration()
    {
        Console.WriteLine("2. DYNAMIC DECORATION AT RUNTIME");
        Console.WriteLine("Build coffee based on customer preferences\n");

        // Customer 1: Simple preferences
        Console.WriteLine("Customer 1: \"I want an espresso with milk\"");
        ICoffee order1 = new Espresso();
        order1 = new MilkDecorator(order1);
        PrintOrder(order1);

        Console.WriteLine();

        // Customer 2: More complex preferences
        Console.WriteLine("Customer 2: \"Dark roast with sugar and whipped cream\"");
        ICoffee order2 = new DarkRoast();
        order2 = new SugarDecorator(order2);
        order2 = new WhippedCreamDecorator(order2);
        PrintOrder(order2);

        Console.WriteLine();

        // Customer 3: Very specific preferences
        Console.WriteLine("Customer 3: \"Decaf with milk, sugar, caramel, and whipped cream\"");
        ICoffee order3 = new Decaf();
        order3 = new MilkDecorator(order3);
        order3 = new SugarDecorator(order3);
        order3 = new CaramelDecorator(order3);
        order3 = new WhippedCreamDecorator(order3);
        PrintOrder(order3);

        Console.WriteLine("\n✓ Decorators can be added dynamically based on runtime conditions");
    }

    // 3. Coffee Shop Menu with All Combinations
    static void DemonstrateCoffeeShopMenu()
    {
        Console.WriteLine("3. COFFEE SHOP MENU");
        Console.WriteLine("Popular combinations with calculated prices\n");

        var menu = new[]
        {
            ("Simple Espresso", CreateCoffee(new Espresso())),
            ("Latte (Espresso + Milk)", CreateCoffee(new Espresso(), milk: true)),
            ("Cappuccino (Espresso + Milk + Whipped Cream)", CreateCoffee(new Espresso(), milk: true, whippedCream: true)),
            ("Sweet Coffee (Espresso + Sugar)", CreateCoffee(new Espresso(), sugar: true)),
            ("Caramel Macchiato", CreateCoffee(new Espresso(), milk: true, caramel: true)),
            ("Mocha (Dark Roast + Milk + Whipped Cream)", CreateCoffee(new DarkRoast(), milk: true, whippedCream: true)),
            ("Decaf Delight (Decaf + Milk + Sugar + Caramel)", CreateCoffee(new Decaf(), milk: true, sugar: true, caramel: true)),
            ("Supreme Coffee (All Toppings)", CreateCoffee(new DarkRoast(), milk: true, sugar: true, caramel: true, whippedCream: true))
        };

        foreach (var (name, coffee) in menu)
        {
            Console.WriteLine($"{name,-45} ${coffee.GetCost():F2}");
        }

        Console.WriteLine("\n✓ Same base components, different decorations = flexible menu");
    }

    // 4. Decorator Chaining
    static void DemonstrateDecoratorChaining()
    {
        Console.WriteLine("4. DECORATOR CHAINING");
        Console.WriteLine("Each decorator wraps the previous one\n");

        Console.WriteLine("Building coffee step by step:\n");

        ICoffee coffee = new Espresso();
        Console.WriteLine($"1. Start:           {coffee.GetDescription(),-40} ${coffee.GetCost():F2}");

        coffee = new MilkDecorator(coffee);
        Console.WriteLine($"2. Add Milk:        {coffee.GetDescription(),-40} ${coffee.GetCost():F2}");

        coffee = new SugarDecorator(coffee);
        Console.WriteLine($"3. Add Sugar:       {coffee.GetDescription(),-40} ${coffee.GetCost():F2}");

        coffee = new CaramelDecorator(coffee);
        Console.WriteLine($"4. Add Caramel:     {coffee.GetDescription(),-40} ${coffee.GetCost():F2}");

        coffee = new WhippedCreamDecorator(coffee);
        Console.WriteLine($"5. Add Whipped Cream: {coffee.GetDescription(),-40} ${coffee.GetCost():F2}");

        Console.WriteLine("\n✓ Each decorator adds to the chain, calculating cost cumulatively");
    }

    // 5. Multiple Decorations of Same Type
    static void DemonstrateMultipleDecorations()
    {
        Console.WriteLine("5. MULTIPLE DECORATIONS OF SAME TYPE");
        Console.WriteLine("Can apply same decorator multiple times\n");

        ICoffee coffee = new Espresso();
        Console.WriteLine($"Base:               {coffee.GetDescription(),-30} ${coffee.GetCost():F2}");

        // Add extra milk (double milk)
        coffee = new MilkDecorator(coffee);
        Console.WriteLine($"Add Milk:           {coffee.GetDescription(),-30} ${coffee.GetCost():F2}");

        coffee = new MilkDecorator(coffee);
        Console.WriteLine($"Add Extra Milk:     {coffee.GetDescription(),-30} ${coffee.GetCost():F2}");

        // Add triple sugar
        coffee = new SugarDecorator(coffee);
        Console.WriteLine($"Add Sugar:          {coffee.GetDescription(),-30} ${coffee.GetCost():F2}");

        coffee = new SugarDecorator(coffee);
        Console.WriteLine($"Add More Sugar:     {coffee.GetDescription(),-30} ${coffee.GetCost():F2}");

        coffee = new SugarDecorator(coffee);
        Console.WriteLine($"Add Even More Sugar: {coffee.GetDescription(),-30} ${coffee.GetCost():F2}");

        Console.WriteLine("\n✓ Same decorator can be applied multiple times for cumulative effect");
    }

    // 6. Decorator Ordering Matters
    static void DemonstrateDecoratorOrdering()
    {
        Console.WriteLine("6. DECORATOR ORDERING");
        Console.WriteLine("Order doesn't affect cost, but may affect description\n");

        // Order 1: Milk -> Sugar -> Whipped Cream
        ICoffee coffee1 = new WhippedCreamDecorator(
            new SugarDecorator(
                new MilkDecorator(
                    new Espresso()
                )
            )
        );

        // Order 2: Sugar -> Whipped Cream -> Milk
        ICoffee coffee2 = new MilkDecorator(
            new WhippedCreamDecorator(
                new SugarDecorator(
                    new Espresso()
                )
            )
        );

        Console.WriteLine("Order 1: Espresso → Milk → Sugar → Whipped Cream");
        Console.WriteLine($"   {coffee1.GetDescription()}");
        Console.WriteLine($"   Cost: ${coffee1.GetCost():F2}\n");

        Console.WriteLine("Order 2: Espresso → Sugar → Whipped Cream → Milk");
        Console.WriteLine($"   {coffee2.GetDescription()}");
        Console.WriteLine($"   Cost: ${coffee2.GetCost():F2}\n");

        Console.WriteLine($"Same cost? {coffee1.GetCost() == coffee2.GetCost()}");
        Console.WriteLine($"Same description? {coffee1.GetDescription() == coffee2.GetDescription()}");

        Console.WriteLine("\n✓ Cost is same regardless of order, but description may differ");
    }

    // 7. Problem Without Decorator Pattern
    static void DemonstrateProblemWithoutDecorator()
    {
        Console.WriteLine("7. PROBLEM WITHOUT DECORATOR PATTERN");
        Console.WriteLine("Class explosion with inheritance\n");

        Console.WriteLine("❌ Legacy approach (inheritance):");
        Console.WriteLine("```csharp");
        Console.WriteLine("class Espresso { }");
        Console.WriteLine("class EspressoWithMilk : Espresso { }");
        Console.WriteLine("class EspressoWithSugar : Espresso { }");
        Console.WriteLine("class EspressoWithMilkAndSugar : EspressoWithMilk { }");
        Console.WriteLine("class EspressoWithMilkAndSugarAndWhippedCream : EspressoWithMilkAndSugar { }");
        Console.WriteLine("// ... 2^n combinations!");
        Console.WriteLine("```\n");

        Console.WriteLine("Problems:");
        Console.WriteLine("   1. Class explosion: 2^n classes for n decorators");
        Console.WriteLine("   2. Not flexible: Can't add new combinations at runtime");
        Console.WriteLine("   3. Code duplication: Similar code in many classes");
        Console.WriteLine("   4. Hard to maintain: Changes propagate to many classes");
        Console.WriteLine("   5. Violates Open/Closed: Must modify existing code to add features\n");

        // Calculate class explosion
        int numDecorators = 4; // Milk, Sugar, Caramel, Whipped Cream
        int numBases = 3; // Espresso, Dark Roast, Decaf
        int combinationsPerBase = (int)Math.Pow(2, numDecorators);
        int totalClasses = numBases * combinationsPerBase;

        Console.WriteLine($"Example: {numBases} base coffees × 2^{numDecorators} combinations = {totalClasses} classes!\n");

        Console.WriteLine("✅ Decorator pattern solves all these issues:");
        Console.WriteLine("   1. Linear growth: Only n+1 classes (1 base + n decorators)");
        Console.WriteLine("   2. Flexible: Add decorators at runtime");
        Console.WriteLine("   3. DRY: Each decorator implements once");
        Console.WriteLine("   4. Easy to maintain: Changes isolated to single decorator");
        Console.WriteLine("   5. Open/Closed: Add new decorators without modifying existing code");
    }

    // Helper Methods
    static void PrintOrder(ICoffee coffee)
    {
        Console.WriteLine($"   Order: {coffee.GetDescription()}");
        Console.WriteLine($"   Total: ${coffee.GetCost():F2}");
    }

    static ICoffee CreateCoffee(ICoffee baseCoffee, bool milk = false, bool sugar = false,
                                bool caramel = false, bool whippedCream = false)
    {
        ICoffee coffee = baseCoffee;
        if (milk) coffee = new MilkDecorator(coffee);
        if (sugar) coffee = new SugarDecorator(coffee);
        if (caramel) coffee = new CaramelDecorator(coffee);
        if (whippedCream) coffee = new WhippedCreamDecorator(coffee);
        return coffee;
    }
}

// ============================================================================
// DECORATOR PATTERN COMPONENTS
// ============================================================================

/// <summary>
/// Component Interface - Defines the interface for objects that can have
/// responsibilities added to them dynamically
/// </summary>
public interface ICoffee
{
    string GetDescription();
    decimal GetCost();
}

// ============================================================================
// CONCRETE COMPONENTS
// ============================================================================

/// <summary>
/// Concrete Component - Espresso
/// Base implementation to which decorators can be attached
/// </summary>
public class Espresso : ICoffee
{
    public string GetDescription() => "Espresso";
    public decimal GetCost() => 2.00m;
}

/// <summary>
/// Concrete Component - Dark Roast
/// Another base coffee type
/// </summary>
public class DarkRoast : ICoffee
{
    public string GetDescription() => "Dark Roast";
    public decimal GetCost() => 2.50m;
}

/// <summary>
/// Concrete Component - Decaf
/// Decaffeinated coffee option
/// </summary>
public class Decaf : ICoffee
{
    public string GetDescription() => "Decaf";
    public decimal GetCost() => 1.50m;
}

// ============================================================================
// BASE DECORATOR
// ============================================================================

/// <summary>
/// Base Decorator - Abstract class that implements ICoffee and wraps a coffee object
/// All concrete decorators extend this class
/// </summary>
public abstract class CoffeeDecorator : ICoffee
{
    protected readonly ICoffee _coffee;

    protected CoffeeDecorator(ICoffee coffee)
    {
        _coffee = coffee ?? throw new ArgumentNullException(nameof(coffee));
    }

    // Delegate to wrapped component by default
    public virtual string GetDescription() => _coffee.GetDescription();
    public virtual decimal GetCost() => _coffee.GetCost();
}

// ============================================================================
// CONCRETE DECORATORS
// ============================================================================

/// <summary>
/// Concrete Decorator - Milk
/// Adds milk to the coffee
/// </summary>
public class MilkDecorator : CoffeeDecorator
{
    public MilkDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription() => $"{_coffee.GetDescription()}, Milk";
    public override decimal GetCost() => _coffee.GetCost() + 0.50m;
}

/// <summary>
/// Concrete Decorator - Sugar
/// Adds sugar to the coffee
/// </summary>
public class SugarDecorator : CoffeeDecorator
{
    public SugarDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription() => $"{_coffee.GetDescription()}, Sugar";
    public override decimal GetCost() => _coffee.GetCost() + 0.20m;
}

/// <summary>
/// Concrete Decorator - Whipped Cream
/// Adds whipped cream to the coffee
/// </summary>
public class WhippedCreamDecorator : CoffeeDecorator
{
    public WhippedCreamDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription() => $"{_coffee.GetDescription()}, Whipped Cream";
    public override decimal GetCost() => _coffee.GetCost() + 0.70m;
}

/// <summary>
/// Concrete Decorator - Caramel
/// Adds caramel syrup to the coffee
/// </summary>
public class CaramelDecorator : CoffeeDecorator
{
    public CaramelDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription() => $"{_coffee.GetDescription()}, Caramel";
    public override decimal GetCost() => _coffee.GetCost() + 0.60m;
}
