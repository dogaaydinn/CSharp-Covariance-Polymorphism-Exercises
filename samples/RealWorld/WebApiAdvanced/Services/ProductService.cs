using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using WebApiAdvanced.Models;

namespace WebApiAdvanced.Services;

/// <summary>
/// Product service implementation with in-memory storage and caching
/// Demonstrates caching pattern
/// </summary>
public class ProductService : IProductService
{
    private readonly ConcurrentDictionary<Guid, Product> _products = new();
    private readonly IMemoryCache _cache;
    private readonly ILogger<ProductService> _logger;
    private const string AllProductsCacheKey = "all_products";

    public ProductService(IMemoryCache cache, ILogger<ProductService> logger)
    {
        _cache = cache;
        _logger = logger;
        SeedData();
    }

    private void SeedData()
    {
        // Add some initial data
        var products = new[]
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Laptop",
                Description = "High-performance laptop",
                Price = 1299.99m,
                Stock = 10
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Mouse",
                Description = "Wireless mouse",
                Price = 29.99m,
                Stock = 50
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Keyboard",
                Description = "Mechanical keyboard",
                Price = 89.99m,
                Stock = 25
            }
        };

        foreach (var product in products)
        {
            _products.TryAdd(product.Id, product);
        }
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        // Check cache first
        if (_cache.TryGetValue(AllProductsCacheKey, out IEnumerable<Product>? cachedProducts))
        {
            _logger.LogInformation("Products retrieved from cache");
            return cachedProducts!;
        }

        // If not in cache, get from "database"
        var products = _products.Values.ToList();

        // Cache for 5 minutes
        _cache.Set(AllProductsCacheKey, products, TimeSpan.FromMinutes(5));

        _logger.LogInformation("Products retrieved from database and cached");
        return await Task.FromResult(products);
    }

    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        var cacheKey = $"product_{id}";

        // Check cache
        if (_cache.TryGetValue(cacheKey, out Product? cachedProduct))
        {
            _logger.LogInformation("Product {ProductId} retrieved from cache", id);
            return cachedProduct;
        }

        // Get from "database"
        _products.TryGetValue(id, out var product);

        if (product != null)
        {
            // Cache for 5 minutes
            _cache.Set(cacheKey, product, TimeSpan.FromMinutes(5));
            _logger.LogInformation("Product {ProductId} retrieved from database and cached", id);
        }

        return await Task.FromResult(product);
    }

    public async Task<Product> CreateProductAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _products.TryAdd(product.Id, product);

        // Invalidate cache
        _cache.Remove(AllProductsCacheKey);

        _logger.LogInformation("Product {ProductId} created: {ProductName}", product.Id, product.Name);

        return await Task.FromResult(product);
    }

    public async Task<Product?> UpdateProductAsync(Guid id, UpdateProductRequest request)
    {
        if (!_products.TryGetValue(id, out var product))
            return null;

        // Update only provided fields
        if (request.Name != null) product.Name = request.Name;
        if (request.Description != null) product.Description = request.Description;
        if (request.Price.HasValue) product.Price = request.Price.Value;
        if (request.Stock.HasValue) product.Stock = request.Stock.Value;
        if (request.IsActive.HasValue) product.IsActive = request.IsActive.Value;

        product.UpdatedAt = DateTime.UtcNow;

        // Invalidate caches
        _cache.Remove(AllProductsCacheKey);
        _cache.Remove($"product_{id}");

        _logger.LogInformation("Product {ProductId} updated", id);

        return await Task.FromResult(product);
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var removed = _products.TryRemove(id, out _);

        if (removed)
        {
            // Invalidate caches
            _cache.Remove(AllProductsCacheKey);
            _cache.Remove($"product_{id}");

            _logger.LogInformation("Product {ProductId} deleted", id);
        }

        return await Task.FromResult(removed);
    }
}
