# ÇÖZÜM 3: ENTERPRISE RATE LIMITING (Token Bucket + Analytics)

## 🎯 ÇÖZÜM ÖZETİ

Silicon Valley şirketlerinin kullandığı production-grade rate limiting: Token Bucket algorithm + Multi-tier + Analytics + Abuse Detection

**Kimler için:** Enterprise sistemler, high-traffic APIs, SaaS platformları

---

## 🏗️ TOKEN BUCKET ALGORITHM

**Nasıl Çalışır:**
```
Bucket: 100 token capacity
Refill Rate: 10 token/second

t=0s  → Bucket: 100 tokens → 50 request → 50 tokens kaldı
t=5s  → Bucket: 50 + (5*10) = 100 tokens (max cap)
t=10s → Burst: 100 request → 0 tokens → Next request WAIT or REJECT
```

**Avantajları:**
- ✅ Burst traffic'e izin verir (legitimate use case)
- ✅ Smooth rate limiting (sudden block yok)
- ✅ Flexible (capacity + refill rate ayrı configure edilebilir)

---

## 💻 IMPLEMENTATION

```csharp
public class TokenBucketRateLimiter
{
    private readonly IConnectionMultiplexer _redis;
    private const string LuaScript = @"
        local key = KEYS[1]
        local capacity = tonumber(ARGV[1])
        local refill_rate = tonumber(ARGV[2])
        local requested = tonumber(ARGV[3])
        local now = tonumber(ARGV[4])
        
        local bucket = redis.call('HMGET', key, 'tokens', 'last_refill')
        local tokens = tonumber(bucket[1])
        local last_refill = tonumber(bucket[2])
        
        if tokens == nil then
            tokens = capacity
            last_refill = now
        end
        
        -- Calculate refill
        local elapsed = now - last_refill
        local refill_tokens = elapsed * refill_rate
        tokens = math.min(capacity, tokens + refill_tokens)
        
        local allowed = 0
        if tokens >= requested then
            tokens = tokens - requested
            allowed = 1
        end
        
        redis.call('HMSET', key, 'tokens', tokens, 'last_refill', now)
        redis.call('EXPIRE', key, 3600)
        
        return {allowed, math.floor(tokens), capacity}
    ";

    public async Task<RateLimitResult> CheckAsync(
        string clientId, 
        TierConfig tier,
        int requestTokens = 1)
    {
        var db = _redis.GetDatabase();
        var key = $"tokenbucket:{clientId}";
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var result = await db.ScriptEvaluateAsync(
            LuaScript,
            new RedisKey[] { key },
            new RedisValue[] 
            { 
                tier.BucketCapacity,
                tier.RefillRatePerSecond,
                requestTokens,
                now
            });

        var resultArray = (RedisValue[])result;
        return new RateLimitResult
        {
            IsAllowed = (int)resultArray[0] == 1,
            RemainingTokens = (int)resultArray[1],
            Capacity = (int)resultArray[2]
        };
    }
}

public class TierConfig
{
    public int BucketCapacity { get; set; }
    public double RefillRatePerSecond { get; set; }
}

// Configuration
var tiers = new Dictionary<string, TierConfig>
{
    ["Free"] = new() { BucketCapacity = 100, RefillRatePerSecond = 0.028 }, // ~100/hour
    ["Premium"] = new() { BucketCapacity = 1000, RefillRatePerSecond = 2.78 }, // ~10K/hour
    ["Enterprise"] = new() { BucketCapacity = 10000, RefillRatePerSecond = 27.8 } // ~100K/hour
};
```

---

## 📊 ANALYTICS & ABUSE DETECTION

```csharp
public class RateLimitAnalytics
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RateLimitAnalytics> _logger;

    public async Task TrackViolationAsync(RateLimitViolation violation)
    {
        var db = _redis.GetDatabase();
        
        // Store violation
        var key = $"violations:{violation.ClientId}:{DateTime.UtcNow:yyyyMMdd}";
        await db.StringIncrementAsync(key);
        await db.KeyExpireAsync(key, TimeSpan.FromDays(7));
        
        // Check abuse pattern
        var violations = await db.StringGetAsync(key);
        if ((int)violations > 100) // 100+ violations per day
        {
            await TriggerAbuseAlertAsync(violation.ClientId, (int)violations);
        }
    }

    public async Task<RateLimitMetrics> GetMetricsAsync(string clientId)
    {
        var db = _redis.GetDatabase();
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        
        return new RateLimitMetrics
        {
            TotalRequests = await GetTotalRequestsAsync(clientId, today),
            BlockedRequests = await GetBlockedRequestsAsync(clientId, today),
            ViolationsCount = await GetViolationsAsync(clientId, today),
            Tier = await GetClientTierAsync(clientId)
        };
    }

    private async Task TriggerAbuseAlertAsync(string clientId, int violationCount)
    {
        _logger.LogCritical(
            "ABUSE DETECTED: ClientId {ClientId} has {ViolationCount} rate limit violations today",
            clientId, violationCount);
        
        // Send alert to ops team
        // Potentially auto-ban or throttle further
    }
}
```

---

## 🎯 MULTI-LEVEL RATE LIMITING

