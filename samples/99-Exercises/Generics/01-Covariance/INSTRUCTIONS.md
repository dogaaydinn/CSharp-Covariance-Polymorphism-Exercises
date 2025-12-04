# Covariance Exercise

## 📚 Learning Objectives
- **Covariance**: Using `out` modifier for type parameters
- **Type Safety**: Understanding when covariance is safe vs unsafe
- **Built-in Covariance**: `IEnumerable<out T>`, `Func<out T>`, `IReadOnlyList<out T>`
- **Array Covariance**: Historical feature and its dangers
- **Practical Patterns**: Repository pattern with covariance

## 🎯 Exercise Tasks

Complete **6 TODO methods**:
1. ✅ **ICovariantRepository<out T>** - Define covariant interface
2. ✅ **AnimalRepository** - Implement covariant repository
3. ✅ **ConvertToAnimals()** - IEnumerable covariance
4. ✅ **GetAnimalFactory()** - Func delegate covariance
5. ✅ **DemonstrateArrayCovariance()** - Array covariance dangers
6. ✅ **GetSafeAnimalList()** - Safe covariance with IReadOnlyList

## 🚀 Getting Started

```bash
cd samples/99-Exercises/Generics/01-Covariance
dotnet test  # Should see ~15 FAILED tests
```

## 🧠 What is Covariance?

**Covariance** allows you to use a **more derived type** than originally specified.

```csharp
// Example: IEnumerable<Dog> → IEnumerable<Animal>
IEnumerable<Dog> dogs = GetDogs();
IEnumerable<Animal> animals = dogs;  // ✅ Covariance!
```

**Key Rule**: Covariance works when the type parameter appears only in **OUTPUT positions** (return types).

### The `out` Keyword

```csharp
public interface IProducer<out T>  // 'out' = covariant
{
    T Get();              // ✅ OK - T in output position
    IEnumerable<T> GetAll(); // ✅ OK - T in output position

    void Add(T item);     // ❌ ERROR - T in input position
}
```

**Why?** If `IProducer<Dog>` can be assigned to `IProducer<Animal>`, then:
- `Get()` returns `Dog`, which is also an `Animal` ✅
- `Add(Animal)` would accept `Cat`, breaking type safety ❌

## 💡 Quick Hints

### TODO 1: ICovariantRepository<out T>

```csharp
public interface ICovariantRepository<out T>
{
    T Get(int id);           // Returns T - OK for covariance
    IEnumerable<T> GetAll(); // Returns IEnumerable<T> - OK

    // ❌ Don't add: void Add(T item) - breaks covariance!
}
```

**Why `out`?**
- Repository only **produces** items (output)
- Never **consumes** items (no input)
- Safe for `ICovariantRepository<Dog>` → `ICovariantRepository<Animal>`

### TODO 2: AnimalRepository

