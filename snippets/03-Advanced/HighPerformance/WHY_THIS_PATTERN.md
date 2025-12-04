# NEDEN HIGH-PERFORMANCE PATTERNS KULLANIYORUZ?

## 🎯 PROBLEM TANIMI

### Gerçek Dünya Senaryosu

Bir fintech startup'ındasınız ve real-time cryptocurrency trading platformu geliştiriyorsunuz. Platform saniyede 100,000+ fiyat güncellesi alıyor ve bunları anlık olarak işleyip kullanıcılara göstermeniz gerekiyor.

İlk implementasyonunuz şöyle:

```csharp
// Trading bot'unuz her saniye binlerce fiyat alıyor
public class TradingBot
{
    private List<PriceUpdate> _priceHistory = new();

    public void ProcessPriceUpdate(string symbol, decimal price, DateTime timestamp)
    {
        // Her güncelleme için yeni string ve object allocation
        var update = new PriceUpdate
        {
            Symbol = symbol, // String allocation
            Price = price,
            Timestamp = timestamp
        };

        _priceHistory.Add(update); // List reallocation

        // Analiz için string manipulation
        string analysis = $"Price for {symbol} is {price} at {timestamp}";
        Console.WriteLine(analysis); // String allocation

        CalculateMovingAverage(symbol); // Heavyweight LINQ
    }

    private void CalculateMovingAverage(string symbol)
    {
        // LINQ chain - multiple enumerations
        var prices = _priceHistory
            .Where(p => p.Symbol == symbol)  // Allocation
            .OrderByDescending(p => p.Timestamp) // Allocation
            .Take(20) // Allocation
            .Select(p => p.Price) // Allocation
            .ToList(); // Allocation

        var average = prices.Average();
    }
}
```

**Ne oldu?**
- Saniyede 100,000 request → 10 saniyede 1,000,000 allocation
- Garbage Collector (GC) her 2 saniyede bir çalışıyor → 100ms pause
- Kullanıcılar lag yaşıyor: "Fiyatlar donuyor!"
- Memory 500MB'dan 4GB'a fırladı
- Server CPU %90'ın üzerinde

### Teknik Problem

**Problem 1: Excessive Allocations (Aşırı Bellek Tahsisi)**
- Her string manipulation yeni allocation
- Her LINQ operation ara collection yaratıyor
- List<T> büyüdükçe reallocation (array copying)
- Result: GC pressure → Performance degradation

**Problem 2: String Manipulation Overhead**
- String immutable → Her değişiklik yeni string
- String concat işlemi O(n) complexity
- StringBuilder kullanılsa bile allocation var
- 1 million string operation = Gigabyte'larca garbage

**Problem 3: LINQ Performance Tax**
- IEnumerable<T> lazy evaluation → Iterator overhead
- Where, Select, OrderBy her biri allocation
- ToList(), ToArray() tüm veriyi kopyalar
- Nested LINQ chains → Exponential overhead

**Problem 4: Array/Collection Copy Overhead**
- Array.Copy(), List.CopyTo() → O(n) time, n bytes memory
- Büyük arrays (10MB+) kopyalamak pahalı
- Concurrent collections lock overhead
- Read-heavy scenarios'da gereksiz copying

### Kötü Çözüm Örneği

```csharp
// BU KODU ASLA YAZMAYIN! (Production'da Felaket)
public class BadPriceAnalyzer
{
    private List<string> _symbols = new();
    private Dictionary<string, List<decimal>> _prices = new();

    public void AnalyzePrices(string[] symbolArray)
    {
        // Problem 1: Array to List conversion (allocation)
        var symbolList = symbolArray.ToList(); // Copying entire array

        foreach (var symbol in symbolList)
        {
            // Problem 2: String manipulation (allocation)
            string upperSymbol = symbol.ToUpper(); // New string
            string trimmedSymbol = upperSymbol.Trim(); // Another new string

            // Problem 3: Dictionary lookup + List operations
            if (!_prices.ContainsKey(trimmedSymbol))
            {
                _prices[trimmedSymbol] = new List<decimal>(); // Allocation
            }

            // Problem 4: Heavyweight LINQ on hot path
            var last100Prices = _prices[trimmedSymbol]
                .OrderByDescending(p => p) // Allocation + sorting
                .Take(100) // Allocation
                .ToList(); // Allocation

            // Problem 5: String concatenation in loop
            string report = "";
            foreach (var price in last100Prices)
            {
                report += $"{price},"; // NEW STRING EACH ITERATION!
            }

            // Problem 6: Boxing for Console.WriteLine
            Console.WriteLine($"Symbol: {symbol}, Prices: {report}");
        }
    }
}
```