```csharp
public class MultiLevelRateLimiter
{
    // Level 1: Per-endpoint (most restrictive)
    // Level 2: Per-user (account-wide)
    // Level 3: Global (system-wide protection)
    
    public async Task<RateLimitResult> CheckAsync(RateLimitContext context)
    {
        // Level 1: Endpoint-specific limit
        var endpointResult = await _endpointLimiter.CheckAsync(
            context.ClientId, 
            context.Endpoint,
            context.EndpointTier);
        
        if (!endpointResult.IsAllowed)
            return endpointResult.WithReason("Endpoint rate limit exceeded");

        // Level 2: User account limit
        var userResult = await _userLimiter.CheckAsync(
            context.ClientId,
            context.UserTier);
        
        if (!userResult.IsAllowed)
            return userResult.WithReason("Account rate limit exceeded");

        // Level 3: Global system limit
        var globalResult = await _globalLimiter.CheckAsync();
        
        if (!globalResult.IsAllowed)
            return globalResult.WithReason("System overloaded, try again later");

        return RateLimitResult.Allowed();
    }
}

// Example configuration
{
  "RateLimits": {
    "Endpoints": {
      "/api/expensive-operation": {
        "Free": { "RequestsPerHour": 10 },
        "Premium": { "RequestsPerHour": 100 }
      },
      "/api/reports": {
        "Free": { "RequestsPerHour": 5 },
        "Premium": { "RequestsPerHour": 50 }
      }
    },
    "User": {
      "Free": { "RequestsPerHour": 100 },
      "Premium": { "RequestsPerHour": 10000 }
    },
    "Global": {
      "RequestsPerSecond": 10000
    }
  }
}
```

---

## 💰 COST OPTIMIZATION

**Problem:** Redis memory expensive!

**Çözüm: Hybrid Approach**
```csharp
public class HybridRateLimiter
{
    private readonly IMemoryCache _localCache; // L1 cache
    private readonly IRedisRateLimiter _redisLimiter; // L2 cache

    public async Task<RateLimitResult> CheckAsync(string clientId)
    {
        // L1: Check local memory (fast, but not distributed)
        var localKey = $"local:{clientId}";
        if (_localCache.TryGetValue(localKey, out RateLimitCounter counter))
        {
            if (counter.Count < counter.LocalThreshold)
            {
                counter.Count++;
                return RateLimitResult.Allowed();
            }
        }

        // L2: Check Redis (distributed, accurate)
        var redisResult = await _redisLimiter.CheckAsync(clientId);
        
        // Update local cache
        _localCache.Set(localKey, new RateLimitCounter
        {
            Count = 0,
            LocalThreshold = 10 // Allow 10 requests locally before checking Redis
        }, TimeSpan.FromMinutes(1));

        return redisResult;
    }
}
```

**Sonuç:**
- Redis calls reduced by 90%
- Latency: 1ms (instead of Redis ~1-2ms)
- Cost: Redis memory usage reduced 10x

---

## 🚨 CIRCUIT BREAKER FOR REDIS

```csharp
public class ResilientRateLimiter
{
    private readonly IRedisRateLimiter _redisLimiter;
    private readonly IMemoryCache _fallbackCache;
    private readonly CircuitBreakerPolicy _circuitBreaker;

    public ResilientRateLimiter()
    {
        _circuitBreaker = Policy
            .Handle<RedisException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromMinutes(1));
    }

    public async Task<RateLimitResult> CheckAsync(string clientId)
    {
        try
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
                await _redisLimiter.CheckAsync(clientId));
        }
        catch (BrokenCircuitException)
        {
            // Redis is down, fallback to local memory
            return await _fallbackCache.CheckAsync(clientId);
        }
    }
}
```

---

## 📈 REAL-WORLD EXAMPLES

**Stripe API:**
- Token bucket algorithm
- 100 requests/second per API key
- Burst capacity: 1000 requests

**GitHub API:**
- 5000 requests/hour (authenticated)
- 60 requests/hour (unauthenticated)
- Sliding window

**Twitter API:**
- 15-minute windows
- Different limits per endpoint
- App-level + User-level limits

---

## 🎓 KARİYER ETKİSİ

Bu çözümü implement edersen:
- ✅ **Staff Engineer** level system design
- ✅ Token Bucket algorithm biliyorsun
- ✅ Multi-tier architecture tasarlayabiliyorsun
- ✅ Cost optimization yapabiliyorsun
- ✅ Production incident'lara hazırlıklısın

**Interview'da diyebileceklerin:**
> "Enterprise rate limiting system design ettim. Token bucket kullandım çünkü burst traffic'e izin vermeliyiz ama abuse'i de engellemeliyiz. Multi-level rate limiting var: endpoint, user, global. Redis'i optimize ettik, hybrid approach ile Redis call'ları %90 azalttık. Circuit breaker pattern kullandık, Redis down olsa bile sistem çalışmaya devam ediyor."

---

## 🚀 ÖNERİLER

1. **Başlangıç:** BASIC ile başla
2. **Scaling:** ADVANCED'e geç (Redis)
3. **Enterprise:** Bu çözümü implement et
4. **Alternatif:** Managed service kullan (AWS API Gateway, Azure APIM)

**Son Not:** Rate limiting kompleks bir problem. Perfect solution yok, trade-off'lar var. İhtiyacına göre seç!
