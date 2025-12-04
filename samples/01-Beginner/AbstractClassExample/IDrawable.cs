namespace AbstractClassExample;

/// <summary>
/// Interface - Sadece contract, implementasyon yok (state yok)
/// Abstract class ile karşılaştırma için
/// </summary>
public interface IDrawable
{
    void Draw();
    void Erase();
}

/// <summary>
/// Interface - Measurement contract
/// </summary>
public interface IMeasurable
{
    double CalculateArea();
    double CalculatePerimeter();
}

/// <summary>
/// Concrete class - Hem abstract class hem de interface'leri implement eder
/// Abstract class: Single inheritance (sadece 1 base class)
/// Interface: Multiple inheritance (birden fazla interface)
/// </summary>
public class ColoredCircle : Shape, IDrawable, IMeasurable
{
    public double Radius { get; set; }
    public string BorderStyle { get; set; }

    public ColoredCircle(double radius, string color, string borderStyle) : base(color)
    {
        Radius = radius;
        BorderStyle = borderStyle;
    }

    // Abstract class metodları - ZORUNLU
    public override double CalculateArea()
    {
        return Math.PI * Radius * Radius;
    }

    public override double CalculatePerimeter()
    {
        return 2 * Math.PI * Radius;
    }

    // IDrawable interface - ZORUNLU (interface'den geldiği için)
    public void Erase()
    {
        Console.WriteLine($"🧹 {Color} renkli daire siliniyor...");
    }

    // Shape'ten gelen Draw() zaten var (override edilmiş)
    public override void Draw()
    {
        Console.WriteLine($"🎨 Özel daire çiziliyor: r={Radius}, renk={Color}, stil={BorderStyle}");
    }
}

/// <summary>
/// Interface karşılaştırması için - Interface'den türetilmiş class
/// Abstract class YOK, sadece interface var
/// </summary>
public class Point : IDrawable
{
    public double X { get; set; }
    public double Y { get; set; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    // Interface metodları implement et
    public void Draw()
    {
        Console.WriteLine($"📍 Nokta çiziliyor: ({X}, {Y})");
    }

    public void Erase()
    {
        Console.WriteLine($"🧹 Nokta siliniyor: ({X}, {Y})");
    }
}
