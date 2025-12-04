using AspireCloudStack.ApiService.Models;
using AspireCloudStack.ApiService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AspireCloudStack.ApiService.Controllers;

/// <summary>
/// Products API controller with caching and rate limiting
/// </summary>
[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("fixed")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Get all products (cached for 5 minutes)
    /// </summary>
    [HttpGet]
    [ResponseCache(Duration = 300)]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        _logger.LogInformation("Getting all products");
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    /// <summary>
    /// Get product by ID (cached for 5 minutes)
    /// </summary>
    [HttpGet("{id}")]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "id" })]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        _logger.LogInformation("Getting product {Id}", id);
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound(new { Message = $"Product with ID {id} not found" });
        }

        return Ok(product);
    }

    /// <summary>
    /// Create a new product (requires authentication)
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Product>> Create([FromBody] Product product)
    {
        _logger.LogInformation("Creating product: {Name}", product.Name);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var created = await _productService.CreateAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update a product (requires authentication)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<Product>> Update(int id, [FromBody] Product product)
    {
        _logger.LogInformation("Updating product {Id}", id);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updated = await _productService.UpdateAsync(id, product);
        if (updated == null)
        {
            return NotFound(new { Message = $"Product with ID {id} not found" });
        }

        return Ok(updated);
    }

    /// <summary>
    /// Delete a product (requires authentication, soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting product {Id}", id);

        var deleted = await _productService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(new { Message = $"Product with ID {id} not found" });
        }

        return NoContent();
    }
}
