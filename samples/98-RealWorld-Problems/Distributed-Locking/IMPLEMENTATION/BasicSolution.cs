using Microsoft.EntityFrameworkCore;

namespace RealWorldProblems.DistributedLocking.Basic;

// Database Pessimistic Locking
public class PessimisticLockingService
{
    private readonly AppDbContext _db;

    public async Task<bool> UpdateStockWithLockAsync(int productId, int quantity)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            // Lock row with FOR UPDATE
            var product = await _db.Products
                .FromSqlRaw("SELECT * FROM Products WHERE Id = {0} FOR UPDATE", productId)
                .FirstOrDefaultAsync();
            
            if (product == null || product.Stock < quantity)
            {
                await transaction.RollbackAsync();
                return false;
            }
            
            product.Stock -= quantity;
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            
            Console.WriteLine($"[Pessimistic Lock] Stock updated: {product.Stock} remaining");
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"[Lock Failed] {ex.Message}");
            return false;
        }
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Stock { get; set; }
    public int Version { get; set; }
}

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
