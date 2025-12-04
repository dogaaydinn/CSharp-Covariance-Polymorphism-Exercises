using System;
using System.Collections.Generic;

namespace DesignPatterns.Structural;

/// <summary>
/// DECORATOR PATTERN - Adds behavior to objects dynamically without altering their structure
///
/// Problem:
/// - Need to add responsibilities to objects dynamically
/// - Inheritance is inflexible (static behavior at compile time)
/// - Want to extend functionality without modifying existing code
/// - Subclassing would lead to explosion of classes
///
/// UML Structure:
/// ┌──────────────┐
/// │  Component   │ (interface)
/// └──────────────┘
///        △
///        │ implements
///        │
/// ┌──────┴───────────────────────┐
/// │                               │
/// │                        ┌──────────────┐
/// │                        │  Decorator   │ (abstract)
/// │                        └──────────────┘
/// │                               △
/// │                               │ extends
/// │                        ┌──────┴──────┐
/// │                        │ ConcreteA   │ConcreteB
/// │                        │ Decorator   │Decorator
/// │                        └─────────────┴─────────
/// │
/// ┌──────┴───────┐
/// │   Concrete   │
/// │  Component   │
/// └──────────────┘
///
/// When to Use:
/// - Add responsibilities dynamically
/// - Extend functionality without modifying code
/// - Avoid class explosion from subclassing
/// - Mix and match features flexibly
///
/// Benefits:
/// - Open/Closed Principle
/// - Single Responsibility Principle
/// - Compose behaviors at runtime
/// - More flexible than inheritance
/// </summary>

#region Coffee Shop Example

/// <summary>
/// Component interface
/// </summary>
public interface ICoffee
{
    string GetDescription();
    decimal GetCost();
}

/// <summary>
/// Concrete component: Simple Coffee
/// </summary>
public class SimpleCoffee : ICoffee
{
    public string GetDescription() => "Coffee";
    public decimal GetCost() => 2.00m;
}

/// <summary>
/// Concrete component: Espresso
/// </summary>
public class Espresso : ICoffee
{
    public string GetDescription() => "Espresso";
    public decimal GetCost() => 2.50m;
}

/// <summary>
/// Concrete component: Cappuccino
/// </summary>
public class Cappuccino : ICoffee
{
    public string GetDescription() => "Cappuccino";
    public decimal GetCost() => 3.50m;
}

/// <summary>
/// Base Decorator class
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
}

/// <summary>
/// Concrete decorator: Milk
/// </summary>
public class MilkDecorator : CoffeeDecorator
{
    public MilkDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription() => $"{_coffee.GetDescription()}, Milk";
    public override decimal GetCost() => _coffee.GetCost() + 0.50m;
}

/// <summary>
/// Concrete decorator: Sugar
/// </summary>
public class SugarDecorator : CoffeeDecorator
{
    private readonly int _cubes;

    public SugarDecorator(ICoffee coffee, int cubes = 1) : base(coffee)
    {
        _cubes = cubes;
    }

    public override string GetDescription() => $"{_coffee.GetDescription()}, {_cubes} Sugar Cube{(_cubes > 1 ? "s" : "")}";
    public override decimal GetCost() => _coffee.GetCost() + (0.25m * _cubes);
}

/// <summary>
/// Concrete decorator: Whipped Cream
/// </summary>
public class WhippedCreamDecorator : CoffeeDecorator
{
    public WhippedCreamDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription() => $"{_coffee.GetDescription()}, Whipped Cream";
    public override decimal GetCost() => _coffee.GetCost() + 0.75m;
}

/// <summary>
/// Concrete decorator: Vanilla Syrup
/// </summary>
public class VanillaSyrupDecorator : CoffeeDecorator
{
    public VanillaSyrupDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription() => $"{_coffee.GetDescription()}, Vanilla Syrup";
    public override decimal GetCost() => _coffee.GetCost() + 0.60m;
}

