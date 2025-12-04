// Decorator Pattern: Add responsibilities dynamically

namespace DecoratorPattern;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Decorator Pattern Demo ===\n");

        // ❌ BAD: Class explosion
        Console.WriteLine("❌ BAD - Class for every combination:");
        Console.WriteLine("EspressoWithMilk, EspressoWithMilkAndSugar,");
        Console.WriteLine("EspressoWithMilkAndSugarAndWhippedCream...");
        Console.WriteLine("= 2^n combinations!\n");

        // ✅ GOOD: Decorator Pattern
        Console.WriteLine("✅ GOOD - Decorator Pattern:");

        ICoffee espresso = new Espresso();
        Console.WriteLine($"{espresso.GetDescription()} = ${espresso.GetCost():F2}");

        ICoffee milkCoffee = new MilkDecorator(new Espresso());
        Console.WriteLine($"{milkCoffee.GetDescription()} = ${milkCoffee.GetCost():F2}");

        ICoffee fancyCoffee = new WhippedCreamDecorator(
            new SugarDecorator(
                new MilkDecorator(
                    new Espresso())));
        Console.WriteLine($"{fancyCoffee.GetDescription()} = ${fancyCoffee.GetCost():F2}");

        ICoffee megaCoffee = new CaramelDecorator(
            new WhippedCreamDecorator(
                new SugarDecorator(
                    new MilkDecorator(
                        new DarkRoast()))));
        Console.WriteLine($"{megaCoffee.GetDescription()} = ${megaCoffee.GetCost():F2}");

        Console.WriteLine("\n=== Decorator Pattern Applied ===");
    }
}

// Component interface
public interface ICoffee
{
    string GetDescription();
    decimal GetCost();
}

// Concrete components
public class Espresso : ICoffee
{
    public string GetDescription() => "Espresso";
    public decimal GetCost() => 2.00m;
}

public class DarkRoast : ICoffee
{
    public string GetDescription() => "Dark Roast";
    public decimal GetCost() => 2.50m;
}

public class Decaf : ICoffee
{
    public string GetDescription() => "Decaf";
    public decimal GetCost() => 1.50m;
}

// Base Decorator
public abstract class CoffeeDecorator : ICoffee
{
    protected readonly ICoffee _coffee;

    protected CoffeeDecorator(ICoffee coffee)
    {
        _coffee = coffee;
    }

    public virtual string GetDescription() => _coffee.GetDescription();
    public virtual decimal GetCost() => _coffee.GetCost();
}

// Concrete Decorators
public class MilkDecorator : CoffeeDecorator
{
    public MilkDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription() => $"{_coffee.GetDescription()}, Milk";
    public override decimal GetCost() => _coffee.GetCost() + 0.50m;
}

public class SugarDecorator : CoffeeDecorator
{
    public SugarDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription() => $"{_coffee.GetDescription()}, Sugar";
    public override decimal GetCost() => _coffee.GetCost() + 0.20m;
}

public class WhippedCreamDecorator : CoffeeDecorator
{
    public WhippedCreamDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription() => $"{_coffee.GetDescription()}, Whipped Cream";
    public override decimal GetCost() => _coffee.GetCost() + 0.70m;
}

public class CaramelDecorator : CoffeeDecorator
{
    public CaramelDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription() => $"{_coffee.GetDescription()}, Caramel";
    public override decimal GetCost() => _coffee.GetCost() + 0.60m;
}

// BENCHMARK
// Approach           | Classes Needed    | Flexibility
// -------------------|-------------------|------------
// Inheritance        | 2^n (explosion!)  | Low
// Decorator          | n + 1             | High
