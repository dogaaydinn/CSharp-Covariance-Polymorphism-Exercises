// SCENARIO: E-ticaret Ürün Validasyonu
// BAD PRACTICE: Public fields - validation yok, encapsulation yok
// GOOD PRACTICE: Properties - validation, computed values, notifications
// ADVANCED: Init-only (C# 9+), required (C# 11+), INotifyPropertyChanged

using PropertyExamples;

class Program
{
    static void Main()
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   🛒  PROPERTY PATTERNS - E-TİCARET ÜRÜN VALİDASYONU   ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        // 1. BAD PRACTICE
        Console.WriteLine("═══ 1. ❌ BAD PRACTICE: Public Fields ═══\n");
        DemonstrateBadPractice();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 2. Auto-Properties
        Console.WriteLine("═══ 2. ✅ AUTO-PROPERTIES (C# 3.0+) ═══\n");
        DemonstrateAutoProperties();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 3. Full Properties with Validation
        Console.WriteLine("═══ 3. ✅ FULL PROPERTIES - Validation ═══\n");
        DemonstrateValidation();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 4. Computed Properties
        Console.WriteLine("═══ 4. ✅ COMPUTED PROPERTIES ═══\n");
        DemonstrateComputedProperties();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 5. Init-Only Properties (C# 9+)
        Console.WriteLine("═══ 5. ✅ INIT-ONLY PROPERTIES (C# 9+) ═══\n");
        DemonstrateInitOnlyProperties();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 6. Property Change Notifications
        Console.WriteLine("═══ 6. ✅ PROPERTY CHANGE NOTIFICATIONS ═══\n");
        DemonstratePropertyChangeNotifications();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 7. Property Patterns (C# 8+)
        Console.WriteLine("═══ 7. ✅ PROPERTY PATTERNS (C# 8+) ═══\n");
        DemonstratePropertyPatterns();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 8. Real-World E-Commerce Scenario
        Console.WriteLine("═══ 8. ✅ REAL-WORLD: E-Commerce Scenario ═══\n");
        DemonstrateECommerceScenario();

        // Final Summary
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    📊 ÖZET                                ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("✅ ÖĞRENİLENLER:");
        Console.WriteLine("   • Public fields yerine properties kullan (encapsulation)");
        Console.WriteLine("   • Auto-properties: Basit get/set için");
        Console.WriteLine("   • Full properties: Validation ve side effects için");
        Console.WriteLine("   • Computed properties: Hesaplanan değerler (read-only)");
        Console.WriteLine("   • Init-only (C# 9+): Sadece initialization'da set");
        Console.WriteLine("   • Required (C# 11+): Mutlaka initialize edilmeli");
        Console.WriteLine("   • INotifyPropertyChanged: UI binding için (WPF, MAUI)");
        Console.WriteLine();
        Console.WriteLine("💡 BEST PRACTICES:");
        Console.WriteLine("   • Validation her zaman setter'da yapılmalı");
        Console.WriteLine("   • Computed properties immutable olmalı (read-only)");
        Console.WriteLine("   • Private setter: Controlled mutation için");
        Console.WriteLine("   • Side effects: Logging, notifications, cascade updates");
    }

    /// <summary>
    /// ❌ BAD PRACTICE: Public fields yerine properties kullanılmalı
    /// </summary>
    static void DemonstrateBadPractice()
    {
        Console.WriteLine("💀 Bad Practice: Public fields örneği\n");

        BadProduct bad = new BadProduct();
        bad.Name = "Laptop";
        bad.Price = 15000m;
        bad.Stock = 10;
        bad.Discount = 20;

        Console.WriteLine($"Ürün: {bad.Name}");
        Console.WriteLine($"Fiyat: {bad.Price:C}");
        Console.WriteLine($"Stok: {bad.Stock}");
        Console.WriteLine($"İndirim: %{bad.Discount}");
        Console.WriteLine($"Final Fiyat: {bad.GetFinalPrice():C}");

        Console.WriteLine("\n⚠️  SORUNLAR:");

        // Problem 1: Negatif fiyat (validation yok!)
        bad.Price = -1000m;
        Console.WriteLine($"   ❌ Negatif fiyat atandı: {bad.Price:C} (KABUL EDİLDİ!)");

        // Problem 2: Negatif stok (validation yok!)
        bad.Stock = -50;
        Console.WriteLine($"   ❌ Negatif stok atandı: {bad.Stock} (KABUL EDİLDİ!)");

        // Problem 3: Aşırı indirim (validation yok!)
        bad.Discount = 150m;  // %150 indirim?!
        Console.WriteLine($"   ❌ %150 indirim atandı (KABUL EDİLDİ!)");
        Console.WriteLine($"   ❌ Final fiyat: {bad.GetFinalPrice():C} (NEGATİF!)");

        // Problem 4: Direct field access - encapsulation yok
        Console.WriteLine("\n   ❌ Public fields → Validation yok!");
        Console.WriteLine("   ❌ Public fields → Side effects yok (log, notification)!");
        Console.WriteLine("   ❌ Public fields → Breaking changes riski (field → property)!");
    }

    /// <summary>
    /// ✅ GOOD: Auto-properties ile basit encapsulation
    /// </summary>
    static void DemonstrateAutoProperties()
    {
        Console.WriteLine("🎯 Auto-properties: Compiler backing field oluşturur\n");

        SimpleProduct product = new SimpleProduct
        {
            Name = "Wireless Mouse",
            Category = "Electronics",
            Price = 250m,
            Stock = 50
        };

        Console.WriteLine($"Ürün: {product.Name}");
        Console.WriteLine($"Kategori: {product.Category}");
        Console.WriteLine($"Fiyat: {product.Price:C}");
        Console.WriteLine($"Stok: {product.Stock}");
        Console.WriteLine($"Toplam Değer: {product.TotalValue:C} (Computed property)");
        Console.WriteLine($"Oluşturulma: {product.CreatedDate:yyyy-MM-dd HH:mm:ss}");

        Console.WriteLine("\n💡 AUTO-PROPERTY ÖZELLİKLERİ:");
        Console.WriteLine("   ✅ Compiler otomatik backing field oluşturur");
        Console.WriteLine("   ✅ Encapsulation sağlanır (get/set)");
        Console.WriteLine("   ✅ Default value atanabilir (= \"value\")");
        Console.WriteLine("   ✅ Private setter: Controlled mutation");
        Console.WriteLine("   ✅ Expression-bodied: Computed properties (=>)");

        // Private setter demonstration
        Console.WriteLine("\n🔒 Private Setter:");
        Console.WriteLine($"   CreatedDate (private set): {product.CreatedDate:HH:mm:ss}");
        // product.CreatedDate = DateTime.Now;  // ❌ COMPILE ERROR!
        Console.WriteLine("   ❌ product.CreatedDate = ... → COMPILE ERROR (private set)");

        // Method ile update
        product.UpdateCreatedDate(DateTime.Now.AddDays(-7));
        Console.WriteLine($"   ✅ UpdateCreatedDate() ile güncellendi: {product.CreatedDate:yyyy-MM-dd}");
    }

    /// <summary>
    /// ✅ BEST: Full properties ile validation ve side effects
    /// </summary>
    static void DemonstrateValidation()
    {
        Console.WriteLine("🎯 Full properties: Backing field + validation\n");

        Console.WriteLine("Ürün oluşturuluyor:\n");
        ValidatedProduct product = new ValidatedProduct
        {
            Name = "Gaming Keyboard"  // Setter çağrılır → validation
        };

        product.Category = "Electronics";  // Validation: Allowed categories
        product.Price = 800m;              // Validation: >= 0
        product.Stock = 15;                // Validation: >= 0
        product.Discount = 10;             // Validation: 0-100 clamp

        Console.WriteLine("\n═══ Validation Scenarios ═══\n");

        // Scenario 1: Negatif fiyat (REJECTED)
        Console.WriteLine("1️⃣  Negatif fiyat testi:");
        try
        {
            product.Price = -500m;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"   ❌ EXCEPTION: {ex.Message}");
        }

        // Scenario 2: Negatif stok (REJECTED)
        Console.WriteLine("\n2️⃣  Negatif stok testi:");
        try
        {
            product.Stock = -10;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"   ❌ EXCEPTION: {ex.Message}");
        }

