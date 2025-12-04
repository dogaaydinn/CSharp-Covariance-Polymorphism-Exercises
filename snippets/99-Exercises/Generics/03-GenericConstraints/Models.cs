namespace GenericConstraints;

// Base entity class
public abstract class Entity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }

    protected Entity()
    {
        CreatedAt = DateTime.UtcNow;
    }
}

// Concrete entities
public class Product : Entity, IComparable<Product>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public Product() { }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public int CompareTo(Product? other)
    {
        if (other == null) return 1;
        return Price.CompareTo(other.Price);
    }
}

public class Customer : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public Customer() { }

    public Customer(string name, string email)
    {
        Name = name;
        Email = email;
    }
}

// Disposable resource for testing
public class Resource : IDisposable
{
    public string Name { get; set; } = string.Empty;
    public bool IsDisposed { get; private set; }

    public Resource() { }

    public Resource(string name)
    {
        Name = name;
    }

    public void Dispose()
    {
        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}

// Value types for testing
public struct Point : IComparable<Point>
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int CompareTo(Point other)
    {
        int distThis = X * X + Y * Y;
        int distOther = other.X * other.X + other.Y * other.Y;
        return distThis.CompareTo(distOther);
    }
}
