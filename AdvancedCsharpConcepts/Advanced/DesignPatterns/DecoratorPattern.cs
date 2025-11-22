namespace AdvancedCsharpConcepts.Advanced.DesignPatterns;

/// <summary>
/// Decorator Pattern - Structural design pattern for adding behavior dynamically.
/// Silicon Valley best practice: Extend functionality without modifying existing code (Open/Closed Principle).
/// </summary>
public static class DecoratorPattern
{
    /// <summary>
    /// Component interface for coffee beverages.
    /// </summary>
    public interface ICoffee
    {
        string GetDescription();
        decimal GetCost();
        int GetCalories();
    }

    /// <summary>
    /// Concrete component: Basic espresso.
    /// </summary>
    public class Espresso : ICoffee
    {
        public string GetDescription() => "Espresso";
        public decimal GetCost() => 1.99m;
        public int GetCalories() => 5;
    }

    /// <summary>
    /// Concrete component: House blend coffee.
    /// </summary>
    public class HouseBlend : ICoffee
    {
        public string GetDescription() => "House Blend Coffee";
        public decimal GetCost() => 0.89m;
        public int GetCalories() => 10;
    }

    /// <summary>
    /// Concrete component: Dark roast coffee.
    /// </summary>
    public class DarkRoast : ICoffee
    {
        public string GetDescription() => "Dark Roast Coffee";
        public decimal GetCost() => 0.99m;
        public int GetCalories() => 8;
    }

    /// <summary>
    /// Abstract decorator base class.
    /// </summary>
    public abstract class CoffeeDecorator : ICoffee
    {
        protected readonly ICoffee _coffee;

        protected CoffeeDecorator(ICoffee coffee)
        {
            _coffee = coffee ?? throw new ArgumentNullException(nameof(coffee));
        }

        public virtual string GetDescription() => _coffee.GetDescription();
        public virtual decimal GetCost() => _coffee.GetCost();
        public virtual int GetCalories() => _coffee.GetCalories();
    }

    /// <summary>
    /// Concrete decorator: Milk.
    /// </summary>
    public class Milk : CoffeeDecorator
    {
        public Milk(ICoffee coffee) : base(coffee) { }

        public override string GetDescription() => $"{_coffee.GetDescription()}, Milk";
        public override decimal GetCost() => _coffee.GetCost() + 0.50m;
        public override int GetCalories() => _coffee.GetCalories() + 50;
    }

    /// <summary>
    /// Concrete decorator: Mocha (chocolate).
    /// </summary>
    public class Mocha : CoffeeDecorator
    {
        public Mocha(ICoffee coffee) : base(coffee) { }

        public override string GetDescription() => $"{_coffee.GetDescription()}, Mocha";
        public override decimal GetCost() => _coffee.GetCost() + 0.70m;
        public override int GetCalories() => _coffee.GetCalories() + 80;
    }

    /// <summary>
    /// Concrete decorator: Whipped cream.
    /// </summary>
    public class Whip : CoffeeDecorator
    {
        public Whip(ICoffee coffee) : base(coffee) { }

        public override string GetDescription() => $"{_coffee.GetDescription()}, Whip";
        public override decimal GetCost() => _coffee.GetCost() + 0.60m;
        public override int GetCalories() => _coffee.GetCalories() + 100;
    }

    /// <summary>
    /// Concrete decorator: Caramel syrup.
    /// </summary>
    public class Caramel : CoffeeDecorator
    {
        public Caramel(ICoffee coffee) : base(coffee) { }

        public override string GetDescription() => $"{_coffee.GetDescription()}, Caramel";
        public override decimal GetCost() => _coffee.GetCost() + 0.55m;
        public override int GetCalories() => _coffee.GetCalories() + 70;
    }

    /// <summary>
    /// Concrete decorator: Extra shot of espresso.
    /// </summary>
    public class ExtraShot : CoffeeDecorator
    {
        public ExtraShot(ICoffee coffee) : base(coffee) { }

