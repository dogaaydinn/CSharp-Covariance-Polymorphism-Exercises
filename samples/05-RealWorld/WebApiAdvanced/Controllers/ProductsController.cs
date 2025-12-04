using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApiAdvanced.Models;
using WebApiAdvanced.Services;

namespace WebApiAdvanced.Controllers;

/// <summary>
/// Products API controller
/// Demonstrates RESTful API, JWT auth, rate limiting, and caching
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require JWT authentication
[EnableRateLimiting("fixed")] // Apply rate limiting policy
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Get all products
    /// Uses memory caching for better performance
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<Product>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Getting all products");

        var products = await _productService.GetAllProductsAsync();
        return Ok(ApiResponse<IEnumerable<Product>>.SuccessResult(products));
    }

    /// <summary>
    /// Get product by ID
    /// Uses memory caching for better performance
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        _logger.LogInformation("Getting product {ProductId}", id);

        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found", id);
            return NotFound(ApiResponse<Product>.ErrorResult("Product not found"));
        }

        return Ok(ApiResponse<Product>.SuccessResult(product));
    }

    /// <summary>
    /// Create new product
    /// Requires Admin role
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")] // Only admins can create products
    [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        _logger.LogInformation("Creating product: {ProductName}", request.Name);

        // Validation
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(ApiResponse<Product>.ErrorResult("Product name is required"));
        }

        if (request.Price <= 0)
        {
            return BadRequest(ApiResponse<Product>.ErrorResult("Price must be positive"));
        }

        var product = await _productService.CreateProductAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            ApiResponse<Product>.SuccessResult(product, "Product created successfully")
        );
    }

    /// <summary>
    /// Update existing product
    /// Requires Admin role
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")] // Only admins can update products
    [ProducesResponseType(typeof(ApiResponse<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        _logger.LogInformation("Updating product {ProductId}", id);

        var product = await _productService.UpdateProductAsync(id, request);

        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found for update", id);
            return NotFound(ApiResponse<Product>.ErrorResult("Product not found"));
        }

        return Ok(ApiResponse<Product>.SuccessResult(product, "Product updated successfully"));
    }

    /// <summary>
    /// Delete product
    /// Requires Admin role
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // Only admins can delete products
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Deleting product {ProductId}", id);

        var deleted = await _productService.DeleteProductAsync(id);

        if (!deleted)
        {
            _logger.LogWarning("Product {ProductId} not found for deletion", id);
            return NotFound(ApiResponse<object>.ErrorResult("Product not found"));
        }

        return NoContent();
    }
}
