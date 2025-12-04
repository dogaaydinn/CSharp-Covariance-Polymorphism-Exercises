namespace SOLIDPrinciples.OpenClosed;

// ❌ BAD: Violates OCP - must modify class for new discount types
public class BadDiscountCalculator
{
    public decimal Calculate(decimal amount, string customerType)
    {
        return customerType switch
        {
            "Student" => amount * 0.8m,  // 20% off
            "Senior" => amount * 0.7m,   // 30% off
            // Need to modify this class to add new types!
            _ => amount
        };
    }
}

// ✅ GOOD: Open for extension, closed for modification
public interface IDiscountStrategy
{
    decimal Calculate(decimal amount);
}

public class StudentDiscount : IDiscountStrategy
{
    public decimal Calculate(decimal amount) => amount * 0.8m; // 20% off
}

public class SeniorDiscount : IDiscountStrategy
{
    public decimal Calculate(decimal amount) => amount * 0.7m; // 30% off
}

public class VipDiscount : IDiscountStrategy
{
    public decimal Calculate(decimal amount) => amount * 0.5m; // 50% off
}

public class NoDiscount : IDiscountStrategy
{
    public decimal Calculate(decimal amount) => amount;
}

// WHY?
// - Add new discount types without modifying existing code
// - Each discount strategy is testable independently
// - Easy to add BlackFridayDiscount, EarlyBirdDiscount, etc.
// - Follows Strategy pattern
