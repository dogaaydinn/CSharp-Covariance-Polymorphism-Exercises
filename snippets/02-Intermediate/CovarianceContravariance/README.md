# Covariance and Contravariance

## Learning Objectives

By completing this tutorial, you will:
- ✅ Understand what variance means in generics
- ✅ Master covariance (out T) for return types
- ✅ Master contravariance (in T) for parameter types
- ✅ Understand why invariance exists
- ✅ Apply variance in real-world scenarios

## What is Variance?

**Variance** refers to how subtyping between complex types relates to subtyping between their components.

### The Three Types of Variance

| Type | Keyword | Use Case | Example |
|------|---------|----------|---------|
| **Covariance** | `out` | Return types only | `IEnumerable<out T>` |
| **Contravariance** | `in` | Parameter types only | `IComparer<in T>` |
| **Invariance** | none | Both input & output | `IList<T>` |

---

## Covariance (out T)

**Covariance** allows you to use a more derived type than originally specified.

### Real-World Analogy

Think of a **dog shelter**:
- The shelter returns dogs (produces)
- If something expects animals, dogs work fine (dogs ARE animals)
- You can treat a "dog producer" as an "animal producer"

### Code Example

```csharp
// IEnumerable<out T> is covariant
IEnumerable<Dog> dogs = GetDogs();
IEnumerable<Animal> animals = dogs;  // ✓ Works! Covariance

foreach (var animal in animals)
{
    animal.Eat();  // Calls Dog.Eat() polymorphically
}
```

### Why It Works

```csharp
public interface IEnumerable<out T>  // 'out' = covariant
{
    IEnumerator<T> GetEnumerator();  // T only in OUTPUT position
}
```

**Rules:**
- ✅ Can return `T`
- ❌ Cannot accept `T` as parameter
- Why? You can safely read `Dog` as `Animal` (upcasting)

### Common Covariant Interfaces

```csharp
IEnumerable<out T>
IEnumerator<out T>
IQueryable<out T>
IGrouping<out TKey, out TElement>
Func<out TResult>
Task<out TResult>
IObservable<out T>
```

---

## Contravariance (in T)

**Contravariance** allows you to use a more generic type than originally specified.

### Real-World Analogy

Think of a **universal pet feeder**:
- The feeder can feed any animal
- If you need something that feeds dogs, an animal feeder works
- More general capability can handle more specific needs

### Code Example

```csharp
// IComparer<in T> is contravariant
IComparer<Animal> animalComparer = new AnimalNameComparer();
IComparer<Dog> dogComparer = animalComparer;  // ✓ Works! Contravariance

List<Dog> dogs = GetDogs();
dogs.Sort(dogComparer);  // Uses AnimalComparer to sort Dogs
```

### Why It Works

```csharp
public interface IComparer<in T>  // 'in' = contravariant
{
    int Compare(T x, T y);  // T only in INPUT position
}
```

**Rules:**
- ✅ Can accept `T` as parameter
- ❌ Cannot return `T`
- Why? If it can compare animals, it can compare dogs (dogs are animals)

### Common Contravariant Interfaces

```csharp
IComparer<in T>
IEqualityComparer<in T>
IComparable<in T>
Action<in T>
Predicate<in T>
Comparison<in T>
```

---

## Invariance (No Variance)

**Invariance** means you must use the exact type - no conversion allowed.

### Why Invariance Exists

```csharp
public interface IList<T>  // No 'in' or 'out' = invariant
{
    T this[int index] { get; set; }  // T in BOTH positions
    void Add(T item);                 // T as input
    T RemoveAt(int index);            // T as output
}
```

**The Problem If It Was Variant:**

```csharp
// Imagine this was allowed (IT'S NOT!)
IList<Dog> dogs = new List<Dog>();
IList<Animal> animals = dogs;  // ❌ NOT allowed (would break type safety)

// If it was allowed:
animals.Add(new Cat());  // Would add Cat to Dog list!
Dog dog = dogs[0];       // Would retrieve Cat as Dog - RUNTIME ERROR! 💥
```

**Therefore:** `IList<T>` is **invariant** to prevent type safety violations.

### Common Invariant Interfaces

```csharp
IList<T>
ICollection<T>
IDictionary<TKey, TValue>
List<T>
Dictionary<TKey, TValue>
```

---

## Memory Aid: PECS Principle

**Producer Extends, Consumer Super**

Or in C# terms:
- **Producers** (return T) → `out` → **Covariant**
- **Consumers** (accept T) → `in` → **Contravariant**

```csharp
// Producer - returns T
interface IProducer<out T>
{
    T Produce();  // ✓ Returns T
}

// Consumer - accepts T
interface IConsumer<in T>
{
    void Consume(T item);  // ✓ Accepts T
}

// Both - cannot have variance
interface IRepository<T>
{
    T Get();      // Returns T (wants 'out')
    void Add(T);  // Accepts T (wants 'in')
    // Result: INVARIANT (no variance keyword)
}
```

---

## Running the Sample

```bash
cd samples/02-Intermediate/CovarianceContravariance
dotnet run
```

## Code Walkthrough

### Example 1: Covariance

```csharp
List<Dog> dogs = new() { new Dog { Name = "Buddy" } };

// Covariance: IEnumerable<Dog> → IEnumerable<Animal>
IEnumerable<Animal> animals = dogs;  // ✓ Works!

foreach (var animal in animals)
{
    Console.WriteLine(animal.Name);  // Polymorphism in action
}
```

