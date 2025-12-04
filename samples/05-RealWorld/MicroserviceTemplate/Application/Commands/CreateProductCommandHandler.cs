using MediatR;
using MicroserviceTemplate.Application.DTOs;
using MicroserviceTemplate.Domain.Entities;
using MicroserviceTemplate.Domain.Repositories;
using MicroserviceTemplate.Domain.ValueObjects;

namespace MicroserviceTemplate.Application.Commands;

/// <summary>
/// Handler for CreateProductCommand
/// Contains business logic orchestration (Application layer)
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;

    public CreateProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Create value object
        var price = new Money(request.Price, request.Currency);

        // Create entity using factory method (enforces business rules)
        var product = Product.Create(
            request.Name,
            request.Description,
            price,
            request.Stock
        );

        // Persist
        await _repository.AddAsync(product, cancellationToken);

        // Map to DTO
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
