# ÇÖZÜM 1: IN-MEMORY CACHING (IMemoryCache)

## 🎯 ÇÖZÜM ÖZETİ

ASP.NET Core'un built-in IMemoryCache kullanarak basit, performanslı caching.

**Kimler için:** Single-server, prototypes, internal tools

---

## 💻 IMPLEMENTATION

```csharp
public class ProductService
{
    private readonly IMemoryCache _cache;
    private readonly AppDbContext _context;

    public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        var cacheKey = $"products_category_{categoryId}";
        
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            entry.SetPriority(CacheItemPriority.Normal);
            
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .OrderByDescending(p => p.SalesRank)
                .ToListAsync();
        });
    }

    public void InvalidateProductCache(int categoryId)
    {
        _cache.Remove($"products_category_{categoryId}");
    }
}
```

**Avantajlar:**
- ✅ Super fast (<1ms)
- ✅ No external dependency
- ✅ Easy to implement

**Dezavantajlar:**
- ❌ Not distributed (each instance has own cache)
- ❌ Memory limited
- ❌ Lost on restart
