using System.ComponentModel;

namespace PropertyExamples;

// ═══════════════════════════════════════════════════════════
// ❌ BAD PRACTICE: Public Fields - No Encapsulation
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ❌ BAD PRACTICE: Public fields yerine properties kullanılmalı
/// Problem: Validation yok, encapsulation yok, side effects yok
/// </summary>
public class BadProduct
{
    // ❌ BAD: Public fields - validation yok!
    public string Name = "Unknown";
    public decimal Price = 0;           // Negatif olabilir! ❌
    public int Stock = 0;               // Negatif olabilir! ❌
    public decimal Discount = 0;        // %100'den fazla olabilir! ❌
    public string Category = "General";

    // ❌ BAD: Her defasında hesaplanmalı - cache yok
    public decimal GetFinalPrice()
    {
        return Price * (1 - Discount / 100);
    }

    // ❌ BAD: Total value hesaplama - property olmalı
    public decimal GetTotalValue()
    {
        return Price * Stock;
    }
}

// ═══════════════════════════════════════════════════════════
// ✅ GOOD PRACTICE: Auto-Properties (C# 3.0+)
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ GOOD: Auto-properties ile basit encapsulation
/// Use case: Basit property'ler, validation gerekmediğinde
/// </summary>
public class SimpleProduct
{
    // Auto-property - compiler backing field oluşturur
    public string Name { get; set; } = "Unknown";

    // Auto-property with default value
    public string Category { get; set; } = "General";

    // Auto-property
    public decimal Price { get; set; }

    // Auto-property
    public int Stock { get; set; }

    // Auto-property with private setter - sadece class içinden değiştirilebilir
    public DateTime CreatedDate { get; private set; } = DateTime.Now;

    // Computed property (expression-bodied)
    public decimal TotalValue => Price * Stock;

    // Method ile update (private setter kullanımı)
    public void UpdateCreatedDate(DateTime date)
    {
        CreatedDate = date;
    }
}

// ═══════════════════════════════════════════════════════════
// ✅ BEST PRACTICE: Full Properties with Validation
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ BEST: Full properties ile validation ve side effects
/// Use case: Validation gerekli, business rules var
/// </summary>
public class ValidatedProduct
{
    // Private backing fields
    private string _name = "Unknown";
    private decimal _price;
    private int _stock;
    private decimal _discount;
    private string _category = "General";

    // ═══════════════════════════════════════════════════════
    // FULL PROPERTIES WITH VALIDATION
    // ═══════════════════════════════════════════════════════

    /// <summary>
    /// Name property with validation
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            // Validation: Null/empty check
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Ürün adı boş olamaz!", nameof(Name));

            // Validation: Length check
            if (value.Length > 100)
                throw new ArgumentException("Ürün adı 100 karakterden uzun olamaz!", nameof(Name));

