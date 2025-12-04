# Path 3 - Months 4-6 Capstone: Performance Optimization Challenge

**Difficulty**: ⭐⭐⭐⭐⭐ (Senior/Performance Expert)
**Estimated Time**: 80-100 hours
**Prerequisites**: Months 1-3 of Path 3 completed

---

## 🎯 Project Overview

Take a deliberately slow application and optimize it to 10x faster through systematic profiling, analysis, and optimization techniques. Demonstrate expert-level performance engineering skills.

### Learning Objectives

- ✅ Advanced profiling (dotTrace, PerfView, BenchmarkDotNet)
- ✅ Memory profiling and leak detection
- ✅ GC tuning and optimization
- ✅ Lock-free programming
- ✅ Database query optimization
- ✅ Cache strategies
- ✅ Async/await patterns

---

## 📋 The Challenge

### Starting Application

You'll be provided with (or create) a "slow" web API that has numerous performance issues:

**Initial Performance**:
- Average response time: 2000ms
- Throughput: 50 req/sec
- Memory usage: 500MB baseline, 2GB under load
- CPU usage: 80% constant
- GC collections: 100+ per second
- Database queries: N+1 problems everywhere

**Target Performance** (10x improvement):
- Average response time: < 200ms
- Throughput: > 500 req/sec
- Memory usage: < 200MB baseline, < 500MB under load
- CPU usage: < 30% average
- GC collections: < 10 per second
- Database queries: Optimized, no N+1

---

## 🐌 Intentional Performance Issues

The starting application will have these common problems:

### 1. Excessive Allocations
```csharp
// ❌ BAD: Boxing in hot path
public void ProcessOrders(List<object> orders)
{
    foreach (object order in orders) // Boxing!
    {
        int orderId = (int)order; // Unboxing!
        ProcessOrder(orderId);
    }
}

// ❌ BAD: String concatenation in loop
public string GenerateReport(List<Order> orders)
{
    string report = "";
    foreach (var order in orders)
    {
        report += $"Order {order.Id}: ${order.Total}\n"; // Allocation per iteration!
    }
    return report;
}
```

### 2. Synchronous Operations
```csharp
// ❌ BAD: Blocking async calls
public IActionResult GetOrders()
{
    var orders = _orderService.GetAllAsync().Result; // Blocks thread!
    return Ok(orders);
}

// ❌ BAD: Sequential async calls
public async Task<OrderSummary> GetOrderSummaryAsync(int orderId)
{
    var order = await _orderRepo.GetByIdAsync(orderId);
    var customer = await _customerRepo.GetByIdAsync(order.CustomerId);
    var items = await _itemRepo.GetByOrderIdAsync(orderId);

    // These could run in parallel!
    return new OrderSummary(order, customer, items);
}
```

### 3. N+1 Query Problem
```csharp
// ❌ BAD: N+1 queries
public async Task<List<OrderDto>> GetOrdersAsync()
{
    var orders = await _context.Orders.ToListAsync();

    foreach (var order in orders)
    {
        // Query for EACH order!
        order.Customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == order.CustomerId);

        order.Items = await _context.OrderItems
            .Where(i => i.OrderId == order.Id)
            .ToListAsync();
    }

    return orders.Select(o => ToDto(o)).ToList();
}
```

### 4. No Caching
```csharp
// ❌ BAD: Database hit every time
public async Task<Product> GetProductAsync(int id)
{
    return await _context.Products.FindAsync(id);
}
```

### 5. Lock Contention
```csharp
// ❌ BAD: Coarse-grained locking
private static readonly object _lock = new();
private static readonly Dictionary<int, Order> _orders = new();

public Order GetOrder(int id)
{
    lock (_lock) // Contention!
    {
        return _orders[id];
    }
}
```

---

## 🚀 Optimization Journey

### Phase 1: Profiling & Baseline

**Tools to Use**:
- dotTrace (CPU profiling)
- dotMemory (Memory profiling)
- PerfView (Advanced CPU & memory)
- BenchmarkDotNet (Micro-benchmarks)
- Application Insights (Production profiling)

**Establish Baseline**:
```bash
# Load test with k6
k6 run --vus 100 --duration 30s load-test.js

# Record baseline metrics
- Response time: 2000ms (p95)
- Throughput: 50 req/sec
- Memory: 2GB
- CPU: 80%
```

**Create Benchmarks**:
```csharp
[MemoryDiagnoser]
public class OrderProcessingBenchmarks
{
    [Benchmark(Baseline = true)]
    public async Task<List<Order>> GetOrders_Original()
    {
        return await _orderService.GetAllAsync();
    }

    [Benchmark]
    public async Task<List<Order>> GetOrders_Optimized()
    {
        // Will implement optimization here
        return await _orderService.GetAllOptimizedAsync();
    }
}
```

### Phase 2: Fix Allocations

**Optimization 1**: Use Span<T> and ArrayPool<T>
```csharp
// ✅ GOOD: Zero allocations
public void ProcessOrders(ReadOnlySpan<int> orderIds)
{
    foreach (int orderId in orderIds) // No boxing!
    {
        ProcessOrder(orderId);
    }
}

// ✅ GOOD: StringBuilder
public string GenerateReport(List<Order> orders)
{
    var sb = new StringBuilder(orders.Count * 50); // Pre-allocate
    foreach (var order in orders)
    {
        sb.AppendLine($"Order {order.Id}: ${order.Total}");
    }
    return sb.ToString();
}
```

**Results**:
- Allocations: 10M → 1000 (99.99% reduction)
- GC collections: 100/sec → 5/sec
- Memory: 2GB → 800MB

### Phase 3: Fix N+1 Queries

