using Microsoft.EntityFrameworkCore;

namespace RealWorldProblems.NPlusOne.Basic;

/// <summary>
/// Basic Solution: Eager Loading with Include()
/// Solves N+1 by loading related data in a single query
/// </summary>
public class BasicBlogService
{
    private readonly AppDbContext _context;

    public BasicBlogService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// BAD: N+1 Query Problem
    /// Executes 1 + N queries (lazy loading)
    /// </summary>
    public async Task<List<PostDto>> GetPostsBadAsync()
    {
        // Query 1: Load all posts
        var posts = await _context.Posts.ToListAsync();

        var result = new List<PostDto>();

        // Queries 2-101: Load comments for each post
        foreach (var post in posts)
        {
            result.Add(new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                // This triggers a separate query for EACH post!
                CommentCount = post.Comments.Count(),
                Comments = post.Comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    AuthorName = c.AuthorName
                }).ToList()
            });
        }

        return result;
    }

    /// <summary>
    /// GOOD: Eager Loading with Include()
    /// Executes 1 query with LEFT JOIN
    /// </summary>
    public async Task<List<PostDto>> GetPostsGoodAsync()
    {
        // Single query with LEFT JOIN
        var posts = await _context.Posts
            .Include(p => p.Comments)  // Eager loading!
            .AsNoTracking()            // No change tracking needed
            .ToListAsync();

        return posts.Select(p => new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            Content = p.Content,
            CommentCount = p.Comments.Count,
            Comments = p.Comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Text = c.Text,
                AuthorName = c.AuthorName
            }).ToList()
        }).ToList();
    }

    /// <summary>
    /// Multiple Includes
    /// Loads multiple related entities
    /// </summary>
    public async Task<List<PostWithAllDataDto>> GetPostsWithAllDataAsync()
    {
        var posts = await _context.Posts
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .Include(p => p.Author)
            .AsNoTracking()
            .ToListAsync();

        return posts.Select(p => new PostWithAllDataDto
        {
            Id = p.Id,
            Title = p.Title,
            AuthorName = p.Author?.Name ?? "Unknown",
            CommentCount = p.Comments.Count,
            LikeCount = p.Likes.Count
        }).ToList();
    }

    /// <summary>
    /// Nested Includes with ThenInclude
    /// Loads nested related entities
    /// </summary>
    public async Task<List<PostDto>> GetPostsWithCommentAuthorsAsync()
    {
        var posts = await _context.Posts
            .Include(p => p.Comments)
                .ThenInclude(c => c.Author)  // Nested include
            .AsNoTracking()
            .ToListAsync();

        return posts.Select(p => new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            Content = p.Content,
            Comments = p.Comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Text = c.Text,
                AuthorName = c.Author?.Name ?? "Anonymous"
            }).ToList()
        }).ToList();
    }

    /// <summary>
    /// Filtered Include (EF Core 5+)
    /// Include with filtering
    /// </summary>
    public async Task<List<PostDto>> GetPostsWithApprovedCommentsAsync()
    {
        var posts = await _context.Posts
            .Include(p => p.Comments.Where(c => c.IsApproved))
            .AsNoTracking()
            .ToListAsync();

        return posts.Select(p => new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            Content = p.Content,
            Comments = p.Comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Text = c.Text,
                AuthorName = c.AuthorName
            }).ToList()
        }).ToList();
    }

    /// <summary>
    /// Include with Pagination
    /// Only include data for the current page
    /// </summary>
    public async Task<PagedResult<PostDto>> GetPostsPagedAsync(int page, int pageSize)
    {
        var query = _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .AsNoTracking();

        var total = await query.CountAsync();

        var posts = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .Include(p => p.Comments)
            .ToListAsync();

        return new PagedResult<PostDto>
        {
            Items = posts.Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CommentCount = p.Comments.Count,
                Comments = p.Comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    AuthorName = c.AuthorName
                }).ToList()
            }).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Conditional Include
    /// Include based on parameter
    /// </summary>
    public async Task<List<PostDto>> GetPostsConditionalAsync(bool includeComments)
    {
        var query = _context.Posts.AsQueryable();

        if (includeComments)
        {
            query = query.Include(p => p.Comments);
        }

        var posts = await query
            .AsNoTracking()
            .ToListAsync();

        return posts.Select(p => new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            Content = p.Content,
            CommentCount = includeComments ? p.Comments.Count : 0,
            Comments = includeComments
                ? p.Comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    AuthorName = c.AuthorName
                }).ToList()
                : new List<CommentDto>()
        }).ToList();
    }
}

/// <summary>
/// Performance Comparison Demo
/// </summary>
public class PerformanceDemo
{
    private readonly AppDbContext _context;

    public PerformanceDemo(AppDbContext context)
    {
        _context = context;
    }

    public async Task RunComparisonAsync()
    {
        Console.WriteLine("=== N+1 Query Performance Test ===\n");

        // Test 1: Without Include (N+1)
        var sw1 = System.Diagnostics.Stopwatch.StartNew();
        var posts1 = await _context.Posts.ToListAsync();
        foreach (var post in posts1)
        {
            var count = post.Comments.Count(); // Lazy load
        }
        sw1.Stop();

        // Test 2: With Include (Single Query)
        var sw2 = System.Diagnostics.Stopwatch.StartNew();
        var posts2 = await _context.Posts
            .Include(p => p.Comments)
            .ToListAsync();
        foreach (var post in posts2)
        {
            var count = post.Comments.Count;
        }
        sw2.Stop();

        // Test 3: With Include + AsNoTracking
        var sw3 = System.Diagnostics.Stopwatch.StartNew();
        var posts3 = await _context.Posts
            .Include(p => p.Comments)
            .AsNoTracking()
            .ToListAsync();
        sw3.Stop();

        Console.WriteLine($"Without Include (N+1):       {sw1.ElapsedMilliseconds}ms");
        Console.WriteLine($"With Include:                {sw2.ElapsedMilliseconds}ms");
        Console.WriteLine($"With Include + AsNoTracking: {sw3.ElapsedMilliseconds}ms");
        Console.WriteLine($"\nImprovement: {(sw1.ElapsedMilliseconds - sw3.ElapsedMilliseconds) * 100.0 / sw1.ElapsedMilliseconds:F1}%");
    }
}

// Entity Models
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
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
    public int? AuthorId { get; set; }

    public Post Post { get; set; } = null!;
    public User? Author { get; set; }
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
    public List<Post> Posts { get; set; } = new();
}

// DTOs
public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int CommentCount { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
}

public class CommentDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
}

public class PostWithAllDataDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public int CommentCount { get; set; }
    public int LikeCount { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(Total / (double)PageSize);
}

// DbContext
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

        modelBuilder.Entity<Post>()
            .HasOne(p => p.Author)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.AuthorId);
    }
}
