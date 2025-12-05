// SCENARIO: Otopark ücret hesaplama sistemi
// Type checking örnekleri: typeof, GetType(), is, as operatörleri
// Pattern matching (C# 9+) ve type casting

using TypeChecking;

class Program
{
    static void Main()
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║       🅿️  OTOPARK ÜCRET HESAPLAMA SİSTEMİ          ║");
        Console.WriteLine("║   Type Checking & Pattern Matching Demonstrasyonu    ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
        Console.WriteLine();

        // Otoparka giren araçlar
        List<Vehicle> vehicles = new()
        {
            new Car("Toyota Corolla", "34ABC123", 2023, 4, "Hybrid"),
            new Truck("Volvo FH16", "06TIR456", 2022, 15000, 3),
            new Motorcycle("Harley Davidson", "35MOT789", 2024, 1200, false),
            new Car("BMW 320i", "06BMW321", 2021, 4, "Diesel"),
            new Motorcycle("Honda Gold Wing", "34MOT111", 2023, 1800, true)
        };

        Console.WriteLine($"📊 Otoparkta {vehicles.Count} araç var\n");

        // 1. typeof - Compile-time type checking
        Console.WriteLine("═══ 1. typeof OPERATOR (Compile-Time Type) ═══\n");
        DemonstrateTypeof();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 2. GetType() - Runtime type checking
        Console.WriteLine("═══ 2. GetType() METHOD (Runtime Type) ═══\n");
        DemonstrateGetType(vehicles);

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 3. is operator - Type checking
        Console.WriteLine("═══ 3. is OPERATOR (Type Checking) ═══\n");
        DemonstrateIsOperator(vehicles);

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 4. as operator - Safe casting
        Console.WriteLine("═══ 4. as OPERATOR (Safe Casting) ═══\n");
        DemonstrateAsOperator(vehicles);

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 5. Type casting - Explicit casting
        Console.WriteLine("═══ 5. TYPE CASTING (Explicit Casting) ═══\n");
        DemonstrateTypeCasting(vehicles);

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 6. Pattern matching - C# 9+ features
        Console.WriteLine("═══ 6. PATTERN MATCHING (C# 9+) ═══\n");
        DemonstratePatternMatching(vehicles);

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 7. Otopark ücret hesaplama - Type-based fee calculation
        Console.WriteLine("═══ 7. 💰 OTOPARK ÜCRET HESAPLAMA ═══\n");
        CalculateParkingFees(vehicles);

        // Final analysis
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    📊 ÖZET                            ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("✅ ÖĞRENME HEDEFLERİ:");
        Console.WriteLine("   • typeof      - Compile-time type literal (Type nesnesi)");
        Console.WriteLine("   • GetType()   - Runtime type bilgisi (instance'tan)");
        Console.WriteLine("   • is          - Type checking (inheritance-aware)");
        Console.WriteLine("   • as          - Safe casting (null döner, exception atmaz)");
        Console.WriteLine("   • (Type)cast  - Explicit casting (exception atabilir)");
        Console.WriteLine("   • Pattern matching - Modern C# type checking");
        Console.WriteLine();
        Console.WriteLine("💡 BEST PRACTICES:");
        Console.WriteLine("   • typeof için: Type bilgisi almak (compile-time)");
        Console.WriteLine("   • GetType() için: Runtime type'ı öğrenmek");
        Console.WriteLine("   • is için: Güvenli type checking + pattern matching");
        Console.WriteLine("   • as için: Null-check yapılabilecek safe casting");
        Console.WriteLine("   • Pattern matching için: Modern, okunabilir kod");
    }

