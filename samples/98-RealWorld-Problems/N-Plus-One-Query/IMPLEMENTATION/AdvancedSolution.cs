using Microsoft.EntityFrameworkCore;

namespace RealWorldProblems.NPlusOne.Advanced;

/// <summary>
/// Advanced Solution: Projection + Split Queries
/// Selects only needed fields and splits large JOINs
/// </summary>
public class AdvancedBlogService
{
    private readonly AppDbContext _context;

    public AdvancedBlogService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Projection: Select only needed fields
    /// Reduces data transfer by 90%
    /// </summary>
    public async Task<List<PostSummaryDto>> GetPostSummariesAsync()
    {
        var posts = await _context.Posts
            .Select(p => new PostSummaryDto
            {
                Id = p.Id,
                Title = p.Title,
                // Efficient aggregation in SQL
                CommentCount = p.Comments.Count(),
                LikeCount = p.Likes.Count(),
                // Only first 3 comments
                TopComments = p.Comments
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(3)
                    .Select(c => c.Text)
                    .ToList(),
                // Content preview (first 200 chars)
                Preview = p.Content.Substring(0, 200)
            })
            .ToListAsync();

        return posts;
    }

    /// <summary>
    /// Split Query: Prevents cartesian explosion
    /// Uses multiple queries instead of large JOIN
    /// </summary>
    public async Task<List<PostDto>> GetPostsWithSplitQueryAsync()
    {
        var posts = await _context.Posts
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Include(p => p.Author)
            .AsSplitQuery()  // Magic line!
            .AsNoTracking()
            .ToListAsync();

        return posts.Select(p => new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            AuthorName = p.Author?.Name ?? "Unknown",
            CommentCount = p.Comments.Count,
            LikeCount = p.Likes.Count
        }).ToList();
    }

    /// <summary>
    /// Projection with GroupBy
    /// Efficient aggregation queries
    /// </summary>
    public async Task<List<CategoryStats>> GetCategoryStatsAsync()
    {
        var stats = await _context.Posts
            .GroupBy(p => p.CategoryId)
            .Select(g => new CategoryStats
            {
                CategoryId = g.Key,
                PostCount = g.Count(),
                TotalComments = g.Sum(p => p.Comments.Count()),
                TotalLikes = g.Sum(p => p.Likes.Count()),
                AvgCommentsPerPost = g.Average(p => p.Comments.Count()),
                MostPopularPost = g
                    .OrderByDescending(p => p.Likes.Count())
                    .Select(p => p.Title)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return stats;
    }

    /// <summary>
    /// Paginated Projection
    /// Only load data for current page
    /// </summary>
    public async Task<PagedResult<PostSummaryDto>> GetPostsPagedAsync(
        int page,
        int pageSize)
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
                CommentCount = p.Comments.Count(),
                LikeCount = p.Likes.Count(),
                TopComments = p.Comments
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(3)
                    .Select(c => c.Text)
                    .ToList(),
                Preview = p.Content.Substring(0, Math.Min(200, p.Content.Length))
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

    /// <summary>
    /// Filtered Include with Projection
    /// Combines multiple optimization techniques
    /// </summary>
    public async Task<List<PostDetailDto>> GetPostDetailsAsync(int[] postIds)
    {
        var posts = await _context.Posts
            .Where(p => postIds.Contains(p.Id))
            .Select(p => new PostDetailDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                AuthorName = p.Author != null ? p.Author.Name : "Unknown",
                CreatedAt = p.CreatedAt,
                // Only approved comments
                Comments = p.Comments
                    .Where(c => c.IsApproved)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(10)
                    .Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Text = c.Text,
                        AuthorName = c.AuthorName,
                        CreatedAt = c.CreatedAt
                    })
                    .ToList(),
                LikeCount = p.Likes.Count()
            })
            .ToListAsync();

        return posts;
    }

    /// <summary>
    /// Manual Batch Loading
    /// Complete control over queries
    /// </summary>
    public async Task<List<PostWithDataDto>> GetPostsWithBatchLoadingAsync()
    {
        // Step 1: Load posts
        var posts = await _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Take(20)
            .AsNoTracking()
            .ToListAsync();

        var postIds = posts.Select(p => p.Id).ToList();

        // Step 2: Batch load comments
        var comments = await _context.Comments
            .Where(c => postIds.Contains(c.PostId))
            .AsNoTracking()
            .ToListAsync();

        var commentsByPostId = comments
            .GroupBy(c => c.PostId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Step 3: Batch load likes
        var likes = await _context.Likes
            .Where(l => postIds.Contains(l.PostId))
            .AsNoTracking()
            .ToListAsync();

        var likesByPostId = likes
            .GroupBy(l => l.PostId)
            .ToDictionary(g => g.Key, g => g.Count());

        // Step 4: Combine results
        return posts.Select(p => new PostWithDataDto
        {
            Id = p.Id,
            Title = p.Title,
            Content = p.Content,
            CommentCount = commentsByPostId.GetValueOrDefault(p.Id, new List<Comment>()).Count,
            LikeCount = likesByPostId.GetValueOrDefault(p.Id, 0),
            Comments = commentsByPostId
                .GetValueOrDefault(p.Id, new List<Comment>())
                .Take(5)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    AuthorName = c.AuthorName
                })
                .ToList()
        }).ToList();
    }

    /// <summary>
    /// Complex Projection with Subqueries
    /// Shows SQL generation capabilities
    /// </summary>
    public async Task<List<PostAnalyticsDto>> GetPostAnalyticsAsync()
    {
        var analytics = await _context.Posts
            .Select(p => new PostAnalyticsDto
            {
                Id = p.Id,
                Title = p.Title,
                Stats = new PostStats
                {
                    TotalComments = p.Comments.Count(),
                    ApprovedComments = p.Comments.Count(c => c.IsApproved),
                    PendingComments = p.Comments.Count(c => !c.IsApproved),
                    TotalLikes = p.Likes.Count(),
                    UniqueCommentAuthors = p.Comments
                        .Select(c => c.AuthorName)
                        .Distinct()
                        .Count()
                },
                LatestComment = p.Comments
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Text = c.Text,
                        AuthorName = c.AuthorName,
                        CreatedAt = c.CreatedAt
                    })
                    .FirstOrDefault(),
                TopCommenter = p.Comments
                    .GroupBy(c => c.AuthorName)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return analytics;
    }
}