**Performans analizi (1000 symbol, saniyede 100 call)**:
- **Allocations**: 100,000+ per second
- **GC Collections**: Gen0: 50/sec, Gen1: 10/sec, Gen2: 1/sec
- **Memory**: 2GB+ garbage per minute
- **Latency**: p99 = 500ms (acceptable: 10ms)
- **CPU**: 80% GC, 20% actual work

**Neden kötü?**
1. **Memory Explosion**: Saniyede MB'larca garbage
2. **GC Pauses**: 100-500ms freeze'ler
3. **CPU Waste**: %80 GC overhead
4. **Latency**: Kullanıcı deneyimi kötü
5. **Scalability**: 10x yük = Crash

---

## 💡 ÇÖZÜM: HIGH-PERFORMANCE PATTERNS

### Pattern'in Özü

**High-performance C#**, zero-allocation/low-allocation code yazmak için modern C# özelliklerini kullanır: `Span<T>`, `Memory<T>`, `ArrayPool<T>`, `stackalloc`, ve `ref` keyword'leri.

### Nasıl Çalışır?

1. **Span<T>**: Stack üzerinde allocation-free array slice işlemleri
2. **Memory<T>**: Heap üzerinde allocation-free memory regions
3. **ArrayPool<T>**: Array'leri recycle et, yeni allocation yapma
4. **stackalloc**: Stack'te geçici buffer'lar oluştur (heap yok!)
5. **ref struct**: Stack-only types, zero allocation
6. **Parallel.For**: Multi-threading ile throughput artır

### Ne Zaman Kullanılır?

- ✅ Saniyede 10,000+ operation yapılıyorsa
- ✅ GC pause'ları kullanıcı deneyimini bozuyorsa
- ✅ Memory budget sınırlıysa (IoT, mobile, containerized apps)
- ✅ Latency kritikse (trading, gaming, real-time)
- ✅ High-frequency data processing (log parsing, packet processing)

### Bu Repo'daki Implementasyon

#### Örnek 1: Span<T> ile Zero-Allocation String Parsing

```csharp
// samples/03-Advanced/HighPerformance/SpanMemoryExamples.cs

// ❌ KÖTÜ: Traditional approach (allocations)
public decimal[] ParsePricesBad(string csvLine)
{
    string[] parts = csvLine.Split(','); // Allocation: Array + strings
    decimal[] prices = new decimal[parts.Length]; // Allocation: Array

    for (int i = 0; i < parts.Length; i++)
    {
        prices[i] = decimal.Parse(parts[i]); // Multiple allocations
    }

    return prices;
}

// ✅ İYİ: Span<T> approach (ZERO allocations!)
public int ParsePricesGood(ReadOnlySpan<char> csvLine, Span<decimal> output)
{
    int count = 0;
    int start = 0;

    for (int i = 0; i <= csvLine.Length; i++)
    {
        if (i == csvLine.Length || csvLine[i] == ',')
        {
            // Slice without allocation!
            ReadOnlySpan<char> numberSpan = csvLine.Slice(start, i - start);

            // Parse directly from span (no allocation)
            if (decimal.TryParse(numberSpan, out decimal price))
            {
                output[count++] = price;
            }

            start = i + 1;
        }
    }

    return count;
}

// Kullanım
string csvData = "100.5,200.75,300.25";
Span<decimal> prices = stackalloc decimal[10]; // Stack allocation!
int count = ParsePricesGood(csvData, prices);

// Result: ZERO heap allocations!
```

**Performance Comparison**:
| Metric | Bad (Traditional) | Good (Span<T>) | Improvement |
|--------|------------------|----------------|-------------|
| Allocations | 3 per call | 0 | ∞ |
| Memory | 240 bytes | 0 bytes | -100% |
| Speed | 1000 ns | 150 ns | 6.6x faster |
| GC Pressure | High | Zero | -100% |

---

#### Örnek 2: ArrayPool<T> ile Memory Reuse