**Key Point:** `IEnumerable<out T>` only returns items, so it's safe to treat `Dog` as `Animal`.

---

### Example 2: Contravariance

```csharp
class AnimalComparer : IComparer<Animal>
{
    public int Compare(Animal x, Animal y)
        => string.Compare(x.Name, y.Name);
}

IComparer<Animal> animalComparer = new AnimalComparer();

// Contravariance: IComparer<Animal> → IComparer<Dog>
IComparer<Dog> dogComparer = animalComparer;  // ✓ Works!

List<Dog> dogs = GetDogs();
dogs.Sort(dogComparer);  // Uses animal comparer for dogs
```

**Key Point:** If it can compare animals, it can compare dogs (since dogs are animals).

---

### Example 3: Custom Interfaces

```csharp
// Covariant - producer
interface IProducer<out T>
{
    T Produce();
}

// Contravariant - consumer
interface IConsumer<in T>
{
    void Consume(T item);
}

// Usage
IProducer<Dog> dogProducer = new DogShelter();
IProducer<Animal> animalProducer = dogProducer;  // ✓ Covariance

IConsumer<Animal> animalConsumer = new AnimalFeeder();
IConsumer<Dog> dogConsumer = animalConsumer;  // ✓ Contravariance
```

---

## Real-World Use Cases

### 1. Repository Pattern

```csharp
// Read-only repository (covariant)
interface IReadOnlyRepository<out T>
{
    IEnumerable<T> GetAll();
    T GetById(int id);
}

// Usage
IReadOnlyRepository<Dog> dogRepo = new DogRepository();
IReadOnlyRepository<Animal> animalRepo = dogRepo;  // ✓ Covariance
```

### 2. Validators (Contravariant)

```csharp
interface IValidator<in T>
{
    bool Validate(T item);
}

class AnimalValidator : IValidator<Animal>
{
    public bool Validate(Animal animal)
        => !string.IsNullOrEmpty(animal.Name);
}

// Use animal validator for dogs
IValidator<Animal> animalValidator = new AnimalValidator();
IValidator<Dog> dogValidator = animalValidator;  // ✓ Contravariance
```

### 3. Event Handlers

```csharp
// Contravariant event handlers
Action<Animal> handleAnimal = (animal) => Console.WriteLine(animal.Name);
Action<Dog> handleDog = handleAnimal;  // ✓ Works!

handleDog(new Dog { Name = "Rex" });  // Calls animal handler with dog
```

---

## Common Mistakes

### Mistake 1: Trying to Make IList Covariant

```csharp
// ❌ This doesn't compile
IList<Dog> dogs = new List<Dog>();
IList<Animal> animals = dogs;  // ERROR: Cannot convert

// ✅ Use IEnumerable for read-only
IEnumerable<Animal> readOnlyAnimals = dogs;  // OK!
```

### Mistake 2: Wrong Variance Direction

```csharp
// ❌ Wrong direction
IEnumerable<Animal> animals = GetAnimals();
IEnumerable<Dog> dogs = animals;  // ERROR: Would require contravariance

// ✅ Correct direction
IEnumerable<Dog> dogs = GetDogs();
IEnumerable<Animal> animals = dogs;  // OK! Covariance
```

### Mistake 3: Confusing in/out Keywords

```csharp
// ❌ Wrong
interface IProducer<in T>  // 'in' is for consumers!
{
    T Produce();  // ERROR: Can't return T in contravariant interface
}

// ✅ Correct
interface IProducer<out T>  // 'out' for producers
{
    T Produce();  // OK!
}
```

---

## Quick Reference Table

| Interface | Variance | Can Convert | Example |
|-----------|----------|-------------|---------|
| `IEnumerable<out T>` | Covariant | `IEnumerable<Dog>` → `IEnumerable<Animal>` | ✅ |
| `IComparer<in T>` | Contravariant | `IComparer<Animal>` → `IComparer<Dog>` | ✅ |
| `IList<T>` | Invariant | `IList<Dog>` ↔ `IList<Animal>` | ❌ |
| `Func<out TResult>` | Covariant | `Func<Dog>` → `Func<Animal>` | ✅ |
| `Action<in T>` | Contravariant | `Action<Animal>` → `Action<Dog>` | ✅ |

---

## Key Takeaways

### When to Use Variance

✅ **Use covariance (`out`) when:**
- Your interface only returns `T`
- You're creating a producer/factory
- Example: repositories, queries, observables

✅ **Use contravariance (`in`) when:**
- Your interface only accepts `T`
- You're creating a consumer/handler
- Example: comparers, validators, event handlers

❌ **Cannot use variance when:**
- Type appears in both input and output
- You need both read and write operations
- Example: collections, dictionaries, lists

### Type Safety Rules

Remember:
- **Covariance** = Can substitute with MORE SPECIFIC return type
- **Contravariance** = Can substitute with MORE GENERAL parameter type
- **Invariance** = Must use EXACT type

---

## Further Reading

- [Covariance and Contravariance (C# Programming Guide)](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/)
- [Variance in Generic Interfaces](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/variance-in-generic-interfaces)
- [Creating Variant Generic Interfaces](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/creating-variant-generic-interfaces)

---

## Next Steps

After completing this tutorial, move on to:
- [BoxingPerformance](../BoxingPerformance/) - Value types and boxing
- [GenericConstraints](../GenericConstraints/) - where T : constraints

---

**Completed?** ✅ Mark this in your learning tracker!
