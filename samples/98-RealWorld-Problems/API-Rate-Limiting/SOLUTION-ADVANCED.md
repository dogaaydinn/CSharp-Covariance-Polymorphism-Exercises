# Advanced Solution: Sliding Window Rate Limiter

## 🎯 Yaklaşım

**Sliding Window** algoritması, Fixed Window'un burst problemini çözer. Sabit pencereler yerine **kayan bir pencere** kullanarak daha hassas rate limiting sağlar.

## 🔧 Nasıl Çalışır?

```
Fixed Window (❌ Burst Problem):
├─────── 1 min ───────┤├─────── 1 min ───────┤
10 requests (00:59)      10 requests (01:00)
= 20 requests in 2 seconds!

Sliding Window (✅ Çözülmüş):
        ├─────── 1 min ───────┤
00:30   00:45   01:00   01:15   01:30
  3       3       2       2       0
Current window = Last 60 seconds from now
```

### Algoritma Adımları

1. **Timestamp Kaydet**: Her isteğin zamanını logla
2. **Window Hesapla**: Son 60 saniyedeki istekleri say
3. **Eski İstekleri Temizle**: Window dışındakileri sil
4. **Limit Kontrolü**: Count < Limit?
5. **Güncelle**: Yeni isteği ekle

## 💾 Veri Yapısı

```csharp
Dictionary<string, List<DateTime>> requests
```

**Key**: `userId`
**Value**: `[timestamp1, timestamp2, timestamp3, ...]`

### Örnek Veriler

```
"user123" → [
    2024-01-01 10:15:23,
    2024-01-01 10:15:45,
    2024-01-01 10:16:12,
    2024-01-01 10:16:18
]
```

## 📊 Sliding Window İllüstrasyonu

```
Current Time: 10:16:30

Window: [10:15:30 ─────────────────→ 10:16:30]

Requests:
10:15:20 ❌ (outside window)
10:15:35 ✅ (inside window)
10:15:50 ✅ (inside window)
10:16:10 ✅ (inside window)
10:16:25 ✅ (inside window)

Count in window: 4
Limit: 10
Result: Allow ✅
```

## ✅ Avantajlar

1. **Burst Problemini Çözer**: Pencere sınırında 2x limit olmaz
2. **Hassas**: Her istek tam zamanıyla kaydedilir
3. **Fair**: Tüm kullanıcılar eşit muamele görür
4. **Flexible**: Window boyutu değiştirilebilir

## ❌ Dezavantajlar

1. **Memory Overhead**: Her istek için timestamp saklanır
   ```
   Fixed Window: O(1) per user
   Sliding Window: O(limit) per user
   ```

2. **CPU Overhead**: Her istekte window temizliği
3. **Complexity**: Implement etmesi daha zor

## 📈 Memory Karşılaştırması

| Users | Requests/min | Fixed Window | Sliding Window |
|-------|--------------|--------------|----------------|
| 1,000 | 10 | 8 KB | 80 KB |
| 10,000 | 10 | 80 KB | 800 KB |
| 100,000 | 100 | 800 KB | 80 MB |
| 1,000,000 | 100 | 8 MB | 800 MB |

## 🎯 Kullanım Senaryoları

### İdeal İçin
- ✅ Production APIs
- ✅ Public APIs (GitHub, Stripe gibi)
- ✅ Payment sistemleri
- ✅ High-traffic uygulamalar
- ✅ Strict SLA gereken yerler

### İdeal Değil
- ❌ Çok yüksek limit (>1000 req/min)
- ❌ Memory-constrained sistemler
- ❌ Basit internal servisler

## 🔧 Implementasyon Detayları

### 1. Request Timestamp Kaydetme

```csharp
public bool AllowRequest(string userId)
{
    var now = DateTime.UtcNow;
    var windowStart = now.AddSeconds(-_windowSeconds);

    _requests.AddOrUpdate(
        userId,
        new List<DateTime> { now },
        (key, existing) =>
        {
            // Clean old requests outside window
            existing.RemoveAll(ts => ts < windowStart);

            if (existing.Count < _limit)
            {
                existing.Add(now);
            }

            return existing;
        }
    );

    return _requests[userId].Count <= _limit;
}
```

### 2. Optimized Cleanup

