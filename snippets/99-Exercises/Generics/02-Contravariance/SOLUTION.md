# ⚠️ SPOILER WARNING ⚠️

**DO NOT READ UNTIL YOU'VE TRIED YOURSELF!**

---

# Contravariance - Complete Solutions

All solutions are provided in INSTRUCTIONS.md hints section.

## Key Takeaways

1. **Contravariance Basics**:
   - Use `in` keyword for type parameters
   - Type can only appear in INPUT positions (parameters)
   - Enables `IProcessor<Animal>` → `IProcessor<Dog>`
   - "If you can handle Animal, you can handle Dog"

2. **Built-in Contravariant Types**:
   - `Action<in T>` - Most common
   - `IComparer<in T>` - Comparisons
   - `EventHandler<in TEventArgs>` - Events
   - `Func<in T, out TResult>` - Combined variance!

3. **Combined Variance**:
   - `Func<in T, out TResult>` uses BOTH
   - Contravariant in input (T)
   - Covariant in output (TResult)

4. **Type Safety Rules**:
   - ✅ Safe: `Action<Animal>` → `Action<Dog>`
   - ❌ Unsafe: `Func<Animal>` → `Func<Dog>` (covariance, not contravariance)
   - ✅ Safe: `IComparer<Animal>` → `IComparer<Dog>`

## Complete Code Examples

See INSTRUCTIONS.md for full implementations.

## Complexity Analysis

| Operation | Time | Space | Notes |
|-----------|------|-------|-------|
| Contravariant assignment | O(1) | O(1) | Compile-time only |
| Action invocation | O(1) | O(1) | Normal method call |
| IComparer.Compare | O(1) | O(1) | Depends on comparison |

## Real-World Usage

### 1. Generic Processors
```csharp
public interface IProcessor<in T>
{
    void Process(T item);
}

IProcessor<Animal> animalProcessor = new AnimalProcessor();
IProcessor<Dog> dogProcessor = animalProcessor;  // Contravariance!
```

### 2. Event Handlers
```csharp
EventHandler<AnimalEventArgs> handler = (s, e) => { /* handle */ };
EventHandler<DogEventArgs> dogHandler = handler;  // If DogEventArgs : AnimalEventArgs
```

### 3. LINQ Comparers
```csharp
var animals = new List<Animal> { /* ... */ };
animals.Sort(Comparer<Animal>.Create((x, y) => x.Age.CompareTo(y.Age)));
```

**Congratulations! 🎉**

You've mastered **Contravariance** in C#!

**Next**: Generic Constraints (`where T : ...`)
