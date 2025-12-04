namespace PropertyExamples;

public class Product
{
    private decimal _price;
    private int _stock;

    // Auto-property (C# 3.0+)
    public string Name { get; set; } = string.Empty;

    // Auto-property with init (C# 9.0+)
    public string Category { get; init; } = "General";

    // Property with validation
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
                throw new ArgumentException("Fiyat negatif olamaz!");
            _price = value;
        }
    }

    // Property with validation and side effects
    public int Stock
    {
        get => _stock;
        set
        {
            if (value < 0)
                throw new ArgumentException("Stok negatif olamaz!");

            if (value == 0 && _stock > 0)
                Console.WriteLine($"⚠️  {Name} tükendi!");

            _stock = value;
        }
    }

    // Read-only property
    public decimal TotalValue => Price * Stock;

    // Property with private setter
    public DateTime CreatedDate { get; private set; } = DateTime.Now;

    // Required property (C# 11+)
    public required string Barcode { get; init; }
}

public class SmartProduct
{
    private decimal _discount;

    // Expression-bodied property
    public string Name { get; init; } = string.Empty;
    public decimal BasePrice { get; init; }

    // Property with computed value
    public decimal Discount
    {
        get => _discount;
        set => _discount = Math.Clamp(value, 0, 100);
    }

    // Computed property
    public decimal FinalPrice => BasePrice * (1 - Discount / 100);

    // Property with lazy initialization
    private List<string>? _tags;
    public List<string> Tags => _tags ??= new List<string>();
}

public class User
{
    // Modern C# property patterns
    public required string Username { get; init; }
    public string Email { get; init; } = string.Empty;

    // Nullable property
    public string? PhoneNumber { get; set; }

    // Property with validation in init
    private string _password = string.Empty;
    public string Password
    {
        get => "***";  // Never expose password
        init
        {
            if (value.Length < 8)
                throw new ArgumentException("Şifre en az 8 karakter olmalı!");
            _password = value;
        }
    }
}
