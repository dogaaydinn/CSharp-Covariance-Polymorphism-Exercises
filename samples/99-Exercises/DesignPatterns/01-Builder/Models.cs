namespace BuilderPattern;

// Enums for pizza properties
public enum PizzaSize
{
    Small,
    Medium,
    Large,
    ExtraLarge
}

public enum DoughType
{
    Thin,
    Thick,
    Stuffed,
    GlutenFree
}

public enum SauceType
{
    Tomato,
    WhiteSauce,
    BBQ,
    Pesto,
    None
}

// TODO 1: Pizza class - the complex object to build
public class Pizza
{
    // TODO: Add properties
    // - Size (PizzaSize)
    // - Dough (DoughType)
    // - Sauce (SauceType)
    // - Toppings (List<string>)
    // - ExtraCheese (bool)
    // - SpicyLevel (int, 0-5)

    public PizzaSize Size { get; set; }
    public DoughType Dough { get; set; }
    public SauceType Sauce { get; set; }
    public List<string> Toppings { get; set; }
    public bool ExtraCheese { get; set; }
    public int SpicyLevel { get; set; }

    public Pizza()
    {
        // TODO: Initialize Toppings list
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        // TODO: Return a nice string representation
        throw new NotImplementedException();
    }
}

// TODO 2: PizzaBuilder - fluent interface for building Pizza
public class PizzaBuilder
{
    // TODO: Create a private Pizza instance to build
    private Pizza _pizza;

    public PizzaBuilder()
    {
        // TODO: Initialize Pizza
        throw new NotImplementedException();
    }

    // TODO: Implement fluent methods that return 'this'
    // Example:
    // public PizzaBuilder WithSize(PizzaSize size) { ... return this; }
    // public PizzaBuilder WithDough(DoughType dough) { ... return this; }
    // public PizzaBuilder WithSauce(SauceType sauce) { ... return this; }
    // public PizzaBuilder AddTopping(string topping) { ... return this; }
    // public PizzaBuilder WithExtraCheese() { ... return this; }
    // public PizzaBuilder WithSpicyLevel(int level) { ... return this; }

    public PizzaBuilder WithSize(PizzaSize size)
    {
        throw new NotImplementedException();
    }

    public PizzaBuilder WithDough(DoughType dough)
    {
        throw new NotImplementedException();
    }

    public PizzaBuilder WithSauce(SauceType sauce)
    {
        throw new NotImplementedException();
    }

    public PizzaBuilder AddTopping(string topping)
    {
        throw new NotImplementedException();
    }

    public PizzaBuilder WithExtraCheese()
    {
        throw new NotImplementedException();
    }

    public PizzaBuilder WithSpicyLevel(int level)
    {
        throw new NotImplementedException();
    }

    // TODO 5: Build() method - returns the constructed Pizza
    public Pizza Build()
    {
        // TODO: Validate and return the pizza
        // Validation examples:
        // - Check if Sauce is set
        // - Check if at least one topping exists
        throw new NotImplementedException();
    }
}

// TODO 3: PizzaDirector - creates specific pizza types
public class PizzaDirector
{
    // TODO: Create pre-configured pizza recipes
    // Example:
    // public static Pizza CreateMargherita() { ... }
    // public static Pizza CreatePepperoni() { ... }
    // public static Pizza CreateVeggie() { ... }
    // public static Pizza CreateMeatLovers() { ... }

    public static Pizza CreateMargherita()
    {
        // TODO: Use PizzaBuilder to create a Margherita pizza
        // Size: Medium, Dough: Thin, Sauce: Tomato
        // Toppings: Mozzarella, Basil
        throw new NotImplementedException();
    }

    public static Pizza CreatePepperoni()
    {
        // TODO: Create a Pepperoni pizza
        // Size: Large, Dough: Thick, Sauce: Tomato
        // Toppings: Pepperoni, Mozzarella
        // Extra Cheese: Yes, Spicy: Level 2
        throw new NotImplementedException();
    }

    public static Pizza CreateVeggie()
    {
        // TODO: Create a Veggie pizza
        // Size: Medium, Dough: Thin, Sauce: Pesto
        // Toppings: Bell Peppers, Mushrooms, Olives, Onions
        throw new NotImplementedException();
    }

    public static Pizza CreateMeatLovers()
    {
        // TODO: Create a Meat Lovers pizza
        // Size: ExtraLarge, Dough: Thick, Sauce: BBQ
        // Toppings: Pepperoni, Sausage, Bacon, Ham
        // Extra Cheese: Yes, Spicy: Level 3
        throw new NotImplementedException();
    }
}

// TODO 4: GlutenFreePizzaBuilder - specialized builder
public class GlutenFreePizzaBuilder : PizzaBuilder
{
    public GlutenFreePizzaBuilder()
    {
        // TODO: Automatically set dough to GlutenFree
        throw new NotImplementedException();
    }

    // Override Build to ensure gluten-free requirements
    public new Pizza Build()
    {
        // TODO: Validate that only gluten-free ingredients are used
        throw new NotImplementedException();
    }
}
