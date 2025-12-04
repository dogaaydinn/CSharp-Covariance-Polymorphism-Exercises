namespace StrategyPattern;

/// <summary>
/// Concrete strategy for PayPal payments.
///
/// This is a COMPLETE implementation provided as a reference.
/// </summary>
public class PayPalPaymentStrategy : IPaymentStrategy
{
    private readonly string _email;

    public PayPalPaymentStrategy(string email)
    {
        _email = email;
    }

    public string ProcessPayment(decimal amount, string description)
    {
        // In a real system, this would:
        // 1. Authenticate with PayPal API
        // 2. Initiate payment flow
        // 3. Redirect user to PayPal for authorization
        // 4. Receive callback when payment is complete
        // 5. Return transaction ID

        // Simplified for learning purposes
        return $"PayPal payment of ${amount:F2} processed successfully. " +
               $"Account: {_email}. " +
               $"Description: {description}";
    }
}