    /// <summary>
    /// 1. typeof - Compile-time type literal
    /// Type nesnesi döner, reflection için kullanılır
    /// </summary>
    static void DemonstrateTypeof()
    {
        Console.WriteLine("typeof operatörü compile-time'da çalışır:\n");

        Type carType = typeof(Car);
        Type truckType = typeof(Truck);
        Type vehicleType = typeof(Vehicle);

        Console.WriteLine($"📦 Car Type Info:");
        Console.WriteLine($"   Name: {carType.Name}");
        Console.WriteLine($"   FullName: {carType.FullName}");
        Console.WriteLine($"   IsAbstract: {carType.IsAbstract}");
        Console.WriteLine($"   IsClass: {carType.IsClass}");
        Console.WriteLine($"   BaseType: {carType.BaseType?.Name}");

        Console.WriteLine($"\n📦 Vehicle Type Info:");
        Console.WriteLine($"   Name: {vehicleType.Name}");
        Console.WriteLine($"   IsAbstract: {vehicleType.IsAbstract}");

        Console.WriteLine("\n💡 typeof kullanım alanları:");
        Console.WriteLine("   • Reflection için Type nesnesi almak");
        Console.WriteLine("   • Generic type constraints kontrol etmek");
        Console.WriteLine("   • Compile-time type karşılaştırması");
    }

    /// <summary>
    /// 2. GetType() - Runtime type bilgisi
    /// Instance'tan type bilgisi döner
    /// </summary>
    static void DemonstrateGetType(List<Vehicle> vehicles)
    {
        Console.WriteLine("GetType() runtime'da instance'ın gerçek tipini döner:\n");

        foreach (var vehicle in vehicles)
        {
            Type runtimeType = vehicle.GetType();
            Console.WriteLine($"🚗 {vehicle.PlateNumber} ({vehicle.Brand}):");
            Console.WriteLine($"   Static type: Vehicle");
            Console.WriteLine($"   Runtime type: {runtimeType.Name}");
            Console.WriteLine($"   Is exact Car: {runtimeType == typeof(Car)}");
            Console.WriteLine($"   Is Vehicle or derived: {vehicle is Vehicle}");
            Console.WriteLine();
        }

        Console.WriteLine("💡 GetType() kullanım alanları:");
        Console.WriteLine("   • Runtime'da instance'ın gerçek tipini öğrenmek");
        Console.WriteLine("   • Polymorphic referanslarda gerçek tip kontrolü");
        Console.WriteLine("   • Exact type comparison (inheritance-agnostic)");
    }

    /// <summary>
    /// 3. is operator - Type checking (inheritance-aware)
    /// Boolean döner, pattern matching destekler
    /// </summary>
    static void DemonstrateIsOperator(List<Vehicle> vehicles)
    {
        Console.WriteLine("is operatörü inheritance-aware type checking yapar:\n");

        foreach (var vehicle in vehicles)
        {
            Console.WriteLine($"🔍 {vehicle.PlateNumber} type checks:");
            Console.WriteLine($"   is Vehicle: {vehicle is Vehicle}");
            Console.WriteLine($"   is Car: {vehicle is Car}");
            Console.WriteLine($"   is Truck: {vehicle is Truck}");
            Console.WriteLine($"   is Motorcycle: {vehicle is Motorcycle}");

            // Pattern matching with is
            if (vehicle is Car car)
            {
                Console.WriteLine($"   ✅ Car detected! Doors: {car.Doors}, Fuel: {car.FuelType}");
            }
            else if (vehicle is Truck truck)
            {
                Console.WriteLine($"   ✅ Truck detected! Capacity: {truck.LoadCapacity}kg");
            }
            else if (vehicle is Motorcycle moto)
            {
                Console.WriteLine($"   ✅ Motorcycle detected! Engine: {moto.EngineCC}cc");
            }

            Console.WriteLine();
        }

        Console.WriteLine("💡 is operatörü özellikleri:");
        Console.WriteLine("   • Inheritance-aware (base type check de true döner)");
        Console.WriteLine("   • Pattern matching ile değişken ataması yapabilir");
        Console.WriteLine("   • Null-safe (null için false döner)");
    }

