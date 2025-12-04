// SCENARIO: Araç türü kontrolü - typeof, GetType(), is operatörleri
// BAD PRACTICE: String karşılaştırma ile tip kontrolü
// GOOD PRACTICE: typeof, GetType(), is operatörü ile tip-güvenli kontrol

using TypeChecking;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Type Checking: typeof, GetType(), is ===\n");

        List<Vehicle> vehicles = new()
        {
            new Car("Toyota", 2023, 4, "Hybrid"),
            new Truck("Volvo", 2022, 15000, 3),
            new Motorcycle("Harley", 2024, 1200, false)
        };

        Console.WriteLine("=== 1. typeof - Compile-Time Type ===\n");
        DemonstrateTypeof();

        Console.WriteLine("\n=== 2. GetType() - Runtime Type ===\n");
        DemonstrateGetType(vehicles);

        Console.WriteLine("\n=== 3. is Operator - Type Checking ===\n");
        DemonstrateIsOperator(vehicles);

        Console.WriteLine("\n=== 4. Type Comparison ===\n");
        DemonstrateTypeComparison(vehicles);

        Console.WriteLine("\n=== 5. Pattern Matching ===\n");
        DemonstratePatternMatching(vehicles);

        Console.WriteLine("\n=== Analysis ===");
        Console.WriteLine("• typeof: Compile-time, Type literal alır");
        Console.WriteLine("• GetType(): Runtime, instance'tan tip döner");
        Console.WriteLine("• is: Runtime type checking, inheritance aware");
        Console.WriteLine("• ==: Exact type comparison, inheritance-agnostic");
    }

    static void DemonstrateTypeof()
    {
        Type carType = typeof(Car);
        Type vehicleType = typeof(Vehicle);

        Console.WriteLine($"Car type: {carType.Name}");
        Console.WriteLine($"Full name: {carType.FullName}");
        Console.WriteLine($"Assembly: {carType.Assembly.GetName().Name}");
        Console.WriteLine($"Is abstract: {carType.IsAbstract}");
        Console.WriteLine($"Is class: {carType.IsClass}");
        Console.WriteLine($"Base type: {carType.BaseType?.Name}");
    }

    static void DemonstrateGetType(List<Vehicle> vehicles)
    {
        foreach (var vehicle in vehicles)
        {
            Type runtimeType = vehicle.GetType();
            Console.WriteLine($"\n{vehicle.Brand}:");
            Console.WriteLine($"  Runtime type: {runtimeType.Name}");
            Console.WriteLine($"  Static type: Vehicle");
            Console.WriteLine($"  Is Vehicle: {vehicle is Vehicle}");
        }
    }

    static void DemonstrateIsOperator(List<Vehicle> vehicles)
    {
        foreach (var vehicle in vehicles)
        {
            Console.WriteLine($"\n{vehicle.Brand} ({vehicle.GetType().Name}):");
            Console.WriteLine($"  is Vehicle: {vehicle is Vehicle}");
            Console.WriteLine($"  is Car: {vehicle is Car}");
            Console.WriteLine($"  is Truck: {vehicle is Truck}");
            Console.WriteLine($"  is Motorcycle: {vehicle is Motorcycle}");
        }
    }

    static void DemonstrateTypeComparison(List<Vehicle> vehicles)
    {
        foreach (var vehicle in vehicles)
        {
            // Exact type check
            if (vehicle.GetType() == typeof(Car))
            {
                Console.WriteLine($"✅ {vehicle.Brand} tam olarak Car tipi");
            }

            // Inheritance-aware check
            if (vehicle is Vehicle)
            {
                Console.WriteLine($"✅ {vehicle.Brand} Vehicle veya türevi");
            }
        }
    }

    static void DemonstratePatternMatching(List<Vehicle> vehicles)
    {
        foreach (var vehicle in vehicles)
        {
            string info = vehicle switch
            {
                Car c => $"🚗 Araba: {c.Doors} kapı, {c.FuelType}",
                Truck t => $"🚚 Kamyon: {t.LoadCapacity}kg, {t.Axles} aks",
                Motorcycle m => $"🏍️  Motor: {m.EngineCC}cc, Sepet: {m.HasSidecar}",
                _ => "Bilinmeyen araç"
            };
            Console.WriteLine($"{vehicle.Brand}: {info}");
        }
    }
}