```csharp
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

### TODO 3: ConvertToAnimals()

```csharp
public static IEnumerable<Animal> ConvertToAnimals<T>(IEnumerable<T> items)
    where T : Animal
{
    return items;  // That's it! IEnumerable<out T> is covariant
}
```

**Why does this work?**
- `IEnumerable<out T>` is covariant
- `IEnumerable<Dog>` is automatically `IEnumerable<Animal>`
- No casting needed!

### TODO 4: GetAnimalFactory()

```csharp
public static Func<Animal> GetAnimalFactory(string animalType)
{
    // Func<out TResult> is covariant in return type
    if (animalType == "Dog")
    {
        Func<Dog> dogFactory = () => new Dog("Rex", 5, "Labrador");
        return dogFactory;  // Func<Dog> → Func<Animal> (covariance!)
    }

    if (animalType == "Cat")
    {
        Func<Cat> catFactory = () => new Cat("Whiskers", 2, true);
        return catFactory;  // Func<Cat> → Func<Animal>
    }

    Func<Bird> birdFactory = () => new Bird("Tweety", 1, true);
    return birdFactory;  // Func<Bird> → Func<Animal>
}
```

**Key**: `Func<out TResult>` is covariant because it only **produces** results.

### TODO 5: DemonstrateArrayCovariance()

```csharp
public static void DemonstrateArrayCovariance()
{
    // Arrays are covariant (legacy feature from before generics)
    Dog[] dogs = new Dog[]
    {
        new Dog("Rex", 5, "German Shepherd"),
        new Dog("Max", 3, "Golden Retriever")
    };

    // This compiles! Array covariance
    Animal[] animals = dogs;

    Console.WriteLine("Array covariance allows Dog[] → Animal[]");
    Console.WriteLine($"First animal: {animals[0].Name}");

    try
    {
        // ⚠️ DANGER: This compiles but throws at runtime!
        animals[0] = new Cat("Whiskers", 2, true);
        // ArrayTypeMismatchException: Attempted to access an element as a type
        // incompatible with the array.
    }
    catch (ArrayTypeMismatchException ex)
    {
        Console.WriteLine($"\n❌ Runtime error: {ex.Message}");
        Console.WriteLine("Array covariance is NOT type-safe!");
        Console.WriteLine("Use IEnumerable<T> or IReadOnlyList<T> instead.");
    }
}
```

**Key Learning**: Array covariance is **not type-safe** at compile time!

### TODO 6: GetSafeAnimalList()

```csharp
public static IReadOnlyList<Animal> GetSafeAnimalList()
{
    // Create a list of dogs
    List<Dog> dogs = new()
    {
        new Dog("Rex", 5, "German Shepherd"),
        new Dog("Max", 3, "Golden Retriever"),
        new Dog("Buddy", 4, "Beagle")
    };

    // IReadOnlyList<out T> is covariant and safe
    IReadOnlyList<Animal> animals = dogs;  // ✅ Covariance!

    return animals;
}
```

**Why is this safe?**
- `IReadOnlyList<out T>` has **no Add/Set methods**
- Only allows reading (output-only)
- Can't violate type safety by adding wrong types

## 📊 Covariance vs Arrays

| Feature | Arrays | IEnumerable<T> | IReadOnlyList<T> |
|---------|--------|----------------|------------------|
| **Covariant?** | ✅ Yes | ✅ Yes | ✅ Yes |
| **Compile-time safe?** | ❌ No | ✅ Yes | ✅ Yes |
| **Can modify?** | ✅ Yes (dangerous!) | ❌ No (read-only) | ❌ No (read-only) |
| **Runtime check?** | ✅ Yes | N/A | N/A |
| **Recommended?** | ❌ No | ✅ Yes | ✅ Yes |

**Key Takeaway**: Use `IEnumerable<T>` or `IReadOnlyList<T>`, not arrays!

## 🎓 Interview Tips

### When to Use Covariance?

✅ **Use covariance when:**
1. Type parameter only appears in **output positions** (return types)
2. Need to return more derived types as base types
3. Working with read-only collections (`IEnumerable<T>`, `IReadOnlyList<T>`)
4. Using `Func<out TResult>` delegates

❌ **Don't use covariance when:**
1. Type parameter appears in **input positions** (method parameters)
2. Need to modify the collection
3. Type safety can't be guaranteed at compile time

### Common Covariant Interfaces in .NET

```csharp
IEnumerable<out T>          // Read-only sequence
IEnumerator<out T>          // Iterator
IReadOnlyList<out T>        // Read-only indexed collection
IReadOnlyCollection<out T>  // Read-only sized collection
IGrouping<out TKey, out TElement>  // LINQ grouping

// Delegates
Func<out TResult>           // Function with return value
IObservable<out T>          // Rx observable
```

### Interview Question: "Why can't IList<T> be covariant?"

**Answer**:
```csharp
// If IList<T> were covariant:
IList<Dog> dogs = new List<Dog>();
IList<Animal> animals = dogs;  // Hypothetical covariance
animals.Add(new Cat(...));     // ❌ Would break type safety!
// Now dogs list contains a Cat!
```

`IList<T>` has `Add(T item)` method (input position), so it **cannot** be covariant.

## 🔍 Common Mistakes

### Mistake 1: Trying to make IList<T> covariant

```csharp
// ❌ WRONG - IList<T> is NOT covariant
IList<Dog> dogs = new List<Dog>();
IList<Animal> animals = dogs;  // Compile error!
```

**Fix**: Use `IEnumerable<T>` or `IReadOnlyList<T>`:
```csharp
// ✅ CORRECT
IReadOnlyList<Dog> dogs = new List<Dog>();
IReadOnlyList<Animal> animals = dogs;  // Works!
```

### Mistake 2: Using array covariance unsafely

```csharp
// ❌ WRONG - Compiles but throws at runtime
Dog[] dogs = new Dog[2];
Animal[] animals = dogs;
animals[0] = new Cat(...);  // ArrayTypeMismatchException!
```

**Fix**: Use generic collections:
```csharp
// ✅ CORRECT - Compile-time safe
List<Dog> dogs = new();
IEnumerable<Animal> animals = dogs;  // Safe!
// animals has no Add method - type-safe!
```

### Mistake 3: Adding `in` and `out` together incorrectly

```csharp
// ❌ WRONG - Can't be both covariant and contravariant
public interface IBadInterface<in T, out T>  // T appears twice!
{
    T Process(T input);  // Compile error!
}
```

**Fix**: Use different type parameters:
```csharp
// ✅ CORRECT
public interface IGoodInterface<in TInput, out TOutput>
{
    TOutput Process(TInput input);  // OK!
}
```

## 📚 Resources

- [Covariance and Contravariance in C# (Microsoft Docs)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/)
- [Understanding Variance in C# (Eric Lippert's Blog)](https://ericlippert.com/category/covariance-and-contravariance/)

---

**Good luck! 🎉** Check `SOLUTION.md` after trying.
