using Content.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace Content.API.Infrastructure;

public class VideoDbContext : DbContext
{
    public VideoDbContext(DbContextOptions<VideoDbContext> options) : base(options)
    {
    }

    public DbSet<Video> Videos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Url).IsRequired();
            entity.Property(e => e.Status).IsRequired();
        });
    }
}