/// <summary>
/// Concrete decorator: Caramel
/// </summary>
public class CaramelDecorator : CoffeeDecorator
{
    public CaramelDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription() => $"{_coffee.GetDescription()}, Caramel";
    public override decimal GetCost() => _coffee.GetCost() + 0.70m;
}

/// <summary>
/// Concrete decorator: Extra Shot
/// </summary>
public class ExtraShotDecorator : CoffeeDecorator
{
    private readonly int _shots;

    public ExtraShotDecorator(ICoffee coffee, int shots = 1) : base(coffee)
    {
        _shots = shots;
    }

    public override string GetDescription() => $"{_coffee.GetDescription()}, {_shots} Extra Shot{(_shots > 1 ? "s" : "")}";
    public override decimal GetCost() => _coffee.GetCost() + (1.00m * _shots);
}

#endregion

#region Data Stream Example

/// <summary>
/// Component interface for data streams
/// </summary>
public interface IDataStream
{
    string Read();
    void Write(string data);
}

/// <summary>
/// Concrete component: File Stream
/// </summary>
public class FileStream : IDataStream
{
    private readonly string _filename;
    private string _data = string.Empty;

    public FileStream(string filename)
    {
        _filename = filename;
    }

    public string Read()
    {
        Console.WriteLine($"  [Decorator] Reading from file: {_filename}");
        return _data;
    }

    public void Write(string data)
    {
        Console.WriteLine($"  [Decorator] Writing to file: {_filename}");
        _data = data;
    }
}

/// <summary>
/// Base decorator for data streams
/// </summary>
public abstract class DataStreamDecorator : IDataStream
{
    protected readonly IDataStream _stream;

    protected DataStreamDecorator(IDataStream stream)
    {
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
    }

    public virtual string Read() => _stream.Read();
    public virtual void Write(string data) => _stream.Write(data);
}

/// <summary>
/// Concrete decorator: Encryption
/// </summary>
public class EncryptionDecorator : DataStreamDecorator
{
    public EncryptionDecorator(IDataStream stream) : base(stream) { }

    public override string Read()
    {
        var data = _stream.Read();
        return Decrypt(data);
    }

    public override void Write(string data)
    {
        var encrypted = Encrypt(data);
        _stream.Write(encrypted);
    }

    private string Encrypt(string data)
    {
        Console.WriteLine("  [Decorator] Encrypting data...");
        // Simple encryption simulation (reverse string)
        var chars = data.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    private string Decrypt(string data)
    {
        Console.WriteLine("  [Decorator] Decrypting data...");
        // Simple decryption simulation (reverse string)
        var chars = data.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }
}

/// <summary>
/// Concrete decorator: Compression
/// </summary>
public class CompressionDecorator : DataStreamDecorator
{
    public CompressionDecorator(IDataStream stream) : base(stream) { }

    public override string Read()
    {
        var data = _stream.Read();
        return Decompress(data);
    }

    public override void Write(string data)
    {
        var compressed = Compress(data);
        _stream.Write(compressed);
    }

    private string Compress(string data)
    {
        Console.WriteLine($"  [Decorator] Compressing data (size: {data.Length} bytes)");
        // Simulation: just add marker
        return $"[COMPRESSED]{data}";
    }

    private string Decompress(string data)
    {
        Console.WriteLine("  [Decorator] Decompressing data...");
        // Simulation: remove marker
        return data.Replace("[COMPRESSED]", "");
    }
}

/// <summary>
/// Concrete decorator: Buffering
/// </summary>
public class BufferedDecorator : DataStreamDecorator
{
    private readonly Queue<string> _buffer = new();

    public BufferedDecorator(IDataStream stream) : base(stream) { }

    public override string Read()
    {
        if (_buffer.Count > 0)
        {
            Console.WriteLine("  [Decorator] Reading from buffer");
            return _buffer.Dequeue();
        }
        return _stream.Read();
    }

    public override void Write(string data)
    {
        Console.WriteLine("  [Decorator] Writing to buffer");
        _buffer.Enqueue(data);
        Flush();
    }

