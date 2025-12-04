# ADR-0016: Cache-Aside Pattern for Data Access

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** Caching strategy implementation

## Context

With Redis as our caching layer (ADR-0005), we need a strategy for when and how to cache. Common patterns:
- **Cache-Aside**: Application checks cache, loads from DB on miss, updates cache
- **Read-Through**: Cache automatically loads from DB
- **Write-Through**: Write to cache and DB simultaneously
- **Write-Behind**: Write to cache, async write to DB

## Decision

Implement **Cache-Aside** pattern for all cached queries.

Flow:
1. Check cache (Redis GET)
2. If HIT: Return cached data
3. If MISS: Query database
4. Store in cache with TTL
5. Return data

Invalidation:
- On UPDATE/DELETE: Invalidate cache key
- On INSERT: Invalidate list cache

## Consequences

### Positive

- **Simple**: Easy to understand and implement
- **Flexible**: Application controls caching logic
- **Consistent**: Database is source of truth
- **Performance**: 100x faster reads (cache hit)

### Negative

- **Cache Invalidation**: Must manually invalidate on writes
- **Stampeding Herd**: Multiple requests on cache miss (mitigated with locks)
- **Stale Data**: Cache may be out of sync for TTL duration

## Implementation Example

```csharp
app.MapGet("/api/videos/{id}", async (int id, VideoDbContext db, IDatabase cache) =>
{
    var cacheKey = $"video:{id}";
    
    // Try cache first
    var cached = await cache.StringGetAsync(cacheKey);
    if (!cached.IsNullOrEmpty)
        return Results.Ok(JsonSerializer.Deserialize<Video>(cached!));
    
    // Cache miss - query database
    var video = await db.Videos.FindAsync(id);
    if (video == null) return Results.NotFound();
    
    // Update cache with 10-minute TTL
    await cache.StringSetAsync(cacheKey,
        JsonSerializer.Serialize(video),
        TimeSpan.FromMinutes(10));
    
    return Results.Ok(video);
});

app.MapPut("/api/videos/{id}", async (int id, Video updated, VideoDbContext db, IDatabase cache) =>
{
    var video = await db.Videos.FindAsync(id);
    if (video == null) return Results.NotFound();
    
    // Update database
    video.Title = updated.Title;
    await db.SaveChangesAsync();
    
    // Invalidate cache
    await cache.KeyDeleteAsync($"video:{id}");
    await cache.KeyDeleteAsync("videos:all");
    
    return Results.Ok(video);
});
```

## Related Decisions

- [ADR-0005](0005-redis-distributed-caching.md): Redis for caching
- [ADR-0006](0006-stackexchange-redis-client.md): Client library

## Notes

- **TTL Strategy**:
  - Hot data: 30-60 minutes
  - Warm data: 10-30 minutes
  - Cold data: 5-10 minutes

- **Key Naming**:
  - `{entity}:{id}` for single items
  - `{entity}:all` for lists
  - `{entity}:{category}:{id}` for hierarchical

- **Stampeding Herd Prevention**:
  ```csharp
  var lockKey = $"lock:{cacheKey}";
  if (await cache.StringSetAsync(lockKey, "1", TimeSpan.FromSeconds(10), When.NotExists))
  {
      // This thread loads from DB
      var data = await LoadFromDatabase();
      await cache.StringSetAsync(cacheKey, data, ttl);
      await cache.KeyDeleteAsync(lockKey);
  }
  else
  {
      // Wait and retry cache
      await Task.Delay(100);
      return await cache.StringGetAsync(cacheKey);
  }
  ```