            _name = value;
            Console.WriteLine($"  ✅ Name set: {_name}");
        }
    }

    /// <summary>
    /// Price property with validation
    /// </summary>
    public decimal Price
    {
        get => _price;
        set
        {
            // Validation: Negatif fiyat kontrolü
            if (value < 0)
                throw new ArgumentException("Fiyat negatif olamaz!", nameof(Price));

            // Validation: Çok yüksek fiyat uyarısı
            if (value > 1_000_000m)
                Console.WriteLine($"  ⚠️  Uyarı: Çok yüksek fiyat - {value:C}");

            decimal oldPrice = _price;
            _price = value;

            // Side effect: Fiyat değişikliği logla
            if (oldPrice > 0 && oldPrice != value)
            {
                decimal change = ((value - oldPrice) / oldPrice) * 100;
                Console.WriteLine($"  📊 Fiyat değişti: {oldPrice:C} → {value:C} ({change:+0.0;-0.0}%)");
            }
        }
    }

    /// <summary>
    /// Stock property with validation and side effects
    /// </summary>
    public int Stock
    {
        get => _stock;
        set
        {
            // Validation: Negatif stok kontrolü
            if (value < 0)
                throw new ArgumentException("Stok negatif olamaz!", nameof(Stock));

            int oldStock = _stock;
            _stock = value;

            // Side effect: Stok bittiğinde uyarı
            if (value == 0 && oldStock > 0)
                Console.WriteLine($"  ⚠️  STOK BİTTİ: {Name}");

            // Side effect: Kritik stok seviyesi uyarısı
            if (value > 0 && value <= 5)
                Console.WriteLine($"  ⚠️  KRİTİK STOK: {Name} - Kalan: {value}");

            // Side effect: Stok yenilendi
            if (oldStock == 0 && value > 0)
                Console.WriteLine($"  ✅ STOK YENİLENDİ: {Name} - Yeni Stok: {value}");
        }
    }

    /// <summary>
    /// Discount property with clamping (0-100 range)
    /// </summary>
    public decimal Discount
    {
        get => _discount;
        set
        {
            // Validation + Auto-correction: Clamp to 0-100 range
            decimal clamped = Math.Clamp(value, 0, 100);

            if (value != clamped)
                Console.WriteLine($"  ⚠️  İndirim düzeltildi: %{value} → %{clamped}");

            _discount = clamped;
        }
    }

    /// <summary>
    /// Category property with validation
    /// </summary>
    public string Category
    {
        get => _category;
        set
        {
            // Validation: Allowed categories
            string[] allowedCategories = { "Electronics", "Clothing", "Food", "Books", "General" };

            if (!allowedCategories.Contains(value))
            {
                Console.WriteLine($"  ⚠️  Geçersiz kategori '{value}', 'General' kullanılıyor");
                _category = "General";
            }
            else
            {
                _category = value;
            }
        }
    }

    // ═══════════════════════════════════════════════════════
    // COMPUTED PROPERTIES (Read-Only)
    // ═══════════════════════════════════════════════════════

    /// <summary>
    /// Final price after discount (computed)
    /// </summary>
    public decimal FinalPrice => Price * (1 - Discount / 100);

    /// <summary>
    /// Total value (price × stock)
    /// </summary>
    public decimal TotalValue => Price * Stock;

    /// <summary>
    /// Total value after discount
    /// </summary>
    public decimal DiscountedTotalValue => FinalPrice * Stock;

    /// <summary>
    /// Is in stock? (computed boolean)
    /// </summary>
    public bool IsInStock => Stock > 0;

    /// <summary>
    /// Is low stock? (< 10 units)
    /// </summary>
    public bool IsLowStock => Stock > 0 && Stock < 10;

    /// <summary>
    /// Is on sale? (discount > 0)
    /// </summary>
    public bool IsOnSale => Discount > 0;

    // Auto-property with private setter
    public DateTime CreatedDate { get; private set; } = DateTime.Now;

    // Display method
    public void DisplayInfo()
    {
        Console.WriteLine($"\n📦 {Name}");
        Console.WriteLine($"   Kategori: {Category}");
        Console.WriteLine($"   Fiyat: {Price:C} {(IsOnSale ? $"(İndirimli: {FinalPrice:C}, -%{Discount})" : "")}");
        Console.WriteLine($"   Stok: {Stock} {(IsLowStock ? "⚠️ DÜŞÜK STOK" : IsInStock ? "✅" : "❌ TÜKENDİ")}");
        Console.WriteLine($"   Toplam Değer: {TotalValue:C}");
    }
}

