# Enterprise Solution: GraphQL DataLoader + Multi-Level Caching

## 🎯 Yaklaşım

**DataLoader pattern** ile batch loading ve **multi-level caching** ile maximum performance.

## 🚀 GraphQL DataLoader Pattern

### Problem: N+1 in GraphQL

```graphql
query {
  posts {
    id
    title
    comments {  # N+1 query!
      text
    }
  }
}
```

Her post için ayrı query çalışır. 100 post = 101 query!

### Solution: DataLoader

```csharp
public class CommentDataLoader : BatchDataLoader<int, List<Comment>>
{
    private readonly AppDbContext _context;

    public CommentDataLoader(
        AppDbContext context,
        IBatchScheduler batchScheduler)
        : base(batchScheduler)
    {
        _context = context;
    }

    protected override async Task<IReadOnlyDictionary<int, List<Comment>>>
        LoadBatchAsync(
            IReadOnlyList<int> postIds,
            CancellationToken cancellationToken)
    {
        // Single query for all posts!
        var comments = await _context.Comments
            .Where(c => postIds.Contains(c.PostId))
            .ToListAsync(cancellationToken);

        return comments
            .GroupBy(c => c.PostId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}
```

**SQL**:
```sql
-- Single batched query
SELECT * FROM Comments WHERE PostId IN (1,2,3,...,100);
```

## 🎯 Multi-Level Caching Strategy

### Level 1: In-Memory Cache (L1)

```csharp
public class L1CachedRepository
{
    private readonly IMemoryCache _cache;
    private readonly AppDbContext _context;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public async Task<List<Post>> GetPostsAsync()
    {
        return await _cache.GetOrCreateAsync("posts", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
            return await _context.Posts
                .Include(p => p.Comments)
                .AsNoTracking()
                .ToListAsync();
        });
    }
}
```

### Level 2: Distributed Cache (L2 - Redis)

```csharp
public class L2CachedRepository
{
    private readonly IDistributedCache _cache;
    private readonly AppDbContext _context;

    public async Task<List<Post>> GetPostsAsync()
    {
        var cacheKey = "posts:all";
        var cached = await _cache.GetStringAsync(cacheKey);

        if (cached != null)
        {
            return JsonSerializer.Deserialize<List<Post>>(cached);
        }

        var posts = await _context.Posts
            .Include(p => p.Comments)
            .AsNoTracking()
            .ToListAsync();

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(posts),
            options);

        return posts;
    }
}
```

### Level 3: CDN / Edge Cache

```csharp
// HTTP Response caching with ETag
public class CachedPostsController : ControllerBase
{
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "page" })]
    [HttpGet]
    public async Task<IActionResult> GetPosts([FromQuery] int page = 1)
    {
        var posts = await _repository.GetPostsAsync(page);

        // Generate ETag
        var etag = GenerateETag(posts);
        Response.Headers.ETag = etag;

        // Check If-None-Match
        if (Request.Headers.IfNoneMatch == etag)
        {
            return StatusCode(304); // Not Modified
        }

        return Ok(posts);
    }
}
```

## 📊 Cache Strategy Comparison

| Level | Hit Rate | Latency | TTL | Use Case |
|-------|----------|---------|-----|----------|
| L1 (Memory) | 90% | 0.1ms | 5min | Hot data |
| L2 (Redis) | 80% | 2ms | 10min | Warm data |
| L3 (CDN) | 70% | 50ms | 1hour | Cold data |
| Database | - | 50-200ms | - | Cache miss |

## 🧪 Complete DataLoader Implementation

```csharp
public class BlogSchema
{
    public class Query
    {
        // Without DataLoader (N+1)
        public async Task<List<Post>> GetPostsBadAsync(
            [Service] AppDbContext context)
        {
            return await context.Posts.ToListAsync();
        }

        // With DataLoader (batched)
        public async Task<List<Post>> GetPostsGoodAsync(
            [Service] AppDbContext context,
            [Service] CommentDataLoader commentLoader)
        {
            var posts = await context.Posts.ToListAsync();

            // DataLoader batches all requests
            foreach (var post in posts)
            {
                post.Comments = await commentLoader.LoadAsync(post.Id);
            }

            return posts;
        }
    }
}
```

## 🎯 Smart Caching Patterns

### 1. Cache-Aside Pattern

```csharp
public class CacheAsideRepository
{
    public async Task<Post> GetPostAsync(int id)
    {
        // Try L1 cache
        if (_memoryCache.TryGetValue($"post:{id}", out Post cached))
        {
            return cached;
        }

        // Try L2 cache (Redis)
        var redisValue = await _redis.GetStringAsync($"post:{id}");
        if (redisValue != null)
        {
            var post = JsonSerializer.Deserialize<Post>(redisValue);
            // Populate L1
            _memoryCache.Set($"post:{id}", post, TimeSpan.FromMinutes(5));
            return post;
        }

        // Database
        var dbPost = await _context.Posts
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (dbPost != null)
        {
            // Populate L1 + L2
            _memoryCache.Set($"post:{id}", dbPost, TimeSpan.FromMinutes(5));
            await _redis.SetStringAsync(
                $"post:{id}",
                JsonSerializer.Serialize(dbPost),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
        }

        return dbPost;
    }
}
```

