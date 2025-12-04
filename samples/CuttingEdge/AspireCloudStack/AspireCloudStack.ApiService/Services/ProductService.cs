using AspireCloudStack.ApiService.Data;
using AspireCloudStack.ApiService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AspireCloudStack.ApiService.Services;

/// <summary>
/// Product service with Redis caching
/// </summary>
public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ProductService> _logger;
    private const string CacheKeyPrefix = "product:";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

    public ProductService(
        ApplicationDbContext context,
        IDistributedCache cache,
        ILogger<ProductService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        const string cacheKey = $"{CacheKeyPrefix}all";

        // Try cache first
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Products retrieved from cache");
            return JsonSerializer.Deserialize<List<Product>>(cachedData) ?? new List<Product>();
        }

        // Cache miss - fetch from database
        var products = await _context.Products.Where(p => p.IsActive).ToListAsync();

        // Store in cache
        var serialized = JsonSerializer.Serialize(products);
        await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheExpiration
        });

        _logger.LogInformation("Products retrieved from database and cached");
        return products;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";

        // Try cache first
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Product {Id} retrieved from cache", id);
            return JsonSerializer.Deserialize<Product>(cachedData);
        }

        // Cache miss - fetch from database
        var product = await _context.Products.FindAsync(id);
        if (product != null && product.IsActive)
        {
            // Store in cache
            var serialized = JsonSerializer.Serialize(product);
            await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpiration
            });

            _logger.LogInformation("Product {Id} retrieved from database and cached", id);
        }

        return product;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Invalidate cache
        await _cache.RemoveAsync($"{CacheKeyPrefix}all");

        _logger.LogInformation("Product {Id} created", product.Id);
        return product;
    }

    public async Task<Product?> UpdateAsync(int id, Product product)
    {
        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null)
        {
            return null;
        }

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.Stock = product.Stock;
        existingProduct.IsActive = product.IsActive;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Invalidate cache
        await _cache.RemoveAsync($"{CacheKeyPrefix}{id}");
        await _cache.RemoveAsync($"{CacheKeyPrefix}all");

        _logger.LogInformation("Product {Id} updated", id);
        return existingProduct;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return false;
        }

        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Invalidate cache
        await _cache.RemoveAsync($"{CacheKeyPrefix}{id}");
        await _cache.RemoveAsync($"{CacheKeyPrefix}all");

        _logger.LogInformation("Product {Id} deleted (soft delete)", id);
        return true;
    }
}
