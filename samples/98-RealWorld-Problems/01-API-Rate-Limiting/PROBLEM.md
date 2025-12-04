# GERÇEK DÜNYA PROBLEMİ: API Rate Limiting

## 🚨 PROBLEM SENARYOSU

**Şirket Durumu:**
- Mid-size SaaS şirketi (150 çalışan)
- Tech Stack: ASP.NET Core 8.0, PostgreSQL, Redis, Azure Cloud
- API Gateway kullanıyoruz (Azure APIM)
- 3 backend developer, 2 DevOps engineer
- Günlük 5M API request

**Olay:**
Pazartesi sabahı 09:00'da alarm sistemi patladı. Production API'miz yanıt vermiyor, database connection pool dolmuş, Redis'te spike var. Incident timeline:

- **09:00** - Alert: API response time 500ms → 15 saniye
- **09:05** - Database CPU %95'e çıktı
- **09:10** - Müşterilerden şikayet yağmaya başladı
- **09:15** - Root cause bulundu: Bir müşteri yanlışlıkla sonsuz loop'a girmiş ve bizim API'mizi saniyede 10,000+ kere çağırıyor
- **09:20** - Manuel olarak o müşterinin API key'ini disable ettik
- **09:25** - Sistem yavaş yavaş düzeliyor
- **09:45** - Sistem normale döndü

**Postmortem:** Rate limiting yok! Herhangi bir müşteri (hatta saldırgan) sistemimizi çökertebilir.

---

## 📊 TEKNİK DETAYLAR

### Mevcut Sistem Mimarisi

