# ADR-0006: StackExchange.Redis Client Library

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** Redis client library selection

## Context

Having chosen Redis as our caching layer (ADR-0005), we need a .NET client library to interact with Redis. The client must provide:
- High performance and low latency
- Connection pooling and multiplexing
- Async/await support
- Pipeline and batch operations
- Pub/Sub support
- Strong typing where possible
- Production-grade reliability
- Active maintenance

Several Redis clients exist in the .NET ecosystem with different trade-offs.

## Decision

We will use **StackExchange.Redis 2.8+** as the Redis client library.

Integration:
- Package: `Aspire.StackExchange.Redis` (includes StackExchange.Redis)
- Configuration: Via Aspire service defaults
- Injection: `IDatabase` injected into API endpoints
- Connection: Multiplexed (single connection for all operations)

## Consequences

### Positive

- **Performance**: Highly optimized, minimal overhead
- **Connection Multiplexing**: Single connection handles 1000s of operations/sec
- **Async-First**: Native async/await support
- **Pipeline Support**: Batch operations for improved throughput
- **Pub/Sub**: Built-in support for Redis messaging
- **Lua Scripts**: Support for atomic server-side operations
- **Reconnection**: Automatic reconnection on connection loss
- **Monitoring**: Built-in performance counters
- **Aspire Integration**: First-class Aspire support
- **Production-Proven**: Used by Stack Overflow, Microsoft, thousands of companies
- **Active Maintenance**: Backed by Stack Overflow team
- **Documentation**: Comprehensive docs and examples

### Negative

- **Complexity**: Advanced features have learning curve
- **Synchronous API**: Some operations have sync-over-async patterns
- **Error Handling**: Redis errors sometimes surfaced as exceptions, sometimes as null
- **Connection Strings**: Complex configuration options
- **No Built-in Serialization**: Must serialize objects manually

### Neutral

- **Breaking Changes**: Major versions can have breaking changes (2.0 → 3.0)
- **Firewall-Friendly**: Uses single multiplexed connection (good and bad)

## Alternatives Considered

### Alternative 1: ServiceStack.Redis

**Pros:**
- **High-Level API**: More abstractions than StackExchange.Redis
- **Built-in Serialization**: Automatic JSON serialization
- **Typed Clients**: Strong typing with generics
- **Complete**: Includes ORM-like features

**Cons:**
- **Commercial License**: Free for <10 tables, paid for commercial ($4500+)
- **Heavier**: More dependencies and abstractions
- **Slower**: Additional abstraction layers reduce performance
- **No Aspire Integration**: No official Aspire component
- **Smaller Community**: Less widely adopted than StackExchange.Redis

**Why rejected:** Licensing costs inappropriate for open-source samples. StackExchange.Redis is faster and has broader adoption.

### Alternative 2: CSRedis

**Pros:**
- **Free**: Completely open source
- **Cluster Support**: Good Redis Cluster support
- **Chinese Community**: Large Chinese developer community

**Cons:**
- **Less Mature**: Newer project, less battle-tested
- **Documentation**: Limited English documentation
- **No Aspire Integration**: No official support
- **Smaller Ecosystem**: Fewer resources and examples

**Why rejected:** StackExchange.Redis is the de facto standard. No compelling reason to choose alternative.

### Alternative 3: Microsoft.Extensions.Caching.StackExchangeRedis

**Pros:**
- **Microsoft Official**: Part of .NET Extensions
- **IDistributedCache**: Standard interface
- **Simple API**: Minimal learning curve

**Cons:**
- **Limited Features**: Only caching, no Pub/Sub or advanced features
- **Abstraction Overhead**: Additional layer over StackExchange.Redis
- **Serialization Required**: Must serialize everything
- **Not Aspire-Native**: Aspire uses StackExchange.Redis directly

**Why rejected:** This is a wrapper around StackExchange.Redis. Using StackExchange.Redis directly provides more features and control.

### Alternative 4: FreeRedis

**Pros:**
- **Modern**: Designed for .NET 6+
- **Pipeline First**: Optimized for batch operations
- **Cluster Support**: Good cluster support

**Cons:**
- **New**: Less mature than StackExchange.Redis
- **Smaller Community**: Limited adoption
- **No Aspire Integration**: Not supported by Aspire
- **Unknown Longevity**: Uncertain long-term maintenance

**Why rejected:** StackExchange.Redis is proven and Aspire-integrated. No need for alternatives.

## Related Decisions

- [ADR-0005](0005-redis-distributed-caching.md): Redis chosen for caching
- [ADR-0002](0002-using-dotnet-aspire.md): Aspire provides Redis integration
- [ADR-0016](0016-cache-aside-pattern.md): Implementation pattern

## Related Links

- [StackExchange.Redis Documentation](https://stackexchange.github.io/StackExchange.Redis/)
- [StackExchange.Redis GitHub](https://github.com/StackExchange/StackExchange.Redis)
- [Aspire Redis Component](https://learn.microsoft.com/dotnet/aspire/caching/stackexchange-redis-component)
- [Redis Client Comparison](https://redis.io/clients#dotnet)

## Notes

- **Connection Configuration**:
  ```csharp
  builder.AddRedisClient("cache"); // Aspire handles configuration
  ```

- **Usage Pattern**:
  ```csharp
  var cache = app.Services.GetRequiredService<IConnectionMultiplexer>();
  var db = cache.GetDatabase();
  await db.StringSetAsync("key", "value");
  ```

- **Performance Tips**:
  - Use `StringGetAsync` for simple values
  - Use `HashGetAllAsync` for complex objects
  - Use pipelines (`IBatch`) for multiple operations
  - Avoid `Keys` command in production (use `Scan` instead)

- **Error Handling**:
  - Wrap Redis operations in try-catch
  - Degrade gracefully if Redis unavailable
  - Log Redis errors but don't crash app

- **Monitoring**:
  - Track connection count: `GetStatus()`
  - Monitor operation latency
  - Watch for `TimeoutException` (indicates Redis overload)

- **Production Considerations**:
  - Use connection string with `abortConnect=false`
  - Set `ConnectRetry=3` for automatic reconnection
  - Enable `ResolveDns=true` for DNS changes
  - Use SSL/TLS in production: `ssl=true`

- **Future**: Consider Redis JSON module for native JSON support
