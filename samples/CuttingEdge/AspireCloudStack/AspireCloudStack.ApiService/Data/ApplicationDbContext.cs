using Microsoft.EntityFrameworkCore;
using AspireCloudStack.ApiService.Models;

namespace AspireCloudStack.ApiService.Data;

/// <summary>
/// Application database context for PostgreSQL
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.HasIndex(e => e.Name);
        });

        // WeatherForecast configuration
        modelBuilder.Entity<WeatherForecast>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Summary).HasMaxLength(100);
        });

        // Seed data
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Laptop",
                Description = "High-performance laptop for development",
                Price = 1299.99m,
                Stock = 10,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 2,
                Name = "Mouse",
                Description = "Wireless ergonomic mouse",
                Price = 49.99m,
                Stock = 50,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 3,
                Name = "Keyboard",
                Description = "Mechanical RGB keyboard",
                Price = 149.99m,
                Stock = 25,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
