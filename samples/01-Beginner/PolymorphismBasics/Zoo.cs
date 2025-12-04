namespace PolymorphismBasics;

/// <summary>
/// Hayvanat bahçesi yönetim sınıfı.
/// Polimorfizm kullanarak farklı hayvan türlerini tek bir koleksiyonda yönetir.
/// </summary>
public class Zoo
{
    private readonly List<Animal> _animals = new();
    public string Name { get; set; }

    public Zoo(string name)
    {
        Name = name;
    }

    // Hayvan ekleme - Polimorfik parametre kabul eder
    public void AddAnimal(Animal animal)
    {
        _animals.Add(animal);
        Console.WriteLine($"✅ {animal.Name} hayvanat bahçesine eklendi.");
    }

    // Tüm hayvanları besle - Polimorfik davranış
    public void FeedAllAnimals()
    {
        Console.WriteLine($"\n🍽️  === {Name} - Beslenme Zamanı ===");

        foreach (var animal in _animals)
        {
            // Polimorfizm: Her hayvan kendi MakeSound() metodunu çalıştırır
            animal.MakeSound();
            Console.WriteLine($"   {animal.Name} besleniyor...\n");
        }
    }

    // Tüm hayvanları egzersiz yaptır
    public void ExerciseAllAnimals()
    {
        Console.WriteLine($"\n🏃 === {Name} - Egzersiz Zamanı ===");

        foreach (var animal in _animals)
        {
            // Polimorfizm: Her hayvan kendi Move() metodunu çalıştırır
            animal.Move();
            Console.WriteLine();
        }
    }

    // Hayvan türüne göre özel aktiviteler
    public void PerformSpecialActivities()
    {
        Console.WriteLine($"\n🎪 === {Name} - Özel Aktiviteler ===");

        foreach (var animal in _animals)
        {
            animal.DisplayInfo();

            // Tür kontrolü ile özel metodlara erişim
            switch (animal)
            {
                case Lion lion:
                    lion.Hunt();
                    break;
                case Elephant elephant:
                    elephant.SprayWater();
                    break;
                case Monkey monkey:
                    monkey.SwingOnVine();
                    break;
            }

            Console.WriteLine();
        }
    }

    // Hayvanat bahçesi istatistikleri
    public void DisplayStatistics()
    {
        Console.WriteLine($"\n📊 === {Name} - İstatistikler ===");
        Console.WriteLine($"Toplam Hayvan Sayısı: {_animals.Count}");
        Console.WriteLine($"Aslan: {_animals.OfType<Lion>().Count()}");
        Console.WriteLine($"Fil: {_animals.OfType<Elephant>().Count()}");
        Console.WriteLine($"Maymun: {_animals.OfType<Monkey>().Count()}");
    }
}
