namespace GroupingAggregation;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime AddedDate { get; set; }
    public bool IsActive { get; set; }
    public string Supplier { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"[{Id}] {Name} - {Category} - ${Price} (Stock: {StockQuantity})";
    }
}
