using FluentValidation;
using FluentValidation.Results;

namespace AdvancedCsharpConcepts.Advanced.Resilience;

/// <summary>
/// FluentValidation - Enterprise-grade validation framework.
/// NVIDIA/Silicon Valley best practice: Expressive, testable validation rules.
/// </summary>
public static class FluentValidationExamples
{
    /// <summary>
    /// Domain models for validation examples.
    /// </summary>
    public record Customer(
        string FirstName,
        string LastName,
        string Email,
        int Age,
        string? PhoneNumber,
        Address Address);

    public record Address(
        string Street,
        string City,
        string ZipCode,
        string Country);

    public record Order(
        string OrderId,
        Customer Customer,
        List<OrderItem> Items,
        decimal TotalAmount,
        DateTime OrderDate);

    public record OrderItem(
        string ProductName,
        int Quantity,
        decimal UnitPrice);

    /// <summary>
    /// Customer Validator - Demonstrates basic validation rules.
    /// </summary>
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            // Simple rules
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .Length(2, 50).WithMessage("First name must be between 2 and 50 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("First name can only contain letters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .Length(2, 50);

            // Email validation
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("Please provide a valid email address");

            // Age validation with custom logic
            RuleFor(x => x.Age)
                .InclusiveBetween(18, 120).WithMessage("Age must be between 18 and 120")
                .Must(BeAValidAge).WithMessage("Age seems invalid");

            // Optional phone number validation
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Phone number format is invalid");

            // Nested object validation
            RuleFor(x => x.Address)
                .SetValidator(new AddressValidator());
        }

        private static bool BeAValidAge(int age)
        {
            return age > 0 && age < 150;
        }
    }

    /// <summary>
    /// Address Validator - Nested validation example.
    /// </summary>
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Street is required")
                .MinimumLength(3);

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required");

            RuleFor(x => x.ZipCode)
                .NotEmpty()
                .Matches(@"^\d{5}(-\d{4})?$")
                .WithMessage("Zip code must be in format 12345 or 12345-6789");

            RuleFor(x => x.Country)
                .NotEmpty()
                .Must(BeAValidCountry).WithMessage("Invalid country");
        }

        private static bool BeAValidCountry(string country)
        {
            var validCountries = new[] { "USA", "Canada", "UK", "Germany", "France" };
            return validCountries.Contains(country, StringComparer.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Order Validator - Complex validation with collection rules.
    /// </summary>
    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty()
                .Matches(@"^ORD-\d{6}$").WithMessage("Order ID must be in format ORD-123456");

            RuleFor(x => x.Customer)
                .NotNull()
                .SetValidator(new CustomerValidator());

            // Collection validation
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must have at least one item")
                .Must(HaveValidQuantities).WithMessage("All items must have quantity > 0");

            // Individual item validation
            RuleForEach(x => x.Items)
                .SetValidator(new OrderItemValidator());

            // Cross-property validation
            RuleFor(x => x.TotalAmount)
                .GreaterThan(0)
                .Must((order, total) => total == CalculateTotal(order.Items))
                .WithMessage("Total amount doesn't match sum of items");

            RuleFor(x => x.OrderDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Order date cannot be in the future")
                .GreaterThan(DateTime.Now.AddYears(-1)).WithMessage("Order date cannot be more than 1 year old");
        }

        private static bool HaveValidQuantities(List<OrderItem> items)
        {
            return items.All(i => i.Quantity > 0);
        }

        private static decimal CalculateTotal(List<OrderItem> items)
        {
            return items.Sum(i => i.Quantity * i.UnitPrice);
        }
    }

    /// <summary>
    /// Order Item Validator.
    /// </summary>
    public class OrderItemValidator : AbstractValidator<OrderItem>
    {
        public OrderItemValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be positive")
                .LessThanOrEqualTo(1000).WithMessage("Quantity cannot exceed 1000");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Unit price must be positive")
                .LessThanOrEqualTo(999999.99m).WithMessage("Price is too high");
        }
    }

    /// <summary>
    /// Demonstrates FluentValidation usage patterns.
    /// </summary>
    public static async Task RunExamples()
    {
        Console.WriteLine("=== FluentValidation Examples ===\n");

        // Example 1: Valid customer
        Console.WriteLine("1. Valid Customer:");
        var validCustomer = new Customer(
            "John",
            "Doe",
            "john.doe@example.com",
            30,
            "+1234567890",
            new Address("123 Main St", "New York", "10001", "USA")
        );

        var customerValidator = new CustomerValidator();
        var result1 = await customerValidator.ValidateAsync(validCustomer);
        PrintValidationResult(result1);

        // Example 2: Invalid customer (multiple errors)
        Console.WriteLine("\n2. Invalid Customer (Multiple Validation Errors):");
        var invalidCustomer = new Customer(
            "J", // Too short
            "",  // Empty
            "not-an-email", // Invalid email
            15, // Too young
            "invalid-phone",
            new Address("", "NYC", "invalid-zip", "InvalidCountry")
        );

        var result2 = await customerValidator.ValidateAsync(invalidCustomer);
        PrintValidationResult(result2);

        // Example 3: Valid order
        Console.WriteLine("\n3. Valid Order:");
        var validOrder = new Order(
            "ORD-123456",
            validCustomer,
            new List<OrderItem>
            {
                new("Product A", 2, 10.99m),
                new("Product B", 1, 25.50m)
            },
            47.48m,
            DateTime.Now.AddHours(-1)
        );

        var orderValidator = new OrderValidator();
        var result3 = await orderValidator.ValidateAsync(validOrder);
        PrintValidationResult(result3);

        // Example 4: Invalid order (wrong total)
        Console.WriteLine("\n4. Invalid Order (Wrong Total Amount):");
        var invalidOrder = new Order(
            "INVALID-ID",
            validCustomer,
            new List<OrderItem>
            {
                new("Product A", 2, 10.99m)
            },
            100.00m, // Wrong total
            DateTime.Now.AddDays(1) // Future date
        );

        var result4 = await orderValidator.ValidateAsync(invalidOrder);
        PrintValidationResult(result4);

        // Example 5: Partial validation (specific properties only)
        Console.WriteLine("\n5. Partial Validation (Email only):");
        var customer = new Customer("Jane", "Smith", "invalid-email", 25, null,
            new Address("456 Elm St", "Boston", "02101", "USA"));

        var result5 = await customerValidator.ValidateAsync(customer, options =>
        {
            options.IncludeProperties(x => x.Email);
        });
        PrintValidationResult(result5);

        Console.WriteLine("\n✓ FluentValidation patterns demonstrated!");
    }

    private static void PrintValidationResult(ValidationResult result)
    {
        if (result.IsValid)
        {
            Console.WriteLine("   ✓ Validation passed");
        }
        else
        {
            Console.WriteLine($"   ✗ Validation failed ({result.Errors.Count} errors):");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"      - {error.PropertyName}: {error.ErrorMessage}");
            }
        }
    }
}