    /// <summary>
    /// 4. as operator - Safe casting (null döner, exception atmaz)
    /// </summary>
    static void DemonstrateAsOperator(List<Vehicle> vehicles)
    {
        Console.WriteLine("as operatörü safe casting yapar (exception atmaz):\n");

        foreach (var vehicle in vehicles)
        {
            Console.WriteLine($"🔄 {vehicle.PlateNumber} casting attempts:");

            // Safe casting with as
            Car? carRef = vehicle as Car;
            Truck? truckRef = vehicle as Truck;
            Motorcycle? motoRef = vehicle as Motorcycle;

            Console.WriteLine($"   as Car: {(carRef != null ? "✅ Success" : "❌ Null")}");
            Console.WriteLine($"   as Truck: {(truckRef != null ? "✅ Success" : "❌ Null")}");
            Console.WriteLine($"   as Motorcycle: {(motoRef != null ? "✅ Success" : "❌ Null")}");

            // Null-conditional operator ile kullanım
            string? fuelType = (vehicle as Car)?.FuelType;
            double? loadCapacity = (vehicle as Truck)?.LoadCapacity;

            if (fuelType != null)
                Console.WriteLine($"   Fuel type: {fuelType}");
            if (loadCapacity != null)
                Console.WriteLine($"   Load capacity: {loadCapacity}kg");

            Console.WriteLine();
        }

        Console.WriteLine("💡 as operatörü özellikleri:");
        Console.WriteLine("   • Casting başarısız olursa null döner");
        Console.WriteLine("   • InvalidCastException atmaz");
        Console.WriteLine("   • Null-conditional operator (?.) ile kullanılabilir");
        Console.WriteLine("   • Reference types için çalışır (value types için değil)");
    }

    /// <summary>
    /// 5. Type casting - Explicit casting (exception atabilir)
    /// </summary>
    static void DemonstrateTypeCasting(List<Vehicle> vehicles)
    {
        Console.WriteLine("Explicit type casting (exception atabilir):\n");

        foreach (var vehicle in vehicles)
        {
            Console.WriteLine($"⚙️  {vehicle.PlateNumber} explicit casting:");

            try
            {
                // Güvenli önce kontrol et
                if (vehicle is Car)
                {
                    Car car = (Car)vehicle;  // Explicit cast
                    Console.WriteLine($"   ✅ Cast to Car successful: {car.Doors} doors");
                }
                else if (vehicle is Truck)
                {
                    Truck truck = (Truck)vehicle;  // Explicit cast
                    Console.WriteLine($"   ✅ Cast to Truck successful: {truck.Axles} axles");
                }
                else if (vehicle is Motorcycle)
                {
                    Motorcycle moto = (Motorcycle)vehicle;  // Explicit cast
                    Console.WriteLine($"   ✅ Cast to Motorcycle successful: {moto.EngineCC}cc");
                }

                // ❌ BAD: Önce kontrol etmeden cast - Exception atabilir!
                // Car wrongCast = (Car)vehicle;  // InvalidCastException!

            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine($"   ❌ Cast failed: {ex.Message}");
            }

            Console.WriteLine();
        }

        Console.WriteLine("💡 Type casting best practices:");
        Console.WriteLine("   • Explicit cast: (Type)object - Exception atabilir");
        Console.WriteLine("   • Önce is ile kontrol et, sonra cast yap");
        Console.WriteLine("   • Ya da as kullan (null döner, exception atmaz)");
        Console.WriteLine("   • Pattern matching kullan (modern C#)");
    }

