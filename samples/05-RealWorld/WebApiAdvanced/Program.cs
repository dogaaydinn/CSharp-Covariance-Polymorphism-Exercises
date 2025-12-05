using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 5;
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseRateLimiter();

// Minimal API endpoints
app.MapGet("/products", () => new[]
{
    new Product(1, "Laptop", 999.99m),
    new Product(2, "Mouse", 29.99m),
    new Product(3, "Keyboard", 79.99m)
}).RequireRateLimiting("fixed");

app.MapGet("/products/{id}", (int id) =>
    id == 1 ? Results.Ok(new Product(id, "Laptop", 999.99m)) : Results.NotFound());

app.MapPost("/products", (Product product) => Results.Created($"/products/{product.Id}", product));

app.Run();

record Product(int Id, string Name, decimal Price);