```csharp
// Binary search kullanarak eski istekleri hızlı temizle
private void CleanupOldRequests(List<DateTime> requests, DateTime cutoff)
{
    // Timestamps sorted olduğu için binary search
    int index = requests.BinarySearch(cutoff);
    if (index < 0)
        index = ~index; // First item >= cutoff

    if (index > 0)
        requests.RemoveRange(0, index);
}
```

### 3. Memory Optimization

```csharp
// Ring buffer kullanarak fixed-size list
private class RingBuffer<T>
{
    private readonly T[] _buffer;
    private int _head = 0;

    public RingBuffer(int capacity)
    {
        _buffer = new T[capacity];
    }

    public void Add(T item)
    {
        _buffer[_head] = item;
        _head = (_head + 1) % _buffer.Length;
    }

    public int CountInWindow(Func<T, bool> predicate)
    {
        return _buffer.Count(predicate);
    }
}
```

## 📊 Performance Karşılaştırması

| Operation | Fixed Window | Sliding Window | Optimized Sliding |
|-----------|--------------|----------------|-------------------|
| Allow Request | O(1) | O(n) | O(log n) |
| Memory/User | O(1) | O(limit) | O(limit) |
| Cleanup | O(n) users | O(limit) per req | O(1) amortized |
| Latency | < 5ms | < 20ms | < 10ms |

## 🧪 Test Durumları

### Test 1: Burst Protection

```csharp
[Fact]
public async Task PreventsBurstAtWindowBoundary()
{
    var limiter = new SlidingWindowRateLimiter(10, 60);

    // 59. saniyede 10 request
    for (int i = 0; i < 10; i++)
        Assert.True(limiter.AllowRequest("user"));

    await Task.Delay(1000); // 60. saniye

    // 60. saniyede hala limitli (9 eski + 1 yeni = 10)
    Assert.False(limiter.AllowRequest("user"));
}
```

### Test 2: Gradual Window Slide

```csharp
[Fact]
public async Task AllowsRequestsAsWindowSlides()
{
    var limiter = new SlidingWindowRateLimiter(10, 60);

    // Fill window
    for (int i = 0; i < 10; i++)
        limiter.AllowRequest("user");

    // Wait for half window
    await Task.Delay(30000);

    // Some old requests dropped, new ones allowed
    int allowed = 0;
    for (int i = 0; i < 10; i++)
        if (limiter.AllowRequest("user")) allowed++;

    Assert.InRange(allowed, 3, 7); // ~5 should be allowed
}
```

## 🚀 Hybrid Approach: Weighted Sliding Window

Daha da optimize edilmiş bir yaklaşım:

```csharp
public class WeightedSlidingWindow
{
    // Previous window + Current window
    private ConcurrentDictionary<string, (int prevCount, int currCount, DateTime windowStart)> _cache;

    public bool AllowRequest(string userId)
    {
        var now = DateTime.UtcNow;
        var windowStart = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        var elapsed = (now - windowStart).TotalSeconds;
        var windowSeconds = 60.0;

        var data = _cache.AddOrUpdate(userId,
            k => (prevCount: 0, currCount: 1, windowStart),
            (k, existing) =>
            {
                if (windowStart > existing.windowStart)
                {
                    // New window
                    return (prevCount: existing.currCount, currCount: 1, windowStart);
                }
                else
                {
                    // Same window
                    return (existing.prevCount, currCount: existing.currCount + 1, existing.windowStart);
                }
            }
        );

        // Weighted calculation
        var weight = 1.0 - (elapsed / windowSeconds);
        var estimated = data.prevCount * weight + data.currCount;

        return estimated <= _limit;
    }
}
```

## 🎓 Ne Zaman Kullanmalı?

**Kullan**:
- ✅ Production APIs
- ✅ Strict rate limiting
- ✅ Fair distribution
- ✅ Public APIs

**Kullanma**:
- ❌ Çok yüksek limit (>1000)
- ❌ Memory kısıtlı sistemler
- ❌ Ultra-low latency (<1ms)

## 📝 Özet

Sliding Window, Fixed Window'a göre **daha hassas** ve **burst-safe** bir çözümdür. Production-grade API'ler için ideal, ancak memory ve CPU overhead'i vardır.

**Trade-off**: Accuracy vs Efficiency

**Sonraki Adım**: `SOLUTION-ENTERPRISE.md` - Token Bucket + Redis ile distributed rate limiting