    /// <summary>
    /// 6. Pattern matching - C# 9+ features
    /// Type patterns, property patterns, relational patterns
    /// </summary>
    static void DemonstratePatternMatching(List<Vehicle> vehicles)
    {
        Console.WriteLine("Modern C# pattern matching özellikleri:\n");

        foreach (var vehicle in vehicles)
        {
            // Switch expression with type patterns
            string vehicleInfo = vehicle switch
            {
                Car { Doors: 4, FuelType: "Hybrid" } => "🚗 Hibrit 4 kapılı araba (çevreci!)",
                Car { FuelType: "Diesel" } c => $"🚗 Dizel araba: {c.Doors} kapı",
                Car c => $"🚗 Araba: {c.Doors} kapı, {c.FuelType}",
                Truck { Axles: > 2 } t => $"🚚 Ağır kamyon: {t.Axles} aks, {t.LoadCapacity}kg",
                Truck t => $"🚚 Kamyon: {t.LoadCapacity}kg",
                Motorcycle { HasSidecar: true } m => $"🏍️  Sepetli motor: {m.EngineCC}cc",
                Motorcycle m => $"🏍️  Motosiklet: {m.EngineCC}cc",
                _ => "❓ Bilinmeyen araç"
            };

            Console.WriteLine($"{vehicle.PlateNumber}: {vehicleInfo}");

            // Property pattern with relational patterns (C# 9+)
            string ageCategory = vehicle.Year switch
            {
                >= 2024 => "Sıfır araç",
                >= 2020 => "Yeni araç",
                >= 2015 => "Orta yaşlı araç",
                _ => "Eski araç"
            };

            Console.WriteLine($"   Yaş kategorisi: {ageCategory} ({vehicle.Year})");
            Console.WriteLine();
        }

        Console.WriteLine("💡 Pattern matching özellikleri:");
        Console.WriteLine("   • Type patterns: Car c => ...");
        Console.WriteLine("   • Property patterns: { Doors: 4 }");
        Console.WriteLine("   • Relational patterns: >= 2024, > 2");
        Console.WriteLine("   • Switch expressions: Modern, expression-based");
        Console.WriteLine("   • Discard pattern: _ (default case)");
    }

    /// <summary>
    /// 7. Otopark ücret hesaplama - Type-based logic
    /// Her araç tipine göre farklı ücret hesaplama
    /// </summary>
    static void CalculateParkingFees(List<Vehicle> vehicles)
    {
        Console.WriteLine("Otopark ücretleri hesaplanıyor...\n");

        double totalRevenue = 0;

        // Araçlar 3 saat park etmiş varsayalım
        TimeSpan parkingDuration = TimeSpan.FromHours(3);

        foreach (var vehicle in vehicles)
        {
            vehicle.ExitTime = vehicle.EntryTime + parkingDuration;

            double hours = parkingDuration.TotalHours;
            double hourlyRate = vehicle.GetHourlyRate();
            double fee = hours * hourlyRate;

            // Type checking ile indirim uygula
            double discount = vehicle switch
            {
                Car { FuelType: "Hybrid" or "Electric" } => 0.2,  // %20 indirim (çevre dostu)
                Motorcycle { EngineCC: < 600 } => 0.1,            // %10 indirim (küçük motor)
                _ => 0.0
            };

            double finalFee = fee * (1 - discount);
            totalRevenue += finalFee;

            Console.WriteLine($"🎫 {vehicle.PlateNumber} ({vehicle.GetType().Name}):");
            Console.WriteLine($"   Marka: {vehicle.Brand}");
            Console.WriteLine($"   Süre: {hours:F1} saat");
            Console.WriteLine($"   Saatlik ücret: {hourlyRate:F2} TL");
            Console.WriteLine($"   Brüt tutar: {fee:F2} TL");

            if (discount > 0)
            {
                Console.WriteLine($"   İndirim: %{discount * 100:F0} ({fee * discount:F2} TL)");
            }

            Console.WriteLine($"   💰 Ödenecek: {finalFee:F2} TL");
            Console.WriteLine();
        }

        Console.WriteLine($"📊 TOPLAM GELİR: {totalRevenue:F2} TL");
        Console.WriteLine($"📊 ARAÇ SAYISI: {vehicles.Count} araç");
        Console.WriteLine($"📊 ORTALAMA ÜCRET: {totalRevenue / vehicles.Count:F2} TL/araç");

        // Araç tiplerine göre dağılım
        Console.WriteLine("\n📈 Araç Tipi Dağılımı:");
        var carCount = vehicles.Count(v => v is Car);
        var truckCount = vehicles.Count(v => v is Truck);
        var motoCount = vehicles.Count(v => v is Motorcycle);

        Console.WriteLine($"   🚗 Araba: {carCount} ({(double)carCount / vehicles.Count * 100:F1}%)");
        Console.WriteLine($"   🚚 Kamyon: {truckCount} ({(double)truckCount / vehicles.Count * 100:F1}%)");
        Console.WriteLine($"   🏍️  Motor: {motoCount} ({(double)motoCount / vehicles.Count * 100:F1}%)");
    }
}
