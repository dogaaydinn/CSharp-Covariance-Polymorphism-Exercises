using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Text.Json;

namespace RealWorldProblems.NPlusOne.Enterprise;

/// <summary>
/// DataLoader Pattern (GraphQL-style)
/// Batches and caches data loading
/// </summary>
public class CommentDataLoader
{
    private readonly AppDbContext _context;
    private readonly ConcurrentDictionary<int, Task<List<Comment>>> _cache = new();
    private readonly List<int> _batch = new();
    private readonly object _lock = new();
    private Timer? _timer;

    public CommentDataLoader(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Comment>> LoadAsync(int postId)
    {
        // Check cache first
        if (_cache.TryGetValue(postId, out var cachedTask))
        {
            return await cachedTask;
        }

        // Add to batch
        lock (_lock)
        {
            _batch.Add(postId);

            // Start timer to execute batch (debounce 10ms)
            _timer?.Dispose();
            _timer = new Timer(async _ => await ExecuteBatchAsync(), null, 10, Timeout.Infinite);
        }

        // Return task that will complete when batch executes
        var tcs = new TaskCompletionSource<List<Comment>>();
        _cache[postId] = tcs.Task;
        return await tcs.Task;
    }

    private async Task ExecuteBatchAsync()
    {
        List<int> currentBatch;
        lock (_lock)
        {
            currentBatch = new List<int>(_batch);
            _batch.Clear();
        }

        if (currentBatch.Count == 0)
            return;

        Console.WriteLine($"[DataLoader] Batch loading comments for {currentBatch.Count} posts");

        // Single query for all posts
        var comments = await _context.Comments
            .Where(c => currentBatch.Contains(c.PostId))
            .AsNoTracking()
            .ToListAsync();

        var commentsByPostId = comments
            .GroupBy(c => c.PostId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Complete all pending tasks
        foreach (var postId in currentBatch)
        {
            if (_cache.TryGetValue(postId, out var task) && task is Task<List<Comment>> typedTask)
            {
                var tcs = (TaskCompletionSource<List<Comment>>)task.AsyncState!;
                var postComments = commentsByPostId.GetValueOrDefault(postId, new List<Comment>());
                tcs.SetResult(postComments);
            }
        }
    }
}

/// <summary>
/// Multi-Level Cache: L1 (Memory) + L2 (Redis)
/// </summary>
public class CachedBlogService
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _l1Cache;
    private readonly IDistributedCache _l2Cache;

    public CachedBlogService(
        AppDbContext context,
        IMemoryCache l1Cache,
        IDistributedCache l2Cache)
    {
        _context = context;
        _l1Cache = l1Cache;
        _l2Cache = l2Cache;
    }

    /// <summary>
    /// Get posts with L1 + L2 caching
    /// </summary>
    public async Task<List<PostDto>> GetPostsAsync(bool bypassCache = false)
    {
        var cacheKey = "posts:all";

        // Try L1 cache (Memory)
        if (!bypassCache && _l1Cache.TryGetValue(cacheKey, out List<PostDto>? cachedPosts))
        {
            Console.WriteLine("[L1 Cache HIT] Memory cache");
            return cachedPosts!;
        }

        // Try L2 cache (Redis)
        if (!bypassCache)
        {
            var redisValue = await _l2Cache.GetStringAsync(cacheKey);
            if (redisValue != null)
            {
                Console.WriteLine("[L2 Cache HIT] Redis cache");
                var posts = JsonSerializer.Deserialize<List<PostDto>>(redisValue)!;

                // Populate L1
                _l1Cache.Set(cacheKey, posts, TimeSpan.FromMinutes(5));
                return posts;
            }
        }

        // Database query
        Console.WriteLine("[Database QUERY]");
        var dbPosts = await _context.Posts
            .Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CommentCount = p.Comments.Count(),
                LikeCount = p.Likes.Count()
            })
            .AsNoTracking()
            .ToListAsync();

        // Store in L1 + L2
        _l1Cache.Set(cacheKey, dbPosts, TimeSpan.FromMinutes(5));

        await _l2Cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(dbPosts),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

        return dbPosts;
    }

    /// <summary>
    /// Get single post with caching
    /// </summary>
    public async Task<PostDetailDto?> GetPostAsync(int id, bool bypassCache = false)
    {
        var cacheKey = $"post:{id}";

        // Try L1 cache
        if (!bypassCache && _l1Cache.TryGetValue(cacheKey, out PostDetailDto? cached))
        {
            Console.WriteLine($"[L1 Cache HIT] Post {id}");
            return cached;
        }

        // Try L2 cache
        if (!bypassCache)
        {
            var redisValue = await _l2Cache.GetStringAsync(cacheKey);
            if (redisValue != null)
            {
                Console.WriteLine($"[L2 Cache HIT] Post {id}");
                var post = JsonSerializer.Deserialize<PostDetailDto>(redisValue);

                // Populate L1
                _l1Cache.Set(cacheKey, post, TimeSpan.FromMinutes(5));
                return post;
            }
        }

        // Database query
        Console.WriteLine($"[Database QUERY] Post {id}");
        var dbPost = await _context.Posts
            .Where(p => p.Id == id)
            .Select(p => new PostDetailDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                AuthorName = p.Author != null ? p.Author.Name : "Unknown",
                Comments = p.Comments
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(10)
                    .Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Text = c.Text,
                        AuthorName = c.AuthorName
                    })
                    .ToList(),
                LikeCount = p.Likes.Count()
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (dbPost != null)
        {
            // Store in L1 + L2
            _l1Cache.Set(cacheKey, dbPost, TimeSpan.FromMinutes(5));
            await _l2Cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(dbPost),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
        }

        return dbPost;
    }

    /// <summary>
    /// Invalidate cache when post is updated
    /// </summary>
    public async Task UpdatePostAsync(Post post)
    {
        // Update database
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();

        // Invalidate caches
        _l1Cache.Remove($"post:{post.Id}");
        _l1Cache.Remove("posts:all");

        await _l2Cache.RemoveAsync($"post:{post.Id}");
        await _l2Cache.RemoveAsync("posts:all");

        Console.WriteLine($"[Cache Invalidated] Post {post.Id}");
    }
}

