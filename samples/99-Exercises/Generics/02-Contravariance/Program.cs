namespace Contravariance;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Contravariance Exercise");
        Console.WriteLine("Run 'dotnet test' to check your solutions\n");

        // Uncomment when implemented
        /*
        // Example: Contravariance allows Action<Animal> -> Action<Dog>
        Action<Animal> processAnimal = (animal) =>
            Console.WriteLine($"Processing: {animal.Name}");

        Action<Dog> processDog = processAnimal; // Contravariance!
        processDog(new Dog("Rex", 5, 25.5, "German Shepherd"));
        */
    }

    // TODO 1: Implement contravariant comparer interface
    // HINT: Use 'in' keyword for contravariance
    // Contravariance allows: IContravariantComparer<Animal> -> IContravariantComparer<Dog>
    // RULE: 'in' means T can only appear in INPUT positions (parameters)
    public interface IContravariantComparer<in T>
    {
        // TODO: Add method signatures that demonstrate contravariance
        // Example: int Compare(T x, T y);
        //          bool Equals(T x, T y);
        //
        // IMPORTANT: You CANNOT have methods like:
        //   T GetDefault();  // ❌ T in output position breaks contravariance
        //
        // Why? If IContravariantComparer<Animal> -> IContravariantComparer<Dog>
        // then GetDefault() returning Animal isn't a Dog, breaking type safety!

        int Compare(T x, T y);
        bool Equals(T x, T y);
    }

    // TODO 2: Implement an animal comparer by weight
    // HINT: This should implement IContravariantComparer<Animal>
    public class AnimalWeightComparer : IContravariantComparer<Animal>
    {
        // TODO: Implement IContravariantComparer<Animal>
        // Compare animals by weight
        // Equals checks if weights are equal (within tolerance)

        public int Compare(Animal x, Animal y)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Animal x, Animal y)
        {
            throw new NotImplementedException();
        }
    }

    // TODO 3: Demonstrate Action<in T> contravariance
    // HINT: Action<T> is contravariant (uses 'in T')
    // Action<Animal> can be assigned to Action<Dog>
    public static void ProcessAnimals(Action<Animal> animalProcessor, List<Dog> dogs)
    {
        // TODO: Process all dogs using the animalProcessor
        //
        // Algorithm:
        // 1. Loop through each dog in dogs
        // 2. Call animalProcessor(dog) - contravariance allows this!
        // 3. Action<Animal> accepts Animal or any derived type
        //
        // Why does this work?
        // Action<in T> - the 'in' keyword enables contravariance
        // If you can handle Animal, you can handle Dog
        throw new NotImplementedException();
    }

    // TODO 4: Demonstrate Func<in T, out TResult> variance
    // HINT: Func<T, TResult> is contravariant in T, covariant in TResult
    // This combines both variance types!
    public static Func<Dog, string> GetAnimalNameGetter()
    {
        // TODO: Return a Func<Animal, string> assigned to Func<Dog, string>
        //
        // Algorithm:
        // 1. Create a Func<Animal, string> that returns animal.Name
        // 2. Return it as Func<Dog, string>
        // 3. This works because:
        //    - Contravariant in T: Animal -> Dog (can accept more specific)
        //    - Covariant in TResult: string -> string (return type unchanged)
        //
        // Example:
        // Func<Animal, string> animalNameGetter = animal => animal.Name;
        // return animalNameGetter; // Contravariance in input!
        throw new NotImplementedException();
    }

    // TODO 5: Demonstrate IComparer<in T> real-world usage
    // HINT: Sort a list of dogs using an animal comparer
    // This shows practical contravariance with built-in interfaces
    public static List<Dog> SortDogsUsingAnimalComparer(List<Dog> dogs)
    {
        // TODO: Sort dogs using an IComparer<Animal>
        //
        // Algorithm:
        // 1. Create an AnimalWeightComparer (which is IComparer<Animal>)
        // 2. Use it to sort the list of Dogs
        // 3. This works because IComparer<in T> is contravariant
        // 4. IComparer<Animal> can be used as IComparer<Dog>
        //
        // Hint: Use dogs.Sort(IComparer<Dog>) method
        // But pass an IComparer<Animal> - contravariance makes this work!
        throw new NotImplementedException();
    }

    // TODO 6: Demonstrate event handler contravariance
    // HINT: EventHandler<TEventArgs> is contravariant in TEventArgs
    public static EventHandler<AnimalEventArgs> GetDogEventHandler()
    {
        // TODO: Return an event handler for AnimalEventArgs
        //
        // Algorithm:
        // 1. Create an EventHandler<AnimalEventArgs> that handles any animal event
        // 2. Return it - can be used for more specific event types
        // 3. EventHandler<in TEventArgs> is contravariant
        //
        // Example:
        // EventHandler<AnimalEventArgs> handler = (sender, e) =>
        // {
        //     Console.WriteLine($"Event: {e.EventType}, Animal: {e.Animal.Name}");
        // };
        // return handler;
        throw new NotImplementedException();
    }
}
