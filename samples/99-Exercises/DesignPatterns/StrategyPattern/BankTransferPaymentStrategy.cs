namespace StrategyPattern;

/// <summary>
/// Concrete strategy for bank transfer payments.
///
/// ⚠️ INCOMPLETE - YOU NEED TO IMPLEMENT THIS! ⚠️
/// </summary>
public class BankTransferPaymentStrategy : IPaymentStrategy
{
    private readonly string _accountNumber;

    public BankTransferPaymentStrategy(string accountNumber)
    {
        _accountNumber = accountNumber;
    }

    public string ProcessPayment(decimal amount, string description)
    {
        // TODO: Implement bank transfer payment processing
        //
        // Requirements:
        // 1. Return a string message confirming the bank transfer
        // 2. Include the amount formatted with 2 decimal places (use $"{amount:F2}")
        // 3. Include the account number
        // 4. Include the description
        //
        // Expected format:
        // "Bank transfer of $299.99 from account US1234567890. Reference: Order with 1 item(s)"
        //
        // Hint: Look at CreditCardPaymentStrategy or PayPalPaymentStrategy for examples

        throw new NotImplementedException("TODO: Implement bank transfer payment processing");
    }
}
