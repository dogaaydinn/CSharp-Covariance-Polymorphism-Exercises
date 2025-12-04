namespace AbstractClassExample;

/// <summary>
/// Abstract base class - Ortak davranış ve state içerir
/// </summary>
public abstract class Shape
{
    // Field - State (abstract class'larda olabilir)
    private static int _shapeCount = 0;

    // Properties - Ortak özellikler
    public string Color { get; set; }
    public int Id { get; private set; }

    // Constructor - Abstract class'larda olabilir
    protected Shape(string color)
    {
        Color = color;
        Id = ++_shapeCount;
        Console.WriteLine($"[Shape Constructor] Şekil #{Id} oluşturuldu");
    }

    // Abstract method - Alt sınıflar implement ETMEK ZORUNDA
    public abstract double CalculateArea();
    public abstract double CalculatePerimeter();

    // Virtual method - Alt sınıflar override EDEBİLİR (isteğe bağlı)
    public virtual void Draw()
    {
        Console.WriteLine($"🎨 Çiziliyor: {GetType().Name}, Renk: {Color}, Alan: {CalculateArea():F2}");
    }

    // Concrete method - Tüm şekiller için ortak implementasyon
    public void DisplayInfo()
    {
        Console.WriteLine($"\n📊 Şekil Bilgileri:");
        Console.WriteLine($"   ID: {Id}");
        Console.WriteLine($"   Tür: {GetType().Name}");
        Console.WriteLine($"   Renk: {Color}");
        Console.WriteLine($"   Alan: {CalculateArea():F2} birim²");
        Console.WriteLine($"   Çevre: {CalculatePerimeter():F2} birim");
    }

    // Static method - Tüm şekiller için ortak
    public static int GetShapeCount() => _shapeCount;
}

/// <summary>
/// Circle - Abstract class'tan türetilmiş concrete class
/// </summary>
public class Circle : Shape
{
    public double Radius { get; set; }

    public Circle(double radius, string color) : base(color)
    {
        Radius = radius;
        Console.WriteLine($"[Circle Constructor] Yarıçap: {radius}");
    }

    // Abstract metodları implement etmek ZORUNLU
    public override double CalculateArea()
    {
        return Math.PI * Radius * Radius;
    }

    public override double CalculatePerimeter()
    {
        return 2 * Math.PI * Radius;
    }

    // Virtual metodu override etmek İSTEĞE BAĞLI
    public override void Draw()
    {
        Console.WriteLine($"🔵 Daire çiziliyor (r={Radius}, renk={Color})");
    }
}

/// <summary>
/// Rectangle - Abstract class'tan türetilmiş concrete class
/// </summary>
public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public Rectangle(double width, double height, string color) : base(color)
    {
        Width = width;
        Height = height;
        Console.WriteLine($"[Rectangle Constructor] Boyutlar: {width}x{height}");
    }

    public override double CalculateArea()
    {
        return Width * Height;
    }

    public override double CalculatePerimeter()
    {
        return 2 * (Width + Height);
    }

    public override void Draw()
    {
        Console.WriteLine($"🟦 Dikdörtgen çiziliyor ({Width}x{Height}, renk={Color})");
    }
}

/// <summary>
/// Triangle - Abstract class'tan türetilmiş concrete class
/// </summary>
public class Triangle : Shape
{
    public double Base { get; set; }
    public double Height { get; set; }
    public double SideA { get; set; }
    public double SideB { get; set; }
    public double SideC { get; set; }

    public Triangle(double baseLength, double height, double sideA, double sideB, double sideC, string color)
        : base(color)
    {
        Base = baseLength;
        Height = height;
        SideA = sideA;
        SideB = sideB;
        SideC = sideC;
        Console.WriteLine($"[Triangle Constructor] Taban: {baseLength}, Yükseklik: {height}");
    }

    public override double CalculateArea()
    {
        return (Base * Height) / 2;
    }

    public override double CalculatePerimeter()
    {
        return SideA + SideB + SideC;
    }

    public override void Draw()
    {
        Console.WriteLine($"🔺 Üçgen çiziliyor (taban={Base}, yükseklik={Height}, renk={Color})");
    }
}
