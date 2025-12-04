namespace BuilderPattern;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Builder Pattern Exercise");
        Console.WriteLine("Run 'dotnet test' to check your solutions\n");

        // Uncomment when implemented
        /*
        Console.WriteLine("=== Using PizzaBuilder (Fluent Interface) ===");
        var customPizza = new PizzaBuilder()
            .WithSize(PizzaSize.Large)
            .WithDough(DoughType.Thin)
            .WithSauce(SauceType.Tomato)
            .AddTopping("Mozzarella")
            .AddTopping("Mushrooms")
            .AddTopping("Olives")
            .WithExtraCheese()
            .WithSpicyLevel(2)
            .Build();

        Console.WriteLine($"Custom Pizza: {customPizza}");

        Console.WriteLine("\n=== Using PizzaDirector (Pre-configured) ===");
        var margherita = PizzaDirector.CreateMargherita();
        Console.WriteLine($"Margherita: {margherita}");

        var pepperoni = PizzaDirector.CreatePepperoni();
        Console.WriteLine($"Pepperoni: {pepperoni}");

        Console.WriteLine("\n=== Using GlutenFreePizzaBuilder ===");
        var glutenFreePizza = new GlutenFreePizzaBuilder()
            .WithSize(PizzaSize.Medium)
            .WithSauce(SauceType.Tomato)
            .AddTopping("Cheese")
            .Build();

        Console.WriteLine($"Gluten-Free Pizza: {glutenFreePizza}");
        */
    }
}
