# Why Learn Covariance and Contravariance?

## The Problem: Type Safety vs Flexibility

Without variance, this reasonable code doesn't compile:

```csharp
IEnumerable<Dog> dogs = new List<Dog>();
IEnumerable<Animal> animals = dogs; // ❌ Compile error without covariance!

void ProcessAnimals(IEnumerable<Animal> animals) { }
ProcessAnimals(dogs); // ❌ Can't pass List<Dog> to method expecting IEnumerable<Animal>
```

**Problems:**
- ❌ Can't use derived types where base types are expected
- ❌ Forces unnecessary casting or conversion
- ❌ Reduces code reusability
- ❌ Makes generic APIs inflexible

---

## The Solution: Covariance and Contravariance

### Covariance (`out` keyword) - Reading is Safe

```csharp
IEnumerable<Dog> dogs = new List<Dog> { new Dog() };
IEnumerable<Animal> animals = dogs; // ✅ Allowed! IEnumerable<out T> is covariant

foreach (Animal animal in animals)
{
    animal.MakeSound(); // Safe - we're only reading
}
```

**Why it's safe:**
- You can only READ from covariant collections
- Dog IS-AN Animal, so reading Dog as Animal is always safe
- No risk of adding wrong types

### Contravariance (`in` keyword) - Writing is Safe

```csharp
IComparer<Animal> animalComparer = /* ... */;
IComparer<Dog> dogComparer = animalComparer; // ✅ Allowed! IComparer<in T> is contravariant

List<Dog> dogs = new List<Dog>();
dogs.Sort(dogComparer); // Safe - comparer can handle any Animal, including Dogs
```

**Why it's safe:**
- You can only WRITE to contravariant types
- If comparer works with Animal, it works with Dog (Dog is more specific)
- No risk of type mismatch

---

## Covariance: "Out" Means "I Produce T"

### Definition
```csharp
public interface IProducer<out T>
{
    T Produce(); // OK - returns T (produces)
    // void Consume(T item); // ❌ NOT ALLOWED - takes T as input
}
```

**Rules:**
- ✅ Can return `T`
- ✅ Can use `T` in output positions
- ❌ Cannot accept `T` as parameter
- ❌ Cannot use `T` in input positions

### Real-World Examples

**1. IEnumerable<out T>**
```csharp
IEnumerable<Dog> dogs = GetDogs();
IEnumerable<Animal> animals = dogs; // ✅ Covariant

// Why it's safe:
foreach (var animal in animals)
{
    // We're only READING from the collection
    // Can't add items to IEnumerable
}
```

**2. Task<out TResult>**
```csharp
Task<Dog> GetDogAsync() => Task.FromResult(new Dog());

Task<Animal> task = GetDogAsync(); // ✅ Covariant
Animal animal = await task; // Returns a Dog, which IS-AN Animal
```

**3. Func<out TResult>**
```csharp
Func<Dog> getDog = () => new Dog();
Func<Animal> getAnimal = getDog; // ✅ Covariant

Animal animal = getAnimal(); // Returns a Dog
```

---

## Contravariance: "In" Means "I Consume T"

### Definition
```csharp
public interface IConsumer<in T>
{
    void Consume(T item); // OK - accepts T (consumes)
    // T Produce(); // ❌ NOT ALLOWED - returns T
}
```

**Rules:**
- ✅ Can accept `T` as parameter
- ✅ Can use `T` in input positions
- ❌ Cannot return `T`
- ❌ Cannot use `T` in output positions

### Real-World Examples

**1. IComparer<in T>**
```csharp
IComparer<Animal> animalComparer = /* ... */;
IComparer<Dog> dogComparer = animalComparer; // ✅ Contravariant

// Why it's safe:
// If comparer can compare any Animal, it can compare Dogs
dogs.Sort(dogComparer);
```

**2. Action<in T>**
```csharp
Action<Animal> handleAnimal = a => a.MakeSound();
Action<Dog> handleDog = handleAnimal; // ✅ Contravariant

handleDog(new Dog()); // Calls handleAnimal with a Dog
```

