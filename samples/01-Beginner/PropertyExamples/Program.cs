// SCENARIO: Property patterns - Auto-property, validation, computed properties
// BAD PRACTICE: Public fields, validation yok
// GOOD PRACTICE: Properties ile encapsulation ve validation

using PropertyExamples;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Property Patterns ===\n");

        Console.WriteLine("=== 1. Auto-Property ===\n");
        DemonstrateAutoProperty();

        Console.WriteLine("\n=== 2. Property with Validation ===\n");
        DemonstrateValidation();

        Console.WriteLine("\n=== 3. Computed Properties ===\n");
        DemonstrateComputedProperties();

        Console.WriteLine("\n=== 4. Modern Property Patterns ===\n");
        DemonstrateModernPatterns();

        Console.WriteLine("\n=== Analysis ===");
        Console.WriteLine("• Auto-property: Basit getter/setter");
        Console.WriteLine("• Validation: set içinde kontrol");
        Console.WriteLine("• Computed: Expression-bodied properties");
        Console.WriteLine("• init: Sadece initialization'da set edilir");
        Console.WriteLine("• required: Constructor'da mutlaka set edilmeli (C# 11+)");
    }

    static void DemonstrateAutoProperty()
    {
        var product = new Product
        {
            Barcode = "123456789",
            Name = "Laptop",
            Price = 15000m,
            Stock = 10
        };

        Console.WriteLine($"Ürün: {product.Name}");
        Console.WriteLine($"Fiyat: {product.Price:C}");
        Console.WriteLine($"Stok: {product.Stock}");
        Console.WriteLine($"Toplam Değer: {product.TotalValue:C}");
    }

    static void DemonstrateValidation()
    {
        var product = new Product
        {
            Barcode = "ABC123",
            Name = "Mouse",
            Price = 150m,
            Stock = 5
        };

        try
        {
            Console.WriteLine("Fiyatı -100 yapılıyor...");
            product.Price = -100m;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"❌ Hata: {ex.Message}");
        }

        try
        {
            Console.WriteLine("\nStoğu -5 yapılıyor...");
            product.Stock = -5;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"❌ Hata: {ex.Message}");
        }

        Console.WriteLine("\nStok 0 yapılıyor...");
        product.Stock = 0;  // Side effect: Warning mesajı
    }

    static void DemonstrateComputedProperties()
    {
        var product = new SmartProduct
        {
            Name = "Keyboard",
            BasePrice = 500m,
            Discount = 20  // %20 indirim
        };

        Console.WriteLine($"Ürün: {product.Name}");
        Console.WriteLine($"Baz Fiyat: {product.BasePrice:C}");
        Console.WriteLine($"İndirim: %{product.Discount}");
        Console.WriteLine($"Final Fiyat: {product.FinalPrice:C}");

        product.Discount = 150;  // %150 - Clamp'lenecek
        Console.WriteLine($"\nİndirim %150 yapıldı → %{product.Discount} (clamped)");
        Console.WriteLine($"Final Fiyat: {product.FinalPrice:C}");
    }

    static void DemonstrateModernPatterns()
    {
        // Required property (C# 11+)
        var user = new User
        {
            Username = "johndoe",  // Required!
            Email = "john@example.com",
            Password = "SecurePass123"
        };

        Console.WriteLine($"Username: {user.Username}");
        Console.WriteLine($"Email: {user.Email}");
        Console.WriteLine($"Password: {user.Password}");  // ***

        // init - Sadece initialization'da
        // user.Username = "newname";  // ❌ Error! init-only

        // Nullable property
        user.PhoneNumber = "+90 555 123 4567";
        Console.WriteLine($"Phone: {user.PhoneNumber}");
    }
}