    private void Flush()
    {
        Console.WriteLine($"  [Decorator] Flushing buffer ({_buffer.Count} items)");
        while (_buffer.Count > 0)
        {
            _stream.Write(_buffer.Dequeue());
        }
    }
}

#endregion

/// <summary>
/// Example demonstrating Decorator pattern
/// </summary>
public static class DecoratorExample
{
    public static void Run()
    {
        Console.WriteLine();
        Console.WriteLine("4. DECORATOR PATTERN - Adds behavior to objects dynamically");
        Console.WriteLine("-".PadRight(70, '-'));
        Console.WriteLine();

        // Example 1: Coffee Shop Orders
        Console.WriteLine("Example 1: Coffee Shop Orders");
        Console.WriteLine();

        // Simple coffee
        ICoffee coffee1 = new SimpleCoffee();
        Console.WriteLine($"  {coffee1.GetDescription()} - ${coffee1.GetCost():F2}");

        // Coffee with milk
        ICoffee coffee2 = new MilkDecorator(new SimpleCoffee());
        Console.WriteLine($"  {coffee2.GetDescription()} - ${coffee2.GetCost():F2}");

        // Complex order: Cappuccino with milk, sugar, whipped cream, and vanilla
        ICoffee coffee3 = new Cappuccino();
        coffee3 = new MilkDecorator(coffee3);
        coffee3 = new SugarDecorator(coffee3, 2);
        coffee3 = new WhippedCreamDecorator(coffee3);
        coffee3 = new VanillaSyrupDecorator(coffee3);
        Console.WriteLine($"  {coffee3.GetDescription()} - ${coffee3.GetCost():F2}");

        // Ultra complex order
        ICoffee coffee4 = new ExtraShotDecorator(
            new CaramelDecorator(
                new WhippedCreamDecorator(
                    new MilkDecorator(
                        new Espresso()
                    )
                )
            ), 2
        );
        Console.WriteLine($"  {coffee4.GetDescription()} - ${coffee4.GetCost():F2}");

        Console.WriteLine();

        // Example 2: Data Stream Processing
        Console.WriteLine("Example 2: Data Stream with Encryption and Compression");
        Console.WriteLine();

        const string originalData = "Sensitive data that needs protection";
        Console.WriteLine($"  Original data: \"{originalData}\"");
        Console.WriteLine();

        // File stream with encryption and compression
        IDataStream stream = new FileStream("data.txt");
        stream = new EncryptionDecorator(stream);
        stream = new CompressionDecorator(stream);

        Console.WriteLine("  Writing with decorators (compression + encryption):");
        stream.Write(originalData);

        Console.WriteLine();
        Console.WriteLine("  Reading with decorators (decryption + decompression):");
        var retrieved = stream.Read();
        Console.WriteLine($"  Retrieved data: \"{retrieved}\"");

        Console.WriteLine();

        // Example 3: Showing decorator flexibility
        Console.WriteLine("Example 3: Mix and Match Decorators");
        Console.WriteLine();

        // Only encryption
        IDataStream stream1 = new EncryptionDecorator(new FileStream("secure.txt"));
        Console.WriteLine("  Stream with encryption only:");
        stream1.Write("Secret message");

        Console.WriteLine();

        // Only compression
        IDataStream stream2 = new CompressionDecorator(new FileStream("compressed.txt"));
        Console.WriteLine("  Stream with compression only:");
        stream2.Write("Large data");

        Console.WriteLine();

        // All decorators
        IDataStream stream3 = new BufferedDecorator(
            new CompressionDecorator(
                new EncryptionDecorator(
                    new FileStream("buffered.txt")
                )
            )
        );
        Console.WriteLine("  Stream with buffering + compression + encryption:");
        stream3.Write("Multi-decorated data");

        Console.WriteLine();
        Console.WriteLine("  Key Benefit: Add features dynamically without changing existing code!");
    }
}
