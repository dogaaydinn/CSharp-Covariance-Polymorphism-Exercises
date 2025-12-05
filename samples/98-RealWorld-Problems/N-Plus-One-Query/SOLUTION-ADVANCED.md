# Advanced Solution: Projection + Split Queries

## 🎯 Yaklaşım

**Projection** ile sadece ihtiyacınız olan field'ları seçin. **Split Queries** ile büyük JOIN'leri böl.

## 🔧 Projection (Select)

### Problem: Over-fetching

```csharp
// ❌ Tüm data yüklenir
var posts = await _context.Posts
    .Include(p => p.Comments)
    .ToListAsync();

// Post: Id, Title, Content, CreatedAt, UpdatedAt, ...
// Comment: Id, PostId, Text, AuthorName, CreatedAt, ...
// 50 field × 1000 rows = 50,000 values!
```

### Solution: Select Only What You Need

```csharp
// ✅ Sadece gerekli field'lar
var posts = await _context.Posts
    .Select(p => new PostDto
    {
        Id = p.Id,
        Title = p.Title,
        CommentCount = p.Comments.Count(),
        CommentTexts = p.Comments
            .Select(c => c.Text)
            .Take(5)  // Only first 5 comments
            .ToList()
    })
    .ToListAsync();
```

**SQL**:
```sql
SELECT
    p.Id,
    p.Title,
    COUNT(c.Id) as CommentCount,
    (SELECT TOP 5 Text FROM Comments WHERE PostId = p.Id) as CommentTexts
FROM Posts p
LEFT JOIN Comments c ON p.Id = c.PostId
GROUP BY p.Id, p.Title
```

## 📊 Performance Comparison

| Method | Data Size | Memory | Duration |
|--------|-----------|--------|----------|
| Include (all fields) | 5 MB | 500 MB | 150ms |
| Projection (5 fields) | 500 KB | 50 MB | 50ms |
| **Improvement** | **90%** | **90%** | **67%** |

## 🔄 Split Queries

### Problem: Cartesian Explosion

```csharp
// Single query with multiple collections
var posts = await _context.Posts
    .Include(p => p.Comments)  // 100 comments each
    .Include(p => p.Likes)     // 50 likes each
    .ToListAsync();

// Cartesian product: 10 posts × 100 comments × 50 likes
// = 50,000 rows transferred! (should be 1,600)
```

### Solution: Split into Multiple Queries

```csharp
// ✅ Split into separate queries
var posts = await _context.Posts
    .Include(p => p.Comments)
    .Include(p => p.Likes)
    .AsSplitQuery()  // Magic line!
    .ToListAsync();
```

**Generated SQL**:
```sql
-- Query 1: Posts
SELECT * FROM Posts;

-- Query 2: Comments
SELECT * FROM Comments WHERE PostId IN (1,2,3,...,10);

-- Query 3: Likes
SELECT * FROM Likes WHERE PostId IN (1,2,3,...,10);

-- Total: 3 queries, but 1,600 rows instead of 50,000!
```

## 🎯 Advanced Patterns

### 1. Conditional Include

```csharp
public async Task<List<Post>> GetPostsAsync(bool includeComments)
{
    var query = _context.Posts.AsQueryable();

    if (includeComments)
    {
        query = query.Include(p => p.Comments);
    }

    return await query.ToListAsync();
}
```

### 2. Filtered Include (EF Core 5+)

```csharp
var posts = await _context.Posts
    .Include(p => p.Comments.Where(c => c.IsApproved))
    .ToListAsync();
```

**SQL**:
```sql
SELECT * FROM Posts p
LEFT JOIN Comments c ON p.Id = c.PostId AND c.IsApproved = 1
```

### 3. Projection with GroupBy

```csharp
var postStats = await _context.Posts
    .GroupBy(p => p.CategoryId)
    .Select(g => new
    {
        CategoryId = g.Key,
        PostCount = g.Count(),
        TotalComments = g.Sum(p => p.Comments.Count()),
        AvgCommentsPerPost = g.Average(p => p.Comments.Count())
    })
    .ToListAsync();
```