```csharp
// ✅ GOOD: Eager loading
public async Task<List<OrderDto>> GetOrdersAsync()
{
    var orders = await _context.Orders
        .Include(o => o.Customer)      // 1 query for customers
        .Include(o => o.Items)         // 1 query for items
        .ToListAsync();                // Total: 3 queries instead of N+1!

    return orders.Select(o => ToDto(o)).ToList();
}

// ✅ BETTER: Projection
public async Task<List<OrderDto>> GetOrdersOptimizedAsync()
{
    return await _context.Orders
        .Select(o => new OrderDto
        {
            OrderId = o.Id,
            CustomerName = o.Customer.Name,  // All in 1 query!
            ItemCount = o.Items.Count,
            TotalAmount = o.Items.Sum(i => i.Price * i.Quantity)
        })
        .ToListAsync();
}
```

**Results**:
- Database queries: 1001 → 1
- Query time: 5000ms → 50ms (100x faster)
- Database CPU: 95% → 10%

### Phase 4: Add Caching

```csharp
// ✅ GOOD: Multi-level caching
public class ProductService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _redisCache;
    private readonly ApplicationDbContext _context;

    public async Task<Product> GetProductAsync(int id)
    {
        // L1: Memory cache (nanoseconds)
        if (_memoryCache.TryGetValue($"product:{id}", out Product product))
            return product;

        // L2: Redis cache (milliseconds)
        var cached = await _redisCache.GetStringAsync($"product:{id}");
        if (cached != null)
        {
            product = JsonSerializer.Deserialize<Product>(cached);
            _memoryCache.Set($"product:{id}", product, TimeSpan.FromMinutes(5));
            return product;
        }

        // L3: Database (10-50ms)
        product = await _context.Products.FindAsync(id);

        // Populate caches
        await _redisCache.SetStringAsync(
            $"product:{id}",
            JsonSerializer.Serialize(product),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });

        _memoryCache.Set($"product:{id}", product, TimeSpan.FromMinutes(5));

        return product;
    }
}
```

**Results**:
- Cache hit rate: 0% → 95%
- Average response time: 50ms → 5ms (10x faster)
- Database load: -95%

### Phase 5: Parallelize Operations

```csharp
// ✅ GOOD: Parallel async calls
public async Task<OrderSummary> GetOrderSummaryAsync(int orderId)
{
    // Run in parallel!
    var orderTask = _orderRepo.GetByIdAsync(orderId);
    var customerTask = _customerRepo.GetByIdAsync(customerId); // Need to get customerId first
    var itemsTask = _itemRepo.GetByOrderIdAsync(orderId);

    await Task.WhenAll(orderTask, customerTask, itemsTask);

    return new OrderSummary(
        orderTask.Result,
        customerTask.Result,
        itemsTask.Result);
}

// ✅ GOOD: Parallel processing
public async Task<List<OrderResult>> ProcessOrdersAsync(List<Order> orders)
{
    var tasks = orders.Select(async order =>
    {
        return await ProcessOrderAsync(order);
    });

    return (await Task.WhenAll(tasks)).ToList();
}
```

**Results**:
- Processing time: 300ms → 100ms (3x faster)
- Throughput: 50 req/sec → 150 req/sec

### Phase 6: Fix Lock Contention

```csharp
// ✅ GOOD: ConcurrentDictionary (lock-free)
private static readonly ConcurrentDictionary<int, Order> _orders = new();

public Order GetOrder(int id)
{
    return _orders[id]; // No locking needed!
}

// ✅ GOOD: Fine-grained locking
private readonly ConcurrentDictionary<int, object> _locks = new();

public async Task UpdateOrderAsync(int orderId, Order update)
{
    var orderLock = _locks.GetOrAdd(orderId, _ => new object());

    lock (orderLock) // Lock only this order
    {
        // Update logic
    }
}
```

**Results**:
- Lock contention: -99%
- Throughput under high concurrency: 2x improvement

---

## 📊 Final Results

| Metric | Before | After | Improvement |
|--------|--------|-------|-------------|
| Response Time (p95) | 2000ms | 150ms | 13x faster |
| Throughput | 50 req/sec | 600 req/sec | 12x |
| Memory (baseline) | 500MB | 150MB | 70% reduction |
| Memory (under load) | 2GB | 400MB | 80% reduction |
| CPU Usage | 80% | 25% | 69% reduction |
| GC/sec | 100+ | <5 | 95% reduction |
| Database Queries | 1001 | 1 | 99.9% reduction |

**Overall**: More than **10x performance improvement** achieved!

---

## 📝 Deliverables

1. **Performance Report** (30+ pages):
   - Baseline measurements
   - Profiling analysis
   - Optimization techniques applied
   - Before/after comparisons
   - Benchmark results

2. **Optimized Code**:
   - All optimizations implemented
   - BenchmarkDotNet suite
   - Load test scripts

3. **Presentation** (20 slides):
   - Problem identification
   - Optimization journey
   - Results and learnings
   - Recommendations

4. **Blog Post**:
   - "How I Made My API 10x Faster"
   - Technical deep dive
   - Share on dev.to or Medium

---

## ✅ Evaluation Criteria

| Criterion | Weight |
|-----------|--------|
| Performance Improvement (10x) | 40% |
| Profiling & Analysis | 20% |
| Code Quality | 15% |
| Documentation & Report | 15% |
| Presentation | 10% |

**Minimum Pass**: 80% AND 8x performance improvement

---

## 📚 Resources

- dotTrace: https://www.jetbrains.com/profiler/
- PerfView: https://github.com/microsoft/perfview
- BenchmarkDotNet: https://benchmarkdotnet.org/
- Pro .NET Memory Management: Book by Konrad Kokosa
- Writing High-Performance .NET Code: Book by Ben Watson

---

*Template Version: 1.0*
