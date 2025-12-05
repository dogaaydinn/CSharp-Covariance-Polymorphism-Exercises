# Basic Solution: Fixed Window Rate Limiter

## 🎯 Yaklaşım

**Fixed Window** algoritması en basit rate limiting yaklaşımıdır. Zamanı sabit pencerелere böler ve her pencerede maksimum istek sayısını kontrol eder.

## 🔧 Nasıl Çalışır?

```
Window 1 (00:00-00:59)    Window 2 (01:00-01:59)    Window 3 (02:00-02:59)
├──────────────────┤      ├──────────────────┤      ├──────────────────┤
10 requests ✅           10 requests ✅           10 requests ✅
```

### Adımlar

1. **Window Key Oluştur**: `userId:timestamp_minute`
2. **Counter Kontrol Et**: Penceredeki istek sayısını oku
3. **Limit Kontrolü**: Counter < Limit?
4. **Counter Artır**: İstek kabul edilirse counter++
5. **Cleanup**: Eski pencereleri temizle

## 💾 Veri Yapısı

```csharp
Dictionary<string, (int count, DateTime windowStart)>
```

**Key**: `userId:windowId`
**Value**: `(count: 7, windowStart: 2024-01-01 10:15:00)`

### Örnek Veriler

```
"user123:202401011015" → (count: 7, windowStart: 10:15:00)
"user456:202401011015" → (count: 3, windowStart: 10:15:00)
"user123:202401011016" → (count: 2, windowStart: 10:16:00)
```

## ✅ Avantajlar

1. **Basit**: Anlaması ve implemente etmesi kolay
2. **Hızlı**: O(1) complexity
3. **Memory Efficient**: Sadece aktif pencereleri tutar
4. **Thread-Safe**: ConcurrentDictionary kullanarak

## ❌ Dezavantajlar

1. **Burst Problem**: Pencere sınırında 2x limit aşılabilir
   ```
   00:00:59 → 10 requests ✅
   00:01:00 → 10 requests ✅
   Total: 20 requests in 2 seconds! (should be 10/minute)
   ```

2. **Unfair**: Pencere başında gelenler avantajlı
3. **Hard Reset**: Pencere bitince limit aniden sıfırlanır

## 📊 Burst Problem Örneği

```
Time:     00:59:58  00:59:59  01:00:00  01:00:01  01:00:02
Window:   Window 1  Window 1  Window 2  Window 2  Window 2
Requests:    5         5         5         5         0
Status:     ✅        ✅        ✅        ✅        ❌ (limit)

Problem: 59. saniyede 10, 60. saniyede 10 = 20 request in 2 seconds!
```

## 🎯 Kullanım Senaryoları

### İdeal İçin
- ✅ Basit API'ler
- ✅ Düşük trafikli uygulamalar
- ✅ Internal servisler
- ✅ Prototype/MVP projeler

### İdeal Değil
- ❌ High-traffic APIs
- ❌ Strict rate limiting gereken sistemler
- ❌ Payment/billing sistemleri
- ❌ Public API'ler

## 🔧 Implementasyon Detayları

### 1. Window Key Oluşturma

```csharp
private string GetWindowKey(string userId)
{
    var now = DateTime.UtcNow;
    var windowId = now.ToString("yyyyMMddHHmm");
    return $"{userId}:{windowId}";
}
```

### 2. Limit Kontrolü

```csharp
public bool AllowRequest(string userId)
{
    var key = GetWindowKey(userId);

    var data = _cache.AddOrUpdate(
        key,
        k => (count: 1, windowStart: DateTime.UtcNow),
        (k, existing) => (count: existing.count + 1, existing.windowStart)
    );

    return data.count <= _limit;
}
```

### 3. Cleanup Stratejisi

```csharp
// Background task ile eski pencereleri temizle
private void CleanupOldWindows()
{
    var cutoff = DateTime.UtcNow.AddMinutes(-2);
    var keysToRemove = _cache
        .Where(kvp => kvp.Value.windowStart < cutoff)
        .Select(kvp => kvp.Key)
        .ToList();

    foreach (var key in keysToRemove)
    {
        _cache.TryRemove(key, out _);
    }
}
```

## 📈 Performance

| Metrik | Değer |
|--------|-------|
| Lookup | O(1) |
| Insert | O(1) |
| Memory | O(n) - n = active windows |
| Latency | < 5ms |

## 🧪 Test Durumları

### Test 1: Normal Kullanım
```csharp
[Fact]
public void AllowsRequestsWithinLimit()
{
    // 10 request limit, 1 minute window
    for (int i = 0; i < 10; i++)
    {
        Assert.True(limiter.AllowRequest("user123"));
    }
    Assert.False(limiter.AllowRequest("user123")); // 11th request fails
}
```

### Test 2: Window Reset
```csharp
[Fact]
public void ResetsAfterWindowExpires()
{
    // Fill up current window
    for (int i = 0; i < 10; i++)
        limiter.AllowRequest("user123");

    Assert.False(limiter.AllowRequest("user123"));

    // Wait for new window
    Thread.Sleep(61000); // 61 seconds

    Assert.True(limiter.AllowRequest("user123")); // New window, allowed
}
```

## 🎓 Ne Zaman Kullanmalı?

**Kullan**:
- MVP/prototype projeler
- Internal APIs
- Düşük kritiklik
- Basitlik öncelikli

**Kullanma**:
- Production APIs
- Billing sistemleri
- High-traffic uygulamalar
- Strict SLA gereken yerler

## 🚀 İyileştirme Önerileri

1. **Redis'e Taşı**: Distributed sistem için
2. **Sliding Window'a Geç**: Burst problemini çöz
3. **Token Bucket Kullan**: Smooth rate limiting
4. **Multiple Tiers**: Farklı kullanıcı seviyeleri

## 📝 Özet

Fixed Window algoritması **basit** ama **sınırlı** bir çözümdür. Prototip ve düşük trafikli sistemler için yeterlidir, ancak production-grade sistemler için **Sliding Window** veya **Token Bucket** algoritmaları tercih edilmelidir.

**Sonraki Adım**: `SOLUTION-ADVANCED.md` - Sliding Window implementasyonu
