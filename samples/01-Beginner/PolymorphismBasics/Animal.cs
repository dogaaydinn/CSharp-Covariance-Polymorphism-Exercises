namespace PolymorphismBasics;

/// <summary>
/// Hayvan sınıfının temel soyut sınıfı.
/// Virtual metodlar sayesinde alt sınıflar davranışı özelleştirebilir.
/// </summary>
public abstract class Animal
{
    public string Name { get; protected set; }
    public int Age { get; protected set; }

    protected Animal(string name, int age)
    {
        Name = name;
        Age = age;
    }

    // Virtual metod - Alt sınıflar override edebilir
    public virtual void MakeSound()
    {
        Console.WriteLine($"{Name} bir ses çıkarıyor...");
    }

    // Virtual metod - Davranış gösterimi
    public virtual void Move()
    {
        Console.WriteLine($"{Name} hareket ediyor...");
    }

    // Sanal olmayan metod - Tüm hayvanlar için ortak
    public void DisplayInfo()
    {
        Console.WriteLine($"Ad: {Name}, Yaş: {Age}");
    }
}

/// <summary>
/// Aslan sınıfı - Animal'dan türetilmiş özelleştirilmiş davranış.
/// </summary>
public class Lion : Animal
{
    public string PrideTerritory { get; set; }

    public Lion(string name, int age, string territory)
        : base(name, age)
    {
        PrideTerritory = territory;
    }

    // Override - Aslan'a özgü ses
    public override void MakeSound()
    {
        Console.WriteLine($"{Name}: 🦁 ROAAAAR! (Kükreme sesi - {PrideTerritory} bölgesinden)");
    }

    // Override - Aslan'a özgü hareket
    public override void Move()
    {
        Console.WriteLine($"{Name} güçlü adımlarla yürüyor ve bölgesini kontrol ediyor.");
    }

    // Aslan'a özel metod
    public void Hunt()
    {
        Console.WriteLine($"{Name} avını takip ediyor... 🎯");
    }
}

/// <summary>
/// Fil sınıfı - Animal'dan türetilmiş farklı davranış implementasyonu.
/// </summary>
public class Elephant : Animal
{
    public double TuskLength { get; set; } // Fildişi uzunluğu (metre)

    public Elephant(string name, int age, double tuskLength)
        : base(name, age)
    {
        TuskLength = tuskLength;
    }

    // Override - Fil'e özgü ses
    public override void MakeSound()
    {
        Console.WriteLine($"{Name}: 🐘 PAAAOOO! (Boru sesi - {TuskLength}m fildişi)");
    }

    // Override - Fil'e özgü hareket
    public override void Move()
    {
        Console.WriteLine($"{Name} ağır ama görkemli adımlarla yürüyor.");
    }

    // Fil'e özel metod
    public void SprayWater()
    {
        Console.WriteLine($"{Name} hortumu ile su püskürtüyor! 💦");
    }
}

/// <summary>
/// Maymun sınıfı - Animal'dan türetilmiş çevik davranış.
/// </summary>
public class Monkey : Animal
{
    public string FavoriteFood { get; set; }

    public Monkey(string name, int age, string favoriteFood)
        : base(name, age)
    {
        FavoriteFood = favoriteFood;
    }

    // Override - Maymun'a özgü ses
    public override void MakeSound()
    {
        Console.WriteLine($"{Name}: 🐵 OOH OOH AAH AAH! (En sevdiği yemek: {FavoriteFood})");
    }

    // Override - Maymun'a özgü hareket
    public override void Move()
    {
        Console.WriteLine($"{Name} ağaçtan ağaca atlayarak hızla hareket ediyor.");
    }

    // Maymun'a özel metod
    public void SwingOnVine()
    {
        Console.WriteLine($"{Name} liandan sarılarak sallanıyor! 🌿");
    }
}
