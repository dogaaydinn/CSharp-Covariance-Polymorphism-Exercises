# Problem: Distributed Locking

## 📋 Problem Tanımı

Multi-server e-commerce uygulamasında aynı ürün 2 server tarafından aynı anda satılıyor. Stok kontrolü yetersiz, overselling gerçekleşiyor.

### Gerçek Dünya Senaryosu

**Şirket**: Multi-region e-commerce platform (5 servers)
**Problem**: 100 ürün stoğu var, 150 sipariş alınıyor (overs selling!)
**Sebep**: Her server kendi database connection'ını kullanıyor, race condition
**Etki**:
- Müşteri memnuniyetsizliği
- İade maliyeti
- Reputasyon kaybı

## 🎯 Locking Strategies

### 1. Database Locking (Pessimistic)
```sql
BEGIN TRANSACTION
SELECT * FROM Products WHERE Id = 1 FOR UPDATE  -- Lock row
UPDATE Products SET Stock = Stock - 1 WHERE Id = 1
COMMIT
```

### 2. Optimistic Concurrency (Version/ETag)
```csharp
var product = await _db.Products.FindAsync(id);
product.Stock -= 1;
product.Version += 1;  // Increment version

await _db.SaveChangesAsync();  // Fails if version changed
```

### 3. Distributed Lock (Redis)
```csharp
var lockAcquired = await _redis.LockAsync("product:1", TimeSpan.FromSeconds(10));
if (lockAcquired)
{
    // Critical section
    await UpdateStockAsync(productId, -1);
}
```

## 📊 Comparison

| Strategy | Complexity | Performance | Scalability | Use Case |
|----------|-----------|-------------|-------------|----------|
| Database Lock | Low | Slow | Limited | Single DB |
| Optimistic Concurrency | Medium | Fast | Good | Low contention |
| Distributed Lock (Redis) | High | Medium | Excellent | Multi-server |

## ⚠️ Common Problems

1. **Deadlock**: Two transactions waiting for each other's locks
2. **Lock Timeout**: Lock held too long, other requests fail
3. **Lock Leakage**: Lock not released (crash/exception)
4. **Split Brain**: Network partition creates 2 masters