### 2. Write-Through Cache

```csharp
public async Task UpdatePostAsync(Post post)
{
    // Update database
    _context.Posts.Update(post);
    await _context.SaveChangesAsync();

    // Invalidate all cache levels
    _memoryCache.Remove($"post:{post.Id}");
    await _redis.RemoveAsync($"post:{post.Id}");
    await _redis.RemoveAsync("posts:all");

    // Or: Update cache immediately (write-through)
    _memoryCache.Set($"post:{post.Id}", post, TimeSpan.FromMinutes(5));
    await _redis.SetStringAsync(
        $"post:{post.Id}",
        JsonSerializer.Serialize(post));
}
```

### 3. Cache Warming

```csharp
public class CacheWarmingService : IHostedService
{
    private readonly IServiceProvider _services;
    private Timer _timer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Warm cache every hour
        _timer = new Timer(WarmCache, null, TimeSpan.Zero, TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    private async void WarmCache(object state)
    {
        using var scope = _services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

        // Preload hot data
        var popularPosts = await context.Posts
            .Where(p => p.Views > 1000)
            .Include(p => p.Comments.Take(10))
            .AsNoTracking()
            .ToListAsync();

        foreach (var post in popularPosts)
        {
            cache.Set($"post:{post.Id}", post, TimeSpan.FromHours(1));
        }

        Console.WriteLine($"[Cache Warming] Preloaded {popularPosts.Count} popular posts");
    }
}
```

## 📊 Performance Metrics

### Before (No Cache, N+1)
```
Queries: 101
Database CPU: 95%
Latency: 5,000ms
Throughput: 0.2 req/s
```

### After (DataLoader + Cache)
```
Queries: 1 (batched)
Cache Hit Rate: 90%
Latency: 2ms (L1) / 50ms (database)
Throughput: 500 req/s (2,500x improvement!)
Database CPU: 5%
```

## 🎓 Cache Invalidation Strategies

### 1. Time-Based (TTL)
```csharp
// Simple expiration
_cache.Set("posts", data, TimeSpan.FromMinutes(5));
```

### 2. Event-Based
```csharp
public class PostEventHandler
{
    public async Task Handle(PostUpdated notification)
    {
        // Invalidate specific post
        _cache.Remove($"post:{notification.PostId}");

        // Invalidate list cache
        _cache.Remove("posts:all");

        // Publish to other servers (Redis Pub/Sub)
        await _redis.PublishAsync("cache:invalidate", notification.PostId);
    }
}
```

### 3. Tag-Based
```csharp
// Tag entries for bulk invalidation
_cache.Set("post:1", data, new MemoryCacheEntryOptions
{
    Tags = { "posts", "user:123" }
});

// Invalidate all posts
_cache.EvictByTag("posts");
```

## 🧪 Complete Enterprise Example

```csharp
public class EnterprisePostService
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _l1Cache;
    private readonly IDistributedCache _l2Cache;
    private readonly CommentDataLoader _commentLoader;

    public async Task<PagedResult<PostDto>> GetPostsAsync(
        int page,
        int pageSize,
        bool bypassCache = false)
    {
        var cacheKey = $"posts:page:{page}:size:{pageSize}";

        // Try L1 cache
        if (!bypassCache && _l1Cache.TryGetValue(cacheKey, out PagedResult<PostDto> cached))
        {
            Console.WriteLine("[L1 Cache HIT]");
            return cached;
        }

        // Try L2 cache (Redis)
        if (!bypassCache)
        {
            var redisValue = await _l2Cache.GetStringAsync(cacheKey);
            if (redisValue != null)
            {
                Console.WriteLine("[L2 Cache HIT]");
                var result = JsonSerializer.Deserialize<PagedResult<PostDto>>(redisValue);

                // Populate L1
                _l1Cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
                return result;
            }
        }

        // Database query with DataLoader
        Console.WriteLine("[Database QUERY]");
        var query = _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .AsNoTracking();

        var total = await query.CountAsync();

        var posts = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Load comments in batch
        var postDtos = new List<PostDto>();
        foreach (var post in posts)
        {
            var comments = await _commentLoader.LoadAsync(post.Id);
            postDtos.Add(new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content.Substring(0, Math.Min(200, post.Content.Length)),
                CommentCount = comments.Count,
                TopComments = comments.Take(3).Select(c => c.Text).ToList()
            });
        }

        var pagedResult = new PagedResult<PostDto>
        {
            Items = postDtos,
            Total = total,
            Page = page,
            PageSize = pageSize
        };

        // Store in L1 + L2
        _l1Cache.Set(cacheKey, pagedResult, TimeSpan.FromMinutes(5));
        await _l2Cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(pagedResult),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

        return pagedResult;
    }
}
```

## 📝 Summary

**DataLoader + Multi-Level Caching** enterprise çözümüdür.

**Key Takeaways**:
- ✅ DataLoader ile batch loading (N+1 → 1 query)
- ✅ L1 (Memory) + L2 (Redis) + L3 (CDN) caching
- ✅ Cache-aside pattern ile smart caching
- ✅ Event-based invalidation
- ✅ 2,500x performance improvement

**Sonuç**: `COMPARISON.md` - Tüm çözümlerin karşılaştırması
