# Enterprise Solution: Token Bucket + Redis

## 🎯 Yaklaşım

**Token Bucket** algoritması en esnek ve production-grade rate limiting çözümüdür. Netflix, AWS, Google Cloud gibi büyük şirketler bu algoritmayı kullanır.

## 🪣 Token Bucket Metaforu

Bir kova düşünün:
- Kovada **token'lar** var (başlangıçta dolu)
- Her istek **1 token** harcar
- Token'lar **sabit hızda** yenilenir (refill rate)
- Kova dolarsa, fazla token'lar atılır (max capacity)

```
Bucket Capacity: 10 tokens
Refill Rate: 1 token/second

Time  Tokens  Request  Result
00:00  10     GET      ✅ (9 left)
00:01  10     -        (refilled to 10)
00:02  10     GET x10  ✅ (0 left)
00:03  1      GET      ✅ (0 left)
00:04  2      GET      ✅ (1 left)
00:05  3      GET x5   ✅ (0 left), ❌ (2 rejected)
```

## 🔧 Algoritma Detayları

### 1. Token Yenileme

```
tokens_to_add = (current_time - last_refill_time) * refill_rate
new_tokens = min(current_tokens + tokens_to_add, capacity)
```

### 2. Request İşleme

```
if (bucket.tokens >= 1):
    bucket.tokens -= 1
    return ALLOW
else:
    return REJECT (429)
```

### 3. Burst Handling

```
Normal: 1 req/sec, burst 10 req/sec
Bucket: capacity=10, refill_rate=1/sec

User can burst 10 requests immediately,
then throttled to 1 req/sec
```

## 💾 Veri Yapısı (Redis)

```lua
-- Redis Hash
HSET user:123:bucket tokens 10
HSET user:123:bucket last_refill 1704110400
HSET user:123:bucket capacity 10
HSET user:123:bucket refill_rate 1.0
```

**Alternatif**: Redis String ile serialize edilmiş JSON

```json
{
  "tokens": 7.5,
  "lastRefill": "2024-01-01T10:15:30Z",
  "capacity": 10,
  "refillRate": 1.0
}
```

## ✅ Avantajlar

1. **Smooth Rate Limiting**: Burst'e izin verir ama kontrollü
2. **Flexible**: Farklı refill rate'ler
3. **Fair**: Uniform distribution
4. **Distributed**: Redis ile multi-server support
5. **Memory Efficient**: Fixed size per user
6. **Accurate**: Matematiksel kesinlik

## ❌ Dezavantajlar

1. **Complexity**: İmplement etmesi en zor
2. **Redis Dependency**: Single point of failure
3. **Network Latency**: Redis call overhead (~1-5ms)
4. **Cost**: Redis hosting maliyeti

## 🏗️ Architecture

```
┌─────────┐      ┌─────────┐      ┌─────────┐
│ Client  │─────▶│  API    │─────▶│  Redis  │
│         │      │ Server  │      │ Cluster │
└─────────┘      │         │      │         │
                 │ Token   │      │ Buckets │
                 │ Bucket  │      │         │
                 │ Logic   │      │         │
                 └─────────┘      └─────────┘
                      │
                      ▼
                 ┌─────────┐
                 │Response │
                 │200 / 429│
                 └─────────┘
```

## 🔧 Redis Lua Script (Atomic Operation)

```lua
-- rate_limit.lua
local key = KEYS[1]
local capacity = tonumber(ARGV[1])
local refill_rate = tonumber(ARGV[2])
local requested = tonumber(ARGV[3])
local now = tonumber(ARGV[4])

-- Get current bucket state
local bucket = redis.call('HMGET', key, 'tokens', 'last_refill')
local tokens = tonumber(bucket[1]) or capacity
local last_refill = tonumber(bucket[2]) or now

-- Calculate tokens to add
local elapsed = math.max(0, now - last_refill)
local tokens_to_add = elapsed * refill_rate
tokens = math.min(capacity, tokens + tokens_to_add)

-- Try to consume tokens
if tokens >= requested then
    tokens = tokens - requested

    -- Update bucket
    redis.call('HSET', key, 'tokens', tokens, 'last_refill', now)
    redis.call('EXPIRE', key, 3600)

    return {1, tokens, capacity}  -- Allowed
else
    return {0, tokens, capacity}  -- Rejected
end
```

## 📊 Configuration Patterns

### Pattern 1: Fixed Rate

```csharp
// 100 requests per minute
new TokenBucketConfig
{
    Capacity = 100,
    RefillRate = 100.0 / 60.0,  // 1.66 tokens/sec
    RefillInterval = TimeSpan.FromSeconds(1)
}
```

### Pattern 2: Burst Tolerant

```csharp
// Normal: 10/min, Burst: 50 in first 10 seconds
new TokenBucketConfig
{
    Capacity = 50,              // Allow burst
    RefillRate = 10.0 / 60.0,   // 0.166 tokens/sec
    RefillInterval = TimeSpan.FromSeconds(1)
}
```

### Pattern 3: Multiple Tiers

