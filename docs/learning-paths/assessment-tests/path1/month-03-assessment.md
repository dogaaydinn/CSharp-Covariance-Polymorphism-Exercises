# Month 3 Comprehensive Assessment - Generics & Design Patterns

**Month**: 3 (Weeks 9-12) | **Duration**: 90 min | **Pass**: 80% (24/30) | **Points**: 30

## Section 1: Multiple Choice (15 questions, 0.5 pts each = 7.5 pts)

1. What does the `out` keyword mean in `IEnumerable<out T>`?
   - a) Output parameter | b) Covariance | c) Contravariance | d) Optional

2. Covariance allows:
   - a) IEnumerable<Dog> to IEnumerable<Animal> | b) IEnumerable<Animal> to IEnumerable<Dog> | c) Both | d) Neither

3. What does the `in` keyword mean in `IComparer<in T>`?
   - a) Input parameter | b) Covariance | c) Contravariance | d) Required

4. Contravariance allows:
   - a) Action<Dog> to Action<Animal> | b) Action<Animal> to Action<Dog> | c) Both | d) Neither

5. Which constraint means "must be reference type"?
   - a) where T : struct | b) where T : class | c) where T : new() | d) where T : object

6. `where T : new()` constraint requires:
   - a) Any constructor | b) Parameterless constructor | c) Static constructor | d) Private constructor

7. Can you combine multiple constraints?
   - a) No | b) Yes, with comma | c) Yes, with AND | d) Only two max

8. Builder pattern primary benefit:
   - a) Performance | b) Readability for complex objects | c) Thread safety | d) Immutability

9. Fluent interface means:
   - a) Fast execution | b) Method chaining returning `this` | c) Async methods | d) Interface inheritance

10. Observer pattern implements:
    - a) One-to-one | b) One-to-many | c) Many-to-many | d) Many-to-one

11. IObservable<T> represents:
    - a) Observer | b) Subject/Publisher | c) Subscription | d) Event

12. IObserver<T> has which methods?
    - a) OnNext, OnError, OnCompleted | b) Subscribe, Unsubscribe | c) Notify, Update | d) Publish, Subscribe

13. Why is `List<T>` invariant?
    - a) Performance | b) Safety (read and write) | c) Legacy | d) No reason

14. Can you have `IEnumerable<out T>` and still write to it?
    - a) Yes | b) No, covariance only allows reading | c) Only if T is class | d) Depends

15. Builder vs Factory pattern difference:
    - a) No difference | b) Builder for complex multi-step, Factory for simple creation | c) Factory is newer | d) Builder is faster

## Section 2: Short Answer (7 questions, 2 pts each = 14 pts)

16. Explain covariance with a real example. Why is it safe for `IEnumerable<T>` but not `List<T>`?

17. Explain contravariance. Give example with `IComparer<T>` showing why `IComparer<Animal>` can compare cats.

18. What's the difference between these constraints?
```csharp
where T : class
where T : struct
where T : new()
where T : IComparable<T>
where T : U
```

19. Design a Builder pattern for creating complex email messages. What methods would your fluent interface have?

20. Explain the Observer pattern. What problem does it solve? How does it differ from C# events?

21. When would you use `IEnumerable<out T>` covariance in a repository pattern?

22. Explain the difference between variance in delegates vs interfaces. Give examples.

## Section 3: Code Implementation (4 questions, 2 pts each = 8 pts)

23. Implement a generic `Repository<T>` with proper constraints:
```csharp
// Requirements:
// - T must be a reference type
// - T must have parameterless constructor
// - T must implement IEntity interface (Id property)
// - Methods: Add(T), GetById(int), GetAll()
```

24. Create a covariant interface for a data producer:
```csharp
// Create IProducer<out T> interface with GetNext() method
// Show that IProducer<Dog> can be assigned to IProducer<Animal>
// Demonstrate with concrete implementation
```

25. Implement Builder pattern for QueryBuilder:
```csharp
// Requirements:
// - Fluent interface: Select(), From(), Where(), OrderBy()
// - Build() method returns SQL string
// - Example: query.Select("*").From("Users").Where("Age > 18").Build()
```

26. Implement basic Observer pattern:
```csharp
// Requirements:
// - TemperatureSensor (observable)
// - TemperatureDisplay (observer)
// - Attach/Detach observers
// - Notify all observers when temperature changes
// Don't use IObservable<T>, implement from scratch
```

## Answer Key

**MC**: 1.b | 2.a | 3.c | 4.b | 5.b | 6.b | 7.b | 8.b | 9.b | 10.b | 11.b | 12.a | 13.b | 14.b | 15.b

### Short Answer

