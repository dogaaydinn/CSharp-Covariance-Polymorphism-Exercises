# ⚠️ SPOILER WARNING ⚠️

**DO NOT READ UNTIL YOU'VE TRIED YOURSELF!**

---

# Covariance - Complete Solutions

All solutions are provided in INSTRUCTIONS.md hints section.

## Key Takeaways

1. **Covariance Basics**:
   - Use `out` keyword for type parameters
   - Type can only appear in OUTPUT positions (return types)
   - Enables `ICovariantRepository<Dog>` → `ICovariantRepository<Animal>`

2. **Built-in Covariant Types**:
   - `IEnumerable<out T>` - Most common
   - `IReadOnlyList<out T>` - Indexed read-only
   - `Func<out TResult>` - Delegates

3. **Array Covariance**:
   - Arrays ARE covariant: `Dog[]` → `Animal[]`
   - But NOT type-safe at compile time
   - Throws `ArrayTypeMismatchException` at runtime
   - **Avoid array covariance!**

4. **Type Safety Rules**:
   - ✅ Safe: `IEnumerable<Dog>` → `IEnumerable<Animal>`
   - ❌ Unsafe: `IList<Dog>` → `IList<Animal>` (not allowed)
   - ✅ Safe: `Func<Dog>` → `Func<Animal>`
   - ❌ Unsafe: `Action<Dog>` → `Action<Animal>` (contravariance)

## Complete Code Examples

### TODO 1 & 2: Covariant Repository

```csharp
public interface ICovariantRepository<out T>
{
    T Get(int id);
    IEnumerable<T> GetAll();
}

public class AnimalRepository : ICovariantRepository<Animal>
{
    private readonly List<Animal> _animals = new()
    {
        new Dog("Rex", 5, "German Shepherd"),
        new Cat("Whiskers", 2, true),
        new Bird("Tweety", 1, true)
    };

    public Animal Get(int id) => _animals[id];
    public IEnumerable<Animal> GetAll() => _animals;
}
```

### TODO 3: IEnumerable Covariance

```csharp
public static IEnumerable<Animal> ConvertToAnimals<T>(IEnumerable<T> items)
    where T : Animal
{
    return items;  // IEnumerable<out T> is covariant!
}
```

### TODO 4: Func Delegate Covariance

```csharp
public static Func<Animal> GetAnimalFactory(string animalType)
{
    if (animalType == "Dog")
        return () => new Dog("Rex", 5, "Labrador");

    if (animalType == "Cat")
        return () => new Cat("Whiskers", 2, true);

    return () => new Bird("Tweety", 1, true);
}
```

### TODO 5: Array Covariance Dangers

```csharp
public static void DemonstrateArrayCovariance()
{
    Dog[] dogs = new Dog[]
    {
        new Dog("Rex", 5, "German Shepherd"),
        new Dog("Max", 3, "Golden Retriever")
    };

    Animal[] animals = dogs;  // Covariance
    Console.WriteLine($"First animal: {animals[0].Name}");

    try
    {
        animals[0] = new Cat("Whiskers", 2, true);  // Runtime error!
    }
    catch (ArrayTypeMismatchException ex)
    {
        Console.WriteLine($"❌ Runtime error: {ex.Message}");
        Console.WriteLine("Array covariance is NOT type-safe!");
    }
}
```

### TODO 6: Safe Covariant Collection

```csharp
public static IReadOnlyList<Animal> GetSafeAnimalList()
{
    List<Dog> dogs = new()
    {
        new Dog("Rex", 5, "German Shepherd"),
        new Dog("Max", 3, "Golden Retriever"),
        new Dog("Buddy", 4, "Beagle")
    };

    return dogs;  // IReadOnlyList<out T> is covariant and safe!
}
```

## Complexity Analysis

| Operation | Time | Space | Notes |
|-----------|------|-------|-------|
| Covariant assignment | O(1) | O(1) | Compile-time only |
| IEnumerable iteration | O(n) | O(1) | No extra allocations |
| Array covariance | O(1) | O(1) | Runtime type check |

## Interview Favorites

✅ **Covariance vs Contravariance** (Very Common)
- Covariance: `out` - return types - "produces"
- Contravariance: `in` - parameters - "consumes"

✅ **Why can't IList<T> be covariant?**
- Has `Add(T item)` - input position
- Would break type safety

✅ **Array covariance problems**
- Historical feature (pre-generics)
- Not type-safe at compile time
- Use `IEnumerable<T>` instead

## Real-World Usage

### 1. Repository Pattern
```csharp
public interface IRepository<out T> where T : Entity
{
    T GetById(int id);
    IEnumerable<T> GetAll();
}

// Now you can do:
IRepository<Dog> dogRepo = new DogRepository();
IRepository<Animal> animalRepo = dogRepo;  // Covariance!
```

### 2. Factory Pattern
```csharp
public interface IFactory<out T>
{
    T Create();
}

IFactory<Dog> dogFactory = new DogFactory();
IFactory<Animal> animalFactory = dogFactory;  // Covariance!
```

### 3. LINQ Queries
```csharp
IEnumerable<Dog> dogs = GetDogs();
IEnumerable<Animal> animals = dogs;  // Covariance!
var oldAnimals = animals.Where(a => a.Age > 5);  // Works!
```

**Congratulations! 🎉**

You've mastered **Covariance** in C#!

**Next**: Contravariance (`in` keyword)