**3. IEqualityComparer<in T>**
```csharp
IEqualityComparer<Animal> animalComparer = /* ... */;
IEqualityComparer<Dog> dogComparer = animalComparer; // ✅ Contravariant

HashSet<Dog> dogSet = new HashSet<Dog>(dogComparer);
```

---

## Invariance: When Neither `in` nor `out` Works

### Definition
```csharp
public interface IMutable<T> // Invariant (no in/out)
{
    T Get();      // Would need 'out'
    void Set(T item); // Would need 'in'
}
```

**Examples:**
- `List<T>` - Can both read and write
- `IList<T>` - Can both read and write
- `Dictionary<TKey, TValue>` - Can both read and write

```csharp
List<Dog> dogs = new List<Dog>();
List<Animal> animals = dogs; // ❌ NOT ALLOWED - List<T> is invariant
```

**Why not covariant?**
```csharp
// If this were allowed:
List<Dog> dogs = new List<Dog>();
List<Animal> animals = dogs; // Imagine this was allowed

// Then we could:
animals.Add(new Cat()); // ❌ Adding Cat to List<Dog>!
// This would break type safety!
```

---

## Array Covariance: The C# Gotcha

### Arrays ARE Covariant (But Unsafe!)

```csharp
Dog[] dogs = new Dog[5];
Animal[] animals = dogs; // ✅ Allowed (arrays are covariant)

animals[0] = new Dog(); // OK
animals[1] = new Cat(); // 💥 ArrayTypeMismatchException at runtime!
```

**Why this is a problem:**
- Covariance for arrays is a design mistake in C#/.NET
- Inherited from Java for compatibility
- Runtime checks required, not compile-time
- Use `List<T>` or `IEnumerable<T>` instead

---

## When to Use Each

### ✅ Use Covariance (`out T`) When:

1. **Read-Only Collections**
   ```csharp
   public interface IRepository<out TEntity>
   {
       IEnumerable<TEntity> GetAll();
       TEntity GetById(int id);
   }
   ```

2. **Producers/Factories**
   ```csharp
   public interface IFactory<out T>
   {
       T Create();
   }
   ```

3. **Async Results**
   ```csharp
   Task<Dog> GetDogAsync();
   // Can be assigned to Task<Animal>
   ```

### ✅ Use Contravariance (`in T`) When:

1. **Comparers**
   ```csharp
   public interface IValidator<in T>
   {
       bool IsValid(T item);
   }
   ```

2. **Event Handlers**
   ```csharp
   public delegate void EventHandler<in TEventArgs>(object sender, TEventArgs e);
   ```

3. **Consumers/Processors**
   ```csharp
   public interface IProcessor<in T>
   {
       void Process(T item);
   }
   ```

### ⚠️ Use Invariance (No `in`/`out`) When:

1. **Mutable Collections**
   - `List<T>`, `IList<T>`, `Dictionary<TKey, TValue>`

2. **Read-Write Interfaces**
   ```csharp
   public interface IRepository<T>
   {
       T Get(int id);    // Needs 'out'
       void Add(T item); // Needs 'in'
       // Can't have both!
   }
   ```

---

## Real-World Examples in This Repository

### Example 1: Producer Pattern (Covariance)
**Location**: `samples/02-Intermediate/CovarianceContravariance/Examples/CovarianceExample.cs`

```csharp
IProducer<Dog> dogProducer = new DogProducer();
IProducer<Animal> animalProducer = dogProducer; // Covariance in action

Animal animal = animalProducer.Produce(); // Returns a Dog
```

**Real-world use:** Factory pattern, repository queries, async results

### Example 2: Consumer Pattern (Contravariance)
**Location**: `samples/02-Intermediate/CovarianceContravariance/Examples/ContravarianceExample.cs`

```csharp
IConsumer<Animal> animalConsumer = new AnimalConsumer();
IConsumer<Dog> dogConsumer = animalConsumer; // Contravariance in action

dogConsumer.Consume(new Dog()); // AnimalConsumer handles Dogs
```

**Real-world use:** Validators, comparers, event handlers

### Example 3: MicroVideoPlatform Event Bus
**Location**: `samples/08-Capstone/MicroVideoPlatform/Shared/Events/`