// ═══════════════════════════════════════════════════════════
// ✅ MODERN PROPERTIES: Init-Only, Required (C# 9+, C# 11+)
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ MODERN: Init-only ve required properties (C# 9+, C# 11+)
/// Use case: Immutable objects, factory pattern
/// </summary>
public class ModernProduct
{
    // Required property (C# 11+) - MUST be initialized
    public required string Barcode { get; init; }

    // Init-only property (C# 9+) - Can only be set during initialization
    public string Name { get; init; } = "Unknown";

    // Init-only with validation
    private readonly decimal _basePrice;
    public decimal BasePrice
    {
        get => _basePrice;
        init
        {
            if (value < 0)
                throw new ArgumentException("Base price cannot be negative");
            _basePrice = value;
        }
    }

    // Regular property (mutable)
    public decimal Discount { get; set; }

    // Init-only
    public string Category { get; init; } = "General";

    // Computed property
    public decimal FinalPrice => BasePrice * (1 - Discount / 100);

    // Auto-property with private setter
    public DateTime CreatedDate { get; private set; } = DateTime.Now;

    // Nullable property
    public string? Description { get; init; }

    // Static factory method
    public static ModernProduct CreateWithDiscount(string barcode, string name, decimal price, decimal discount)
    {
        return new ModernProduct
        {
            Barcode = barcode,
            Name = name,
            BasePrice = price,
            Discount = discount
        };
    }
}

// ═══════════════════════════════════════════════════════════
// ✅ ADVANCED: Property Change Notification (INotifyPropertyChanged)
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ ADVANCED: INotifyPropertyChanged ile property change tracking
/// Use case: WPF, MAUI, data binding scenarios
/// </summary>
public class NotifyingProduct : INotifyPropertyChanged
{
    private string _name = "Unknown";
    private decimal _price;
    private int _stock;

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        Console.WriteLine($"  🔔 PropertyChanged: {propertyName}");
    }

    /// <summary>
    /// Name property with change notification
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    /// <summary>
    /// Price property with change notification
    /// </summary>
    public decimal Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                if (value < 0)
                    throw new ArgumentException("Price cannot be negative");

                _price = value;
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(TotalValue));  // Dependent property
            }
        }
    }

    /// <summary>
    /// Stock property with change notification
    /// </summary>
    public int Stock
    {
        get => _stock;
        set
        {
            if (_stock != value)
            {
                if (value < 0)
                    throw new ArgumentException("Stock cannot be negative");

                _stock = value;
                OnPropertyChanged(nameof(Stock));
                OnPropertyChanged(nameof(TotalValue));  // Dependent property
                OnPropertyChanged(nameof(IsInStock));   // Dependent property
            }
        }
    }

    // Computed properties
    public decimal TotalValue => Price * Stock;
    public bool IsInStock => Stock > 0;

    // Subscribe to property changes
    public void SubscribeToChanges()
    {
        PropertyChanged += (sender, e) =>
        {
            Console.WriteLine($"  📣 UI Update Needed: {e.PropertyName} changed!");
        };
    }
}

// ═══════════════════════════════════════════════════════════
// ✅ REAL-WORLD: E-Commerce Product with All Features
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ PRODUCTION-READY: E-commerce ürün class'ı
/// Combines: Validation, computed properties, init-only, notifications
/// </summary>
public class ECommerceProduct
{
    private decimal _price;
    private int _stock;
    private decimal _discount;
    private int _viewCount;

    // Required + Init-only
    public required string ProductId { get; init; }
    public required string Name { get; init; }

    // Init-only
    public string Category { get; init; } = "General";
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }

    // Validated properties
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
                throw new ArgumentException("Price must be >= 0");
            _price = value;
        }
    }

    public int Stock
    {
        get => _stock;
        set
        {
            if (value < 0)
                throw new ArgumentException("Stock must be >= 0");
            _stock = value;
        }
    }

    public decimal Discount
    {
        get => _discount;
        set => _discount = Math.Clamp(value, 0, 100);  // Auto-clamp
    }

    // Mutable property
    public int ViewCount
    {
        get => _viewCount;
        private set => _viewCount = value;
    }

    // Computed properties
    public decimal FinalPrice => Price * (1 - Discount / 100);
    public decimal TotalValue => Price * Stock;
    public bool IsInStock => Stock > 0;
    public bool IsOnSale => Discount > 0;
    public bool IsTrending => ViewCount > 1000;
    public string StockStatus => Stock switch
    {
        0 => "Tükendi",
        < 5 => "Son ürünler",
        < 20 => "Stokta az",
        _ => "Stokta"
    };

    // Private setters
    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public DateTime? LastUpdatedDate { get; private set; }

    // Methods
    public void IncrementViewCount()
    {
        ViewCount++;
    }

    public void UpdateStock(int newStock)
    {
        Stock = newStock;
        LastUpdatedDate = DateTime.Now;
    }

    public void ApplyDiscount(decimal discountPercentage)
    {
        Discount = discountPercentage;
        LastUpdatedDate = DateTime.Now;
    }

    public override string ToString()
    {
        return $"{Name} - {FinalPrice:C} ({StockStatus})";
    }
}
