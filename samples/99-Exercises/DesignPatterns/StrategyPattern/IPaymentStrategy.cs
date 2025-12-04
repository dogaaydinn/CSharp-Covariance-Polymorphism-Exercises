namespace StrategyPattern;

/// <summary>
/// Strategy interface for payment processing.
///
/// The Strategy pattern defines a family of algorithms (payment methods),
/// encapsulates each one, and makes them interchangeable.
/// </summary>
public interface IPaymentStrategy
{
    /// <summary>
    /// Processes a payment transaction.
    /// </summary>
    /// <param name="amount">The amount to charge</param>
    /// <param name="description">Description of the transaction</param>
    /// <returns>A confirmation message with transaction details</returns>
    string ProcessPayment(decimal amount, string description);
}
