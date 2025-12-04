# NEDEN RESILIENCE PATTERNS (POLLY) KULLANIYORUZ?

## 🎯 PROBLEM TANIMI

**Gerçek Dünya Senaryosu:**

Black Friday. Sitenizde saniyede 10,000 request var. Ödeme servisi (external payment gateway) anlık yük altında %1 request'lerde "timeout" veriyor.

Junior developer'ın kodu:

```csharp
// ❌ BAD: Hiç retry yok
public async Task<PaymentResult> ProcessPaymentAsync(Order order)
{
    var response = await _httpClient.PostAsync("https://payment-api.com/charge", ...);
    return await response.Content.ReadAsAsync<PaymentResult>();
}
```

**Sonuç:**
- 10,000 req/sec × 1% fail rate = **100 failed payment/second**
- Her failed payment → User frustrasyonu → Abandoned cart
- **$500,000 revenue loss** in 1 hour

**Ama şu gerçek:**
- Payment API geçici yük altında (transient failure)
- **1 saniye sonra retry etseydin → Başarılı olacaktı!**
- Ama retry yok → **100 failed payment that could have succeeded**

---

**Teknik Problem:**

**Problem 1: Transient Failures → Permanent Failures**

```csharp
// ❌ BAD: Transient failure'ı handle etmiyor
public async Task<string> GetDataAsync()
{
    try
    {
        return await _httpClient.GetStringAsync("https://api.example.com/data");
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "API call failed");
        throw; // ❌ Hiç retry etmeden exception fırlat
    }
}

// Kullanım:
var data = await GetDataAsync(); // ❌ 1 timeout = FAIL
// Ama 1 saniye sonra API'ye tekrar isteseydik → SUCCESS olabilirdi!
```

