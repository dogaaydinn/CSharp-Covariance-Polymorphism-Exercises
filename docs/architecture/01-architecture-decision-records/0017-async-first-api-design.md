# ADR-0017: Async-First API Design

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** API performance and scalability

## Context

ASP.NET Core supports both synchronous and asynchronous endpoints. Async is critical for I/O-bound operations (database, cache, HTTP calls) to avoid thread pool exhaustion.

Synchronous approach:
```csharp
app.MapGet("/api/videos", (VideoDbContext db) =>
{
    return db.Videos.ToList(); // BLOCKS thread!
});
```

Asynchronous approach:
```csharp
app.MapGet("/api/videos", async (VideoDbContext db) =>
{
    return await db.Videos.ToListAsync(); // Frees thread!
});
```

## Decision

All API endpoints MUST be asynchronous (`async/await`).

Rules:
- Use `async Task<T>` for all endpoints
- Use `*Async` methods (ToListAsync, FindAsync, etc.)
- Never block on async code (`Wait()`, `Result`)
- Use `ConfigureAwait(false)` where appropriate (library code)

## Consequences

### Positive

- **Scalability**: Can handle 10x more concurrent requests
- **Responsiveness**: Server doesn't block on I/O
- **Thread Pool**: Efficient thread usage (no starvation)
- **Best Practice**: Industry standard for ASP.NET Core

### Negative

- **Complexity**: Async code slightly more complex
- **Debugging**: Async stack traces harder to read
- **Learning Curve**: Developers must understand async/await

## Why Async Matters

**Scenario**: 100 concurrent requests to endpoint that queries database (50ms query time)

**Synchronous** (blocking):
- Each request blocks a thread for 50ms
- Thread pool limited to 100 threads (default)
- Result: 100 requests/second max

**Asynchronous** (non-blocking):
- Threads released during I/O
- Can handle 1000s of concurrent requests
- Result: 2000+ requests/second

## Related Links

- [Async/Await Best Practices](https://learn.microsoft.com/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)
- [ASP.NET Core Performance](https://learn.microsoft.com/aspnet/core/performance/performance-best-practices)

## Notes

- Always use `*Async` methods: `ToListAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync`
- Redis: `StringGetAsync`, `StringSetAsync`
- HTTP: `GetFromJsonAsync`, `PostAsJsonAsync`
- Never: `.Wait()`, `.Result`, `.GetAwaiter().GetResult()`