        public override string GetDescription() => $"{_coffee.GetDescription()}, Extra Shot";
        public override decimal GetCost() => _coffee.GetCost() + 0.75m;
        public override int GetCalories() => _coffee.GetCalories() + 5;
    }

    /// <summary>
    /// Concrete decorator: Soy milk (alternative to regular milk).
    /// </summary>
    public class SoyMilk : CoffeeDecorator
    {
        public SoyMilk(ICoffee coffee) : base(coffee) { }

        public override string GetDescription() => $"{_coffee.GetDescription()}, Soy Milk";
        public override decimal GetCost() => _coffee.GetCost() + 0.65m;
        public override int GetCalories() => _coffee.GetCalories() + 40;
    }

    /// <summary>
    /// Helper method to print coffee details.
    /// </summary>
    private static void PrintCoffee(ICoffee coffee)
    {
        Console.WriteLine($"Order: {coffee.GetDescription()}");
        Console.WriteLine($"Cost: ${coffee.GetCost():F2}");
        Console.WriteLine($"Calories: {coffee.GetCalories()}");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates the Decorator Pattern.
    /// </summary>
    public static void RunExample()
    {
        Console.WriteLine("=== Decorator Pattern Examples ===\n");

        // Example 1: Simple espresso
        Console.WriteLine("1. Simple Espresso:");
        ICoffee coffee1 = new Espresso();
        PrintCoffee(coffee1);

        // Example 2: Espresso with milk
        Console.WriteLine("2. Espresso with Milk:");
        ICoffee coffee2 = new Milk(new Espresso());
        PrintCoffee(coffee2);

        // Example 3: Mocha with whipped cream
        Console.WriteLine("3. Dark Roast with Mocha and Whip:");
        ICoffee coffee3 = new Whip(new Mocha(new DarkRoast()));
        PrintCoffee(coffee3);

        // Example 4: Complex order - Double mocha caramel latte
        Console.WriteLine("4. House Blend - Double Mocha, Caramel, Milk, Whip (The Works!):");
        ICoffee coffee4 = new Whip(
                            new Caramel(
                                new Milk(
                                    new Mocha(
                                        new Mocha(
                                            new HouseBlend())))));
        PrintCoffee(coffee4);

        // Example 5: Vegan option
        Console.WriteLine("5. Vegan Espresso - Soy Milk, Mocha:");
        ICoffee coffee5 = new Mocha(
                            new SoyMilk(
                                new Espresso()));
        PrintCoffee(coffee5);

        // Example 6: Energy boost
        Console.WriteLine("6. Energy Boost - Espresso with 3 Extra Shots:");
        ICoffee coffee6 = new ExtraShot(
                            new ExtraShot(
                                new ExtraShot(
                                    new Espresso())));
        PrintCoffee(coffee6);

        // Example 7: Demonstrate decorator stacking
        Console.WriteLine("7. Ultimate Indulgence - Everything:");
        ICoffee coffee7 = new Whip(
                            new Caramel(
                                new Mocha(
                                    new Milk(
                                        new ExtraShot(
                                            new DarkRoast())))));
        PrintCoffee(coffee7);

        // Example 8: Show cost comparison
        Console.WriteLine("8. Cost Comparison:");
        var basicCoffee = new HouseBlend();
        var fancyCoffee = new Whip(new Mocha(new Milk(new HouseBlend())));

        Console.WriteLine($"Basic House Blend: ${basicCoffee.GetCost():F2}");
        Console.WriteLine($"Fancy House Blend (Milk, Mocha, Whip): ${fancyCoffee.GetCost():F2}");
        Console.WriteLine($"Difference: ${(fancyCoffee.GetCost() - basicCoffee.GetCost()):F2}");
        Console.WriteLine($"Calorie increase: {fancyCoffee.GetCalories() - basicCoffee.GetCalories()} cal");
    }
}
