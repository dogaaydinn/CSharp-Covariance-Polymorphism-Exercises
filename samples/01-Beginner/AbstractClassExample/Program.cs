// SCENARIO: Geometrik şekiller - Abstract class vs Interface
// BAD PRACTICE: Ortak davranış için interface kullanmak (kod tekrarı)
// GOOD PRACTICE: Ortak davranış için abstract class, contract için interface

using AbstractClassExample;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Abstract Class vs Interface Karşılaştırması ===\n");

        Console.WriteLine("=== 1. Abstract Class ile Polymorphism ===\n");
        DemonstrateAbstractClass();

        Console.WriteLine("\n=== 2. Interface ile Multiple Inheritance ===\n");
        DemonstrateInterface();

        Console.WriteLine("\n=== 3. Abstract Class + Interface Kombinasyonu ===\n");
        DemonstrateCombination();

        Console.WriteLine("\n=== 4. Abstract Class vs Interface Farkları ===\n");
        ShowDifferences();

        // Analiz
        Console.WriteLine("\n=== Output Analysis ===");
        Console.WriteLine("1. Abstract class: State (fields) + behavior (methods) içerir");
        Console.WriteLine("2. Abstract class: Constructor, fields, concrete methods olabilir");
        Console.WriteLine("3. Interface: Sadece contract (C# 8+ default implementation hariç)");
        Console.WriteLine("4. Abstract class: Single inheritance (1 base class)");
        Console.WriteLine("5. Interface: Multiple inheritance (birden fazla interface)");

        Console.WriteLine("\n💡 Best Practice:");
        Console.WriteLine("   ✅ Abstract class: Ortak state/behavior için (IS-A ilişkisi)");
        Console.WriteLine("   ✅ Interface: Contract/capability için (CAN-DO ilişkisi)");
        Console.WriteLine("   ✅ İkisini birlikte kullan: abstract class + interface");
    }

    /// <summary>
    /// Abstract class ile polymorphism ve ortak davranış
    /// </summary>
    static void DemonstrateAbstractClass()
    {
        Console.WriteLine("Abstract class'tan türetilmiş şekiller:\n");

        // Polymorphic collection - Base abstract class
        List<Shape> shapes = new()
        {
            new Circle(5.0, "Kırmızı"),
            new Rectangle(4.0, 6.0, "Mavi"),
            new Triangle(3.0, 4.0, 3.0, 4.0, 5.0, "Yeşil")
        };

        Console.WriteLine($"\n✅ Toplam {Shape.GetShapeCount()} şekil oluşturuldu\n");

        // Polymorphic method calls
        foreach (var shape in shapes)
        {
            shape.Draw();  // Virtual method - Her şekil kendi implementasyonunu çağırır
            shape.DisplayInfo();  // Concrete method - Base class'tan gelir, hepsi için aynı
        }

        Console.WriteLine("\n💡 Abstract class faydaları:");
        Console.WriteLine("   - Ortak state (Color, Id fields)");
        Console.WriteLine("   - Ortak constructor logic");
        Console.WriteLine("   - Concrete methods (DisplayInfo)");
        Console.WriteLine("   - Abstract methods (CalculateArea, CalculatePerimeter)");
        Console.WriteLine("   - Static members (GetShapeCount)");
    }

    /// <summary>
    /// Interface ile multiple inheritance ve contract
    /// </summary>
    static void DemonstrateInterface()
    {
        Console.WriteLine("Interface ile multiple inheritance:\n");

        // Interface collection - Farklı class'lar aynı interface'i implement eder
        List<IDrawable> drawables = new()
        {
            new ColoredCircle(3.0, "Mor", "Kesikli"),
            new Point(10, 20),
            new Point(30, 40)
        };

        Console.WriteLine("IDrawable interface'ini implement eden nesneler:\n");
        foreach (var drawable in drawables)
        {
            Console.WriteLine($"Nesne tipi: {drawable.GetType().Name}");
            drawable.Draw();
            drawable.Erase();
            Console.WriteLine();
        }

        Console.WriteLine("💡 Interface faydaları:");
        Console.WriteLine("   - Multiple inheritance (bir class birden fazla interface)");
        Console.WriteLine("   - Contract tanımlar (ne yapacağını söyler, nasıl değil)");
        Console.WriteLine("   - Farklı class hierarchy'lerindeki nesneler aynı interface'i kullanabilir");
        Console.WriteLine("   - Loose coupling (Dependency Injection için ideal)");
    }

    /// <summary>
    /// Abstract class + Interface kombinasyonu
    /// En güçlü yaklaşım: İkisinin avantajlarını birleştirir
    /// </summary>
    static void DemonstrateCombination()
    {
        Console.WriteLine("Abstract class + Interface kombinasyonu:\n");

        // ColoredCircle: Shape (abstract class) + IDrawable + IMeasurable (interface'ler)
        ColoredCircle specialCircle = new(7.0, "Turuncu", "Kalın");

        Console.WriteLine("Shape (abstract class) özellikleri:");
        Console.WriteLine($"   ID: {specialCircle.Id}");
        Console.WriteLine($"   Color: {specialCircle.Color}");
        Console.WriteLine($"   Alan: {specialCircle.CalculateArea():F2}");
        Console.WriteLine();

        Console.WriteLine("IDrawable interface özellikleri:");
        specialCircle.Draw();
        specialCircle.Erase();
        Console.WriteLine();

        Console.WriteLine("ColoredCircle'a özgü özellikler:");
        Console.WriteLine($"   BorderStyle: {specialCircle.BorderStyle}");
        Console.WriteLine();

        // Polymorphic kullanım - Farklı tiplerde referans
        Shape asShape = specialCircle;
        IDrawable asDrawable = specialCircle;
        IMeasurable asMeasurable = specialCircle;

        Console.WriteLine("Aynı nesne, farklı referans tipleri:");
        Console.WriteLine($"   Shape referansı: {asShape.GetType().Name}");
        Console.WriteLine($"   IDrawable referansı: {asDrawable.GetType().Name}");
        Console.WriteLine($"   IMeasurable referansı: {asMeasurable.GetType().Name}");

        Console.WriteLine("\n💡 Kombinasyon faydaları:");
        Console.WriteLine("   ✅ Abstract class'tan: Ortak state ve behavior");
        Console.WriteLine("   ✅ Interface'lerden: Multiple inheritance ve contract");
        Console.WriteLine("   ✅ En esnek ve güçlü yaklaşım");
    }

    /// <summary>
    /// Abstract class vs Interface farkları gösterimi
    /// </summary>
    static void ShowDifferences()
    {
        Console.WriteLine("📊 Abstract Class vs Interface Karşılaştırma Tablosu:\n");

        Console.WriteLine("┌─────────────────────────┬─────────────────┬─────────────────┐");
        Console.WriteLine("│ Özellik                 │ Abstract Class  │ Interface       │");
        Console.WriteLine("├─────────────────────────┼─────────────────┼─────────────────┤");
        Console.WriteLine("│ Fields (state)          │ ✅ Var          │ ❌ Yok          │");
        Console.WriteLine("│ Constructor             │ ✅ Var          │ ❌ Yok          │");
        Console.WriteLine("│ Concrete methods        │ ✅ Var          │ ⚠️  C# 8+ (Sınırlı)│");
        Console.WriteLine("│ Abstract methods        │ ✅ Var          │ ✅ Var          │");
        Console.WriteLine("│ Access modifiers        │ ✅ Var          │ ❌ Yok (public) │");
        Console.WriteLine("│ Multiple inheritance    │ ❌ Yok (single) │ ✅ Var          │");
        Console.WriteLine("│ Static members          │ ✅ Var          │ ⚠️  C# 8+       │");
        Console.WriteLine("└─────────────────────────┴─────────────────┴─────────────────┘");

        Console.WriteLine("\n🎯 Ne Zaman Kullanmalı?\n");

        Console.WriteLine("Abstract Class kullan:");
        Console.WriteLine("   ✅ IS-A ilişkisi (Circle IS-A Shape)");
        Console.WriteLine("   ✅ Ortak state (fields) paylaşımı gerekiyorsa");
        Console.WriteLine("   ✅ Ortak behavior (concrete methods) gerekiyorsa");
        Console.WriteLine("   ✅ Constructor logic paylaşmak istiyorsanız");
        Console.WriteLine("   ✅ Access modifiers gerekiyorsa (protected, private)");

        Console.WriteLine("\nInterface kullan:");
        Console.WriteLine("   ✅ CAN-DO ilişkisi (Shape CAN-BE drawn -> IDrawable)");
        Console.WriteLine("   ✅ Multiple inheritance gerekiyorsa");
        Console.WriteLine("   ✅ Sadece contract tanımlamak istiyorsanız");
        Console.WriteLine("   ✅ Farklı class hierarchy'leri arasında ortak davranış");
        Console.WriteLine("   ✅ Dependency Injection için");

        Console.WriteLine("\nİkisini birlikte kullan:");
        Console.WriteLine("   ✅ En esnek ve güçlü yaklaşım");
        Console.WriteLine("   ✅ Abstract class: Ortak state/behavior");
        Console.WriteLine("   ✅ Interface: Contract ve multiple inheritance");
    }
}