**Neden kötü?**
- Network glitch (50ms timeout spike) → Permanent failure
- Database deadlock (1 saniye retry'la çözülür) → User error görür
- External API rate limit (wait 1 second) → Request failed

**Problem 2: Cascading Failures**

```csharp
// ❌ BAD: Dependency down → Your service down
public class OrderService
{
    public async Task<Order> GetOrderAsync(int id)
    {
        var order = await _db.Orders.FindAsync(id);

        // ❌ Payment service down ise bu request sonsuz bekler!
        var payment = await _paymentClient.GetPaymentAsync(order.PaymentId);

        order.PaymentDetails = payment;
        return order;
    }
}

// Payment service down (takes 30 seconds to timeout)
// → Her GetOrder request 30 saniye takılır
// → Thread pool dolar
// → Senin service'in de down olur! (Cascading failure)
```

**Problem 3: "Thundering Herd" - Dependency Overload**

```csharp
// ❌ BAD: Database geçici down → 10000 client aynı anda retry → DB never recovers!

// Database 2 saniye unresponsive oldu
// 10000 client aynı anda retry ediyor (no backoff)
// Database recovers → Immediate 10000 requests → DB DOWN again!
// Infinite loop of death! 💀
```

**Problem 4: No Timeout = Infinite Wait**

```csharp
// ❌ BAD: Timeout yok
var response = await _httpClient.GetAsync("https://slow-api.com/data");
// Slow API 5 dakika cevap vermedi → 5 dakika bekliyorsun!
// Thread stuck, resources wasted
```

**Problem 5: Single Point of Failure**

```csharp
// ❌ BAD: Cache down → System down
public async Task<Product> GetProductAsync(int id)
{
    var cached = await _redis.GetAsync<Product>($"product:{id}");
    if (cached != null) return cached;

    // ❌ Redis down → Exception → User error!
    // Cache'in down olması sistem'i down etmemeli!
}
```

---

## 💡 ÇÖZÜM: RESILIENCE PATTERNS (POLLY)

**Pattern'in Özü:**

**Resilience = Ability to recover from failures automatically**

Polly library, 6 temel pattern sağlar:

1. **Retry**: Failed? Try again.
2. **Circuit Breaker**: Too many failures? Stop trying, fail fast.
3. **Timeout**: Taking too long? Give up.
4. **Fallback**: Failed? Use plan B.
5. **Bulkhead**: Isolate resources, prevent total failure.
6. **Cache**: Don't call if you have the result.

**Nasıl çalışır:**

1. Polly policy tanımla (örn: "3 kez retry et, exponential backoff ile")
2. Policy'yi wrap et metod çağrısına
3. Failure olunca Polly otomatik handle eder (retry, circuit break, etc.)

**Ne zaman kullanılır:**

- **External API calls** (network failures, timeouts)
- **Database operations** (deadlocks, transient errors)
- **Distributed systems** (microservices calling each other)
- **Production environments** (failures are not IF but WHEN)

---

## 📝 BU REPO'DAKİ IMPLEMENTASYON

### 1. RETRY PATTERN

```csharp
// samples/03-Advanced/ResiliencePatterns/RetryPattern.cs

// ============================================
// Simple Retry (3 attempts)
// ============================================
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .RetryAsync(3);

// Kullanım:
var result = await retryPolicy.ExecuteAsync(async () =>
{
    return await _httpClient.GetStringAsync("https://api.example.com/data");
});

// Failure: Attempt 1 → FAIL
//          Attempt 2 → FAIL
//          Attempt 3 → SUCCESS ✅
// Result: Success after 3 attempts

// ============================================
// Retry with Exponential Backoff
// ============================================
var retryWithBackoff = Policy
    .Handle<HttpRequestException>()
    .Or<TimeoutException>()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
        onRetry: (exception, timeSpan, retryCount, context) =>
        {
            _logger.LogWarning(
                "Retry {RetryCount} after {Delay}ms due to {Exception}",
                retryCount,
                timeSpan.TotalMilliseconds,
                exception.GetType().Name
            );
        }
    );

// Failure: Attempt 1 → FAIL → Wait 2 seconds
//          Attempt 2 → FAIL → Wait 4 seconds
//          Attempt 3 → FAIL → Wait 8 seconds
//          Attempt 4 → SUCCESS ✅

// ============================================
// Retry with Jitter (prevents thundering herd)
// ============================================
var random = new Random();
var retryWithJitter = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt =>
        {
            var exponentialDelay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
            var jitter = TimeSpan.FromMilliseconds(random.Next(0, 1000));
            return exponentialDelay + jitter; // ✅ Randomize retry timing
        }
    );

// Why jitter? 1000 clients retry aynı anda → Server overload
// Jitter ile her client biraz farklı timing'de retry eder
```

### 2. CIRCUIT BREAKER PATTERN

```csharp
// samples/03-Advanced/ResiliencePatterns/CircuitBreakerPattern.cs

// ============================================
// Circuit Breaker: Fail Fast After Repeated Failures
// ============================================
var circuitBreakerPolicy = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(
        exceptionsAllowedBeforeBreaking: 5, // ✅ 5 consecutive failures
        durationOfBreak: TimeSpan.FromSeconds(30), // ✅ Open for 30 seconds
        onBreak: (exception, duration) =>
        {
            _logger.LogWarning(
                "Circuit breaker opened for {Duration}s due to {Exception}",
                duration.TotalSeconds,
                exception.GetType().Name
            );
        },
        onReset: () =>
        {
            _logger.LogInformation("Circuit breaker reset");
        },
        onHalfOpen: () =>
        {
            _logger.LogInformation("Circuit breaker half-open, testing...");
        }
    );

// ============================================
// Circuit States
// ============================================
// CLOSED (Normal):
//   ├─ Request 1 → SUCCESS ✅
//   ├─ Request 2 → SUCCESS ✅
//   └─ Request 3 → SUCCESS ✅

// OPEN (Tripped):
//   ├─ Request 4 → FAIL ❌
//   ├─ Request 5 → FAIL ❌ (5th failure)
//   └─ [CIRCUIT OPENED]
//       ├─ Request 6 → IMMEDIATE FAIL (no call to API) ⚡
//       ├─ Request 7 → IMMEDIATE FAIL (no call to API) ⚡
//       └─ Wait 30 seconds...

// HALF-OPEN (Testing):
//   ├─ Request 8 → TRY (test request)
//   └─ If SUCCESS → CIRCUIT CLOSED ✅
//       If FAIL → CIRCUIT OPEN again ❌

// ============================================
// Advanced Circuit Breaker
// ============================================
var advancedCircuitBreaker = Policy
    .Handle<HttpRequestException>()
    .AdvancedCircuitBreakerAsync(
        failureThreshold: 0.5, // ✅ 50% failure rate
        samplingDuration: TimeSpan.FromSeconds(10), // ✅ Over 10 second window
        minimumThroughput: 20, // ✅ At least 20 requests
        durationOfBreak: TimeSpan.FromSeconds(30)
    );

// Example:
// 10 seconds içinde:
// 30 requests → 15 success, 15 fail (50% fail rate)
// minimumThroughput: 20 ✅ (30 > 20)
// failureThreshold: 0.5 ✅ (50% >= 50%)
// → CIRCUIT OPENED!
```

### 3. TIMEOUT PATTERN

```csharp
// samples/03-Advanced/ResiliencePatterns/TimeoutPattern.cs

// ============================================
// Timeout: Don't Wait Forever
// ============================================
var timeoutPolicy = Policy
    .TimeoutAsync(
        timeout: TimeSpan.FromSeconds(5),
        onTimeoutAsync: async (context, timespan, task) =>
        {
            _logger.LogWarning("Request timed out after {Timeout}s", timespan.TotalSeconds);
        }
    );

// Kullanım:
try
{
    var result = await timeoutPolicy.ExecuteAsync(async () =>
    {
        return await _httpClient.GetStringAsync("https://slow-api.com/data");
    });
}
catch (TimeoutRejectedException)
{
    _logger.LogError("Request exceeded timeout");
    // Fallback logic
}

// Slow API 10 saniye cevap vermiyor
// → 5 saniye sonra TimeoutRejectedException
// → Thread released, resource free ✅
```

### 4. FALLBACK PATTERN

```csharp
// samples/03-Advanced/ResiliencePatterns/FallbackPattern.cs

// ============================================
// Fallback: Plan B When Primary Fails
// ============================================
var fallbackPolicy = Policy<string>
    .Handle<HttpRequestException>()
    .Or<TimeoutRejectedException>()
    .FallbackAsync(
        fallbackValue: "Default value from cache",
        onFallbackAsync: async (outcome, context) =>
        {
            _logger.LogWarning("Fallback triggered due to {Exception}", outcome.Exception);
        }
    );

// Kullanım:
var data = await fallbackPolicy.ExecuteAsync(async () =>
{
    return await _httpClient.GetStringAsync("https://api.example.com/data");
});
// API down → Returns "Default value from cache" instead of exception ✅

// ============================================
// Fallback with Alternative Source
// ============================================
var fallbackToCache = Policy<Product>
    .Handle<HttpRequestException>()
    .FallbackAsync(
        fallbackAction: async (cancellationToken) =>
        {
            _logger.LogWarning("API failed, falling back to cache");
            return await _cache.GetAsync<Product>("product:123");
        }
    );

// Primary: API call → FAIL
// Fallback: Cache → SUCCESS ✅
// User sees data (stale but better than error!)
```

### 5. BULKHEAD PATTERN

```csharp
// samples/03-Advanced/ResiliencePatterns/BulkheadPattern.cs

// ============================================
// Bulkhead: Isolate Resources
// ============================================
var bulkheadPolicy = Policy
    .BulkheadAsync(
        maxParallelization: 10, // ✅ Max 10 concurrent executions
        maxQueuingActions: 20, // ✅ Max 20 queued actions
        onBulkheadRejectedAsync: async (context) =>
        {
            _logger.LogWarning("Bulkhead rejected: Too many concurrent requests");
        }
    );

// Why? Dependency down → 1000 threads waiting → Your service down!
// Bulkhead: Only 10 threads for this dependency, other threads free ✅

// Example:
// 100 concurrent requests to slow API
// Without bulkhead: 100 threads stuck → Thread pool exhausted → Your service down ❌
// With bulkhead: 10 threads for API, 20 queued, rest rejected immediately → Your service alive ✅
```

### 6. POLICY WRAP (Combine Multiple Patterns)

```csharp
// samples/03-Advanced/ResiliencePatterns/PolicyWrap.cs

// ============================================
// Combine: Retry + Circuit Breaker + Timeout + Fallback
// ============================================

// 1. Timeout policy (innermost)
var timeoutPolicy = Policy
    .TimeoutAsync(TimeSpan.FromSeconds(5));

// 2. Retry policy with exponential backoff
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .Or<TimeoutRejectedException>()
    .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

// 3. Circuit breaker
var circuitBreakerPolicy = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

// 4. Fallback (outermost)
var fallbackPolicy = Policy<string>
    .Handle<Exception>()
    .FallbackAsync("Fallback value");

// ✅ WRAP them together (order matters!)
var policyWrap = Policy.WrapAsync(
    fallbackPolicy,      // Outer
    circuitBreakerPolicy,
    retryPolicy,
    timeoutPolicy        // Inner
);

// Execution flow:
// 1. Timeout: If > 5s → TimeoutRejectedException
// 2. Retry: If timeout → Retry 3 times with backoff
// 3. Circuit breaker: If 5 retries fail → Circuit opens
// 4. Fallback: If all else fails → Return fallback value

// User NEVER sees an error! ✅
```

### 7. POLLY WITH HTTPCLIENTFACTORY

```csharp
// samples/03-Advanced/ResiliencePatterns/HttpClientResilience.cs

// ============================================
// Best Practice: Polly + HttpClientFactory
// ============================================
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpClient("PaymentApi", client =>
    {
        client.BaseAddress = new Uri("https://payment-api.com");
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy())
    .AddPolicyHandler(GetTimeoutPolicy());
}

private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // ✅ 5xx, 408, network failures
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
}

private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}

private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
{
    return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));
}

// Kullanım:
public class PaymentService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public async Task<PaymentResult> ChargeAsync(Order order)
    {
        var client = _httpClientFactory.CreateClient("PaymentApi");
        // ✅ Tüm policies otomatik uygulanır!
        var response = await client.PostAsJsonAsync("/charge", order);
        return await response.Content.ReadAsAsync<PaymentResult>();
    }
}
```

---

## 📚 ADIM ADIM NASIL UYGULANIR

### Adım 1: Polly'yi Kur

```bash
dotnet add package Polly
dotnet add package Microsoft.Extensions.Http.Polly
```

### Adım 2: Basit Retry Ekle

```csharp
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .RetryAsync(3);

var result = await retryPolicy.ExecuteAsync(async () =>
{
    return await _httpClient.GetStringAsync("https://api.example.com");
});
```

### Adım 3: Circuit Breaker Ekle

```csharp
var circuitBreaker = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
```

### Adım 4: HttpClient ile Entegre Et

```csharp
services.AddHttpClient("MyApi")
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreaker);
```

---

## ⚖️ TRADE-OFF ANALİZİ

### ✅ Avantajları

**✅ Self-Healing Systems**
- **Neden avantaj?** Transient failures otomatik recover olur
- **Örnek:** 1% timeout rate → 0% user error (retry sayesinde)
- **Ölçülebilir etki:** MTTR (Mean Time To Recovery) %90 azalır

**✅ Prevents Cascading Failures**
- **Hangi durumda kritik?** Microservice architecture'da 1 service down → Tüm sistem down olabilir
- **Circuit breaker sayesinde:** Dependency down → Your service alive ✅

**✅ Resource Protection**
- **Performance etkisi:** Bulkhead ile thread pool exhaustion engelleriz
- **Örnek:** Slow dependency 1000 thread tüketiyor → Bulkhead ile sadece 10 thread

**✅ Better User Experience**
- **Fallback sayesinde:** API down → Cached data göster (stale but better than error)

---

### ❌ Dezavantajları

**❌ Complexity**
- **Ne zaman problem olur?** 5+ policy wrap → Debug zor
- **Çözüm:** Policy'leri adlandır, log ekle

**❌ Masked Issues**
- **Ne zaman problem olur?** Circuit breaker 10 dakika kapalı → Problem var ama kimse bilmiyor
- **Çözüm:** Metrics + alerts (circuit open → PagerDuty alert)

**❌ Retry Storms**
- **Ne zaman problem olur?** 10000 client aynı anda retry → Dependency never recovers
- **Çözüm:** Exponential backoff + jitter

---

## 🚫 NE ZAMAN KULLANMAMALISIN?

### Senaryo 1: Internal Synchronous Calls

```csharp
// ❌ OVERKILL: Same process, no network
public class OrderService
{
    public Order CalculateTotal(Order order)
    {
        // Retry here? Why? It's synchronous, local call!
        var policy = Policy.Handle<Exception>().Retry(3); // ❌ Gereksiz
        return policy.Execute(() => _calculator.Calculate(order));
    }
}
```

### Senaryo 2: Non-Transient Errors

```csharp
// ❌ WRONG: ArgumentNullException için retry?
// Retry etsek de yine başarısız olur!
var policy = Policy
    .Handle<ArgumentNullException>() // ❌ This will NEVER succeed with retry!
    .Retry(3);
```

### Senaryo 3: User Validation Errors

```csharp
// ❌ WRONG: User 400 Bad Request için retry?
var policy = Policy
    .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.BadRequest)
    .Retry(3); // ❌ User hatayı düzeltmedikçe retry işe yaramaz!
```

---

## 💼 KARİYER ETKİSİ

### Mid-Level Developer (2-5 yıl)
- **Görev:** Polly ile retry/timeout eklemek
- **Mülakat:** "Circuit breaker pattern nedir?"
- **Maaş etkisi:** Resilience bilgisi → $90-130K

### Senior Developer (5+ yıl)
- **Görev:** Company-wide resilience strategy
- **Mülakat:** "Cascading failure nasıl önlenir?"
- **Maaş etkisi:** Distributed systems expertise → $130-190K+

### Principal Engineer (10+ yıl)
- **Görev:** Multi-region failover, chaos engineering
- **Mülakat:** "Netflix Chaos Monkey'yi nasıl implement edersiniz?"
- **Maaş etkisi:** Resilience engineering → $200K-350K+

---

## 📚 SONRAKI ADIMLAR

1. **Polly kurun**: `dotnet add package Polly`
2. **Retry ekleyin**: Start simple
3. **Circuit breaker**: Protect your service
4. **Chaos test**: Inject failures, test resilience

---

**Özet:** Resilience patterns = Production system'in hayat sigortası. Failures happen (not IF but WHEN). Polly ile self-healing systems inşa edersin. Retry, Circuit Breaker, Timeout, Fallback, Bulkhead. Her production system'de **MUST HAVE**. 🚀