```csharp
// ❌ KÖTÜ: Creating arrays repeatedly
public void ProcessDataBad(int size)
{
    for (int i = 0; i < 1000; i++)
    {
        byte[] buffer = new byte[size]; // NEW ALLOCATION EVERY TIME!
        // Process data...
    } // 1000 arrays allocated, now garbage
}

// ✅ İYİ: ArrayPool<T> for reuse
public void ProcessDataGood(int size)
{
    var pool = ArrayPool<byte>.Shared;

    for (int i = 0; i < 1000; i++)
    {
        byte[] buffer = pool.Rent(size); // Reuse from pool!
        try
        {
            // Process data...
        }
        finally
        {
            pool.Return(buffer); // Return to pool for reuse
        }
    } // ZERO new allocations after warmup!
}
```

**Performance Impact**:
```
Processing 1000 iterations with 1MB buffers:
❌ Bad: 1000 allocations × 1MB = 1000MB allocated → GC pause 200ms
✅ Good: 10 allocations × 1MB = 10MB allocated → GC pause 2ms (100x better!)
```

---

#### Örnek 3: stackalloc ile Stack Buffer

```csharp
// ❌ KÖTÜ: Heap allocation
public string FormatPriceBad(decimal price)
{
    // StringBuilder allocates on heap
    var builder = new StringBuilder();
    builder.Append("$");
    builder.Append(price.ToString("F2"));
    return builder.ToString(); // String allocation
}

// ✅ İYİ: stackalloc (stack-only, zero heap!)
public string FormatPriceGood(decimal price)
{
    // Small buffer on stack (NO HEAP ALLOCATION!)
    Span<char> buffer = stackalloc char[32];
    int written = 0;

    buffer[written++] = '$';

    // Format directly into span
    if (price.TryFormat(buffer.Slice(written), out int charsWritten, "F2"))
    {
        written += charsWritten;
    }

    // Only allocate final string once
    return new string(buffer.Slice(0, written));
}
```

**Key Benefit**: Buffer on stack → Automatic cleanup, zero GC pressure

---

#### Örnek 4: Parallel Processing ile Throughput

```csharp
// samples/03-Advanced/HighPerformance/ParallelProcessingExamples.cs

// ❌ KÖTÜ: Sequential processing
public void ProcessMillionRecordsBad(List<Record> records)
{
    foreach (var record in records)
    {
        ProcessRecord(record); // 1ms each = 1000 seconds total!
    }
}

// ✅ İYİ: Parallel processing
public void ProcessMillionRecordsGood(List<Record> records)
{
    Parallel.ForEach(records, new ParallelOptions
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount
    },
    record =>
    {
        ProcessRecord(record); // All cores working!
    });
}

// Result: 8 cores → 8x faster (125 seconds instead of 1000!)
```

---

### Adım Adım Nasıl Uygulanır

#### Adım 1: Hot Path'leri Belirle (Profiling)

```csharp
// BenchmarkDotNet ile ölç
[MemoryDiagnoser]
[Benchmark]
public void MyHotPath()
{
    // En sık çağrılan metodunu benchmark et
}
```

**Nelere bak?**
- Allocations/Op > 100 → Problem!
- Mean time > 1ms → Problem!
- Gen0/Gen1 collections > 0 → GC pressure

---

#### Adım 2: String Operations → Span<char>

**Before**:
```csharp
string ProcessString(string input)
{
    string upper = input.ToUpper(); // Allocation
    string trimmed = upper.Trim(); // Allocation
    string result = trimmed.Substring(0, 10); // Allocation
    return result;
}
```

**After**:
```csharp
void ProcessString(ReadOnlySpan<char> input, Span<char> output)
{
    // Zero allocations!
    int written = 0;
    foreach (char c in input)
    {
        if (!char.IsWhiteSpace(c) && written < output.Length)
        {
            output[written++] = char.ToUpper(c);
        }
    }
}
```

---

#### Adım 3: Temporary Arrays → ArrayPool<T>

**Before**:
```csharp
void ProcessBatch(int batchSize)
{
    var buffer = new byte[batchSize]; // Allocation each call
    // Use buffer...
}
```

**After**:
```csharp
void ProcessBatch(int batchSize)
{
    var pool = ArrayPool<byte>.Shared;
    var buffer = pool.Rent(batchSize); // Reuse!
    try
    {
        // Use buffer...
    }
    finally
    {
        pool.Return(buffer);
    }
}
```

