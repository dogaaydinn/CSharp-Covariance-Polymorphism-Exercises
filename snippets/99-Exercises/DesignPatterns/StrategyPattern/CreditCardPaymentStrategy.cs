namespace StrategyPattern;

/// <summary>
/// Concrete strategy for credit card payments.
///
/// This is a COMPLETE implementation provided as a reference.
/// Study this to understand how to implement the Strategy pattern.
/// </summary>
public class CreditCardPaymentStrategy : IPaymentStrategy
{
    private readonly string _cardNumber;
    private readonly string _expiryDate;

    public CreditCardPaymentStrategy(string cardNumber, string expiryDate)
    {
        _cardNumber = cardNumber;
        _expiryDate = expiryDate;
    }

    public string ProcessPayment(decimal amount, string description)
    {
        // In a real system, this would:
        // 1. Validate card number (Luhn algorithm)
        // 2. Check expiry date
        // 3. Connect to payment gateway (Stripe, Square, etc.)
        // 4. Process the transaction
        // 5. Return transaction ID

        // Simplified for learning purposes
        return $"Credit card payment of ${amount:F2} processed successfully. " +
               $"Card: {MaskCardNumber(_cardNumber)}, Expiry: {_expiryDate}. " +
               $"Description: {description}";
    }

    private string MaskCardNumber(string cardNumber)
    {
        // Mask all but last 4 digits: 1234-5678-9012-3456 → ****-****-****-3456
        if (cardNumber.Length <= 4)
            return cardNumber;

        return "****-****-****-" + cardNumber.Substring(cardNumber.Length - 4);
    }
}