**16. Covariance** (2 pts):
- Covariance: More derived type can be used where less derived is expected
- Example: `IEnumerable<Dog> dogs = ...; IEnumerable<Animal> animals = dogs;` ✅ Safe
- Safe for IEnumerable because it's read-only (only `out` T, never accepts T as input)
- Unsafe for List<T>: `List<Dog> dogs = new(); List<Animal> animals = dogs; animals.Add(new Cat());` ❌ Would break type safety
- Rule: Covariance safe when T only appears in output positions

**17. Contravariance** (2 pts):
- Contravariance: Less derived type can be used where more derived is expected (reverse direction)
- Example with IComparer<T>:
```csharp
IComparer<Animal> animalComparer = new AnimalComparer(); // Compares by weight
IComparer<Cat> catComparer = animalComparer; // ✅ Safe!
// Why? animalComparer knows how to compare ANY animal, so it can compare cats
catComparer.Compare(cat1, cat2); // Works fine, treats cats as animals
```
- Safe because T only appears in input positions (consumed, not produced)
- Comparer that handles base type can handle derived type

**18. Constraints** (2 pts):
- `where T : class` - T must be reference type (class, interface, delegate, array)
- `where T : struct` - T must be value type (int, struct, enum) excluding Nullable<T>
- `where T : new()` - T must have public parameterless constructor (for instantiation)
- `where T : IComparable<T>` - T must implement IComparable<T> interface
- `where T : U` - T must be or derive from U (type parameter constraint)

**19. Email Builder Design** (2 pts):
```csharp
public class EmailBuilder
{
    public EmailBuilder To(string recipient) { ... return this; }
    public EmailBuilder From(string sender) { ... return this; }
    public EmailBuilder Subject(string subject) { ... return this; }
    public EmailBuilder Body(string body) { ... return this; }
    public EmailBuilder Attach(string file) { ... return this; }
    public EmailBuilder Cc(string recipient) { ... return this; }
    public Email Build() { return new Email(...); }
}

// Usage
var email = new EmailBuilder()
    .From("me@example.com")
    .To("you@example.com")
    .Subject("Hello")
    .Body("World")
    .Build();
```

**20. Observer Pattern** (2 pts):
- **Problem**: Need to notify multiple objects when state changes without tight coupling
- **Solution**: Subject maintains list of observers, notifies them automatically
- **vs C# events**:
  - Observer: Formal pattern, IObservable<T>/IObserver<T>, subscription lifecycle
  - Events: Language feature, simpler, delegate-based, no subscription object
  - Observer: More control (dispose subscription), async support (Rx)
  - Events: Less ceremony, better for simple scenarios

**21. Covariance in Repository** (2 pts):
```csharp
public interface IRepository<out T> where T : IEntity
{
    IEnumerable<T> GetAll(); // OK - T in output position
    T GetById(int id);       // OK - T in output position
    // void Add(T entity);   // ❌ NOT ALLOWED - T in input position
}

// Usage - covariance allows this
IRepository<Dog> dogRepo = new DogRepository();
IRepository<Animal> animalRepo = dogRepo; // ✅ Safe!
IEnumerable<Animal> animals = animalRepo.GetAll(); // Returns dogs
```

**22. Variance in Delegates vs Interfaces** (2 pts):
- **Delegates**:
```csharp
Func<Dog> dogFunc = () => new Dog();
Func<Animal> animalFunc = dogFunc; // ✅ Covariance (return type)

Action<Animal> animalAction = (a) => Console.WriteLine(a);
Action<Dog> dogAction = animalAction; // ✅ Contravariance (parameter type)
```
- **Interfaces**:
```csharp
IEnumerable<Dog> dogs = ...;
IEnumerable<Animal> animals = dogs; // ✅ Covariance (out T)

IComparer<Animal> animalComparer = ...;
IComparer<Dog> dogComparer = animalComparer; // ✅ Contravariance (in T)
```
- Both support variance, but interfaces require explicit `out`/`in` keywords
- Delegates have built-in variance rules for return/parameter types

### Code Implementation

**23. Repository<T>** (2 pts):
```csharp
public interface IEntity
{
    int Id { get; set; }
}

public class Repository<T> where T : class, IEntity, new()
{
    private readonly List<T> _storage = new();
    private int _nextId = 1;

    public void Add(T entity)
    {
        entity.Id = _nextId++;
        _storage.Add(entity);
    }

    public T GetById(int id)
    {
        return _storage.FirstOrDefault(e => e.Id == id);
    }

    public IEnumerable<T> GetAll()
    {
        return _storage;
    }
}

// Usage
public class User : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}

var repo = new Repository<User>();
repo.Add(new User { Name = "John" });
```

