namespace StrategyPattern;

/// <summary>
/// Represents an item in the shopping cart.
/// This is a simple data class (provided for you).
/// </summary>
public class CartItem
{
    public string Name { get; }
    public decimal Price { get; }

    public CartItem(string name, decimal price)
    {
        Name = name;
        Price = price;
    }
}