### 4. Paginated Projection

```csharp
var posts = await _context.Posts
    .OrderByDescending(p => p.CreatedAt)
    .Skip(page * pageSize)
    .Take(pageSize)
    .Select(p => new PostSummaryDto
    {
        Id = p.Id,
        Title = p.Title,
        CommentCount = p.Comments.Count(),
        LatestComment = p.Comments
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => c.Text)
            .FirstOrDefault()
    })
    .ToListAsync();
```

## 🚀 Batch Loading Pattern

```csharp
public class BatchLoader
{
    private readonly AppDbContext _context;

    public async Task<Dictionary<int, List<Comment>>> LoadCommentsAsync(
        IEnumerable<int> postIds)
    {
        var comments = await _context.Comments
            .Where(c => postIds.Contains(c.PostId))
            .ToListAsync();

        return comments
            .GroupBy(c => c.PostId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}

// Usage
var posts = await _context.Posts.ToListAsync();
var postIds = posts.Select(p => p.Id);
var commentsByPostId = await batchLoader.LoadCommentsAsync(postIds);

foreach (var post in posts)
{
    post.Comments = commentsByPostId.GetValueOrDefault(post.Id, new List<Comment>());
}
```

## 🎓 AsNoTracking() for Read-Only

```csharp
// Without AsNoTracking (tracked entities)
var posts = await _context.Posts
    .Include(p => p.Comments)
    .ToListAsync();
// Memory: 500MB, Duration: 150ms

// With AsNoTracking (no change tracking)
var posts = await _context.Posts
    .Include(p => p.Comments)
    .AsNoTracking()
    .ToListAsync();
// Memory: 250MB, Duration: 80ms (50% improvement!)
```

## 📊 Decision Matrix

| Scenario | Solution |
|----------|----------|
| Need all fields | Include() |
| Need few fields | Projection (Select) |
| Multiple collections | AsSplitQuery() |
| Read-only data | AsNoTracking() |
| Large dataset | Pagination + Projection |
| Complex aggregation | GroupBy + Select |

## 🧪 Complete Example

```csharp
public class AdvancedBlogService
{
    private readonly AppDbContext _context;

    // Optimized with projection
    public async Task<PagedResult<PostSummaryDto>> GetPostsAsync(
        int page, int pageSize)
    {
        var query = _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .AsNoTracking();

        var total = await query.CountAsync();

        var posts = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .Select(p => new PostSummaryDto
            {
                Id = p.Id,
                Title = p.Title,
                AuthorName = p.Author.Name,
                CommentCount = p.Comments.Count(),
                LikeCount = p.Likes.Count(),
                Preview = p.Content.Substring(0, 200)
            })
            .ToListAsync();

        return new PagedResult<PostSummaryDto>
        {
            Items = posts,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    // Optimized with split query
    public async Task<PostDetailDto> GetPostDetailAsync(int id)
    {
        var post = await _context.Posts
            .Include(p => p.Comments.Take(10))
                .ThenInclude(c => c.Author)
            .Include(p => p.Likes)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            return null;

        return new PostDetailDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            Comments = post.Comments.Select(c => new CommentDto
            {
                Text = c.Text,
                AuthorName = c.Author.Name
            }).ToList(),
            LikeCount = post.Likes.Count
        };
    }
}
```

## 📝 Summary

**Projection** ve **Split Queries** ile maximum performans elde edebilirsiniz.

**Key Takeaways**:
- ✅ Select() ile sadece gerekli field'ları al
- ✅ AsSplitQuery() ile cartesian explosion'ı önle
- ✅ AsNoTracking() ile memory kullanımını azalt
- ✅ Pagination ile büyük dataset'leri yönet

**Sonraki Adım**: `SOLUTION-ENTERPRISE.md` - GraphQL DataLoader + Caching
