using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.Creational;

/// <summary>
/// BUILDER PATTERN - Constructs complex objects step by step
///
/// Problem:
/// - Object creation requires many steps
/// - Constructor with many parameters is unwieldy
/// - Need different representations of same object
/// - Want immutable objects with complex initialization
///
/// UML Structure:
/// ┌──────────────┐        ┌──────────────┐
/// │   Director   │───────>│   Builder    │ (interface)
/// └──────────────┘        └──────────────┘
///                                △
///                                │ implements
///                         ┌──────┴──────┐
///                         │ConcreteBuilder│
///                         └───────────────┘
///                                │ builds
///                                ▼
///                         ┌──────────────┐
///                         │   Product    │
///                         └──────────────┘
///
/// When to Use:
/// - Objects have many optional parameters
/// - Need different representations of same object
/// - Construction algorithm should be independent of parts
/// - Want to construct immutable objects
///
/// Benefits:
/// - Constructs objects step-by-step
/// - Reuses construction code
/// - Creates different representations
/// - Better readability than telescoping constructors
/// </summary>

#region Pizza Builder

/// <summary>
/// Complex product: Pizza
/// </summary>
public class Pizza
{
    public string Size { get; init; } = "Medium";
    public string Crust { get; init; } = "Regular";
    public string Sauce { get; init; } = "Tomato";
    public bool ExtraCheese { get; init; }
    public List<string> Toppings { get; init; } = new();
    public List<string> Seasonings { get; init; } = new();
    public bool IsGlutenFree { get; init; }
    public bool IsVegan { get; init; }

    public decimal CalculatePrice()
    {
        decimal basePrice = Size switch
        {
            "Small" => 8.00m,
            "Medium" => 12.00m,
            "Large" => 16.00m,
            "Extra Large" => 20.00m,
            _ => 12.00m
        };

        basePrice += Toppings.Count * 1.50m;
        if (ExtraCheese) basePrice += 2.00m;
        if (IsGlutenFree) basePrice += 3.00m;
        if (IsVegan) basePrice += 2.00m;

        return basePrice;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"  [Builder] Pizza Order:");
        sb.AppendLine($"    Size: {Size}");
        sb.AppendLine($"    Crust: {Crust}");
        sb.AppendLine($"    Sauce: {Sauce}");
        sb.AppendLine($"    Extra Cheese: {(ExtraCheese ? "Yes" : "No")}");

        if (Toppings.Count > 0)
            sb.AppendLine($"    Toppings: {string.Join(", ", Toppings)}");

        if (Seasonings.Count > 0)
            sb.AppendLine($"    Seasonings: {string.Join(", ", Seasonings)}");

        if (IsGlutenFree)
            sb.AppendLine("    Gluten-Free: Yes");

        if (IsVegan)
            sb.AppendLine("    Vegan: Yes");

        sb.Append($"    Total Price: ${CalculatePrice():F2}");

        return sb.ToString();
    }
}

/// <summary>
/// Fluent builder for Pizza
/// </summary>
public class PizzaBuilder
{
    private string _size = "Medium";
    private string _crust = "Regular";
    private string _sauce = "Tomato";
    private bool _extraCheese;
    private readonly List<string> _toppings = new();
    private readonly List<string> _seasonings = new();
    private bool _isGlutenFree;
    private bool _isVegan;

    public PizzaBuilder SetSize(string size)
    {
        _size = size;
        return this;
    }

    public PizzaBuilder SetCrust(string crust)
    {
        _crust = crust;
        return this;
    }

    public PizzaBuilder SetSauce(string sauce)
    {
        _sauce = sauce;
        return this;
    }

    public PizzaBuilder AddExtraCheese()
    {
        _extraCheese = true;
        return this;
    }

    public PizzaBuilder AddTopping(string topping)
    {
        _toppings.Add(topping);
        return this;
    }

    public PizzaBuilder AddToppings(params string[] toppings)
    {
        _toppings.AddRange(toppings);
        return this;
    }

    public PizzaBuilder AddSeasoning(string seasoning)
    {
        _seasonings.Add(seasoning);
        return this;
    }

    public PizzaBuilder MakeGlutenFree()
    {
        _isGlutenFree = true;
        return this;
    }

    public PizzaBuilder MakeVegan()
    {
        _isVegan = true;
        return this;
    }

