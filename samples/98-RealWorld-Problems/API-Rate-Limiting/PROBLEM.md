# Problem: API Rate Limiting

## 📋 Problem Tanımı

Bir public API geliştirdiniz ve bu API'ye aşırı sayıda istek gelmeye başladı. Bazı kullanıcılar API'nizi abuse ediyor ve diğer kullanıcıların hizmet almasını engelliyor.

### Gerçek Dünya Senaryosu

**Şirket**: Video streaming platformu
**Problem**: Free tier kullanıcıları dakikada 1000+ istek gönderiyor
**Etki**:
- Sunucular çöküyor
- Paying customers hizmet alamıyor
- AWS maliyetleri artıyor
- SLA ihlali riski

### Gereksinimler

1. **Rate Limiting**: Kullanıcı başına maksimum istek sayısı
2. **Time Window**: Belirli bir zaman dilimi içinde limit
3. **User Identification**: IP veya API key bazlı
4. **Response**: HTTP 429 (Too Many Requests)
5. **Headers**: Kalan istek sayısı bilgisi

### Örnek Limitler

| Tier | Limit | Window |
|------|-------|--------|
| Free | 10 requests | 1 minute |
| Basic | 100 requests | 1 minute |
| Premium | 1000 requests | 1 minute |
| Enterprise | Unlimited | - |

## 🎯 Beklenen Davranış

### Başarılı İstek
```http
GET /api/videos/123 HTTP/1.1
Authorization: Bearer abc123

HTTP/1.1 200 OK
X-RateLimit-Limit: 10
X-RateLimit-Remaining: 7
X-RateLimit-Reset: 1609459200
```

### Limit Aşılmış İstek
```http
GET /api/videos/456 HTTP/1.1
Authorization: Bearer abc123

HTTP/1.1 429 Too Many Requests
X-RateLimit-Limit: 10
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 1609459200
Retry-After: 45

{
  "error": "Rate limit exceeded",
  "message": "You have exceeded your rate limit. Please try again in 45 seconds."
}
```

## ⚠️ Yaygın Hatalar

1. **Global Counter Kullanmak**
   ```csharp
   // ❌ YANLIŞ - Tüm kullanıcılar için tek counter
   static int _requestCount = 0;
   ```

2. **Thread-Safety Olmayan Kod**
   ```csharp
   // ❌ YANLIŞ - Race condition riski
   if (_counts[userId] < _limit)
   {
       _counts[userId]++;
       return true;
   }
   ```

3. **Memory Leak**
   ```csharp
   // ❌ YANLIŞ - Dictionary sürekli büyüyor
   _counts[userId] = count;  // Hiç temizlenmiyor
   ```

## 📊 Ölçütler

| Metrik | Hedef |
|--------|-------|
| Latency | < 10ms overhead |
| Memory | < 100MB for 10K users |
| Accuracy | > 99% |
| Scalability | 1M requests/sec |

## 🔍 Test Senaryoları

### Senaryo 1: Normal Kullanım
```
User A → 5 requests in 60 sec → All succeed
User B → 3 requests in 60 sec → All succeed
```

### Senaryo 2: Limit Aşımı
```
User A → 10 requests in 10 sec → First 10 succeed, rest fail
User A → Wait 60 sec → New window, requests succeed again
```

### Senaryo 3: Burst Traffic
```
User A → 100 requests in 1 sec → First 10 succeed, 90 fail
```

### Senaryo 4: Distributed System
```
Server 1 → User A sends 5 requests
Server 2 → User A sends 6 requests
Total → Only 10 should succeed (shared counter)
```

## 🎓 Öğrenme Hedefleri

Bu problemi çözerek öğreneceksiniz:
- Rate limiting algoritmaları (Fixed Window, Sliding Window, Token Bucket)
- Thread-safe collections (ConcurrentDictionary)
- Memory management ve cleanup stratejileri
- Distributed caching (Redis)
- ASP.NET Core middleware yazımı
- Performance optimization

## 📚 Referanslar

- [RFC 6585 - HTTP Status Code 429](https://tools.ietf.org/html/rfc6585)
- [Stripe Rate Limiting](https://stripe.com/blog/rate-limiters)
- [GitHub Rate Limiting](https://docs.github.com/en/rest/overview/resources-in-the-rest-api#rate-limiting)
- [Token Bucket Algorithm](https://en.wikipedia.org/wiki/Token_bucket)
