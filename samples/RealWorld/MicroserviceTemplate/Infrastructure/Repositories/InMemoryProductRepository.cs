using System.Collections.Concurrent;
using MicroserviceTemplate.Domain.Entities;
using MicroserviceTemplate.Domain.Repositories;

namespace MicroserviceTemplate.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of IProductRepository
/// In real application, this would use EF Core or Dapper
/// Demonstrates Dependency Inversion Principle (Domain interface, Infrastructure implementation)
/// </summary>
public class InMemoryProductRepository : IProductRepository
{
    // Thread-safe dictionary for concurrent access
    private readonly ConcurrentDictionary<Guid, Product> _products = new();

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _products.TryGetValue(id, out var product);
        return Task.FromResult(product);
    }

    public Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = _products.Values.AsEnumerable();
        return Task.FromResult(products);
    }

    public Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        if (!_products.TryAdd(product.Id, product))
        {
            throw new InvalidOperationException($"Product with ID {product.Id} already exists");
        }

        return Task.FromResult(product);
    }

    public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        if (!_products.ContainsKey(product.Id))
        {
            throw new InvalidOperationException($"Product with ID {product.Id} not found");
        }

        _products[product.Id] = product;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!_products.TryRemove(id, out _))
        {
            throw new InvalidOperationException($"Product with ID {id} not found");
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_products.ContainsKey(id));
    }
}
