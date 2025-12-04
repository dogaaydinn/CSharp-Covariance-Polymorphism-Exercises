# Contravariance Exercise

## 📚 Learning Objectives
- **Contravariance**: Using `in` modifier for type parameters
- **Type Safety**: Understanding when contravariance is safe
- **Built-in Contravariance**: `Action<in T>`, `IComparer<in T>`, `EventHandler<in T>`
- **Combined Variance**: `Func<in T, out TResult>`
- **Practical Patterns**: Comparers, processors, event handlers

## 🎯 Exercise Tasks

Complete **6 TODO methods**:
1. ✅ **IContravariantComparer<in T>** - Define contravariant interface
2. ✅ **AnimalWeightComparer** - Implement contravariant comparer
3. ✅ **ProcessAnimals()** - Action delegate contravariance
4. ✅ **GetAnimalNameGetter()** - Func combined variance
5. ✅ **SortDogsUsingAnimalComparer()** - IComparer contravariance
6. ✅ **GetDogEventHandler()** - EventHandler contravariance

## 🚀 Getting Started

```bash
cd samples/99-Exercises/Generics/02-Contravariance
dotnet test  # Should see ~13 FAILED tests
```

## 🧠 What is Contravariance?

**Contravariance** allows you to use a **less derived type** than originally specified.

```csharp
// Example: Action<Animal> → Action<Dog>
Action<Animal> processAnimal = animal => Console.WriteLine(animal.Name);
Action<Dog> processDog = processAnimal;  // ✅ Contravariance!
```

**Key Rule**: Contravariance works when the type parameter appears only in **INPUT positions** (method parameters).

### The `in` Keyword

```csharp
public interface IProcessor<in T>  // 'in' = contravariant
{
    void Process(T item);     // ✅ OK - T in input position
    bool Validate(T item);    // ✅ OK - T in input position

    T Get();                  // ❌ ERROR - T in output position
}
```

**Why?** If `IProcessor<Animal>` can be assigned to `IProcessor<Dog>`, then:
- `Process(Dog)` receives `Dog`, can handle as `Animal` ✅
- `Get()` returning `Animal` isn't necessarily a `Dog` ❌

## 💡 Quick Hints

### TODO 1 & 2: Contravariant Comparer

```csharp
public interface IContravariantComparer<in T>
{
    int Compare(T x, T y);
    bool Equals(T x, T y);
}

public class AnimalWeightComparer : IContravariantComparer<Animal>
{
    public int Compare(Animal x, Animal y)
    {
        return x.Weight.CompareTo(y.Weight);
    }

    public bool Equals(Animal x, Animal y)
    {
        return Math.Abs(x.Weight - y.Weight) < 0.01;
    }
}
```

### TODO 3: Action Contravariance

```csharp
public static void ProcessAnimals(Action<Animal> animalProcessor, List<Dog> dogs)
{
    foreach (var dog in dogs)
    {
        animalProcessor(dog);  // Contravariance: Dog → Animal
    }
}
```

### TODO 4: Func Combined Variance

```csharp
public static Func<Dog, string> GetAnimalNameGetter()
{
    // Func<in T, out TResult>
    // - Contravariant in T (Animal → Dog)
    // - Covariant in TResult (string stays string)
    Func<Animal, string> animalNameGetter = animal => animal.Name;
    return animalNameGetter;  // Contravariance in input!
}
```

### TODO 5: IComparer Contravariance

```csharp
public static List<Dog> SortDogsUsingAnimalComparer(List<Dog> dogs)
{
    var comparer = new AnimalWeightComparer();
    dogs.Sort(comparer);  // IComparer<Animal> → IComparer<Dog>
    return dogs;
}
```

### TODO 6: EventHandler Contravariance

```csharp
public static EventHandler<AnimalEventArgs> GetDogEventHandler()
{
    EventHandler<AnimalEventArgs> handler = (sender, e) =>
    {
        Console.WriteLine($"Event: {e.EventType}, Animal: {e.Animal.Name}");
    };
    return handler;
}
```

## 📊 Contravariance vs Covariance

| Feature | Covariance (`out`) | Contravariance (`in`) |
|---------|-------------------|----------------------|
| **Type parameter position** | OUTPUT (return types) | INPUT (parameters) |
| **Assignment direction** | More specific → Less specific | Less specific → More specific |
| **Example** | `IEnumerable<Dog>` → `IEnumerable<Animal>` | `Action<Animal>` → `Action<Dog>` |
| **Reasoning** | "If you want Animals, Dogs are fine" | "If you can handle Animals, you can handle Dogs" |

## 🎓 Interview Tips

### Common Contravariant Interfaces in .NET

```csharp
Action<in T>                    // Parameterless action
IComparer<in T>                 // Comparison
IEqualityComparer<in T>         // Equality
EventHandler<in TEventArgs>     // Event handling
Func<in T, out TResult>         // Function (both variance types!)
```

### Interview Question: "Why is Action<T> contravariant?"

**Answer**:
```csharp
// If Action<Animal> can be used as Action<Dog>:
Action<Animal> processAnimal = animal => { /* handle any animal */ };
Action<Dog> processDog = processAnimal;  // Contravariance!

processDog(new Dog(...));  // Calls processAnimal with a Dog
// Safe because: Dog IS-AN Animal, processAnimal can handle it!
```

### Interview Question: "What's the difference from covariance?"

**Answer**:
- **Covariance** (`out`): Return types - "producing" values - `IEnumerable<Dog>` → `IEnumerable<Animal>`
- **Contravariance** (`in`): Parameters - "consuming" values - `Action<Animal>` → `Action<Dog>`
- **Mnemonic**:
  - **OUT** = OUTput (covariant)
  - **IN** = INput (contravariant)

## 📚 Resources

- [Covariance and Contravariance (Microsoft Docs)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/)

---

**Good luck! 🎉** Check `SOLUTION.md` after trying.
