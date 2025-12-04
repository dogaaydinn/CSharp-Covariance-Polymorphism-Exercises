# ADR-0005: Redis for Distributed Caching

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** Caching layer for video metadata and query optimization

## Context

The VideoService API faces several performance challenges:
- Database queries for frequently accessed videos (popular content)
- Repeated reads of video lists (pagination, filtering)
- High read-to-write ratio (100:1 typical for video platforms)
- Need to reduce database load
- Horizontal scaling requires shared cache across instances

Requirements:
- Sub-millisecond read latency
- Support for data structures (strings, lists, sets)
- TTL (time-to-live) expiration
- Atomic operations
- Horizontal scaling support
- Cloud-native deployment
- .NET client library quality

## Decision

We will use **Redis 7** as the distributed caching layer for AspireVideoService.

Implementation:
- Development: Docker container via Aspire (`builder.AddRedis()`)
- Production: Managed Redis (Azure Cache for Redis, AWS ElastiCache)
- Client: StackExchange.Redis
- Management: Redis Commander (included via Aspire)
- Caching Strategy: Cache-aside pattern

## Consequences

### Positive

- **Performance**: 100x faster than database queries (0.1ms vs 10ms)
- **Scalability**: Reduces database load by 80-90%
- **Simple API**: Key-value operations with minimal learning curve
- **Data Structures**: Support for lists, sets, sorted sets, hashes
- **TTL Support**: Automatic cache expiration
- **Atomic Operations**: INCR, DECR for view counts
- **Pub/Sub**: Built-in messaging (future feature enabler)
- **Persistence**: Optional data durability (RDB, AOF)
- **Aspire Integration**: Zero-config setup via `AddRedisClient()`
- **Cloud-Native**: Available in all major clouds
- **Open Source**: No licensing costs
- **Community**: Massive ecosystem and documentation

### Negative

- **Memory Cost**: Caching adds infrastructure cost (memory is expensive)
- **Cache Invalidation**: "Two hard problems in CS" - requires careful strategy
- **Data Staleness**: Cached data may be out-of-date
- **Single Point of Failure**: Without replication, Redis outage affects availability
- **Complexity**: Additional moving part to monitor and maintain
- **Serialization Overhead**: Must serialize .NET objects to/from Redis

### Neutral

- **Eviction Policies**: Must configure appropriate eviction strategy (LRU, LFU)
- **Monitoring Required**: Need to track cache hit rates and memory usage
- **Network Latency**: Redis adds network hop (mitigated by co-location)

## Alternatives Considered

### Alternative 1: In-Memory Cache (IMemoryCache)

**Pros:**
- **Zero Infrastructure**: Built into .NET runtime
- **Zero Latency**: No network round-trip
- **Zero Cost**: No additional infrastructure
- **Simple**: Just use `IMemoryCache` interface

**Cons:**
- **Not Distributed**: Each instance has separate cache (cache duplication)
- **No Horizontal Scaling**: Cache not shared across pods/instances
- **Memory Limits**: Limited to single server's RAM
- **No Persistence**: Lost on app restart
- **No Atomic Operations**: Cannot implement distributed counters

**Why rejected:** Cannot scale horizontally. In Kubernetes with 3+ replicas, each replica caches independently, wasting 3x memory and missing 66% of cache hits.

### Alternative 2: Memcached

**Pros:**
- **Simple**: Pure key-value cache, simpler than Redis
- **Multi-threaded**: Better CPU utilization than Redis
- **Memory Efficient**: Slightly lower memory overhead
- **Mature**: 20+ years of production use

**Cons:**
- **No Data Structures**: Only strings (no lists, sets)
- **No Persistence**: Cache lost on restart
- **No Atomic Operations**: Cannot implement INCR/DECR reliably
- **No Pub/Sub**: Cannot implement real-time features
- **Weaker .NET Support**: Limited client libraries
- **No Aspire Integration**: No built-in Aspire support

**Why rejected:** Redis offers significantly more features at similar performance. No reason to choose Memcached in 2024.

### Alternative 3: Azure Cache for Redis (Managed)

**Pros:**
- **Fully Managed**: No infrastructure management
- **High Availability**: Built-in replication and failover
- **Security**: VNet integration, private endpoints
- **Scaling**: Easy vertical and horizontal scaling

**Cons:**
- **Cost**: Expensive ($0.20+/hour minimum)
- **Azure Lock-in**: Only available on Azure
- **Development**: Still need local Redis for dev (Aspire provides this)

**Why rejected:** This IS the production choice. For development, we use containerized Redis via Aspire. ADR focuses on the technology (Redis), not hosting model.

### Alternative 4: SQL Server In-Memory Tables

**Pros:**
- **Integrated**: No separate infrastructure
- **ACID**: Full transactional guarantees
- **Familiar**: SQL syntax

**Cons:**
- **Expensive**: Requires SQL Server Enterprise ($14,000/core)
- **Slower**: 10x slower than Redis
- **Complex**: Complex configuration and management
- **Not Cloud-Native**: Poor fit for containers

**Why rejected:** Not true caching. In-memory tables are for OLTP performance, not distributed caching.

### Alternative 5: Database Query Cache

**Pros:**
- **No Code Changes**: Transparent caching
- **Simple**: Just enable feature flag

**Cons:**
- **Limited Control**: Cannot control invalidation strategy
- **Not Distributed**: Per-database-server cache
- **PostgreSQL Limitation**: PostgreSQL's query cache is minimal
- **Coarse-Grained**: Cannot cache application-level entities

**Why rejected:** Insufficient control over caching strategy. Application-level caching (Redis) provides fine-grained control.

## Related Decisions

- [ADR-0006](0006-stackexchange-redis-client.md): StackExchange.Redis client library
- [ADR-0016](0016-cache-aside-pattern.md): Cache-aside pattern implementation
- [ADR-0002](0002-using-dotnet-aspire.md): Aspire provides Redis hosting

## Related Links

- [Redis Documentation](https://redis.io/docs/)
- [Redis Best Practices](https://redis.io/docs/manual/patterns/)
- [Aspire Redis Component](https://learn.microsoft.com/dotnet/aspire/caching/stackexchange-redis-component)
- [Cache-Aside Pattern](https://learn.microsoft.com/azure/architecture/patterns/cache-aside)
- [Redis vs Memcached](https://redis.io/glossary/redis-vs-memcached/)

## Notes

- **TTL Strategy**:
  - Video lists: 5 minutes (high change frequency)
  - Individual videos: 10 minutes (lower change frequency)
  - Popular videos: Consider longer TTL with active refresh
- **Key Naming Convention**: `{entity}:{id}` (e.g., `video:123`, `videos:all`)
- **Invalidation Strategy**: Write-through invalidation on updates
- **Memory Limits**: Set `maxmemory-policy` to `allkeys-lru` (Least Recently Used)
- **Monitoring**: Track hit rate (target: >80%), memory usage, evictions
- **Connection Pooling**: StackExchange.Redis handles automatically
- **Production**: Use Redis Cluster for >50GB data or Sentinel for HA
- **Serialization**: Use `System.Text.Json` for .NET objects
- **Future**: Consider Redis Streams for real-time video processing events