    public Pizza Build()
    {
        return new Pizza
        {
            Size = _size,
            Crust = _crust,
            Sauce = _sauce,
            ExtraCheese = _extraCheese,
            Toppings = new List<string>(_toppings),
            Seasonings = new List<string>(_seasonings),
            IsGlutenFree = _isGlutenFree,
            IsVegan = _isVegan
        };
    }

    /// <summary>
    /// Reset builder for reuse
    /// </summary>
    public PizzaBuilder Reset()
    {
        _size = "Medium";
        _crust = "Regular";
        _sauce = "Tomato";
        _extraCheese = false;
        _toppings.Clear();
        _seasonings.Clear();
        _isGlutenFree = false;
        _isVegan = false;
        return this;
    }
}

/// <summary>
/// Director that knows how to build specific pizza types
/// </summary>
public class PizzaDirector
{
    private readonly PizzaBuilder _builder;

    public PizzaDirector(PizzaBuilder builder)
    {
        _builder = builder;
    }

    public Pizza BuildMargherita()
    {
        return _builder
            .Reset()
            .SetSize("Medium")
            .SetCrust("Thin")
            .AddTopping("Mozzarella")
            .AddTopping("Fresh Basil")
            .AddSeasoning("Oregano")
            .Build();
    }

    public Pizza BuildPepperoni()
    {
        return _builder
            .Reset()
            .SetSize("Large")
            .SetCrust("Regular")
            .AddTopping("Pepperoni")
            .AddTopping("Mozzarella")
            .AddExtraCheese()
            .Build();
    }

    public Pizza BuildVeggie()
    {
        return _builder
            .Reset()
            .SetSize("Medium")
            .SetCrust("Whole Wheat")
            .AddToppings("Bell Peppers", "Mushrooms", "Onions", "Olives", "Tomatoes")
            .MakeVegan()
            .Build();
    }

    public Pizza BuildMeatLovers()
    {
        return _builder
            .Reset()
            .SetSize("Extra Large")
            .SetCrust("Thick")
            .AddToppings("Pepperoni", "Sausage", "Bacon", "Ham", "Ground Beef")
            .AddExtraCheese()
            .Build();
    }
}

#endregion

#region Computer Builder

/// <summary>
/// Complex product: Computer
/// </summary>
public class Computer
{
    public string CPU { get; init; } = "Unknown";
    public int RAMInGB { get; init; }
    public int StorageInGB { get; init; }
    public string GPU { get; init; } = "Integrated";
    public string MotherBoard { get; init; } = "Standard";
    public string PowerSupply { get; init; } = "500W";
    public List<string> Peripherals { get; init; } = new();
    public bool HasWiFi { get; init; }
    public bool HasBluetooth { get; init; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"  [Builder] Computer Specs:");
        sb.AppendLine($"    CPU: {CPU}");
        sb.AppendLine($"    RAM: {RAMInGB}GB");
        sb.AppendLine($"    Storage: {StorageInGB}GB");
        sb.AppendLine($"    GPU: {GPU}");
        sb.AppendLine($"    Motherboard: {MotherBoard}");
        sb.AppendLine($"    Power Supply: {PowerSupply}");

        if (Peripherals.Count > 0)
            sb.AppendLine($"    Peripherals: {string.Join(", ", Peripherals)}");

        sb.AppendLine($"    WiFi: {(HasWiFi ? "Yes" : "No")}");
        sb.Append($"    Bluetooth: {(HasBluetooth ? "Yes" : "No")}");

        return sb.ToString();
    }
}

/// <summary>
/// Fluent builder for Computer
/// </summary>
public class ComputerBuilder
{
    private string _cpu = "Unknown";
    private int _ramInGB;
    private int _storageInGB;
    private string _gpu = "Integrated";
    private string _motherBoard = "Standard";
    private string _powerSupply = "500W";
    private readonly List<string> _peripherals = new();
    private bool _hasWiFi;
    private bool _hasBluetooth;

    public ComputerBuilder SetCPU(string cpu)
    {
        _cpu = cpu;
        return this;
    }

    public ComputerBuilder SetRAM(int gigabytes)
    {
        _ramInGB = gigabytes;
        return this;
    }

    public ComputerBuilder SetStorage(int gigabytes)
    {
        _storageInGB = gigabytes;
        return this;
    }

    public ComputerBuilder SetGPU(string gpu)
    {
        _gpu = gpu;
        return this;
    }