/// <summary>
/// Performance Monitoring
/// </summary>
public class PerformanceMonitor
{
    private readonly AppDbContext _context;

    public PerformanceMonitor(AppDbContext context)
    {
        _context = context;
    }

    public async Task ComparePerformanceAsync()
    {
        Console.WriteLine("=== Performance Comparison ===\n");

        // Test 1: Include (all fields)
        var sw1 = System.Diagnostics.Stopwatch.StartNew();
        var posts1 = await _context.Posts
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .AsNoTracking()
            .ToListAsync();
        sw1.Stop();

        var size1 = System.Text.Json.JsonSerializer.Serialize(posts1).Length;

        // Test 2: Projection (selected fields)
        var sw2 = System.Diagnostics.Stopwatch.StartNew();
        var posts2 = await _context.Posts
            .Select(p => new
            {
                p.Id,
                p.Title,
                CommentCount = p.Comments.Count(),
                LikeCount = p.Likes.Count()
            })
            .ToListAsync();
        sw2.Stop();

        var size2 = System.Text.Json.JsonSerializer.Serialize(posts2).Length;

        // Test 3: Split Query
        var sw3 = System.Diagnostics.Stopwatch.StartNew();
        var posts3 = await _context.Posts
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync();
        sw3.Stop();

        Console.WriteLine($"Include (all fields):    {sw1.ElapsedMilliseconds}ms, {size1 / 1024}KB");
        Console.WriteLine($"Projection (selected):   {sw2.ElapsedMilliseconds}ms, {size2 / 1024}KB");
        Console.WriteLine($"Split Query:             {sw3.ElapsedMilliseconds}ms");
        Console.WriteLine($"\nData reduction: {(size1 - size2) * 100.0 / size1:F1}%");
        Console.WriteLine($"Speed improvement: {(sw1.ElapsedMilliseconds - sw2.ElapsedMilliseconds) * 100.0 / sw1.ElapsedMilliseconds:F1}%");
    }
}

// DTOs
public class PostSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Preview { get; set; } = string.Empty;
    public int CommentCount { get; set; }
    public int LikeCount { get; set; }
    public List<string> TopComments { get; set; } = new();
}

public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public int CommentCount { get; set; }
    public int LikeCount { get; set; }
}

public class PostDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int LikeCount { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
}

public class CommentDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PostWithDataDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int CommentCount { get; set; }
    public int LikeCount { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
}

public class CategoryStats
{
    public int CategoryId { get; set; }
    public int PostCount { get; set; }
    public int TotalComments { get; set; }
    public int TotalLikes { get; set; }
    public double AvgCommentsPerPost { get; set; }
    public string? MostPopularPost { get; set; }
}

public class PostAnalyticsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public PostStats Stats { get; set; } = new();
    public CommentDto? LatestComment { get; set; }
    public string? TopCommenter { get; set; }
}

public class PostStats
{
    public int TotalComments { get; set; }
    public int ApprovedComments { get; set; }
    public int PendingComments { get; set; }
    public int TotalLikes { get; set; }
    public int UniqueCommentAuthors { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(Total / (double)PageSize);
}

// Entity Models (shared from BasicSolution)
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int CategoryId { get; set; }
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
    public bool IsApproved { get; set; }
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
