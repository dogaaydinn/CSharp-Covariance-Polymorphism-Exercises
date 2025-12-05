# Distributed Locking Comparison

## Quick Comparison

| Strategy | Latency | Throughput | Complexity | Failure Mode |
|----------|---------|------------|------------|--------------|
| Database Lock | 50ms | 20 req/s | Low | Deadlock |
| Optimistic Concurrency | 10ms | 100 req/s | Medium | Retry storm |
| Redis Distributed Lock | 5ms | 500 req/s | High | Lock timeout |

## When to Use

**Database Lock**: Single server, low traffic (<50 req/s)
**Optimistic Concurrency**: Multi-server, low contention
**Redis Lock**: Multi-server, high concurrency (>100 req/s)

## Real-World Example

E-commerce flash sale: 1000 products, 10,000 buyers in 1 minute
- Database Lock: ~50 successful purchases, many timeouts
- Optimistic: ~300 successful, many retries
- Redis Lock: ~950 successful, minimal failures
