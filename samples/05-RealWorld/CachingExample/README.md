# Caching Example

> In-memory and distributed caching strategies for performance.

## Patterns
- **Cache-Aside** - Check cache first, load from DB on miss
- **IMemoryCache** - In-process caching
- **Expiration** - Time-based invalidation

## Output
```
=== Caching Example ===

  → Cache MISS, querying database...
First call: User42 (from database)
  → Cache HIT!
Second call: User42 (from cache!)
  → Cache MISS, querying database...
After invalidation: User42 (from database again)
```

## Run
```bash
dotnet run
```

## Production Use
For distributed scenarios, replace `IMemoryCache` with `IDistributedCache` (Redis, SQL Server).