    public ComputerBuilder SetMotherBoard(string motherBoard)
    {
        _motherBoard = motherBoard;
        return this;
    }

    public ComputerBuilder SetPowerSupply(string powerSupply)
    {
        _powerSupply = powerSupply;
        return this;
    }

    public ComputerBuilder AddPeripheral(string peripheral)
    {
        _peripherals.Add(peripheral);
        return this;
    }

    public ComputerBuilder AddWiFi()
    {
        _hasWiFi = true;
        return this;
    }

    public ComputerBuilder AddBluetooth()
    {
        _hasBluetooth = true;
        return this;
    }

    public Computer Build()
    {
        return new Computer
        {
            CPU = _cpu,
            RAMInGB = _ramInGB,
            StorageInGB = _storageInGB,
            GPU = _gpu,
            MotherBoard = _motherBoard,
            PowerSupply = _powerSupply,
            Peripherals = new List<string>(_peripherals),
            HasWiFi = _hasWiFi,
            HasBluetooth = _hasBluetooth
        };
    }
}

#endregion

/// <summary>
/// Example demonstrating Builder pattern
/// </summary>
public static class BuilderExample
{
    public static void Run()
    {
        Console.WriteLine();
        Console.WriteLine("3. BUILDER PATTERN - Constructs complex objects step by step");
        Console.WriteLine("-".PadRight(70, '-'));
        Console.WriteLine();

        // Example 1: Pizza Builder (without Director)
        Console.WriteLine("Example 1: Custom Pizza Orders (Fluent Builder)");
        Console.WriteLine();

        var customPizza = new PizzaBuilder()
            .SetSize("Large")
            .SetCrust("Thin")
            .SetSauce("Pesto")
            .AddTopping("Pepperoni")
            .AddTopping("Mushrooms")
            .AddTopping("Extra Cheese")
            .AddSeasoning("Italian Herbs")
            .AddSeasoning("Red Pepper Flakes")
            .Build();

        Console.WriteLine(customPizza);
        Console.WriteLine();

        var veganPizza = new PizzaBuilder()
            .SetSize("Medium")
            .SetCrust("Whole Wheat")
            .AddToppings("Tomatoes", "Spinach", "Olives", "Artichokes")
            .MakeVegan()
            .MakeGlutenFree()
            .Build();

        Console.WriteLine(veganPizza);
        Console.WriteLine();

        // Example 2: Pizza Builder with Director
        Console.WriteLine("Example 2: Predefined Pizza Types (Builder + Director)");
        Console.WriteLine();

        var builder = new PizzaBuilder();
        var director = new PizzaDirector(builder);

        Console.WriteLine("  Popular pizzas:");
        Console.WriteLine();

        var margherita = director.BuildMargherita();
        Console.WriteLine(margherita);
        Console.WriteLine();

        var pepperoni = director.BuildPepperoni();
        Console.WriteLine(pepperoni);
        Console.WriteLine();

        var meatLovers = director.BuildMeatLovers();
        Console.WriteLine(meatLovers);
        Console.WriteLine();

        // Example 3: Computer Builder
        Console.WriteLine("Example 3: Building Custom Computers");
        Console.WriteLine();

        var gamingPC = new ComputerBuilder()
            .SetCPU("Intel Core i9-13900K")
            .SetRAM(32)
            .SetStorage(2000)
            .SetGPU("NVIDIA RTX 4090")
            .SetMotherBoard("ASUS ROG Maximus Z790")
            .SetPowerSupply("1000W 80+ Platinum")
            .AddPeripheral("Gaming Keyboard")
            .AddPeripheral("Gaming Mouse")
            .AddPeripheral("27-inch 4K Monitor")
            .AddWiFi()
            .AddBluetooth()
            .Build();

        Console.WriteLine(gamingPC);
        Console.WriteLine();

        var officePC = new ComputerBuilder()
            .SetCPU("Intel Core i5-13400")
            .SetRAM(16)
            .SetStorage(512)
            .SetMotherBoard("MSI PRO B760M")
            .AddPeripheral("Wireless Keyboard")
            .AddPeripheral("Wireless Mouse")
            .AddWiFi()
            .AddBluetooth()
            .Build();

        Console.WriteLine(officePC);
        Console.WriteLine();

        Console.WriteLine("  Key Benefit: Readable, flexible object construction!");
    }
}
