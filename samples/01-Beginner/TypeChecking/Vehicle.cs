namespace TypeChecking;

public abstract class Vehicle
{
    public string Brand { get; set; }
    public int Year { get; set; }

    protected Vehicle(string brand, int year)
    {
        Brand = brand;
        Year = year;
    }

    public abstract void Start();
}

public class Car : Vehicle
{
    public int Doors { get; set; }
    public string FuelType { get; set; }

    public Car(string brand, int year, int doors, string fuelType) : base(brand, year)
    {
        Doors = doors;
        FuelType = fuelType;
    }

    public override void Start() => Console.WriteLine($"🚗 {Brand} araba çalıştırılıyor...");
}

public class Truck : Vehicle
{
    public double LoadCapacity { get; set; }
    public int Axles { get; set; }

    public Truck(string brand, int year, double loadCapacity, int axles) : base(brand, year)
    {
        LoadCapacity = loadCapacity;
        Axles = axles;
    }

    public override void Start() => Console.WriteLine($"🚚 {Brand} kamyon çalıştırılıyor...");
}

public class Motorcycle : Vehicle
{
    public int EngineCC { get; set; }
    public bool HasSidecar { get; set; }

    public Motorcycle(string brand, int year, int engineCC, bool hasSidecar) : base(brand, year)
    {
        EngineCC = engineCC;
        HasSidecar = hasSidecar;
    }

    public override void Start() => Console.WriteLine($"🏍️  {Brand} motosiklet çalıştırılıyor...");
}