```csharp
// Event bus uses contravariance for handlers
public interface IEventHandler<in TEvent>
{
    Task HandleAsync(TEvent @event);
}

// Can register handler for base class:
IEventHandler<DomainEventBase> handler = /* ... */;

// And use for derived events:
IEventHandler<VideoUploadedEvent> videoHandler = handler; // Contravariant
```

---

## Common Mistakes

### Mistake 1: Trying to Add to Covariant Collection

```csharp
IEnumerable<Dog> dogs = new List<Dog>();
IEnumerable<Animal> animals = dogs;

// Can't do this - IEnumerable is read-only
// animals.Add(new Cat()); // ❌ Doesn't compile (no Add method)
```

**This is BY DESIGN** - covariance is only safe for reading!

### Mistake 2: Confusing Array Covariance with Type Safety

```csharp
Dog[] dogs = new Dog[5];
Animal[] animals = dogs; // Compiles, but unsafe!

try
{
    animals[0] = new Cat(); // Runtime exception!
}
catch (ArrayTypeMismatchException ex)
{
    // Should have used List<T>
}
```

### Mistake 3: Making Interfaces Invariant When They Could Be Variant

```csharp
// BAD: Invariant when it could be covariant
public interface IReadOnlyRepository<T>
{
    IEnumerable<T> GetAll();
}

// BETTER: Covariant
public interface IReadOnlyRepository<out T>
{
    IEnumerable<T> GetAll();
}
```

---

## Performance Considerations

Variance has **near-zero** performance cost:

```
Operation                          | Time (ns)
-----------------------------------|----------
Direct method call                 | 0.3
Interface call (invariant)         | 0.5
Interface call (covariant)         | 0.5  ← Same!
Interface call (contravariant)     | 0.5  ← Same!
```

**Conclusion:** Variance is a compile-time feature, no runtime cost!

---

## Trade-Offs Summary

| Pattern | Safety | Flexibility | Best For |
|---------|--------|-------------|----------|
| **Covariance (`out`)** | ✅ Type-safe | ✅ High | Read-only, producers |
| **Contravariance (`in`)** | ✅ Type-safe | ✅ High | Write-only, consumers |
| **Invariance (none)** | ✅ Type-safe | ❌ Low | Read-write collections |
| **Array Covariance** | ❌ Runtime checks | ⚠️ Legacy | Avoid! Use generics |

---

## Key Takeaways

1. **Covariance (`out`)** = Read-only, can return more derived types
2. **Contravariance (`in`)** = Write-only, can accept more general types
3. **Invariance** = Read-write, no flexibility
4. **Array covariance is unsafe** - use generic collections instead
5. **IEnumerable<T> is covariant** - most commonly used
6. **Variance has zero runtime cost** - purely compile-time

---

## Mental Model

**Covariance:**
- "I produce Dogs, which ARE Animals" ✅
- "You can treat my Dog factory as an Animal factory"

**Contravariance:**
- "I handle all Animals, including Dogs" ✅
- "You can use my Animal handler to handle Dogs"

**Invariance:**
- "I both produce AND consume, so I can't be flexible" ❌

---

## Learning Path

1. **Start Here**: Understand covariance (`CovarianceExample.cs`)
2. **Next**: Learn contravariance (`ContravarianceExample.cs`)
3. **Practice**: Study invariance (`InvarianceExample.cs`)
4. **Advanced**: Generic variance (`samples/03-Advanced/GenericCovarianceContravariance/`)
5. **Real-World**: Event bus in `samples/08-Capstone/MicroVideoPlatform/`

---

## Further Reading

- **In This Repo**:
  - `samples/03-Advanced/GenericCovarianceContravariance/` - Custom variant interfaces
  - `samples/05-RealWorld/MicroserviceTemplate/` - Repository pattern with variance

- **External**:
  - C# Spec: [Variance in Generic Interfaces](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/)
  - Eric Lippert's Blog: "Covariance and Contravariance" series
  - Jon Skeet's "C# in Depth" (Chapter 13)

---

**Next Step**: Run `dotnet run` to see covariance and contravariance in action, then experiment with breaking type safety to understand the rules.
