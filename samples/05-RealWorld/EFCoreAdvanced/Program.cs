using Microsoft.EntityFrameworkCore;

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseInMemoryDatabase("TestDb")
    .Options;

using var db = new AppDbContext(options);

// Seed data
db.Products.AddRange(
    new Product { Name = "Laptop", Price = 999, IsDeleted = false },
    new Product { Name = "Mouse", Price = 29, IsDeleted = false },
    new Product { Name = "OldItem", Price = 10, IsDeleted = true }
);
await db.SaveChangesAsync();

// Global query filter in action (soft delete)
var activeProducts = await db.Products.ToListAsync();
Console.WriteLine($"Active products: {activeProducts.Count}");  // Only 2 (soft delete filters out deleted)

foreach (var p in activeProducts)
{
    Console.WriteLine($"- {p.Name}: ${p.Price}");
}

public class AppDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Global query filter for soft delete
        modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsDeleted { get; set; }
}
