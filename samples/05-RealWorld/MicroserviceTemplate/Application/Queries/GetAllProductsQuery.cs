using MediatR;
using MicroserviceTemplate.Application.DTOs;

namespace MicroserviceTemplate.Application.Queries;

/// <summary>
/// CQRS Query for getting all products
/// </summary>
public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;
