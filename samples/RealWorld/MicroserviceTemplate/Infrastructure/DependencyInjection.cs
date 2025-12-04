using Microsoft.Extensions.DependencyInjection;
using MicroserviceTemplate.Domain.Repositories;
using MicroserviceTemplate.Infrastructure.Repositories;

namespace MicroserviceTemplate.Infrastructure;

/// <summary>
/// Infrastructure layer dependency injection configuration
/// Registers infrastructure services (repositories, external services, etc.)
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register repositories
        services.AddSingleton<IProductRepository, InMemoryProductRepository>();

        // In real application, you would also register:
        // - DbContext
        // - External API clients
        // - File storage services
        // - Caching services
        // - Message queue publishers

        return services;
    }
}
