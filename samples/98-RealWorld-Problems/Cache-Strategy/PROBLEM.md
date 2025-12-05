# Problem: Cache Strategy Selection

## 📋 Problem Tanımı

E-commerce sitesinde ürün bilgileri yüklenirken yavaşlık yaşanıyor. Database her istekte sorgulanıyor ve response time 2 saniye. Doğru cache stratejisi seçilmeli.

### Gerçek Dünya Senaryosu

**Şirket**: Amazon-tarzı e-commerce platformu
**Problem**: Ürün detay sayfası 2 saniye yükleniyor
**Metrikler**:
- 10,000 ürün
- 100,000 ziyaretçi/gün
- Her ürün sayfası: 5 database query
- Database CPU: %85
- Response time: 2,000ms

**Beklenen**:
- Response time: <50ms
- Database CPU: <20%
- Cache hit rate: >90%
- 100x performance improvement

## 🎯 Cache Strategy Türleri

### 1. Cache-Aside (Lazy Loading)

```
Read Flow:
1. Check cache
2. If miss → Read from DB → Write to cache
3. Return data

Write Flow:
1. Write to DB
2. Invalidate/Update cache
```

**Avantajlar**:
- ✅ Basit implementasyon
- ✅ Cache sadece gereken data için dolar
- ✅ Cache failure durumunda app çalışmaya devam eder

**Dezavantajlar**:
- ❌ İlk request yavaş (cache miss)
- ❌ Stale data riski
- ❌ Cache stampede (thundering herd)

### 2. Read-Through Cache

```
Read Flow:
1. Check cache
2. If miss → Cache automatically loads from DB
3. Return data

Cache is responsible for DB access!
```

**Avantajlar**:
- ✅ Uygulama DB'den habersiz
- ✅ Automatic cache population
- ✅ Consistent logic

**Dezavantajlar**:
- ❌ İlk request yavaş
- ❌ Cache layer complexity

### 3. Write-Through Cache

```
Write Flow:
1. Write to cache
2. Cache writes to DB synchronously
3. Return success

Data always in sync!
```

**Avantajlar**:
- ✅ Cache always up-to-date
- ✅ No stale data
- ✅ Data consistency guaranteed

**Dezavantajlar**:
- ❌ Yavaş yazma (sync write)
- ❌ Gereksiz cache writes
- ❌ Higher latency

### 4. Write-Behind (Write-Back) Cache

```
Write Flow:
1. Write to cache (fast!)
2. Return success immediately
3. Async write to DB later

Eventual consistency!
```

**Avantajlar**:
- ✅ Çok hızlı yazma
- ✅ Batch DB writes
- ✅ Reduced DB load

**Dezavantajlar**:
- ❌ Data loss riski (cache crash)
- ❌ Eventual consistency
- ❌ Complex implementation

### 5. Refresh-Ahead

```
Cache proactively refreshes before expiration!

1. Data expires in 10min
2. At 9min → Refresh in background
3. Users never see cache miss
```

**Avantajlar**:
- ✅ No cache miss for users
- ✅ Always fresh data
- ✅ Predictable performance

**Dezavantajlar**:
- ❌ Wasted refreshes
- ❌ Complex logic
- ❌ Higher DB load

## 📊 Performans Karşılaştırması

### Scenario: Ürün Bilgisi Okuma

| Strategy | First Request | Cached Request | Write Latency | Consistency |
|----------|--------------|----------------|---------------|-------------|
| No Cache | 200ms | 200ms | 50ms | Strong |
| Cache-Aside | 200ms | 2ms | 50ms + invalidate | Eventual |
| Read-Through | 200ms | 2ms | 50ms + invalidate | Eventual |
| Write-Through | 2ms | 2ms | 250ms | Strong |
| Write-Behind | 2ms | 2ms | 2ms | Eventual |
| Refresh-Ahead | 2ms | 2ms | 50ms + invalidate | Eventual |

## 🧪 Test Senaryoları

### Senaryo 1: E-commerce Ürün Kataloğu

**Gereksinimler**:
- Read-heavy (95% read, 5% write)
- 10,000 ürün
- Fiyat değişimi günde 2-3 kez
- Stale data acceptable (5 dakika)

**En İyi Strateji**: Cache-Aside + TTL
```csharp
var product = await _cache.GetOrCreateAsync($"product:{id}", async entry =>
{
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
    return await _db.Products.FindAsync(id);
});
```

### Senaryo 2: Stok Yönetimi

**Gereksinimler**:
- Write-heavy (40% read, 60% write)
- Real-time stok güncelleme
- Strong consistency gerekli
- Stale data unacceptable

