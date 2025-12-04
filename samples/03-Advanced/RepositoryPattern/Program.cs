// Repository Pattern: Data access abstraction

namespace RepositoryPattern;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Repository Pattern Demo ===\n");

        // ❌ BAD: Direct database access everywhere
        Console.WriteLine("❌ BAD - Direct database access:");
        var badService = new BadProductService();
        badService.AddProduct(new Product { Id = 1, Name = "Laptop", Price = 999.99m });

        // ✅ GOOD: Repository Pattern
        Console.WriteLine("\n✅ GOOD - Repository Pattern:");
        IRepository<Product> repo = new InMemoryRepository<Product>();

        var product1 = new Product { Id = 1, Name = "Laptop", Price = 999.99m };
        var product2 = new Product { Id = 2, Name = "Mouse", Price = 29.99m };

        repo.Add(product1);
        repo.Add(product2);

        var all = repo.GetAll();
        Console.WriteLine($"Total products: {all.Count()}");

        var laptop = repo.GetById(1);
        Console.WriteLine($"Found: {laptop?.Name}");

        product1.Price = 899.99m;
        repo.Update(product1);
        Console.WriteLine($"Updated price: ${repo.GetById(1)?.Price}");

        repo.Delete(2);
        Console.WriteLine($"After delete: {repo.GetAll().Count()} products");

        Console.WriteLine("\n=== Repository Pattern Applied ===");
    }
}

// Entity
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// ❌ BAD: Direct database access
public class BadProductService
{
    public void AddProduct(Product product)
    {
        // Direct SQL, hard to test, tight coupling
        Console.WriteLine("INSERT INTO Products...");
        Console.WriteLine($"Added {product.Name}");
    }
}

// ✅ GOOD: Repository interface
public interface IRepository<T> where T : class
{
    T? GetById(int id);
    IEnumerable<T> GetAll();
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
}

// In-memory implementation (for testing)
public class InMemoryRepository<T> : IRepository<T> where T : class
{
    private readonly List<T> _data = new();
    private readonly Func<T, int> _getIdFunc;

    public InMemoryRepository()
    {
        // Assuming entity has Id property
        _getIdFunc = entity =>
        {
            var prop = typeof(T).GetProperty("Id");
            return prop != null ? (int)prop.GetValue(entity)! : 0;
        };
    }

    public T? GetById(int id)
    {
        var entity = _data.FirstOrDefault(e => _getIdFunc(e) == id);
        Console.WriteLine($"✅ GetById({id}): {(entity != null ? "Found" : "Not found")}");
        return entity;
    }

    public IEnumerable<T> GetAll()
    {
        Console.WriteLine($"✅ GetAll(): {_data.Count} items");
        return _data;
    }

    public void Add(T entity)
    {
        _data.Add(entity);
        Console.WriteLine($"✅ Added: {typeof(T).Name} (ID: {_getIdFunc(entity)})");
    }

    public void Update(T entity)
    {
        var id = _getIdFunc(entity);
        var existing = _data.FirstOrDefault(e => _getIdFunc(e) == id);
        if (existing != null)
        {
            var index = _data.IndexOf(existing);
            _data[index] = entity;
            Console.WriteLine($"✅ Updated: {typeof(T).Name} (ID: {id})");
        }
    }

    public void Delete(int id)
    {
        var entity = _data.FirstOrDefault(e => _getIdFunc(e) == id);
        if (entity != null)
        {
            _data.Remove(entity);
            Console.WriteLine($"✅ Deleted: {typeof(T).Name} (ID: {id})");
        }
    }
}

// SQL implementation (simulated)
public class SqlRepository<T> : IRepository<T> where T : class
{
    public T? GetById(int id)
    {
        Console.WriteLine($"SELECT * FROM {typeof(T).Name}s WHERE Id = {id}");
        return null; // Simulated
    }

    public IEnumerable<T> GetAll()
    {
        Console.WriteLine($"SELECT * FROM {typeof(T).Name}s");
        return Enumerable.Empty<T>();
    }

    public void Add(T entity)
    {
        Console.WriteLine($"INSERT INTO {typeof(T).Name}s...");
    }

    public void Update(T entity)
    {
        Console.WriteLine($"UPDATE {typeof(T).Name}s...");
    }

    public void Delete(int id)
    {
        Console.WriteLine($"DELETE FROM {typeof(T).Name}s WHERE Id = {id}");
    }
}
