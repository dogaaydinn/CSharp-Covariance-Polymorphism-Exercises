namespace SOLIDPrinciples.LiskovSubstitution;

// ❌ BAD: Violates LSP - Penguin breaks Bird contract
public class BadBird
{
    public virtual void Fly()
    {
        Console.WriteLine("Flying...");
    }
}

public class BadPenguin : BadBird
{
    public override void Fly()
    {
        throw new NotSupportedException("Penguins can't fly!"); // Breaks contract!
    }
}

// ✅ GOOD: Proper abstraction respecting LSP
public interface IFlyingBird
{
    void Fly();
}

public interface ISwimmingBird
{
    void Swim();
}

public class Eagle : IFlyingBird
{
    public void Fly()
    {
        Console.WriteLine("✅ Eagle flying high!");
    }
}

public class Penguin : ISwimmingBird
{
    public void Swim()
    {
        Console.WriteLine("✅ Penguin swimming gracefully!");
    }
}

// Duck does both!
public class Duck : IFlyingBird, ISwimmingBird
{
    public void Fly()
    {
        Console.WriteLine("✅ Duck flying!");
    }

    public void Swim()
    {
        Console.WriteLine("✅ Duck swimming!");
    }
}

// WHY?
// - Subtypes must be substitutable for their base types
// - Don't force subclasses to implement methods they can't support
// - Penguin doesn't pretend to be a flying bird
// - Each interface represents a true capability
