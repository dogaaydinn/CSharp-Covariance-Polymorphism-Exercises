// SCENARIO: Covariance & Contravariance - out/in modifiers
// BAD PRACTICE: Invariant generics - flexibility yok
// GOOD PRACTICE: Variant generics - safe type conversions

using CovarianceContravariance;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Covariance & Contravariance ===\n");

        Console.WriteLine("=== 1. Covariance (out) ===\n");
        DemonstrateCovariance();

        Console.WriteLine("\n=== 2. Contravariance (in) ===\n");
        DemonstrateContravariance();

        Console.WriteLine("\n=== 3. Invariance ===\n");
        DemonstrateInvariance();

        Console.WriteLine("\n=== Analysis ===");
        Console.WriteLine("• Covariance (out): T sadece return - upcasting safe");
        Console.WriteLine("• Contravariance (in): T sadece parameter - downcasting safe");
        Console.WriteLine("• Invariance: T hem input hem output - conversion yok");
    }

    static void DemonstrateCovariance()
    {
        // Covariance: IProducer<Dog> → IProducer<Animal>
        IProducer<Dog> dogProducer = new DogProducer();

        // ✅ Covariance: Derived → Base assignment
        IProducer<Animal> animalProducer = dogProducer;

        Animal animal = animalProducer.Produce();
        animal.MakeSound();

        Console.WriteLine("\n💡 Covariance explained:");
        Console.WriteLine("   IProducer<Dog> → IProducer<Animal>");
        Console.WriteLine("   Dog IS-A Animal, so producer of Dog IS-A producer of Animal");
    }

    static void DemonstrateContravariance()
    {
        // Contravariance: IConsumer<Animal> → IConsumer<Dog>
        IConsumer<Animal> animalConsumer = new AnimalConsumer();

        // ✅ Contravariance: Base → Derived assignment
        IConsumer<Dog> dogConsumer = animalConsumer;

        Dog dog = new Dog { Name = "Max" };
        dogConsumer.Consume(dog);

        Console.WriteLine("\n💡 Contravariance explained:");
        Console.WriteLine("   IConsumer<Animal> → IConsumer<Dog>");
        Console.WriteLine("   Consumer of Animal CAN consume Dog (Dog IS-A Animal)");
    }

    static void DemonstrateInvariance()
    {
        // Invariance: IProcessor<T> - no conversion
        IProcessor<Animal> animalProcessor = new AnimalProcessor();

        // ❌ Invariance: Conversion not allowed
        // IProcessor<Dog> dogProcessor = animalProcessor;  // Error!

        Animal animal = new Dog { Name = "Rocky" };
        animalProcessor.Process(animal);

        Console.WriteLine("\n💡 Invariance explained:");
        Console.WriteLine("   IProcessor<Animal> ≠ IProcessor<Dog>");
        Console.WriteLine("   T hem input hem output - type conversion unsafe");
    }
}