---

#### Adım 4: Small Fixed Arrays → stackalloc

**Before**:
```csharp
void ProcessSmallData()
{
    var buffer = new int[16]; // Heap allocation
    // Use buffer...
}
```

**After**:
```csharp
void ProcessSmallData()
{
    Span<int> buffer = stackalloc int[16]; // Stack allocation!
    // Use buffer...
} // Automatic cleanup, no GC!
```

**⚠️ Dikkat**: stackalloc sadece küçük bufferlar için (< 1KB). Büyük bufferlar stack overflow'a yol açar!

---

## ⚖️ TRADE-OFF ANALİZİ

### Avantajları

✅ **Dramatic Performance Gains**
- **Neden avantaj?** 10-100x hız artışı common
- **Hangi durumda kritik?** Real-time systems (trading, gaming, streaming)
- **Performance etkisi**: Latency p99 1000ms → 10ms

✅ **Zero/Low GC Pressure**
- **Neden avantaj?** GC pauses eliminate edilir
- **Hangi durumda kritik?** User-facing apps (UI freeze olmamalı)
- **Memory etkisi**: Memory kullanımı 4GB → 500MB

✅ **Scalability**
- **Neden avantaj?** Aynı donanımda 10x daha fazla yük
- **Hangi durumda kritik?** Cloud apps (daha az server = daha az maliyet)
- **Örnek**: 10 server yerine 1 server yeterli → $10k/month saving

✅ **Predictable Performance**
- **Neden avantaj?** GC pause yok → Latency stable
- **Hangi durumda kritik?** SLA'lı sistemler (99.9% uptime guarantee)
- **Örnek**: p99 latency guarantee (< 50ms)

✅ **Resource Efficiency**
- **Neden avantaj?** Daha az CPU, daha az memory
- **Hangi durumda kritik?** IoT devices, mobile apps, containerized apps
- **Örnek**: Docker container 2GB RAM limit'te rahat çalışır

### Dezavantajları

❌ **Steep Learning Curve**
- **Ne zaman problem olur?** Junior/Mid-level developer'lar için complex
- **Çözüm**: İyi training, code reviews, pairing
- **Impact**: 1-2 ay öğrenme süreci, uzun vadede worth it

❌ **Increased Code Complexity**
- **Ne zaman problem olur?** Span<T>, ref, unsafe kod daha zor okunur
- **Complexity artışı?** Yüksek - Kod 2-3x daha uzun olabilir
- **Çözüm**: Helper methods, good naming, documentation

❌ **Unsafe Code Risks**
- **Ne zaman problem olur?** stackalloc, Span<T> ile buffer overflow riski
- **Örnek**: `stackalloc byte[userInput]` → Stack overflow!
- **Çözüm**: Input validation, bounds checking, code review

❌ **Limited Async Support**
- **Ne zaman problem olur?** `ref struct` (Span<T>) async metodlarda kullanılamaz
- **Workaround**: Memory<T> kullan (biraz daha fazla overhead)
- **Impact**: Orta - Genelde workaround'lar yeterli

❌ **Platform Limitations**
- **Ne zaman problem olur?** Bazı platformlarda stackalloc disabled (Unity, Blazor WASM)
- **Çözüm**: Conditional compilation, fallback to heap
- **Impact**: Düşük - Mainstream platforms'da problem yok

### Ne Zaman KULLANMAMALISIN?

**Senaryo 1: Low-Traffic Application**
- 1000 req/day olan admin panel için overkill
- Traditional code daha okunabilir, maintainable
- Performance gain'i fark edilmez

**Senaryo 2: Prototype/MVP Phase**
- Hızlı development daha kritik
- Optimization premature olur
- Kullanıcı feedback'i önce, sonra optimize

**Senaryo 3: Junior Team**
- Team Span<T>, unsafe code bilmiyorsa
- Bugların maliyeti performance gain'inden fazla
- Önce team train et, sonra uygula

---

## 🔄 ALTERNATİF PATTERN'LER

### Alternatif 1: Object Pooling (Generic)

**Ne zaman tercih edilir?**
- Ağır nesneler recycle edilmek istendiğinde
- ArrayPool<T> yeterli değilse (complex objects)
- Connection pools, HttpClient pools gibi