```csharp
var configs = new Dictionary<string, TokenBucketConfig>
{
    ["free"] = new() { Capacity = 10, RefillRate = 10/60.0 },
    ["basic"] = new() { Capacity = 100, RefillRate = 100/60.0 },
    ["premium"] = new() { Capacity = 1000, RefillRate = 1000/60.0 },
    ["enterprise"] = new() { Capacity = 10000, RefillRate = 10000/60.0 }
};
```

## 🎯 Distributed Scenario

### Problem: Multiple Servers

```
Server 1 → User A: 5 req
Server 2 → User A: 7 req
Server 3 → User A: 3 req

Without Redis: Each server has own counter (total 15, should be 10)
With Redis: Shared state across servers ✅
```

### Solution: Redis Cluster

```csharp
public class DistributedTokenBucket
{
    private readonly IConnectionMultiplexer _redis;
    private readonly string _luaScript;

    public async Task<bool> AllowRequestAsync(string userId, int tokens = 1)
    {
        var db = _redis.GetDatabase();
        var key = $"rate_limit:{userId}";

        var result = await db.ScriptEvaluateAsync(
            _luaScript,
            new RedisKey[] { key },
            new RedisValue[]
            {
                _capacity,
                _refillRate,
                tokens,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            }
        );

        var values = (RedisValue[])result;
        var allowed = (int)values[0] == 1;
        var remaining = (double)values[1];
        var capacity = (int)values[2];

        return allowed;
    }
}
```

## 🚀 Advanced Features

### 1. Graceful Degradation

```csharp
public async Task<bool> AllowRequestAsync(string userId)
{
    try
    {
        return await _tokenBucket.AllowRequestAsync(userId);
    }
    catch (RedisException ex)
    {
        _logger.LogError(ex, "Redis unavailable, allowing request (fail-open)");
        return true;  // Fail-open: Allow when Redis is down
    }
}
```

### 2. Cost-Based Rate Limiting

```csharp
// Different endpoints consume different tokens
var costs = new Dictionary<string, int>
{
    ["/api/search"] = 1,      // Cheap
    ["/api/report"] = 10,     // Expensive
    ["/api/export"] = 50      // Very expensive
};

await AllowRequestAsync(userId, tokens: costs[endpoint]);
```

### 3. Predictive Refill

```csharp
// Refill faster during off-peak hours
var refillRate = DateTime.UtcNow.Hour is >= 0 and < 6
    ? _baseRate * 2.0  // Double refill rate at night
    : _baseRate;
```

### 4. Retry-After Header

```csharp
if (!allowed)
{
    var waitTime = (1.0 - remaining) / _refillRate;
    context.Response.Headers["Retry-After"] = ((int)waitTime).ToString();
}
```

## 📈 Performance Benchmarks

| Metric | In-Memory | Redis (Local) | Redis (Cluster) |
|--------|-----------|---------------|-----------------|
| Latency (p50) | 0.1ms | 1.2ms | 3.5ms |
| Latency (p99) | 0.5ms | 5ms | 15ms |
| Throughput | 100K req/s | 50K req/s | 20K req/s |
| Memory/User | 32 bytes | 64 bytes | 64 bytes |

## 🧪 Test Scenarios

### Test 1: Token Refill

```csharp
[Fact]
public async Task RefillsTokensOverTime()
{
    var bucket = new TokenBucket(capacity: 10, refillRate: 1.0);

    // Consume all tokens
    for (int i = 0; i < 10; i++)
        Assert.True(await bucket.AllowRequestAsync("user"));

    Assert.False(await bucket.AllowRequestAsync("user"));

    // Wait for 5 seconds = 5 tokens refilled
    await Task.Delay(5000);

    for (int i = 0; i < 5; i++)
        Assert.True(await bucket.AllowRequestAsync("user"));

    Assert.False(await bucket.AllowRequestAsync("user"));
}
```

### Test 2: Burst Handling

```csharp
[Fact]
public async Task AllowsBurstThenThrottles()
{
    var bucket = new TokenBucket(capacity: 10, refillRate: 1.0);

    // Burst: 10 requests immediately
    for (int i = 0; i < 10; i++)
        Assert.True(await bucket.AllowRequestAsync("user"));

    // Now throttled to 1 req/sec
    Assert.False(await bucket.AllowRequestAsync("user"));

    await Task.Delay(1000);
    Assert.True(await bucket.AllowRequestAsync("user"));

    await Task.Delay(1000);
    Assert.True(await bucket.AllowRequestAsync("user"));
}
```

## 🏢 Production Checklist

- [ ] Redis Cluster (high availability)
- [ ] Monitoring (Prometheus/Grafana)
- [ ] Alerting (Redis down, high latency)
- [ ] Graceful degradation (fail-open/fail-closed)
- [ ] Rate limit analytics
- [ ] Different tiers (free/premium)
- [ ] Cost-based limiting
- [ ] Retry-After headers
- [ ] Documentation (API limits)
- [ ] Load testing (1M+ req/s)

## 📝 Özet

Token Bucket + Redis kombinasyonu **enterprise-grade** rate limiting için ideal çözümdür.

**Avantajlar**:
- Distributed
- Scalable
- Accurate
- Flexible

**Requires**:
- Redis expertise
- Monitoring
- Cost consideration

**Sonraki Adım**: `COMPARISON.md` - Üç çözümün detaylı karşılaştırması
