using MediatR;
using MicroserviceTemplate.Application.DTOs;

namespace MicroserviceTemplate.Application.Queries;

/// <summary>
/// CQRS Query for getting a product by ID
/// Implements MediatR IRequest pattern
/// </summary>
public record GetProductQuery : IRequest<ProductDto?>
{
    public Guid Id { get; init; }

    public GetProductQuery(Guid id)
    {
        Id = id;
    }
}