\`\`\`
[Client Apps]
    ↓
[Azure APIM Gateway]
    ↓
[ASP.NET Core Web API - 5 instances]
    ↓
[PostgreSQL Database - Primary + 2 Replicas]
[Redis Cache - 1 Master + 2 Slaves]
\`\`\`

### Trafik Profili

**Normal Günlük Trafik:**
- Total requests: 5,000,000 requests/day
- Peak hours (09:00-17:00): 400,000 requests/hour
- Ortalama: 140 requests/second
- Top 10% müşteri: Trafiğin %60'ını üretir
- Free tier kullanıcılar: %70 kullanıcı, %10 trafik

**API Endpoint Breakdown:**
- \`GET /api/users\`: %30 (read-heavy)
- \`POST /api/data\`: %25 (write-heavy)
- \`GET /api/reports\`: %20 (CPU-intensive)
- \`PUT /api/updates\`: %15 (write-heavy)
- Diğer: %10

### Mevcut Çözüm (YOK!)

\`\`\`csharp
// Şu anki API Controller - Rate limiting YOK!
[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetData(int userId)
    {
        // ❌ Herhangi bir rate limit kontrolü YOK
        // ❌ Abuse detection YOK
        // ❌ Throttling YOK

        var data = await _repository.GetDataAsync(userId);
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> CreateData([FromBody] CreateDataRequest request)
    {
        // ❌ Cost'lu işlem ama rate limit YOK
        var result = await _service.ProcessDataAsync(request);
        return Ok(result);
    }
}
\`\`\`

**Problem:**
- Hiçbir endpoint rate limiting'e tabi değil
- Tek bir kullanıcı tüm sistemi çökertebilir
- Free tier ve Premium tier arasında fark yok
- Abuse detection mekanizması yok
- DDoS saldırısına karşı savunmasızız

---

## 💥 KULLANICI ETKİSİ

### Pain Points

#### 1. Business Impact
- **Revenue Loss:** Incident sırasında 45 dakika downtime → ~$15,000 revenue loss
- **Churn Risk:** 12 enterprise müşteri complaint ticket açtı
- **SLA Violation:** %99.9 SLA'mız var, bu incident bizi %99.7'ye düşürdü
- **Brand Damage:** HackerNews'te thread açıldı, Twitter'da viral oldu

#### 2. Kullanıcı Şikayetleri

**Enterprise Müşteri (Acme Corp):**
> "Sistemimiz sizin API'nize entegre. 45 dakika boyunca hiçbir işlem yapamadık. Üretim hattımız durdu. Binlerce dolar zarar ettik. SLA compensation talep ediyoruz."

**Free Tier Kullanıcı:**
> "API hiç çalışmıyor. Sabahtan beri 500 error alıyorum. Başka alternatif aramaya başladım."

#### 3. Teknik Debt

**Şu anki durumumuz:**
- ❌ Rate limiting yok
- ❌ Throttling yok
- ❌ Quota management yok
- ❌ Abuse detection yok
- ❌ Tier-based limits yok (Free vs Premium vs Enterprise)
- ❌ Burst capacity handling yok
- ❌ Client-side feedback yok (429 Too Many Requests dönmüyoruz)

**Ekip Durumu:**
- Backend team on-call rotation'da sürekli alarm
- DevOps ekip manuel müdahale ile uğraşıyor (API key disable etmek gibi)
- Product team yeni özellik ekleyemiyor (stability öncelikli)

---

## 🎯 PROBLEM STATEMENT

**Ana Soru:**
> "Nasıl bir rate limiting sistemi tasarlayabiliriz ki:
> - Legitimate kullanıcıları etkilemesin
> - Abuse'i otomatik tespit edip engellesin
> - Farklı tier'lar için farklı limitler koysun (Free, Premium, Enterprise)
> - Distributed sistemde tutarlı çalışsın (5 API instance var)
> - Performance overhead minimal olsun
> - Kullanıcıya anlamlı feedback versin (X request kaldı, Y saniye sonra reset)
> - Elastik olsun (Black Friday gibi peak günlerde scale edebilsin)"

---

## 📋 GEREKSINIMLER

### Functional Requirements

1. **Tier-Based Limits**
   - Free Tier: 100 request/hour
   - Premium Tier: 10,000 request/hour
   - Enterprise Tier: 100,000 request/hour

2. **Granularity**
   - Per-user limiting (API key bazında)
   - Per-endpoint limiting (bazı endpoint'ler daha restrictive)
   - Global limiting (tüm sistem için upper bound)

3. **Response Headers**
   \`\`\`
   X-RateLimit-Limit: 100
   X-RateLimit-Remaining: 87
   X-RateLimit-Reset: 1640000000
   Retry-After: 3600
   \`\`\`

4. **Status Codes**
   - 429 Too Many Requests (rate limit aşıldığında)
   - 503 Service Unavailable (global limit aşıldığında)

### Non-Functional Requirements

1. **Performance**
   - Rate limit check <5ms olmalı
   - Database'e her request'te gitmemeli
   - Memory footprint minimal

2. **Scalability**
   - 5 API instance'ı arasında synchronized
   - Horizontal scaling desteklemeli
   - 10M request/day handle edebilmeli

3. **Reliability**
   - Redis down olursa fallback mekanizması
   - Fail-open strategy (rate limiter çökerse API çalışmaya devam etmeli)

4. **Observability**
   - Rate limit violations loglanmalı
   - Metrics: requests blocked, tier distribution, top abusers
   - Alerting: Abuse pattern detection

---

## 🤔 MEVCUT SENARYOLAR

### Senaryo 1: Normal Kullanım
\`\`\`
Premium kullanıcı (limit: 10,000/hour)
- 09:00: 500 request ✅ OK
- 10:00: 800 request ✅ OK
- 11:00: 1,200 request ✅ OK
Toplam: 2,500 < 10,000 ✅
\`\`\`

### Senaryo 2: Burst Traffic
\`\`\`
Enterprise müşteri (limit: 100,000/hour)
- Black Friday sabahı
- İlk 5 dakikada 20,000 request
- Bu legitimate mi yoksa abuse mi?
\`\`\`
**Soru:** Burst'e izin vermeli miyiz? Sliding window mu fixed window mu?

### Senaryo 3: Distributed Counting
\`\`\`
5 API instance var
User X → Instance 1: 30 request
User X → Instance 2: 40 request
User X → Instance 3: 35 request
Toplam: 105 request
Limit: 100 request/hour
\`\`\`
**Soru:** Instance'lar arasında nasıl senkronize ederiz?

### Senaryo 4: Rate Limiter Down
\`\`\`
Redis cluster çöktü
Rate limiter çalışmıyor
Ne yapmalıyız?
A) API'yi kapat (fail-closed)
B) Rate limiting'i skip et (fail-open)
\`\`\`

---

## 💡 ÇÖZÜM ALTERNATİFLERİ (Teaser)

Bu problem için 3 farklı çözüm sunuyoruz:

1. **BASIC SOLUTION:** In-memory rate limiting (tek instance için)
2. **ADVANCED SOLUTION:** Redis-based distributed rate limiting
3. **ENTERPRISE SOLUTION:** Redis + Token Bucket + Sliding Window + Multi-tier + Analytics

Her çözümü ayrı dosyalarda detaylı açıklayacağız:
- \`SOLUTION-BASIC.md\`
- \`SOLUTION-ADVANCED.md\`
- \`SOLUTION-ENTERPRISE.md\`

Sonra da tüm çözümleri karşılaştırıp karar matrisi sunacağız:
- \`COMPARISON.md\`
- \`DECISION-GUIDE.md\`

---

## 🔗 İLGİLİ PATTERN'LER

Bu problemi çözerken kullanacağımız pattern'ler:

- **Throttling Pattern** (\`samples/03-Advanced/ThrottlingPattern/\`)
- **Circuit Breaker Pattern** (\`samples/03-Advanced/ResiliencePatterns/\`)
- **Middleware Pattern** (ASP.NET Core)
- **Distributed Caching** (\`samples/02-Intermediate/CachingStrategies/\`)
- **Token Bucket Algorithm**
- **Sliding Window Algorithm**

---

## 📚 GERÇEK DÜNYA ÖRNEKLERİ

**Bu problemi kim yaşadı?**

1. **GitHub (2018):** API rate limiting olmadan DDoS saldırısına uğradı
2. **Twitter API:** En sofistike rate limiting sistemlerinden biri (15-minute windows)
3. **Stripe API:** Kullanıcı başına 100 req/sec limit, rolling window kullanıyor
4. **AWS API Gateway:** Built-in throttling, burst capacity 5000, steady 10000 req/sec

**Öğrenilecek dersler:**
- Rate limiting olmadan production'a çıkma
- Tier-based limits şart
- Distributed rate limiting Redis ile çözülür
- Fail-open strategy daha güvenli (fail-closed tüm API'yi öldürür)

---

## 🎓 KARİYER ETKİSİ

**Bu problemi çözebilirsen:**

✅ Senior Developer level soru
✅ Distributed systems anlayışı gösterirsin
✅ Production incident experience
✅ Trade-off analysis yapabiliyorsun (performance vs accuracy)
✅ Enterprise-grade solution design

**Interview'da sorulacak follow-up sorular:**
- "Rate limiter Redis'e bağımlı, Redis çökerse ne olur?"
- "Sliding window vs fixed window, hangisini seçerdin?"
- "Token bucket vs leaky bucket?"
- "Global rate limiting mi yoksa per-user mı?"
- "Rate limiting distributed tracing ile nasıl entegre edersin?"

---

## 🚀 SONRAKI ADIM

Şimdi çözümlere bakalım:

1. **SOLUTION-BASIC.md** okuyarak basit bir çözümle başla
2. **SOLUTION-ADVANCED.md** ile production-ready çözümü incele
3. **SOLUTION-ENTERPRISE.md** ile Silicon Valley şirketlerinin nasıl yaptığını gör
4. **COMPARISON.md** ile hangisini seçeceğine karar ver
5. **DECISION-GUIDE.md** ile kendi use case'ine göre seç

**Tavsiye:** Önce BASIC'i oku ve uygula. Sonra ADVANCED'e geç. ENTERPRISE çok complex, önce diğerlerini anla.

---

**Son Not:** Bu problem gerçek. Bu incident yaşandı. Rate limiting hayat kurtarır. Production'a rate limiting olmadan çıkma.
