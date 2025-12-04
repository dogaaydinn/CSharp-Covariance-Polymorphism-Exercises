# Why Learn Type Casting?

## The Problem: Runtime Type Errors

Without proper casting knowledge, you'll write code that crashes:

```csharp
object obj = "Hello";
int number = (int)obj; // BOOM! InvalidCastException at runtime
```

**Problems:**
- ❌ Runtime crashes instead of compile-time errors
- ❌ No safety when working with polymorphic collections
- ❌ Hard to debug `InvalidCastException` in production
- ❌ Lost type information leads to bugs

---

## The Solution: Safe Casting Patterns

```csharp
// Pattern 1: 'is' operator (check before cast)
if (obj is string text)
{
    Console.WriteLine(text.Length); // Safe!
}

// Pattern 2: 'as' operator (returns null if fails)
string? text = obj as string;
if (text != null)
{
    Console.WriteLine(text.Length);
}

// Pattern 3: Pattern matching (C# 9+)
if (obj is string { Length: > 5 } longText)
{
    Console.WriteLine($"Long string: {longText}");
}
```

---

## When to Use Each Casting Approach

### ✅ Use `is` + Pattern Matching (Recommended)

```csharp
if (vehicle is Car { IsElectric: true } electricCar)
{
    electricCar.ChargeBattery();
}
```

**When:**
- Modern C# projects (C# 7.0+)
- You need to check type AND extract value
- You want most concise code
- Pattern matching on properties

**Benefits:**
- ✅ Most readable
- ✅ Type-safe
- ✅ Null-safe
- ✅ Can check properties inline

### ✅ Use `as` Operator

```csharp
var car = vehicle as Car;
if (car != null)
{
    car.Accelerate();
}
```

**When:**
- Need to attempt cast and handle null
- Working with reference types only
- Cast might legitimately fail
- Need nullable result

**Trade-offs:**
- ✅ No exception on failure
- ✅ Clear intent (cast or null)
- ❌ Only works with reference types
- ❌ Requires null check

### ⚠️ Use Direct Cast (Least Recommended)

```csharp
Car car = (Car)vehicle; // Throws if not a Car
```

**When:**
- You're 100% certain of the type
- Failure should crash (fail-fast)
- Performance-critical inner loops (avoid checks)

**Trade-offs:**
- ❌ Throws `InvalidCastException` if wrong
- ❌ Requires try-catch or certainty
- ✅ Fastest performance (no checks)
- ✅ Works with value types

### ❌ Don't Use: `GetType()` Checks

```csharp
// BAD: Exact type check, breaks inheritance
if (vehicle.GetType() == typeof(Car))
{
    ((Car)vehicle).Accelerate();
}
```

**Why bad:**
- Breaks polymorphism
- Won't work with derived types
- Violates Liskov Substitution Principle

---

## Alternatives to Casting

### Alternative 1: Polymorphism (Best)

```csharp
// Instead of casting:
if (vehicle is Car car)
{
    car.Accelerate();
}
else if (vehicle is Bike bike)
{
    bike.PedalFaster();
}

// Use polymorphism:
vehicle.Move(); // Each type implements Move() differently
```

**When to prefer:**
- Multiple types with shared behavior
- Open/Closed Principle compliance
- Extensible design

### Alternative 2: Visitor Pattern

```csharp
public interface IVehicleVisitor
{
    void Visit(Car car);
    void Visit(Bike bike);
}

vehicle.Accept(visitor); // Double dispatch, no casting!
```

**When to prefer:**
- Operations across many types
- Need to add operations without modifying types
- Type-safe dispatch

### Alternative 3: Pattern Matching with `switch`

```csharp
string description = vehicle switch
{
    Car { IsElectric: true } => "Electric car",
    Car => "Gas car",
    Bike => "Bicycle",
    _ => "Unknown vehicle"
};
```

**When to prefer:**
- Many type checks in one place
- Exhaustiveness checking
- Read-only operations

---

## Real-World Examples in This Repository

### Example 1: Safe Downcasting
**Location**: `samples/01-Beginner/CastingExamples/Examples/DowncastingExample.cs`

```csharp
Vehicle vehicle = new Car();
if (vehicle is Car car) // Safe downcast
{
    car.Accelerate(); // Car-specific method
}
```

**Why:**
- Runtime type might not be known
- Collection of mixed types
- Polymorphic behavior with type-specific actions

### Example 2: As Operator Usage
**Location**: `samples/01-Beginner/CastingExamples/Examples/AsOperatorExample.cs`

```csharp
var car = GetVehicle() as Car;
if (car != null)
{
    car.OpenTrunk();
}
```

**Why:**
- Cast might fail (GetVehicle could return Bike)
- Null check is natural flow control
- No exception handling needed

### Example 3: MicroVideoPlatform Event Handling
**Location**: `samples/08-Capstone/MicroVideoPlatform/`

```csharp
void HandleEvent(DomainEventBase baseEvent)
{
    if (baseEvent is VideoUploadedEvent uploadEvent)
    {
        ProcessUpload(uploadEvent);
    }
    else if (baseEvent is VideoProcessingCompletedEvent completedEvent)
    {
        NotifyUser(completedEvent);
    }
}
```