**Bu repo'da nerede görülür?**
- `samples/03-Advanced/HighPerformance/` (object pooling patterns)

**Farkı nedir?**
| Özellik | Span<T> / stackalloc | ObjectPool<T> |
|---------|---------------------|----------------|
| Memory | Stack | Heap (reused) |
| Scope | Method-local | Application-wide |
| Type | Primitives, structs | Complex objects |
| Overhead | Zero | Minimal (lock) |

```csharp
// Object pooling örneği
public class ExpensiveObject
{
    // Ağır initialization
    public byte[] LargeBuffer { get; set; }
    public Dictionary<string, string> Cache { get; set; }
}

public class ExpensiveObjectPool
{
    private ConcurrentBag<ExpensiveObject> _pool = new();

    public ExpensiveObject Rent()
    {
        if (_pool.TryTake(out var obj))
            return obj; // Reuse!

        return new ExpensiveObject(); // Create if pool empty
    }

    public void Return(ExpensiveObject obj)
    {
        // Reset state
        obj.Cache.Clear();
        _pool.Add(obj); // Return to pool
    }
}
```

---

### Alternatif 2: Memory-Mapped Files

**Ne zaman tercih edilir?**
- Çok büyük dosyalar (GB+) işlenirken
- Dosya tamamını memory'e yüklemek imkansızsa
- Shared memory gerektiğinde (IPC)

**Farkı nedir?**
```csharp
// Traditional file reading (tüm dosyayı memory'e yükler)
byte[] data = File.ReadAllBytes("huge-file.dat"); // 10GB → OutOfMemory!

// Memory-mapped file (sadece kullanılan kısım memory'de)
using var mmf = MemoryMappedFile.CreateFromFile("huge-file.dat");
using var accessor = mmf.CreateViewAccessor();
// Sadece ihtiyaç duyulan byte'ları oku
byte b = accessor.ReadByte(1000000); // Hızlı random access
```

---

### Alternatif 3: Value Types (struct) Optimization

**Ne zaman tercih edilir?**
- Küçük, immutable data structures
- Stack allocation istediğinizde
- Heap allocation minimize etmek için

**Farkı nedir?**
```csharp
// ❌ Class (heap allocation)
public class Point
{
    public int X { get; set; }
    public int Y { get; set; }
}

// 1 million points = 1 million allocations + GC pressure
Point[] points = new Point[1_000_000];
for (int i = 0; i < points.Length; i++)
{
    points[i] = new Point { X = i, Y = i }; // Allocation!
}

// ✅ Struct (stack/inline allocation)
public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }
}

// 1 million points = 1 allocation (array itself)
Point[] points = new Point[1_000_000];
// Points inline in array, no GC pressure!
```

---

### Karar Matrisi

| Kriter | Span<T> | ArrayPool<T> | Object Pool | Memory-Mapped | Value Types |
|--------|---------|-------------|-------------|---------------|-------------|
| **Performance** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐☆ | ⭐⭐⭐☆☆ | ⭐⭐⭐⭐☆ | ⭐⭐⭐⭐☆ |
| **Complexity** | ⭐⭐☆☆☆ | ⭐⭐⭐☆☆ | ⭐⭐⭐⭐☆ | ⭐⭐⭐☆☆ | ⭐⭐⭐⭐☆ |
| **Safety** | ⭐⭐⭐☆☆ | ⭐⭐⭐⭐☆ | ⭐⭐⭐⭐☆ | ⭐⭐⭐☆☆ | ⭐⭐⭐⭐⭐ |
| **Async Support** | ⭐☆☆☆☆ | ⭐⭐⭐⭐☆ | ⭐⭐⭐⭐☆ | ⭐⭐⭐⭐☆ | ⭐⭐⭐⭐⭐ |
| **Useability** | ⭐⭐⭐☆☆ | ⭐⭐⭐⭐☆ | ⭐⭐⭐⭐☆ | ⭐⭐☆☆☆ | ⭐⭐⭐⭐☆ |

---

## 🏗️ REAL-WORLD UYGULAMA

### Capstone Projesindeki Kullanımı

