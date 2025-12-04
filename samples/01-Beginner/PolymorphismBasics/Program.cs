// SCENARIO: Hayvanat bahçesi yönetim sistemi
// BAD PRACTICE: Her hayvan türü için ayrı koleksiyon ve metod yazmak
// GOOD PRACTICE: Polimorfizm kullanarak tek bir koleksiyonda tüm hayvanları yönetmek

using PolymorphismBasics;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Polimorfizm Temel Örneği: Hayvanat Bahçesi ===\n");

        // Hayvanat bahçesi oluştur
        var zoo = new Zoo("Doğa Vadisi Hayvanat Bahçesi");

        // ✅ GOOD PRACTICE: Farklı türdeki nesneler aynı base type ile temsil edilir
        Animal lion = new Lion("Simba", 5, "Savana Bölgesi");
        Animal elephant = new Elephant("Dumbo", 12, 1.8);
        Animal monkey = new Monkey("Abu", 3, "Muz");

        // Hayvanları ekle - Polimorfik parametre
        zoo.AddAnimal(lion);
        zoo.AddAnimal(elephant);
        zoo.AddAnimal(monkey);

        // ❌ BAD PRACTICE örneği (gösterim amaçlı)
        Console.WriteLine("\n❌ Kötü Yaklaşım (Her tür için ayrı işlem):");
        Console.WriteLine("   if (animal is Lion) { ((Lion)animal).Hunt(); }");
        Console.WriteLine("   if (animal is Elephant) { ((Elephant)animal).SprayWater(); }");
        Console.WriteLine("   // Her yeni hayvan türü için yeni kod yazmak zorunda kalırız!\n");

        // ✅ GOOD PRACTICE: Polimorfik davranış
        Console.WriteLine("✅ İyi Yaklaşım (Polimorfizm):");
        Console.WriteLine("   foreach(var animal in animals) { animal.MakeSound(); }");
        Console.WriteLine("   // Yeni hayvan türleri eklendiğinde kod değişmez!\n");

        // Polimorfik metodları çağır
        zoo.FeedAllAnimals();
        zoo.ExerciseAllAnimals();
        zoo.PerformSpecialActivities();
        zoo.DisplayStatistics();

        // Polimorfizm analizi
        Console.WriteLine("\n=== Output Analysis ===");
        Console.WriteLine("1. Virtual/Override: Her hayvan MakeSound() için kendi implementasyonunu sağlar");
        Console.WriteLine("2. Liskov Substitution: Animal referansı ile tüm alt sınıflar kullanılabilir");
        Console.WriteLine("3. Open/Closed: Yeni hayvan türü eklemek için mevcut kodu değiştirmeye gerek yok");
        Console.WriteLine("4. Polymorphic Collections: List<Animal> içinde farklı türler tutulabilir");
        Console.WriteLine("5. Type Checking: Pattern matching ile özel davranışlara erişilebilir");

        // Performans notu
        Console.WriteLine("\n💡 Performance Note:");
        Console.WriteLine("Virtual method çağrıları vtable (virtual method table) üzerinden yapılır.");
        Console.WriteLine("Bu minimal bir overhead ekler (~%5) ancak esneklik sağlar.");
    }
}
