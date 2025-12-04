namespace Covariance;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Covariance Exercise");
        Console.WriteLine("Run 'dotnet test' to check your solutions\n");

        // Uncomment when implemented
        /*
        // Example: Covariance allows IEnumerable<Dog> -> IEnumerable<Animal>
        IEnumerable<Dog> dogs = new List<Dog>
        {
            new Dog("Rex", 5, "German Shepherd"),
            new Dog("Max", 3, "Golden Retriever")
        };

        IEnumerable<Animal> animals = dogs; // Covariance!
        Console.WriteLine("Dogs as Animals:");
        foreach (var animal in animals)
        {
            Console.WriteLine($"  {animal.Name}: {animal.MakeSound()}");
        }
        */
    }

    // TODO 1: Implement covariant repository interface
    // HINT: Use 'out' keyword for covariance
    // Covariance allows: ICovariantRepository<Dog> -> ICovariantRepository<Animal>
    // RULE: 'out' means T can only appear in OUTPUT positions (return types)
    public interface ICovariantRepository<out T>
    {
        // TODO: Add method signatures that demonstrate covariance
        // Example: T Get(int id);
        //          IEnumerable<T> GetAll();
        //
        // IMPORTANT: You CANNOT have methods like:
        //   void Add(T item);  // ❌ T in input position breaks covariance
        //
        // Why? If ICovariantRepository<Dog> -> ICovariantRepository<Animal>
        // then Add(Animal animal) would accept Cat, breaking type safety!

        T Get(int id);
        IEnumerable<T> GetAll();
    }

    // TODO 2: Implement a covariant animal repository
    // HINT: This should implement ICovariantRepository<Animal>
    public class AnimalRepository : ICovariantRepository<Animal>
    {
        // TODO: Implement ICovariantRepository<Animal>
        // Store animals in a list
        // Implement Get() and GetAll() methods

        public Animal Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Animal> GetAll()
        {
            throw new NotImplementedException();
        }
    }

    // TODO 3: Demonstrate IEnumerable<out T> covariance
    // HINT: IEnumerable<T> is covariant (uses 'out T')
    // Return type: IEnumerable<Animal>
    // This should accept IEnumerable<Dog>, IEnumerable<Cat>, etc.
    public static IEnumerable<Animal> ConvertToAnimals<T>(IEnumerable<T> items) where T : Animal
    {
        // TODO: Return the items as IEnumerable<Animal>
        //
        // Algorithm:
        // 1. Simply return items - covariance allows this!
        // 2. IEnumerable<Dog> is automatically IEnumerable<Animal>
        // 3. No casting or conversion needed
        //
        // Why does this work?
        // IEnumerable<out T> - the 'out' keyword enables covariance
        throw new NotImplementedException();
    }

    // TODO 4: Demonstrate Func<out T> delegate covariance
    // HINT: Func<T> is covariant in its return type
    // Func<Dog> can be assigned to Func<Animal>
    public static Func<Animal> GetAnimalFactory(string animalType)
    {
        // TODO: Return different Func<> based on animalType
        //
        // Algorithm:
        // 1. If animalType == "Dog", return a Func<Dog> (which is also Func<Animal>)
        // 2. If animalType == "Cat", return a Func<Cat> (which is also Func<Animal>)
        // 3. Otherwise, return a Func<Bird>
        //
        // Example:
        // Func<Dog> dogFactory = () => new Dog("Rex", 5, "Labrador");
        // Func<Animal> animalFactory = dogFactory; // Covariance!
        //
        // Why? Func<out TResult> - covariant in return type
        throw new NotImplementedException();
    }

    // TODO 5: Demonstrate array covariance (and its dangers!)
    // HINT: Arrays are covariant, but it's NOT type-safe at compile time
    // Dog[] can be assigned to Animal[], but this can cause runtime errors!
    public static void DemonstrateArrayCovariance()
    {
        // TODO: Demonstrate array covariance and ArrayTypeMismatchException
        //
        // Algorithm:
        // 1. Create Dog[] dogs = { new Dog(...), new Dog(...) }
        // 2. Assign to Animal[] animals = dogs (covariance!)
        // 3. Try to add animals[0] = new Cat(...) - compiles but throws at runtime!
        // 4. Catch ArrayTypeMismatchException
        // 5. Console.WriteLine explaining the danger
        //
        // Key Learning: Array covariance is NOT safe!
        // - It's a language feature from before generics
        // - Use IEnumerable<T> or IReadOnlyList<T> instead
        //
        // Why is it dangerous?
        // Animal[] animals = new Dog[2]; // Compiles!
        // animals[0] = new Cat(...);      // Compiles! But throws at runtime!
        throw new NotImplementedException();
    }

    // TODO 6: Create a safe covariant collection using IReadOnlyList<out T>
    // HINT: IReadOnlyList<out T> is covariant and safe (no Add method)
    public static IReadOnlyList<Animal> GetSafeAnimalList()
    {
        // TODO: Return an IReadOnlyList<Dog> as IReadOnlyList<Animal>
        //
        // Algorithm:
        // 1. Create List<Dog> dogs = new() { ... }
        // 2. Return dogs (or dogs.AsReadOnly())
        // 3. This is safe because IReadOnlyList<out T> only allows reading
        //
        // Why is this safe?
        // - IReadOnlyList<out T> has no Add/Set methods (output-only)
        // - Can't violate type safety by adding wrong types
        // - Unlike arrays, this is compile-time safe!
        throw new NotImplementedException();
    }
}
