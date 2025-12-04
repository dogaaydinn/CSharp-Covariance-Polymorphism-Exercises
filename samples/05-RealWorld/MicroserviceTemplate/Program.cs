using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentValidation;
using MicroserviceTemplate.Application.Commands;
using MicroserviceTemplate.Application.Queries;
using MicroserviceTemplate.Application.Validators;
using MicroserviceTemplate.Infrastructure;

namespace MicroserviceTemplate;

/// <summary>
/// Microservice Template - Clean Architecture Demonstration
///
/// Architecture Layers:
/// ┌─────────────────────────────────────────────────┐
/// │  API/Presentation Layer (Program.cs)           │  ← You are here
/// ├─────────────────────────────────────────────────┤
/// │  Application Layer (CQRS, MediatR, DTOs)       │  ← Business logic orchestration
/// ├─────────────────────────────────────────────────┤
/// │  Domain Layer (Entities, Value Objects, Events) │  ← Core business rules
/// ├─────────────────────────────────────────────────┤
/// │  Infrastructure Layer (Repositories, External)  │  ← Data access, external services
/// └─────────────────────────────────────────────────┘
///
/// Key Patterns Demonstrated:
/// - Clean Architecture (Uncle Bob)
/// - CQRS (Command Query Responsibility Segregation)
/// - MediatR (Mediator pattern)
/// - Repository Pattern
/// - Dependency Inversion Principle
/// - Domain-Driven Design (DDD)
/// - FluentValidation
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        PrintHeader();

        // Build dependency injection container
        var host = CreateHostBuilder(args).Build();

        // Get MediatR from DI container
        var mediator = host.Services.GetRequiredService<IMediator>();

        // Run demonstrations
        await RunCleanArchitectureDemo(mediator);

        PrintFooter();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Register Infrastructure layer (repositories)
                services.AddInfrastructure();

                // Register MediatR (scans assembly for handlers)
                services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssemblyContaining<Program>();
                });

                // Register FluentValidation validators
                services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();

                // In real application, you would also add:
                // - DbContext
                // - Authentication/Authorization
                // - Logging (Serilog)
                // - Health checks
                // - API versioning
                // - Swagger/OpenAPI
            });

    static async Task RunCleanArchitectureDemo(IMediator mediator)
    {
        PrintSection("1. CQRS Pattern - Creating Products (Command Side)");

        // Command 1: Create laptop
        var createLaptopCommand = new CreateProductCommand
        {
            Name = "Gaming Laptop",
            Description = "High-performance laptop for gaming",
            Price = 1299.99m,
            Currency = "USD",
            Stock = 10
        };

        var laptop = await mediator.Send(createLaptopCommand);
        PrintProduct("Created", laptop);

        // Command 2: Create keyboard
        var createKeyboardCommand = new CreateProductCommand
        {
            Name = "Mechanical Keyboard",
            Description = "RGB backlit mechanical keyboard",
            Price = 149.99m,
            Currency = "USD",
            Stock = 25
        };

        var keyboard = await mediator.Send(createKeyboardCommand);
        PrintProduct("Created", keyboard);

        // Command 3: Create mouse
        var createMouseCommand = new CreateProductCommand
        {
            Name = "Wireless Mouse",
            Description = "Ergonomic wireless mouse",
            Price = 49.99m,
            Currency = "USD",
            Stock = 50
        };

        var mouse = await mediator.Send(createMouseCommand);
        PrintProduct("Created", mouse);

        Console.WriteLine();
        PrintInfo("✓ All products created through CQRS Command handlers");
        PrintInfo("✓ Business rules enforced in Domain layer");
        PrintInfo("✓ Data persisted through Repository pattern");

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
        Console.WriteLine();

        // Query Side
        PrintSection("2. CQRS Pattern - Reading Products (Query Side)");

        // Query 1: Get single product
        var getProductQuery = new GetProductQuery(laptop.Id);
        var retrievedLaptop = await mediator.Send(getProductQuery);

        if (retrievedLaptop != null)
        {
            PrintProduct("Retrieved", retrievedLaptop);
        }

        Console.WriteLine();

        // Query 2: Get all products
        var getAllQuery = new GetAllProductsQuery();
        var allProducts = await mediator.Send(getAllQuery);

        Console.WriteLine($"Retrieved ALL products ({allProducts.Count()} total):");
        Console.WriteLine();

        foreach (var product in allProducts)
        {
            Console.WriteLine($"  • {product.Name} - ${product.Price:F2} ({product.Stock} in stock)");
        }

        Console.WriteLine();
        PrintInfo("✓ Queries handled separately from Commands (CQRS)");
        PrintInfo("✓ Read operations optimized independently");

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
        Console.WriteLine();

        // Validation Demo
        PrintSection("3. FluentValidation - Input Validation");

        try
        {
            // Invalid command - negative price
            var invalidCommand = new CreateProductCommand
            {
                Name = "Invalid Product",
                Description = "This should fail validation",
                Price = -100m,  // ❌ Negative price
                Currency = "USD",
                Stock = 10
            };

            var validator = new CreateProductValidator();
            var validationResult = await validator.ValidateAsync(invalidCommand);

            if (!validationResult.IsValid)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Validation Failed:");
                Console.ResetColor();

                foreach (var error in validationResult.Errors)
                {
                    Console.WriteLine($"  ✗ {error.PropertyName}: {error.ErrorMessage}");
                }
            }

            Console.WriteLine();
            PrintInfo("✓ Input validated before reaching business logic");
            PrintInfo("✓ Clear, descriptive validation messages");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
        Console.WriteLine();

        // Architecture Summary
        PrintSection("4. Clean Architecture Benefits");

        Console.WriteLine("Dependency Flow (Dependency Inversion Principle):");
        Console.WriteLine();
        Console.WriteLine("  ┌─────────────────────────┐");
        Console.WriteLine("  │   Presentation Layer    │  ← Depends on Application");
        Console.WriteLine("  └───────────┬─────────────┘");
        Console.WriteLine("              │");
        Console.WriteLine("              ▼");
        Console.WriteLine("  ┌─────────────────────────┐");
        Console.WriteLine("  │   Application Layer     │  ← Depends on Domain");
        Console.WriteLine("  └───────────┬─────────────┘");
        Console.WriteLine("              │");
        Console.WriteLine("              ▼");
        Console.WriteLine("  ┌─────────────────────────┐");
        Console.WriteLine("  │     Domain Layer        │  ← No dependencies!");
        Console.WriteLine("  └───────────▲─────────────┘");
        Console.WriteLine("              │");
        Console.WriteLine("              │ implements");
        Console.WriteLine("  ┌───────────┴─────────────┐");
        Console.WriteLine("  │  Infrastructure Layer   │  ← Depends on Domain (interfaces)");
        Console.WriteLine("  └─────────────────────────┘");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Key Benefits:");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("  ✓ Testability");
        Console.WriteLine("    - Mock repositories easily");
        Console.WriteLine("    - Test business logic in isolation");
        Console.WriteLine();
        Console.WriteLine("  ✓ Maintainability");
        Console.WriteLine("    - Clear separation of concerns");
        Console.WriteLine("    - Changes isolated to specific layers");
        Console.WriteLine();
        Console.WriteLine("  ✓ Flexibility");
        Console.WriteLine("    - Swap infrastructure (e.g., different database)");
        Console.WriteLine("    - Change UI without touching business logic");
        Console.WriteLine();
        Console.WriteLine("  ✓ Scalability");
        Console.WriteLine("    - CQRS enables read/write optimization");
        Console.WriteLine("    - Handlers can be scaled independently");
        Console.WriteLine();
        Console.WriteLine("  ✓ Business Logic Protection");
        Console.WriteLine("    - Domain layer has no external dependencies");
        Console.WriteLine("    - Core business rules always consistent");

        Console.WriteLine("\nPress any key to finish...");
        Console.ReadKey(true);
    }

    static void PrintHeader()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║     MICROSERVICE TEMPLATE - CLEAN ARCHITECTURE DEMO               ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("This demo shows a complete Clean Architecture implementation with:");
        Console.WriteLine("  • Domain-Driven Design (DDD)");
        Console.WriteLine("  • CQRS Pattern with MediatR");
        Console.WriteLine("  • Repository Pattern");
        Console.WriteLine("  • Dependency Inversion Principle");
        Console.WriteLine("  • FluentValidation");
        Console.WriteLine();
        Console.WriteLine("Press any key to start...");
        Console.ReadKey(true);
        Console.Clear();
        Console.WriteLine();
    }

    static void PrintSection(string title)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("═══════════════════════════════════════════════════════════════════");
        Console.WriteLine($" {title}");
        Console.WriteLine("═══════════════════════════════════════════════════════════════════");
        Console.ResetColor();
        Console.WriteLine();
    }

    static void PrintProduct(string action, Application.DTOs.ProductDto product)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{action} Product:");
        Console.ResetColor();
        Console.WriteLine($"  ID:          {product.Id}");
        Console.WriteLine($"  Name:        {product.Name}");
        Console.WriteLine($"  Description: {product.Description}");
        Console.WriteLine($"  Price:       {product.Price:F2} {product.Currency}");
        Console.WriteLine($"  Stock:       {product.Stock} units");
        Console.WriteLine($"  Created:     {product.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"  Active:      {(product.IsActive ? "Yes" : "No")}");
    }

    static void PrintInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  {message}");
        Console.ResetColor();
    }

    static void PrintFooter()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                     DEMO COMPLETED                                 ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("For more details, see README.md in this directory.");
        Console.WriteLine();
    }
}
