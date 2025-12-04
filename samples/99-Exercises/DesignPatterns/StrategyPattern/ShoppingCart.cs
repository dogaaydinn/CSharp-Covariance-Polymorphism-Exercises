namespace StrategyPattern;

/// <summary>
/// Shopping cart that uses a payment strategy for checkout.
///
/// ⚠️ INCOMPLETE - YOU NEED TO IMPLEMENT THIS! ⚠️
///
/// This is the "Context" class in the Strategy pattern.
/// It delegates payment processing to a strategy object.
/// </summary>
public class ShoppingCart
{
    private readonly List<CartItem> _items = new();

    // TODO: Add a private readonly field to store the payment strategy
    // Hint: The type should be IPaymentStrategy

    // TODO: Add a constructor that accepts IPaymentStrategy as a parameter
    // Hint: public ShoppingCart(IPaymentStrategy paymentStrategy)
    // Hint: Store the paymentStrategy in the field you created above
    public ShoppingCart()
    {
        throw new NotImplementedException("TODO: Implement constructor with IPaymentStrategy parameter");
    }

    /// <summary>
    /// Adds an item to the shopping cart.
    /// This method is COMPLETE - no changes needed.
    /// </summary>
    public void AddItem(CartItem item)
    {
        _items.Add(item);
    }

    /// <summary>
    /// Processes payment for all items in the cart using the configured strategy.
    ///
    /// TODO: Implement this method!
    ///
    /// Requirements:
    /// 1. Calculate the total price of all items in the cart
    /// 2. Create a description like "Order with X item(s)"
    /// 3. Call the payment strategy's ProcessPayment method
    /// 4. Return the result
    /// </summary>
    public string ProcessPayment()
    {
        // TODO: Calculate total from all items
        // Hint: Use LINQ Sum method: _items.Sum(item => item.Price)
        decimal total = 0; // Replace this with actual calculation

        // TODO: Create a description
        // Hint: $"Order with {_items.Count} item(s)"
        string description = string.Empty; // Replace this

        // TODO: Call the payment strategy's ProcessPayment method
        // Hint: _paymentStrategy.ProcessPayment(total, description)
        // Hint: Return the result from the strategy

        throw new NotImplementedException("TODO: Implement payment processing logic");
    }

    /// <summary>
    /// Gets the current total of all items in the cart.
    /// This method is COMPLETE - no changes needed.
    /// </summary>
    public decimal GetTotal()
    {
        return _items.Sum(item => item.Price);
    }

    /// <summary>
    /// Gets the number of items in the cart.
    /// This method is COMPLETE - no changes needed.
    /// </summary>
    public int GetItemCount()
    {
        return _items.Count;
    }
}
