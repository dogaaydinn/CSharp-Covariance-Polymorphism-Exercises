using MediatR;
using MicroserviceTemplate.Application.DTOs;

namespace MicroserviceTemplate.Application.Commands;

/// <summary>
/// CQRS Command for creating a product
/// Implements MediatR IRequest pattern
/// </summary>
public record CreateProductCommand : IRequest<ProductDto>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Currency { get; init; } = "USD";
    public int Stock { get; init; }
}