```csharp
// samples/08-Capstone/MicroVideoPlatform/ - Video streaming service

public class VideoStreamProcessor
{
    private readonly ArrayPool<byte> _bufferPool = ArrayPool<byte>.Shared;

    public async Task ProcessVideoChunkAsync(Stream videoStream)
    {
        // ArrayPool kullan (reuse buffers)
        byte[] buffer = _bufferPool.Rent(8192);
        try
        {
            int bytesRead;
            while ((bytesRead = await videoStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                // Span kullan (zero-copy processing)
                ReadOnlySpan<byte> chunk = buffer.AsSpan(0, bytesRead);
                ProcessChunk(chunk); // Zero-allocation processing
            }
        }
        finally
        {
            _bufferPool.Return(buffer); // Return to pool
        }
    }

    private void ProcessChunk(ReadOnlySpan<byte> chunk)
    {
        // stackalloc for temporary computation
        Span<int> tempBuffer = stackalloc int[64];

        // Process chunk with zero allocations...
    }
}
```

**Impact**: Video streaming service handles 1000+ concurrent streams on single server

---

### Enterprise Projelerdeki Yeri

#### Microsoft'un Kullanımı
**ASP.NET Core Kestrel Web Server**:
- Span<T> for HTTP header parsing (zero-allocation)
- ArrayPool<T> for buffer management
- Result: 7 million req/sec on single server

#### Stack Overflow'un Kullanımı
**High-Performance Question Rendering**:
- Span<T> for markdown parsing
- Object pooling for heavy objects
- Result: 15 million page views/day on minimal hardware

#### Discord'un Kullanımı
**Message Processing Pipeline**:
- Memory<T> for async message handling
- Parallel.ForEach for batch processing
- Result: Billions of messages/day processed

---

### Code Review'da Nelere Bakılır?

#### Kontrol 1: Span<T> Lifetime Doğru mu?

```csharp
// ❌ KÖTÜ: Span<T> cannot escape method scope
public Span<byte> GetBufferBad()
{
    Span<byte> buffer = stackalloc byte[256];
    return buffer; // COMPILE ERROR: Cannot return stack-allocated span
}

// ✅ İYİ: Return Memory<T> or use callback pattern
public void ProcessBuffer(Action<Span<byte>> processor)
{
    Span<byte> buffer = stackalloc byte[256];
    processor(buffer); // Safe: Used within method scope
}
```

#### Kontrol 2: ArrayPool Return Edilmiş mi?

```csharp
// ❌ KÖTÜ: Buffer leaked!
public void ProcessDataBad()
{
    var buffer = ArrayPool<byte>.Shared.Rent(1024);
    // Process...
    // FORGOT TO RETURN! Memory leak!
}

// ✅ İYİ: Always use try-finally
public void ProcessDataGood()
{
    var buffer = ArrayPool<byte>.Shared.Rent(1024);
    try
    {
        // Process...
    }
    finally
    {
        ArrayPool<byte>.Shared.Return(buffer); // Always return!
    }
}
```

#### Kontrol 3: stackalloc Boyutu Güvenli mi?

```csharp
// ❌ KÖTÜ: User-controlled stackalloc (DANGEROUS!)
public void ProcessUserInputBad(int size)
{
    Span<byte> buffer = stackalloc byte[size]; // Stack overflow risk!
}

// ✅ İYİ: Validate and limit size
public void ProcessUserInputGood(int size)
{
    if (size > 1024)
        throw new ArgumentException("Size too large");

    Span<byte> buffer = stackalloc byte[size]; // Safe
}
```

---

## 🚀 BİR SONRAKİ ADIM

### Bu Pattern'i Öğrendikten Sonra

#### Pratik Yap
**Önerilen Exercises**:
- Benchmark mevcut kodunu BenchmarkDotNet ile
- Hot path'leri Span<T>'ye convert et
- ArrayPool<T> ile buffer management implement et

#### Derinleş
**İleri Okuma**:
- `samples/03-Advanced/PerformanceBenchmarks/` - Tüm benchmark'ları çalıştır
- Adam Sitnik'in blog'u: https://adamsitnik.com
- Stephen Toub'un performance blog posts

#### Uygula
**Kendi Projende**:
1. Profiling yap (dotTrace, PerfView)
2. Hot path'leri belirle (>10k calls/sec)
3. Bu pattern'leri uygula
4. Benchmark et ve ölç

---

**Sonraki Adım**: Production'da test et, monitor et, iterate et!

