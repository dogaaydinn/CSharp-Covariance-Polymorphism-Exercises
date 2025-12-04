namespace BasicQueries;

/// <summary>
/// Represents a product in an e-commerce system.
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime AddedDate { get; set; }
    public bool IsActive { get; set; }

    public override string ToString()
    {
        return $"{Name} ({Category}) - ${Price:F2}";
    }
}
