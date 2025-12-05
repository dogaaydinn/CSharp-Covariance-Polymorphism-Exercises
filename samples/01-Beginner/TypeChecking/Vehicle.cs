namespace TypeChecking;

/// <summary>
/// Base class: Vehicle (Araç)
/// Abstract class - Tüm araçların ortak özellikleri
/// </summary>
public abstract class Vehicle
{
    public string Brand { get; set; }
    public string PlateNumber { get; set; }
    public int Year { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }

    protected Vehicle(string brand, string plateNumber, int year)
    {
        Brand = brand;
        PlateNumber = plateNumber;
        Year = year;
        EntryTime = DateTime.Now;
    }

    public abstract void Start();
    public abstract double GetHourlyRate();  // Her araç kendi ücretini bilir
}

/// <summary>
/// Derived class: Car (Araba)
/// Saatlik ücret: 10 TL
/// </summary>
public class Car : Vehicle
{
    public int Doors { get; set; }
    public string FuelType { get; set; }

    public Car(string brand, string plateNumber, int year, int doors, string fuelType)
        : base(brand, plateNumber, year)
    {
        Doors = doors;
        FuelType = fuelType;
    }

    public override void Start() => Console.WriteLine($"🚗 {Brand} araba çalıştırılıyor...");

    public override double GetHourlyRate() => 10.0; // 10 TL/saat
}

/// <summary>
/// Derived class: Truck (Kamyon)
/// Saatlik ücret: 25 TL (ağır araç)
/// </summary>
public class Truck : Vehicle
{
    public double LoadCapacity { get; set; }  // Yük kapasitesi (kg)
    public int Axles { get; set; }            // Aks sayısı

    public Truck(string brand, string plateNumber, int year, double loadCapacity, int axles)
        : base(brand, plateNumber, year)
    {
        LoadCapacity = loadCapacity;
        Axles = axles;
    }

    public override void Start() => Console.WriteLine($"🚚 {Brand} kamyon çalıştırılıyor...");

    public override double GetHourlyRate()
    {
        // Aks sayısına göre ücret artışı
        return Axles > 2 ? 30.0 : 25.0;  // 25-30 TL/saat
    }
}

/// <summary>
/// Derived class: Motorcycle (Motosiklet)
/// Saatlik ücret: 5 TL (küçük araç)
/// </summary>
public class Motorcycle : Vehicle
{
    public int EngineCC { get; set; }     // Motor hacmi
    public bool HasSidecar { get; set; }  // Sepet var mı?

    public Motorcycle(string brand, string plateNumber, int year, int engineCC, bool hasSidecar)
        : base(brand, plateNumber, year)
    {
        EngineCC = engineCC;
        HasSidecar = hasSidecar;
    }

    public override void Start() => Console.WriteLine($"🏍️  {Brand} motosiklet çalıştırılıyor...");

    public override double GetHourlyRate()
    {
        // Sepetli motosikletler daha fazla yer kaplar
        return HasSidecar ? 7.0 : 5.0;  // 5-7 TL/saat
    }
}

/// <summary>
/// Utility class: ParkingSpot (Park yeri)
/// Type checking ve pattern matching için kullanılır
/// </summary>
public class ParkingSpot
{
    public int SpotNumber { get; set; }
    public Vehicle? ParkedVehicle { get; set; }
    public bool IsOccupied => ParkedVehicle != null;
}
