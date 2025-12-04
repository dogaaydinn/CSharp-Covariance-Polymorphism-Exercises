using MediatR;
using MicroserviceTemplate.Application.DTOs;
using MicroserviceTemplate.Domain.Repositories;

namespace MicroserviceTemplate.Application.Queries;

/// <summary>
/// Handler for GetProductQuery
/// Reads data without modifying state (Query side of CQRS)
/// </summary>
public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductDto?>
{
    private readonly IProductRepository _repository;

    public GetProductQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
            return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Currency = product.Price.Currency,
            Stock = product.Stock,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            IsActive = product.IsActive
        };
    }
}
