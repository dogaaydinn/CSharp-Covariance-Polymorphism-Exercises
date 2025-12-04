# Path 2 - Months 1-2 Capstone: High-Performance Log Parser

**Difficulty**: ⭐⭐⭐⭐☆ (Advanced)
**Estimated Time**: 40-50 hours
**Prerequisites**: Path 1 completed OR equivalent (Junior Developer skills)

---

## 🎯 Project Overview

Build a zero-allocation log parser that can process 1GB+ log files in under 10 seconds using Span<T>, Memory<T>, ArrayPool<T>, and parallel processing.

### Learning Objectives

- ✅ Span<T> and Memory<T> mastery
- ✅ ArrayPool<T> for object pooling
- ✅ Stackalloc and stack allocation
- ✅ Zero-allocation parsing
- ✅ Parallel.For and PLINQ
- ✅ BenchmarkDotNet profiling

---

## 📋 Requirements

### Functional Requirements

1. **Parse Multiple Log Formats**:
   - Apache/Nginx access logs
   - Application logs (custom format)
   - JSON logs
   - CSV logs

2. **Parsing Operations**:
   - Extract timestamp, level, message, context
   - Parse IP addresses and URLs
   - Extract key-value pairs
   - Count occurrences
   - Filter by criteria

3. **Aggregations**:
   - Count by log level (ERROR, WARN, INFO)
   - Group by hour/day
   - Top N errors
   - Response time statistics
   - HTTP status code distribution

4. **Performance Requirements**:
   - **1GB file**: Parse in < 10 seconds
   - **100K lines**: Parse in < 1 second
   - **Memory**: < 500MB peak for 1GB file
   - **Allocations**: < 100 allocations for entire file

5. **Output Formats**:
   - Console table
   - JSON summary
   - CSV report

---

## 🏗️ Key Implementation

### Use Span<T> for Zero-Allocation Parsing

```csharp
public class LogParser
{
    // ❌ BAD: Traditional parsing (allocations)
    public LogEntry ParseTraditional(string line)
    {
        string[] parts = line.Split(' '); // Allocation!
        return new LogEntry
        {
            Timestamp = DateTime.Parse(parts[0]), // Allocation!
            Level = parts[1],
            Message = parts[2]
        };
    }

    // ✅ GOOD: Span<T> parsing (zero allocations)
    public bool TryParse(ReadOnlySpan<char> line, ref LogEntry entry)
    {
        int firstSpace = line.IndexOf(' ');
        if (firstSpace == -1) return false;

        ReadOnlySpan<char> timestampSpan = line.Slice(0, firstSpace);
        if (!DateTime.TryParse(timestampSpan, out DateTime timestamp))
            return false;

        entry.Timestamp = timestamp;

        // Continue parsing without allocations
        line = line.Slice(firstSpace + 1);
        // TODO: Parse remaining fields

        return true;
    }
}
```

### Use ArrayPool<T> for Buffer Reuse

```csharp
public class BufferedLogReader
{
    private static readonly ArrayPool<char> _charPool = ArrayPool<char>.Shared;

    public async Task ProcessFileAsync(string filePath)
    {
        char[] buffer = _charPool.Rent(1024 * 1024); // 1MB buffer
        try
        {
            using var stream = File.OpenRead(filePath);
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                int charsRead = await reader.ReadAsync(buffer, 0, buffer.Length);
                ReadOnlySpan<char> span = buffer.AsSpan(0, charsRead);

                ProcessLines(span);
            }
        }
        finally
        {
            _charPool.Return(buffer); // Return buffer to pool
        }
    }

    private void ProcessLines(ReadOnlySpan<char> data)
    {
        // TODO: Split by newlines and process each line
    }
}
```

### Use Parallel Processing

```csharp
public class ParallelLogProcessor
{
    public Dictionary<string, int> CountByLevel(string[] lines)
    {
        // Thread-safe concurrent dictionary
        var results = new ConcurrentDictionary<string, int>();

        Parallel.For(0, lines.Length, i =>
        {
            ReadOnlySpan<char> line = lines[i].AsSpan();

            if (TryExtractLevel(line, out string level))
            {
                results.AddOrUpdate(level, 1, (key, count) => count + 1);
            }
        });

        return results.ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    private bool TryExtractLevel(ReadOnlySpan<char> line, out string level)
    {
        // TODO: Extract log level without allocation
        level = null;
        return false;
    }
}
```

### Benchmarking

```csharp
[MemoryDiagnoser]
public class LogParserBenchmarks
{
    private string[] _lines;

    [GlobalSetup]
    public void Setup()
    {
        _lines = File.ReadAllLines("sample.log");
    }

    [Benchmark(Baseline = true)]
    public int Traditional_Parse()
    {
        int count = 0;
        foreach (var line in _lines)
        {
            string[] parts = line.Split(' ');
            if (parts.Length > 1 && parts[1] == "ERROR")
                count++;
        }
        return count;
    }

    [Benchmark]
    public int Span_Parse()
    {
        int count = 0;
        foreach (var line in _lines)
        {
            ReadOnlySpan<char> span = line.AsSpan();
            // Zero-allocation parsing
            if (ContainsError(span))
                count++;
        }
        return count;
    }

    private bool ContainsError(ReadOnlySpan<char> span)
    {
        // TODO: Implement zero-allocation search
        return false;
    }
}
```

---

## 📊 Performance Targets

| Metric | Traditional | Target (Span<T>) | Improvement |
|--------|-------------|------------------|-------------|
| Time (1GB file) | 60 seconds | < 10 seconds | 6x faster |
| Memory | 2GB | < 500MB | 75% reduction |
| Allocations | 10M+ | < 100 | 99.999% reduction |
| GC Collections | 1000+ | < 10 | 99% reduction |

---

## 🎯 Milestones

1. **Week 1**: Basic Span<T> parsing working
2. **Week 2**: ArrayPool<T> integration
3. **Week 3**: Parallel processing added
4. **Week 4**: Benchmarks show target performance

---

## ✅ Evaluation

| Criteria | Weight | Min Score |
|----------|--------|-----------|
| Performance (meets targets) | 40% | 32/40 |
| Zero-allocation implementation | 30% | 24/30 |
| Code quality | 20% | 16/20 |
| Benchmarks & profiling | 10% | 8/10 |

**Pass**: 75% (75/100)

---

## 📚 Resources

- `samples/03-Advanced/HighPerformance/SpanMemoryExamples.cs`
- `samples/03-Advanced/HighPerformance/ParallelProcessingExamples.cs`
- BenchmarkDotNet: https://benchmarkdotnet.org/
- Span<T> Guide: https://learn.microsoft.com/en-us/archive/msdn-magazine/2018/january/csharp-all-about-span-exploring-a-new-net-mainstay

---

*Template Version: 1.0*
