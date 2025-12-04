# Week 3 Assessment Test - Arrays, Covariance, and Collections

**Week**: 3 | **Duration**: 30 min | **Pass**: 70% | **Points**: 10

## Multiple Choice (5 pts)

1. What is array covariance in C#?
   - a) Arrays of derived types can be assigned to arrays of base types
   - b) Arrays can change size dynamically
   - c) Arrays can hold multiple types
   - d) Arrays are always type-safe

2. Which exception is thrown when array covariance is violated at runtime?
   - a) NullReferenceException
   - b) ArrayTypeMismatchException
   - c) InvalidCastException
   - d) IndexOutOfRangeException

3. Which is type-safe at compile-time?
   - a) `Animal[] animals = new Dog[5];`
   - b) `List<Animal> animals = new List<Dog>();`
   - c) Both
   - d) Neither

4. What does `IEnumerable<T>` provide?
   - a) Random access by index
   - b) Forward-only iteration
   - c) Sorting capabilities
   - d) Type conversion

5. Which collection allows duplicate keys?
   - a) Dictionary<TKey, TValue>
   - b) HashSet<T>
   - c) List<T>
   - d) None

## Short Answer (4.5 pts)

6. (1.5 pts) Explain why `List<Dog>` cannot be assigned to `List<Animal>` but `Dog[]` can be assigned to `Animal[]`.

7. (1.5 pts) What is the difference between `IEnumerable<T>` and `List<T>`? When would you use each?

8. (1.5 pts) Write LINQ code to filter a list of products where price > 100 and sort by name.

## Code Analysis (1.5 pts)

9. What's wrong with this code?
```csharp
Animal[] animals = new Dog[3];
animals[0] = new Dog();
animals[1] = new Cat(); // Problem here?
```

## Answer Key

1. **a** | 2. **b** | 3. **a** (generic lists are invariant) | 4. **b** | 5. **c**

6. Arrays are covariant (reference type arrays), but generic collections are invariant for type safety. Array covariance can cause runtime errors.

7. `IEnumerable<T>` is interface for iteration (deferred execution in LINQ). `List<T>` is concrete collection with indexing, Add, Remove. Use IEnumerable for queries, List for manipulation.

8. `products.Where(p => p.Price > 100).OrderBy(p => p.Name)`

9. Line with Cat throws `ArrayTypeMismatchException` at runtime. Array is Dog[], can't store Cat. Fix: Use `Animal[] animals = new Animal[3];`

**Resources**: `samples/02-Intermediate/ArrayCovariance/`, LINQ/01-BasicQueries
