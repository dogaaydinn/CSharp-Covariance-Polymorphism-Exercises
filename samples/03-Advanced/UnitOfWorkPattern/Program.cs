// Unit of Work: Transaction management

namespace UnitOfWorkPattern;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Unit of Work Pattern ===\n");

        // ❌ BAD: Multiple commits
        Console.WriteLine("❌ BAD - No transaction:");
        var orderRepo = new List<Order>();
        var itemRepo = new List<OrderItem>();
        orderRepo.Add(new Order { Id = 1, Total = 100 });
        // If this fails, order is saved but items aren't!
        itemRepo.Add(new OrderItem { Id = 1, OrderId = 1, Product = "Laptop" });

        // ✅ GOOD: Unit of Work
        Console.WriteLine("\n✅ GOOD - Unit of Work:");
        using var uow = new UnitOfWork();

        var order = new Order { Id = 1, Total = 250.00m };
        uow.Orders.Add(order);

        uow.OrderItems.Add(new OrderItem { Id = 1, OrderId = 1, Product = "Laptop", Price = 200m });
        uow.OrderItems.Add(new OrderItem { Id = 2, OrderId = 1, Product = "Mouse", Price = 50m });

        uow.Commit(); // Atomic: All or nothing

        Console.WriteLine("\n=== Unit of Work Applied ===");
    }
}

public class Order
{
    public int Id { get; set; }
    public decimal Total { get; set; }
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Product { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public interface IUnitOfWork : IDisposable
{
    IRepository<Order> Orders { get; }
    IRepository<OrderItem> OrderItems { get; }
    void Commit();
    void Rollback();
}

public interface IRepository<T>
{
    void Add(T entity);
    void Remove(T entity);
    IEnumerable<T> GetAll();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly List<object> _changes = new();

    public IRepository<Order> Orders { get; }
    public IRepository<OrderItem> OrderItems { get; }

    public UnitOfWork()
    {
        Orders = new Repository<Order>(_changes);
        OrderItems = new Repository<OrderItem>(_changes);
    }

    public void Commit()
    {
        Console.WriteLine($"✅ Committing {_changes.Count} changes as single transaction");
        // In real app: database transaction
        _changes.Clear();
    }

    public void Rollback()
    {
        Console.WriteLine("⚠️  Rolling back changes");
        _changes.Clear();
    }

    public void Dispose()
    {
        // Clean up resources
    }
}

public class Repository<T> : IRepository<T>
{
    private readonly List<object> _changes;
    private readonly List<T> _data = new();

    public Repository(List<object> changes)
    {
        _changes = changes;
    }

    public void Add(T entity)
    {
        _changes.Add(entity!);
        _data.Add(entity);
        Console.WriteLine($"✅ Staged: {typeof(T).Name}");
    }

    public void Remove(T entity)
    {
        _changes.Add(entity!);
        _data.Remove(entity);
    }

    public IEnumerable<T> GetAll() => _data;
}