**24. Covariant Producer** (2 pts):
```csharp
public interface IProducer<out T>
{
    T GetNext();
    bool HasNext { get; }
}

public class Animal { }
public class Dog : Animal { }

public class DogProducer : IProducer<Dog>
{
    private readonly Queue<Dog> _dogs = new();

    public DogProducer()
    {
        _dogs.Enqueue(new Dog());
        _dogs.Enqueue(new Dog());
    }

    public Dog GetNext() => _dogs.Dequeue();
    public bool HasNext => _dogs.Count > 0;
}

// Demonstration of covariance
IProducer<Dog> dogProducer = new DogProducer();
IProducer<Animal> animalProducer = dogProducer; // ✅ Covariance!

Animal animal = animalProducer.GetNext(); // Returns Dog as Animal
```

**25. Query Builder** (2 pts):
```csharp
public class QueryBuilder
{
    private string _select;
    private string _from;
    private string _where;
    private string _orderBy;

    public QueryBuilder Select(string columns)
    {
        _select = columns;
        return this;
    }

    public QueryBuilder From(string table)
    {
        _from = table;
        return this;
    }

    public QueryBuilder Where(string condition)
    {
        _where = condition;
        return this;
    }

    public QueryBuilder OrderBy(string column)
    {
        _orderBy = column;
        return this;
    }

    public string Build()
    {
        var query = $"SELECT {_select} FROM {_from}";
        if (!string.IsNullOrEmpty(_where))
            query += $" WHERE {_where}";
        if (!string.IsNullOrEmpty(_orderBy))
            query += $" ORDER BY {_orderBy}";
        return query;
    }
}

// Usage
var sql = new QueryBuilder()
    .Select("Name, Age")
    .From("Users")
    .Where("Age > 18")
    .OrderBy("Name")
    .Build();
// Result: "SELECT Name, Age FROM Users WHERE Age > 18 ORDER BY Name"
```

**26. Observer Pattern** (2 pts):
```csharp
// Observer interface
public interface IObserver
{
    void Update(float temperature);
}

// Subject (Observable)
public class TemperatureSensor
{
    private readonly List<IObserver> _observers = new();
    private float _temperature;

    public void Attach(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public float Temperature
    {
        get => _temperature;
        set
        {
            _temperature = value;
            Notify();
        }
    }

    private void Notify()
    {
        foreach (var observer in _observers)
        {
            observer.Update(_temperature);
        }
    }
}

// Concrete Observer
public class TemperatureDisplay : IObserver
{
    private readonly string _name;

    public TemperatureDisplay(string name)
    {
        _name = name;
    }

    public void Update(float temperature)
    {
        Console.WriteLine($"{_name}: Temperature is now {temperature}°C");
    }
}

// Usage
var sensor = new TemperatureSensor();
var display1 = new TemperatureDisplay("Display 1");
var display2 = new TemperatureDisplay("Display 2");

sensor.Attach(display1);
sensor.Attach(display2);

sensor.Temperature = 25.5f; // Both displays notified
sensor.Detach(display1);
sensor.Temperature = 30.0f; // Only display2 notified
```

## Grading Rubric

| Section | Max Points | Criteria |
|---------|-----------|----------|
| Multiple Choice | 7.5 | 0.5 per correct answer |
| Short Answer (each) | 2 × 7 = 14 | Full: Complete answer. Partial: 1.0-1.5. Wrong: 0 |
| Code Implementation (each) | 2 × 4 = 8 | Full: Working code + constraints. Partial: 1.0-1.5. Wrong: 0 |
| **Total** | **30** | **Pass: 24 points (80%)** |

---

## Study Resources

**Week 9 - Generic Covariance**:
- `samples/02-Intermediate/CovarianceContravariance/Covariance.cs`
- `samples/99-Exercises/Generics/01-Covariance/`

**Week 10 - Generic Contravariance**:
- `samples/99-Exercises/Generics/02-Contravariance/`

**Week 11 - Generic Constraints**:
- `samples/99-Exercises/Generics/03-GenericConstraints/`

**Week 12 - Builder Pattern**:
- `samples/03-Advanced/DesignPatterns/BuilderPattern.cs`
- `samples/99-Exercises/DesignPatterns/01-Builder/`

**Supplementary**:
- `samples/99-Exercises/DesignPatterns/02-Observer/`

---

## Next Steps

**If you passed (≥24 pts)**: Proceed to Month 4 (Algorithms & Data Structures)

**If you didn't pass (<24 pts)**: Review weak areas:
- Score 0-10: Review all Month 3 materials
- Score 11-18: Focus on variance and constraints
- Score 19-23: Practice Builder and Observer patterns

---

*Assessment Version: 1.0*
*Last Updated: 2025-12-02*