**En İyi Strateji**: Write-Through
```csharp
// Always write to cache + DB together
await _cache.SetAsync($"stock:{id}", stock);
await _db.UpdateStockAsync(id, stock);
```

### Senaryo 3: Yorum Sistemi

**Gereksinimler**:
- Write-heavy
- Eventual consistency OK
- Yüksek throughput gerekli
- Performance critical

**En İyi Strateji**: Write-Behind
```csharp
// Write to cache immediately, queue DB write
await _cache.SetAsync($"comment:{id}", comment);
await _queue.EnqueueAsync(new WriteCommentJob { Comment = comment });
```

### Senaryo 4: Popüler Ürünler (Hot Data)

**Gereksinimler**:
- Very high traffic
- %99.9 uptime
- No cache miss acceptable
- Data changes hourly

**En İyi Strateji**: Refresh-Ahead
```csharp
// Refresh 1 minute before expiration
var product = await _cache.GetOrCreateAsync($"product:{id}", async entry =>
{
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);
    entry.RegisterPostEvictionCallback(async (key, value, reason, state) =>
    {
        // Refresh in background
        await RefreshCacheAsync(key);
    });
    return await _db.Products.FindAsync(id);
});
```

## ⚠️ Common Problems

### Problem 1: Cache Stampede (Thundering Herd)

**Senaryo**: Popüler ürün cache'ten expire oluyor. Aynı anda 10,000 istek geliyor. Hepsi DB'ye gidiyor!

```
Cache expires at 10:00:00
10:00:00.001 → 10,000 requests → All miss cache → 10,000 DB queries!
Database crashes!
```

**Çözüm**: Locking
```csharp
private readonly SemaphoreSlim _lock = new(1, 1);

public async Task<Product> GetProductAsync(int id)
{
    var cached = await _cache.GetAsync($"product:{id}");
    if (cached != null) return cached;

    // Only first request loads from DB
    await _lock.WaitAsync();
    try
    {
        // Double-check after acquiring lock
        cached = await _cache.GetAsync($"product:{id}");
        if (cached != null) return cached;

        var product = await _db.Products.FindAsync(id);
        await _cache.SetAsync($"product:{id}", product, TimeSpan.FromMinutes(5));
        return product;
    }
    finally
    {
        _lock.Release();
    }
}
```

### Problem 2: Cache Penetration

**Senaryo**: Olmayan ürün sürekli sorgulanıyor. Cache'te yok, DB'de de yok. Her request DB'ye gidiyor!

```csharp
// ❌ Bad
var product = await _cache.GetOrCreateAsync($"product:{id}", async entry =>
{
    return await _db.Products.FindAsync(id); // Returns null for non-existent
});
// Next request → Cache miss again → DB query again!
```

**Çözüm**: Cache null values
```csharp
// ✅ Good - Cache null results
var product = await _cache.GetOrCreateAsync($"product:{id}", async entry =>
{
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
    var p = await _db.Products.FindAsync(id);
    return p ?? Product.NotFound; // Cache "not found" state
});
```

### Problem 3: Cache Avalanche

**Senaryo**: Tüm cache entries aynı anda expire oluyor. Tüm requests DB'ye gidiyor!

```csharp
// ❌ Bad - All expire at same time
foreach (var product in products)
{
    await _cache.SetAsync($"product:{product.Id}", product, TimeSpan.FromHours(1));
}
// After 1 hour → All 10,000 products expire at once!
```

**Çözüm**: Add randomness to TTL
```csharp
// ✅ Good - Stagger expiration
var baseExpiration = TimeSpan.FromHours(1);
var jitter = TimeSpan.FromMinutes(Random.Shared.Next(0, 30));
await _cache.SetAsync($"product:{id}", product, baseExpiration + jitter);
```

## 🎓 Öğrenme Hedefleri

Bu problemi çözerek öğreneceksiniz:
- Cache-Aside, Read-Through, Write-Through, Write-Behind, Refresh-Ahead
- Cache stampede, penetration, avalanche problemleri
- Distributed caching (Redis)
- Cache eviction policies (LRU, LFU, TTL)
- Cache warming strategies
- Multi-level caching (L1 + L2)
- Cache monitoring ve metrics

## 📚 Referanslar

- [AWS Caching Best Practices](https://aws.amazon.com/caching/best-practices/)
- [Redis Documentation](https://redis.io/docs/)
- [Cache-Aside Pattern - Microsoft](https://docs.microsoft.com/azure/architecture/patterns/cache-aside)
- [System Design Interview - Alex Xu](https://www.amazon.com/System-Design-Interview-insiders-Second/dp/B08CMF2CQF)