/// <summary>
/// Cache Warming Service
/// Preloads hot data into cache
/// </summary>
public class CacheWarmingService
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly IDistributedCache _distributedCache;

    public CacheWarmingService(
        AppDbContext context,
        IMemoryCache cache,
        IDistributedCache distributedCache)
    {
        _context = context;
        _cache = cache;
        _distributedCache = distributedCache;
    }

    public async Task WarmCacheAsync()
    {
        Console.WriteLine("[Cache Warming] Starting...");

        // Preload popular posts
        var popularPosts = await _context.Posts
            .Where(p => p.Views > 1000)
            .OrderByDescending(p => p.Views)
            .Take(100)
            .Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CommentCount = p.Comments.Count(),
                LikeCount = p.Likes.Count()
            })
            .AsNoTracking()
            .ToListAsync();

        // Store in L1 + L2
        foreach (var post in popularPosts)
        {
            var cacheKey = $"post:{post.Id}";
            _cache.Set(cacheKey, post, TimeSpan.FromHours(1));

            await _distributedCache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(post),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
                });
        }

        Console.WriteLine($"[Cache Warming] Preloaded {popularPosts.Count} popular posts");
    }
}

/// <summary>
/// Complete Enterprise Service
/// Combines DataLoader + Caching + Monitoring
/// </summary>
public class EnterpriseBlogService
{
    private readonly AppDbContext _context;
    private readonly CommentDataLoader _commentLoader;
    private readonly IMemoryCache _cache;
    private readonly IDistributedCache _distributedCache;
    private int _queryCount;

    public EnterpriseBlogService(
        AppDbContext context,
        CommentDataLoader commentLoader,
        IMemoryCache cache,
        IDistributedCache distributedCache)
    {
        _context = context;
        _commentLoader = commentLoader;
        _cache = cache;
        _distributedCache = distributedCache;
    }

    /// <summary>
    /// Get posts with DataLoader batching
    /// </summary>
    public async Task<List<PostWithCommentsDto>> GetPostsWithDataLoaderAsync()
    {
        Interlocked.Exchange(ref _queryCount, 0);

        var posts = await _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Take(20)
            .AsNoTracking()
            .ToListAsync();

        Interlocked.Increment(ref _queryCount);

        // DataLoader batches all comment requests
        var result = new List<PostWithCommentsDto>();
        foreach (var post in posts)
        {
            var comments = await _commentLoader.LoadAsync(post.Id);
            result.Add(new PostWithCommentsDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Comments = comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    AuthorName = c.AuthorName
                }).ToList()
            });
        }

        Console.WriteLine($"[DataLoader] Total queries: {_queryCount}");
        return result;
    }

    /// <summary>
    /// Get posts with full caching strategy
    /// </summary>
    public async Task<PagedResult<PostDto>> GetPostsCachedAsync(
        int page,
        int pageSize,
        bool bypassCache = false)
    {
        var cacheKey = $"posts:page:{page}:size:{pageSize}";

        // Try L1 cache
        if (!bypassCache && _cache.TryGetValue(cacheKey, out PagedResult<PostDto>? cached))
        {
            Console.WriteLine("[L1 Cache HIT]");
            return cached!;
        }

        // Try L2 cache
        if (!bypassCache)
        {
            var redisValue = await _distributedCache.GetStringAsync(cacheKey);
            if (redisValue != null)
            {
                Console.WriteLine("[L2 Cache HIT]");
                var result = JsonSerializer.Deserialize<PagedResult<PostDto>>(redisValue)!;
                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
                return result;
            }
        }

        // Database query
        Console.WriteLine("[Database QUERY]");
        var query = _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .AsNoTracking();

        var total = await query.CountAsync();

        var posts = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content.Substring(0, Math.Min(200, p.Content.Length)),
                CommentCount = p.Comments.Count(),
                LikeCount = p.Likes.Count()
            })
            .ToListAsync();

        var pagedResult = new PagedResult<PostDto>
        {
            Items = posts,
            Total = total,
            Page = page,
            PageSize = pageSize
        };

        // Store in L1 + L2
        _cache.Set(cacheKey, pagedResult, TimeSpan.FromMinutes(5));
        await _distributedCache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(pagedResult),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

        return pagedResult;
    }
}

// DTOs
public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int CommentCount { get; set; }
    public int LikeCount { get; set; }
}

public class PostDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public int LikeCount { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
}

public class PostWithCommentsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<CommentDto> Comments { get; set; } = new();
}

public class CommentDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(Total / (double)PageSize);
}

// Entity Models
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int Views { get; set; }
    public int? AuthorId { get; set; }

    public User? Author { get; set; }
    public List<Comment> Comments { get; set; } = new();
    public List<Like> Likes { get; set; } = new();
}

public class Comment
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Post Post { get; set; } = null!;
}

public class Like
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public Post Post { get; set; } = null!;
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class AppDbContext : DbContext
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>()
            .HasMany(p => p.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId);

        modelBuilder.Entity<Post>()
            .HasMany(p => p.Likes)
            .WithOne(l => l.Post)
            .HasForeignKey(l => l.PostId);
    }
}
