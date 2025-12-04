using Microsoft.EntityFrameworkCore;
using VideoService.API.Models;

namespace VideoService.API.Data;

public class VideoDbContext : DbContext
{
    public VideoDbContext(DbContextOptions<VideoDbContext> options) : base(options)
    {
    }

    public DbSet<Video> Videos => Set<Video>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Url).IsRequired().HasMaxLength(500);
        });
    }
}
