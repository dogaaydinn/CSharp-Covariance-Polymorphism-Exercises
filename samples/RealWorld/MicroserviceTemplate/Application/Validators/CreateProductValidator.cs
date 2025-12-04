using FluentValidation;
using MicroserviceTemplate.Application.Commands;

namespace MicroserviceTemplate.Application.Validators;

/// <summary>
/// FluentValidation validator for CreateProductCommand
/// Validates input before processing
/// </summary>
public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be positive")
            .LessThan(1000000).WithMessage("Price cannot exceed 1,000,000");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be 3 characters (e.g., USD, EUR)")
            .Must(BeValidCurrency).WithMessage("Invalid currency code");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative");
    }

    private bool BeValidCurrency(string currency)
    {
        var validCurrencies = new[] { "USD", "EUR", "GBP", "JPY", "CAD", "AUD" };
        return validCurrencies.Contains(currency.ToUpperInvariant());
    }
}
