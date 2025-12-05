# Basic Solution: Eager Loading with Include()

## 🎯 Yaklaşım

**Eager Loading** ile ilişkili dataları tek sorguda yükleyin. EF Core'un `Include()` metodu bunu otomatik yapar.

## 🔧 Nasıl Çalışır?

```csharp
// ❌ Before (N+1 queries)
var posts = await _context.Posts.ToListAsync();
foreach (var post in posts)
{
    var comments = post.Comments; // Lazy load - separate query!
}

// ✅ After (1 query)
var posts = await _context.Posts
    .Include(p => p.Comments)
    .ToListAsync();
```

### Generated SQL

```sql
-- Single query with LEFT JOIN
SELECT
    p.Id, p.Title, p.Content,
    c.Id, c.PostId, c.Text, c.AuthorName
FROM Posts p
LEFT JOIN Comments c ON p.Id = c.PostId
ORDER BY p.Id
```

## ✅ Avantajlar

1. **Basit**: Tek satır kod eklemek yeterli
2. **Etkili**: N+1 → 1 query
3. **EF Core Native**: Built-in feature
4. **Type-Safe**: Compile-time checking

## ❌ Dezavantajlar

1. **Cartesian Explosion**: Çok JOIN = çok satır
   ```
   10 posts × 100 comments = 1,000 rows transferred
   ```

2. **Over-fetching**: İhtiyaç olmayan data gelir
3. **Memory**: Tüm data memory'ye yüklenir

## 💾 Multiple Includes

```csharp
var posts = await _context.Posts
    .Include(p => p.Comments)
    .Include(p => p.Likes)
    .Include(p => p.Author)
    .ToListAsync();
```

**SQL**:
```sql
SELECT * FROM Posts p
LEFT JOIN Comments c ON p.Id = c.PostId
LEFT JOIN Likes l ON p.Id = l.PostId
LEFT JOIN Users u ON p.AuthorId = u.Id
```

**Problem**: Cartesian product!
```
10 posts × 50 comments × 20 likes = 10,000 rows!
```

## 🔧 Nested Includes

```csharp
var posts = await _context.Posts
    .Include(p => p.Comments)
        .ThenInclude(c => c.Author)
    .ToListAsync();
```

**SQL**:
```sql
SELECT * FROM Posts p
LEFT JOIN Comments c ON p.Id = c.PostId
LEFT JOIN Users u ON c.AuthorId = u.Id
```

## 📊 Performance

| Scenario | Queries | Duration | Memory |
|----------|---------|----------|--------|
| No Include | 101 | 5,050ms | 2GB |
| With Include | 1 | 150ms | 500MB |
| **Improvement** | **99%** | **97%** | **75%** |

## 🎯 Best Practices

### DO ✅

```csharp
// Use Include for related data
var posts = await _context.Posts
    .Include(p => p.Comments)
    .ToListAsync();

// Use AsNoTracking for read-only
var posts = await _context.Posts
    .Include(p => p.Comments)
    .AsNoTracking()
    .ToListAsync();

// Filter before Include
var posts = await _context.Posts
    .Where(p => p.IsPublished)
    .Include(p => p.Comments)
    .ToListAsync();
```

### DON'T ❌

```csharp
// Don't Include everything
var posts = await _context.Posts
    .Include(p => p.Comments)
    .Include(p => p.Likes)
    .Include(p => p.Shares)
    .Include(p => p.Tags)
    .Include(p => p.Categories)  // Too much!
    .ToListAsync();

// Don't Include in loops
foreach (var category in categories)
{
    var posts = await _context.Posts
        .Include(p => p.Comments)  // N queries!
        .Where(p => p.CategoryId == category.Id)
        .ToListAsync();
}
```

## 🧪 Code Example

```csharp
public class BlogService
{
    private readonly AppDbContext _context;

    public BlogService(AppDbContext context)
    {
        _context = context;
    }

    // ❌ BAD - N+1 Query
    public async Task<List<PostDto>> GetPostsBadAsync()
    {
        var posts = await _context.Posts.ToListAsync();

        return posts.Select(p => new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            CommentCount = p.Comments.Count()  // Lazy load!
        }).ToList();
    }

    // ✅ GOOD - Eager Loading
    public async Task<List<PostDto>> GetPostsGoodAsync()
    {
        var posts = await _context.Posts
            .Include(p => p.Comments)
            .AsNoTracking()
            .ToListAsync();

        return posts.Select(p => new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            CommentCount = p.Comments.Count()
        }).ToList();
    }
}
```

## 📝 Summary

**Include()** en basit ve en yaygın çözümdür. Çoğu N+1 query problemi için yeterlidir.

**Ne Zaman Kullanmalı**:
- ✅ İlişkili datanın %80'inden fazlası gerekiyorsa
- ✅ Basit JOIN'ler (1-2 seviye)
- ✅ Orta büyüklükte dataset

**Ne Zaman Kullanmamalı**:
- ❌ Çok fazla JOIN (>3)
- ❌ Büyük collection'lar (>1000 items)
- ❌ Sadece birkaç field gerekiyorsa

**Sonraki Adım**: `SOLUTION-ADVANCED.md` - Projection ve Split Queries