        // Scenario 3: Aşırı indirim (CLAMPED)
        Console.WriteLine("\n3️⃣  Aşırı indirim testi (%120):");
        product.Discount = 120m;  // Clamped to 100
        Console.WriteLine($"   ✅ Discount set to: %{product.Discount}");

        // Scenario 4: Geçersiz kategori (AUTO-CORRECTED)
        Console.WriteLine("\n4️⃣  Geçersiz kategori testi:");
        product.Category = "InvalidCategory";  // Falls back to "General"
        Console.WriteLine($"   ✅ Category set to: {product.Category}");

        // Scenario 5: Stok bitişi (SIDE EFFECT)
        Console.WriteLine("\n5️⃣  Stok bitişi senaryosu:");
        product.Stock = 0;  // Side effect: Warning message

        // Scenario 6: Stok yenileme (SIDE EFFECT)
        Console.WriteLine("\n6️⃣  Stok yenileme senaryosu:");
        product.Stock = 20;  // Side effect: Restock message

        // Scenario 7: Fiyat değişimi (SIDE EFFECT + LOGGING)
        Console.WriteLine("\n7️⃣  Fiyat değişimi senaryosu:");
        product.Price = 1000m;  // Side effect: Price change log

        // Display final info
        Console.WriteLine("\n═══ Final Product Info ═══");
        product.DisplayInfo();
    }

    /// <summary>
    /// ✅ Computed properties: Expression-bodied, read-only
    /// </summary>
    static void DemonstrateComputedProperties()
    {
        Console.WriteLine("🎯 Computed properties: Calculated values\n");

        ValidatedProduct product = new ValidatedProduct
        {
            Name = "Smart Watch"
        };

        product.Price = 2000m;
        product.Stock = 8;
        product.Discount = 25;

        Console.WriteLine("📊 Product Info:");
        Console.WriteLine($"   Name: {product.Name}");
        Console.WriteLine($"   Price: {product.Price:C}");
        Console.WriteLine($"   Stock: {product.Stock}");
        Console.WriteLine($"   Discount: %{product.Discount}");

        Console.WriteLine("\n🔢 Computed Properties (Read-Only):");
        Console.WriteLine($"   • FinalPrice = {product.FinalPrice:C} (Price * (1 - Discount/100))");
        Console.WriteLine($"   • TotalValue = {product.TotalValue:C} (Price * Stock)");
        Console.WriteLine($"   • DiscountedTotalValue = {product.DiscountedTotalValue:C} (FinalPrice * Stock)");
        Console.WriteLine($"   • IsInStock = {product.IsInStock} (Stock > 0)");
        Console.WriteLine($"   • IsLowStock = {product.IsLowStock} (Stock < 10)");
        Console.WriteLine($"   • IsOnSale = {product.IsOnSale} (Discount > 0)");

        Console.WriteLine("\n💡 COMPUTED PROPERTY FAYDAları:");
        Console.WriteLine("   ✅ No backing field needed");
        Console.WriteLine("   ✅ Always up-to-date (recalculated on access)");
        Console.WriteLine("   ✅ Read-only by design (immutable)");
        Console.WriteLine("   ✅ Expression-bodied syntax (=>)");
        Console.WriteLine("   ✅ No manual sync required");

        // Computed properties otomatik güncellenir
        Console.WriteLine("\n🔄 Auto-Update Demonstration:");
        Console.WriteLine($"   Before: FinalPrice = {product.FinalPrice:C}");
        product.Discount = 50;  // %50 indirim
        Console.WriteLine($"   After (Discount=50%): FinalPrice = {product.FinalPrice:C}");
        Console.WriteLine("   ✅ Computed property otomatik güncellendi!");
    }

    /// <summary>
    /// ✅ Init-only properties (C# 9+) ve required properties (C# 11+)
    /// </summary>
    static void DemonstrateInitOnlyProperties()
    {
        Console.WriteLine("🎯 Init-only (C# 9+) ve Required (C# 11+) Properties\n");

        // Init-only: Sadece initialization'da set edilebilir
        Console.WriteLine("1️⃣  Init-Only Properties (C# 9+):\n");

        ModernProduct product1 = new ModernProduct
        {
            Barcode = "1234567890",  // Required!
            Name = "USB-C Cable",
            BasePrice = 150m,
            Category = "Electronics",
            Description = "High-speed USB-C cable"
        };

        Console.WriteLine($"   Barcode: {product1.Barcode} (required)");
        Console.WriteLine($"   Name: {product1.Name} (init)");
        Console.WriteLine($"   BasePrice: {product1.BasePrice:C} (init with validation)");
        Console.WriteLine($"   Category: {product1.Category} (init)");

        // ❌ Init-only properties değiştirilemez!
        Console.WriteLine("\n   ❌ Trying to change init-only property:");
        Console.WriteLine("      // product1.Name = \"New Name\";  ← COMPILE ERROR!");
        Console.WriteLine("      Init-only properties are IMMUTABLE after construction");

        // ✅ Regular properties değiştirilebilir
        Console.WriteLine("\n   ✅ Regular properties can be changed:");
        Console.WriteLine($"      Discount (before): %{product1.Discount}");
        product1.Discount = 20m;
        Console.WriteLine($"      Discount (after): %{product1.Discount}");

        // Factory method demonstration
        Console.WriteLine("\n2️⃣  Factory Method with Init-Only:\n");
        ModernProduct product2 = ModernProduct.CreateWithDiscount(
            barcode: "ABC123",
            name: "Wireless Charger",
            price: 400m,
            discount: 15m
        );

        Console.WriteLine($"   Created via factory: {product2.Name}");
        Console.WriteLine($"   BasePrice: {product2.BasePrice:C}");
        Console.WriteLine($"   Discount: %{product2.Discount}");
        Console.WriteLine($"   FinalPrice: {product2.FinalPrice:C}");

        // Required property demonstration
        Console.WriteLine("\n3️⃣  Required Properties (C# 11+):\n");
        Console.WriteLine("   ✅ Barcode is REQUIRED - must be initialized:");
        Console.WriteLine("      var p = new ModernProduct { Barcode = \"123\" }; ← OK");
        Console.WriteLine("\n   ❌ Missing required property causes compile error:");
        Console.WriteLine("      var p = new ModernProduct { Name = \"Test\" }; ← ERROR!");
        Console.WriteLine("      CS9035: Required member 'Barcode' must be set");

        Console.WriteLine("\n💡 INIT-ONLY vs REGULAR PROPERTIES:");
        Console.WriteLine("   • init: Sadece initialization'da (constructor, initializer)");
        Console.WriteLine("   • set: Her zaman değiştirilebilir");
        Console.WriteLine("   • required + init: Mutlaka initialize edilmeli + sonra immutable");
        Console.WriteLine("   • private set: Sadece class içinden değiştirilebilir");
    }

    /// <summary>
    /// ✅ Property change notifications (INotifyPropertyChanged)
    /// </summary>
    static void DemonstratePropertyChangeNotifications()
    {
        Console.WriteLine("🎯 Property Change Notifications (INotifyPropertyChanged)\n");
        Console.WriteLine("Use case: WPF, MAUI, Blazor - UI data binding\n");

        NotifyingProduct product = new NotifyingProduct();

        // Subscribe to property changes
        Console.WriteLine("1️⃣  Subscribing to PropertyChanged event:\n");
        product.SubscribeToChanges();

        // Change properties → notifications fire
        Console.WriteLine("\n2️⃣  Changing properties (notifications will fire):\n");

        Console.WriteLine("Setting Name = \"Smart TV\":");
        product.Name = "Smart TV";

        Console.WriteLine("\nSetting Price = 25000:");
        product.Price = 25000m;

        Console.WriteLine("\nSetting Stock = 5:");
        product.Stock = 5;

        Console.WriteLine("\n3️⃣  Dependent Properties:\n");
        Console.WriteLine("When Price or Stock changes, TotalValue also needs update!");
        Console.WriteLine("✅ OnPropertyChanged(nameof(TotalValue)) called automatically");

        Console.WriteLine("\n4️⃣  Validation Still Works:\n");
        try
        {
            Console.WriteLine("Trying to set negative price:");
            product.Price = -1000m;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"❌ EXCEPTION: {ex.Message}");
        }

        Console.WriteLine("\n💡 INOTIFYPROPERTYCHANGED BENEFITS:");
        Console.WriteLine("   ✅ UI automatically updates when data changes");
        Console.WriteLine("   ✅ Two-way data binding support");
        Console.WriteLine("   ✅ Decouples UI from business logic");
        Console.WriteLine("   ✅ Essential for MVVM pattern (WPF, MAUI, Avalonia)");
        Console.WriteLine("   ✅ Can notify dependent properties");
    }

    /// <summary>
    /// ✅ Property patterns (C# 8+): Pattern matching on properties
    /// </summary>
    static void DemonstratePropertyPatterns()
    {
        Console.WriteLine("🎯 Property Patterns (C# 8+): Pattern matching\n");

        ValidatedProduct[] products =
        {
            new ValidatedProduct { Name = "Laptop", Price = 15000m, Stock = 2, Discount = 10 },
            new ValidatedProduct { Name = "Mouse", Price = 200m, Stock = 50, Discount = 0 },
            new ValidatedProduct { Name = "Keyboard", Price = 800m, Stock = 0, Discount = 25 },
            new ValidatedProduct { Name = "Monitor", Price = 5000m, Stock = 3, Discount = 15 }
        };

        Console.WriteLine("📊 Product Filtering with Property Patterns:\n");

        // Pattern 1: In-stock products
        Console.WriteLine("1️⃣  In-stock products:");
        foreach (var p in products)
        {
            if (p is { IsInStock: true })
                Console.WriteLine($"   ✅ {p.Name} - {p.Stock} units");
        }

        // Pattern 2: On-sale products
        Console.WriteLine("\n2️⃣  On-sale products (Discount > 0):");
        foreach (var p in products)
        {
            if (p is { IsOnSale: true })
                Console.WriteLine($"   🏷️  {p.Name} - %{p.Discount} off, {p.FinalPrice:C}");
        }

        // Pattern 3: Low stock products
        Console.WriteLine("\n3️⃣  Low stock products (Stock < 10 and Stock > 0):");
        foreach (var p in products)
        {
            if (p is { IsLowStock: true })
                Console.WriteLine($"   ⚠️  {p.Name} - Only {p.Stock} left!");
        }

        // Pattern 4: Out of stock products
        Console.WriteLine("\n4️⃣  Out of stock products:");
        foreach (var p in products)
        {
            if (p is { Stock: 0 })
                Console.WriteLine($"   ❌ {p.Name} - Tükendi");
        }

        // Pattern 5: Expensive products with discount
        Console.WriteLine("\n5️⃣  Expensive products (Price > 1000) with discount:");
        foreach (var p in products)
        {
            if (p is { Price: > 1000m, IsOnSale: true })
                Console.WriteLine($"   💎 {p.Name} - {p.Price:C} → {p.FinalPrice:C} (-%{p.Discount})");
        }

        // Pattern 6: Switch expression with property patterns
        Console.WriteLine("\n6️⃣  Product categorization (switch expression):\n");
        foreach (var p in products)
        {
            string status = p switch
            {
                { Stock: 0 } => "🚫 Tükendi",
                { IsLowStock: true, IsOnSale: true } => "🔥 Son ürünler + İndirimli!",
                { IsLowStock: true } => "⚠️  Az kaldı",
                { IsOnSale: true } => "🏷️  İndirimde",
                { Price: > 10000m } => "💎 Premium ürün",
                _ => "✅ Stokta"
            };

            Console.WriteLine($"   {status}: {p.Name}");
        }

        Console.WriteLine("\n💡 PROPERTY PATTERN MATCHING:");
        Console.WriteLine("   ✅ is { Property: value } syntax");
        Console.WriteLine("   ✅ Switch expressions with property patterns");
        Console.WriteLine("   ✅ Relational patterns (>, <, >=, <=)");
        Console.WriteLine("   ✅ Logical patterns (and, or, not)");
        Console.WriteLine("   ✅ Nested property patterns");
    }

    /// <summary>
    /// ✅ Real-world E-Commerce scenario
    /// </summary>
    static void DemonstrateECommerceScenario()
    {
        Console.WriteLine("🎯 Real-World E-Commerce Product Management\n");

        // Create products
        Console.WriteLine("═══ Creating Products ═══\n");

        ECommerceProduct laptop = new ECommerceProduct
        {
            ProductId = "PROD-001",
            Name = "Gaming Laptop RTX 4070",
            Category = "Electronics",
            Description = "High-performance gaming laptop with RTX 4070",
            ImageUrl = "https://example.com/laptop.jpg",
            Price = 45000m,
            Stock = 5,
            Discount = 0
        };

        ECommerceProduct mouse = new ECommerceProduct
        {
            ProductId = "PROD-002",
            Name = "Wireless Gaming Mouse",
            Category = "Electronics",
            Price = 850m,
            Stock = 50
        };

        ECommerceProduct keyboard = new ECommerceProduct
        {
            ProductId = "PROD-003",
            Name = "Mechanical Keyboard RGB",
            Category = "Electronics",
            Price = 1200m,
            Stock = 0  // Tükendi
        };

        Console.WriteLine($"✅ Created: {laptop}");
        Console.WriteLine($"✅ Created: {mouse}");
        Console.WriteLine($"✅ Created: {keyboard}");

        // Scenario 1: View count tracking
        Console.WriteLine("\n═══ Scenario 1: View Count Tracking ═══\n");
        for (int i = 0; i < 1500; i++)
            laptop.IncrementViewCount();

        Console.WriteLine($"Laptop view count: {laptop.ViewCount}");
        Console.WriteLine($"Is trending? {laptop.IsTrending} (ViewCount > 1000)");

        // Scenario 2: Apply discount
        Console.WriteLine("\n═══ Scenario 2: Flash Sale - Apply Discount ═══\n");
        Console.WriteLine($"Laptop price before: {laptop.Price:C}");
        laptop.ApplyDiscount(20);  // %20 indirim
        Console.WriteLine($"Laptop price after: {laptop.FinalPrice:C} (-%{laptop.Discount})");
        Console.WriteLine($"Savings: {laptop.Price - laptop.FinalPrice:C}");

        // Scenario 3: Stock management
        Console.WriteLine("\n═══ Scenario 3: Stock Management ═══\n");
        Console.WriteLine($"Mouse stock: {mouse.Stock} ({mouse.StockStatus})");
        mouse.UpdateStock(3);  // Kritik stok seviyesi
        Console.WriteLine($"Mouse stock updated: {mouse.Stock} ({mouse.StockStatus})");

        // Scenario 4: Out of stock handling
        Console.WriteLine("\n═══ Scenario 4: Out of Stock Handling ═══\n");
        Console.WriteLine($"Keyboard status: {keyboard.StockStatus}");
        Console.WriteLine($"Is in stock? {keyboard.IsInStock}");

        // Scenario 5: Product listing with filters
        Console.WriteLine("\n═══ Scenario 5: Product Listing (Filtered) ═══\n");

        ECommerceProduct[] inventory = { laptop, mouse, keyboard };

        Console.WriteLine("🛒 All Products:");
        foreach (var p in inventory)
        {
            Console.WriteLine($"   • {p}");
        }

        Console.WriteLine("\n🏷️  On Sale:");
        foreach (var p in inventory.Where(p => p.IsOnSale))
        {
            Console.WriteLine($"   • {p.Name} - {p.Price:C} → {p.FinalPrice:C} (-%{p.Discount})");
        }

        Console.WriteLine("\n✅ Available:");
        foreach (var p in inventory.Where(p => p.IsInStock))
        {
            Console.WriteLine($"   • {p.Name} - {p.Stock} units ({p.StockStatus})");
        }

        Console.WriteLine("\n🔥 Trending:");
        foreach (var p in inventory.Where(p => p.IsTrending))
        {
            Console.WriteLine($"   • {p.Name} - {p.ViewCount} views");
        }

        // Scenario 6: Product details
        Console.WriteLine("\n═══ Scenario 6: Product Details ═══\n");
        Console.WriteLine($"Product ID: {laptop.ProductId}");
        Console.WriteLine($"Name: {laptop.Name}");
        Console.WriteLine($"Category: {laptop.Category}");
        Console.WriteLine($"Description: {laptop.Description}");
        Console.WriteLine($"Base Price: {laptop.Price:C}");
        Console.WriteLine($"Discount: %{laptop.Discount}");
        Console.WriteLine($"Final Price: {laptop.FinalPrice:C}");
        Console.WriteLine($"Stock: {laptop.Stock} ({laptop.StockStatus})");
        Console.WriteLine($"Total Value: {laptop.TotalValue:C}");
        Console.WriteLine($"View Count: {laptop.ViewCount}");
        Console.WriteLine($"Is Trending: {laptop.IsTrending}");
        Console.WriteLine($"Is On Sale: {laptop.IsOnSale}");
        Console.WriteLine($"Created: {laptop.CreatedDate:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Last Updated: {laptop.LastUpdatedDate:yyyy-MM-dd HH:mm:ss}");

        Console.WriteLine("\n💡 PRODUCTION-READY FEATURES:");
        Console.WriteLine("   ✅ Required properties (ProductId, Name)");
        Console.WriteLine("   ✅ Init-only properties (immutable after creation)");
        Console.WriteLine("   ✅ Validated properties (Price, Stock, Discount)");
        Console.WriteLine("   ✅ Computed properties (FinalPrice, TotalValue, Status)");
        Console.WriteLine("   ✅ Private setters (ViewCount, CreatedDate)");
        Console.WriteLine("   ✅ Business logic methods (UpdateStock, ApplyDiscount)");
        Console.WriteLine("   ✅ ToString() override for display");
    }
}
