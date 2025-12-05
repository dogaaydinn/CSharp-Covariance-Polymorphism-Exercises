using Microsoft.EntityFrameworkCore;

namespace RealWorldProblems.DistributedLocking.Advanced;

// Optimistic Concurrency Control
public class OptimisticConcurrencyService
{
    private readonly AppDbContext _db;

    public async Task<bool> UpdateStockOptimisticAsync(int productId, int quantity, int maxRetries = 3)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == productId);
            
            if (product == null || product.Stock < quantity)
                return false;
            
            var originalVersion = product.Version;
            var newStock = product.Stock - quantity;
            var newVersion = product.Version + 1;
            
            try
            {
                var rowsAffected = await _db.Database.ExecuteSqlRawAsync(
                    @"UPDATE Products 
                      SET Stock = {0}, Version = {1}, UpdatedAt = {2}
                      WHERE Id = {3} AND Version = {4}",
                    newStock, newVersion, DateTime.UtcNow, productId, originalVersion);
                
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"[Optimistic] Success on attempt {attempt + 1}");
                    return true;
                }
                
                // Version mismatch, retry with exponential backoff
                await Task.Delay(100 * (int)Math.Pow(2, attempt));
                Console.WriteLine($"[Optimistic] Retry {attempt + 1}/{maxRetries}");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (attempt == maxRetries - 1)
                    throw;
            }
        }
        
        Console.WriteLine("[Optimistic] Max retries exceeded");
        return false;
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Stock { get; set; }
    public int Version { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