**Why:**
- Event bus receives base type
- Need to dispatch to specific handlers
- Type-safe with pattern matching

---

## Common Pitfalls

### Pitfall 1: Casting Value Types to Object (Boxing)

```csharp
int number = 42;
object obj = number;        // Boxing (heap allocation)
int result = (int)obj;      // Unboxing
```

**Problem:**
- ⚠️ Heap allocation for every cast
- ⚠️ GC pressure with many casts
- ⚠️ Performance degradation

**Solution:**
```csharp
// Use generics (no boxing)
List<int> numbers = new List<int>(); // No boxing
numbers.Add(42);
```

**See also:** `samples/02-Intermediate/BoxingPerformance/`

### Pitfall 2: Checking for null After Cast

```csharp
// BAD: Doesn't check for null
Car car = vehicle as Car;
car.Accelerate(); // NullReferenceException if vehicle isn't a Car!
```

**Solution:**
```csharp
if (vehicle is Car car)
{
    car.Accelerate(); // Safe, car is never null here
}
```

### Pitfall 3: Casting in Loops (Performance)

```csharp
// BAD: Cast inside loop
foreach (object obj in collection)
{
    if (obj is Car car)
    {
        car.Drive();
    }
}
```

**Better:**
```csharp
// Filter once, iterate typed collection
var cars = collection.OfType<Car>();
foreach (var car in cars)
{
    car.Drive();
}
```

---

## Performance Comparison

```
Operation                          | Time (ns) | Allocations
-----------------------------------|-----------|-------------
Direct method call                 | 0.3       | 0 bytes
Virtual method call (polymorphism) | 0.5       | 0 bytes
'is' operator check                | 1.2       | 0 bytes
'as' operator cast                 | 1.5       | 0 bytes
Direct cast (valid)                | 0.8       | 0 bytes
Direct cast (invalid, exception)   | 5000+     | ~100 bytes
```

**Conclusion:**
- Type checks are fast (1-2 nanoseconds)
- Exceptions are VERY slow (5000x slower)
- Always prefer safe casting patterns

---

## Trade-Offs Summary

| Pattern | Safety | Performance | Readability | Best For |
|---------|--------|-------------|-------------|----------|
| **`is` pattern** | ✅ High | ✅ Fast | ✅ Excellent | Modern C# code |
| **`as` operator** | ✅ High | ✅ Fast | ✅ Good | Nullable results |
| **Direct cast** | ❌ Low | ✅ Fastest | ⚠️ Risky | Known types only |
| **`GetType()`** | ⚠️ Medium | ⚠️ Slow | ❌ Poor | Avoid! |
| **Polymorphism** | ✅ Highest | ✅ Fast | ✅ Best | Shared behavior |

---

## When Casting Indicates Design Problems

If you find yourself casting a lot, consider:

1. **Too Many Casts → Need Polymorphism**
   ```csharp
   // BAD: Many casts
   if (vehicle is Car car) car.Drive();
   if (vehicle is Bike bike) bike.Pedal();

   // BETTER: Polymorphism
   vehicle.Move(); // Each type implements Move()
   ```

2. **Casting After Creation → Need Factory**
   ```csharp
   // BAD: Create then cast
   object vehicle = CreateVehicle();
   Car car = (Car)vehicle;

   // BETTER: Typed factory
   Car car = CarFactory.Create();
   ```

3. **Casting Collections → Need Generics**
   ```csharp
   // BAD: Non-generic collection
   ArrayList list = new ArrayList();
   foreach (object item in list)
   {
       if (item is int number) { }
   }

   // BETTER: Generic collection
   List<int> list = new List<int>();
   foreach (int number in list) { }
   ```

---

## Key Takeaways

1. **Prefer `is` pattern matching** for modern C# code
2. **Use `as` for nullable casts** when failure is expected
3. **Avoid direct casts** unless you're 100% certain
4. **Never use `GetType()`** for type checking
5. **Consider polymorphism** instead of casting
6. **Watch for boxing** when casting value types
7. **Exceptions are expensive** - avoid cast failures

---

## Learning Path

1. **Start Here**: Understand upcasting vs downcasting
2. **Next**: Master `is`, `as`, and direct casts (`IsOperatorExample.cs`, `AsOperatorExample.cs`)
3. **Then**: Learn pattern matching (`PatternMatchingExample.cs`)
4. **Practice**: Avoid common pitfalls (`CastingPitfallsExample.cs`)
5. **Advanced**: Study covariance impact on casting (`samples/02-Intermediate/CovarianceContravariance/`)
6. **Real-World**: See event casting in `samples/08-Capstone/MicroVideoPlatform/`

---

## Further Reading

- **In This Repo**:
  - `samples/01-Beginner/PolymorphismBasics/` - Understand base concept
  - `samples/02-Intermediate/BoxingPerformance/` - Boxing impact
  - `samples/03-Advanced/DesignPatterns/` - Alternatives to casting

- **External**:
  - Microsoft Docs: [Pattern Matching](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/pattern-matching)
  - "C# in Depth" by Jon Skeet (Chapter 4: C# Types)

---

**Next Step**: Run `dotnet run` to see all casting patterns in action, then experiment with breaking them to understand the errors.
