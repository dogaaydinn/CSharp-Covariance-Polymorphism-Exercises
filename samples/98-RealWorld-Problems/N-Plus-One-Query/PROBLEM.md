# Problem: N+1 Query Problem

## 📋 Problem Tanımı

Entity Framework Core kullanarak blog yazılarını ve yorumlarını listelerken, 1 blog için 100 yorum varsa **101 SQL query** çalışıyor!

### Gerçek Dünya Senaryosu

**Şirket**: Sosyal medya platformu
**Problem**: Anasayfa yüklenirken 10 saniye sürüyor
**Sebep**: 100 post × 50 comment = 5001 SQL query!
**Etki**:
- Database CPU %95
- Response time > 10 saniye
- Kullanıcılar şikayet ediyor
- AWS RDS maliyetleri artıyor

### N+1 Query Nedir?

```
1. SELECT * FROM Posts                    -- 1 query (N posts bulundu)
2. SELECT * FROM Comments WHERE PostId=1  -- Query 1
3. SELECT * FROM Comments WHERE PostId=2  -- Query 2
4. SELECT * FROM Comments WHERE PostId=3  -- Query 3
...
N+1. SELECT * FROM Comments WHERE PostId=N -- Query N

Total: 1 + N queries = N+1 queries!
```

## 🎯 Örnek Kod (Kötü)

```csharp
// ❌ YANLIŞ - N+1 Query problemi
var posts = await _context.Posts.ToListAsync();  // 1 query

foreach (var post in posts)
{
    Console.WriteLine($"Post: {post.Title}");

    // Her post için ayrı query!
    foreach (var comment in post.Comments)  // N queries
    {
        Console.WriteLine($"  - {comment.Text}");
    }
}

// Result: 1 + 100 = 101 queries!
```

### SQL Profiler Çıktısı

```sql
-- Query 1
SELECT * FROM Posts;  -- Returns 100 rows

-- Query 2-101 (100 times!)
SELECT * FROM Comments WHERE PostId = 1;
SELECT * FROM Comments WHERE PostId = 2;
SELECT * FROM Comments WHERE PostId = 3;
...
SELECT * FROM Comments WHERE PostId = 100;
```

## ⚠️ Performans Etkisi

### Before (N+1 Queries)
```
Queries: 101
Duration: 5,050ms (50ms × 101)
CPU: 95%
Memory: 2GB
```

### After (Optimized)
```
Queries: 1
Duration: 50ms
CPU: 15%
Memory: 150MB
```

**95% improvement!**

## 🔍 N+1 Query Nasıl Tespit Edilir?

### 1. EF Core Query Logging

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString)
           .LogTo(Console.WriteLine, LogLevel.Information)
           .EnableSensitiveDataLogging()
           .EnableDetailedErrors();
});
```

### 2. SQL Profiler

```bash
# SQL Server Profiler
- Start trace
- Filter by application name
- Look for repeated queries with different parameters
```

### 3. Application Insights

```csharp
// Track query count
var queryCount = 0;
_context.Database.Log = query => queryCount++;
```

### 4. Warning Signs

- Response time artıyor
- Database CPU yüksek
- Query count = N+1 (linear growth)
- Log'da tekrar eden query'ler

## 📊 Gerçek Dünya Örnekleri

### Example 1: Blog Platformu

```
100 posts × 50 comments = 5,001 queries
Response time: 10 seconds → 50ms (200x improvement)
```

### Example 2: E-commerce

```
Product listing with reviews and images
500 products × 3 queries each = 1,501 queries
Page load: 8 seconds → 100ms (80x improvement)
```

### Example 3: Social Media Feed

```
Feed with posts, comments, likes, shares
100 posts × 4 relations = 401 queries
Memory: 2GB → 200MB (10x improvement)
```

## 🎓 Öğrenme Hedefleri

Bu problemi çözerek öğreneceksiniz:
- EF Core Include() ve ThenInclude()
- Eager loading vs Lazy loading
- Projection (Select) optimizasyonu
- AsNoTracking() performance boost
- AsSplitQuery() vs Single query
- GraphQL DataLoader pattern

## 🧪 Test Senaryoları

### Senaryo 1: Simple Include
```csharp
// Load posts with comments
var posts = await _context.Posts
    .Include(p => p.Comments)
    .ToListAsync();

Expected: 1 query (with LEFT JOIN)
```

### Senaryo 2: Multiple Includes
```csharp
// Load posts with comments, likes, and author
var posts = await _context.Posts
    .Include(p => p.Comments)
    .Include(p => p.Likes)
    .Include(p => p.Author)
    .ToListAsync();

Expected: 1 query with multiple JOINs or 4 split queries
```

### Senaryo 3: Nested Includes
```csharp
// Load posts with comments and comment authors
var posts = await _context.Posts
    .Include(p => p.Comments)
        .ThenInclude(c => c.Author)
    .ToListAsync();

Expected: 1 query with nested JOIN
```

## 📚 Referanslar

- [EF Core Documentation: Loading Related Data](https://docs.microsoft.com/en-us/ef/core/querying/related-data/)
- [N+1 Query Problem - Martin Fowler](https://martinfowler.com/bliki/N1Problem.html)
- [GraphQL DataLoader](https://github.com/graphql/dataloader)
- [Hibernate N+1 Select Problem](https://vladmihalcea.com/n-plus-1-query-problem/)
