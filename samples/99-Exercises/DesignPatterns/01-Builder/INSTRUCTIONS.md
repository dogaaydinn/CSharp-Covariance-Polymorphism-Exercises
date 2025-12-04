# Builder Pattern Exercise

## 🎯 Learning Objectives
- **Builder Pattern**: Separate construction from representation
- **Fluent Interface**: Method chaining with `return this`
- **Director Pattern**: Pre-configured object construction
- **Inheritance**: Specialized builders
- **Real-World Usage**: Complex object construction

## 📋 Exercise Tasks

Complete **5 TODO sections** in `Models.cs`:

### TODO 1: Pizza Class
Create a complex object with multiple properties:
- Size, Dough, Sauce, Toppings (list)
- ExtraCheese (bool), SpicyLevel (int 0-5)
- ToString() method for display

### TODO 2: PizzaBuilder
Implement fluent interface:
- `WithSize()`, `WithDough()`, `WithSauce()`
- `AddTopping()`, `WithExtraCheese()`, `WithSpicyLevel()`
- Each method returns `this` for chaining

### TODO 3: PizzaDirector
Pre-configured pizza recipes:
- `CreateMargherita()` - Classic Italian
- `CreatePepperoni()` - American favorite
- `CreateVeggie()` - Vegetarian option
- `CreateMeatLovers()` - For carnivores

### TODO 4: GlutenFreePizzaBuilder
Specialized builder that extends PizzaBuilder:
- Auto-sets DoughType to GlutenFree
- Validates gluten-free requirements

### TODO 5: Build() Method
- Validates the pizza configuration
- Returns the constructed Pizza object

## 🚀 Getting Started

```bash
cd samples/99-Exercises/DesignPatterns/01-Builder
dotnet restore
dotnet test  # Should see 16 FAILED tests
```

Open `Models.cs` and start completing the TODO sections!

## 💡 Implementation Hints

### TODO 1: Pizza Class

```csharp
public class Pizza
{
    public PizzaSize Size { get; set; }
    public DoughType Dough { get; set; }
    public SauceType Sauce { get; set; }
    public List<string> Toppings { get; set; }
    public bool ExtraCheese { get; set; }
    public int SpicyLevel { get; set; }

    public Pizza()
    {
        Toppings = new List<string>();
        SpicyLevel = 0;
        ExtraCheese = false;
    }

    public override string ToString()
    {
        return $"{Size} {Dough} crust pizza with {Sauce} sauce, " +
               $"Toppings: [{string.Join(", ", Toppings)}], " +
               $"Extra Cheese: {ExtraCheese}, Spicy Level: {SpicyLevel}";
    }
}
```

### TODO 2: PizzaBuilder Fluent Methods

```csharp
public class PizzaBuilder
{
    private Pizza _pizza;

    public PizzaBuilder()
    {
        _pizza = new Pizza();
    }

    public PizzaBuilder WithSize(PizzaSize size)
    {
        _pizza.Size = size;
        return this;  // KEY: Return this for chaining!
    }

    public PizzaBuilder WithDough(DoughType dough)
    {
        _pizza.Dough = dough;
        return this;
    }

    public PizzaBuilder WithSauce(SauceType sauce)
    {
        _pizza.Sauce = sauce;
        return this;
    }

    public PizzaBuilder AddTopping(string topping)
    {
        _pizza.Toppings.Add(topping);
        return this;
    }

    public PizzaBuilder WithExtraCheese()
    {
        _pizza.ExtraCheese = true;
        return this;
    }

    public PizzaBuilder WithSpicyLevel(int level)
    {
        _pizza.SpicyLevel = Math.Clamp(level, 0, 5);
        return this;
    }

    public Pizza Build()
    {
        // Optional validation
        if (_pizza.Toppings.Count == 0)
            throw new InvalidOperationException("Pizza must have at least one topping");

        return _pizza;
    }
}
```

### TODO 3: PizzaDirector

```csharp
public class PizzaDirector
{
    public static Pizza CreateMargherita()
    {
        return new PizzaBuilder()
            .WithSize(PizzaSize.Medium)
            .WithDough(DoughType.Thin)
            .WithSauce(SauceType.Tomato)
            .AddTopping("Mozzarella")
            .AddTopping("Basil")
            .Build();
    }

    public static Pizza CreatePepperoni()
    {
        return new PizzaBuilder()
            .WithSize(PizzaSize.Large)
            .WithDough(DoughType.Thick)
            .WithSauce(SauceType.Tomato)
            .AddTopping("Pepperoni")
            .AddTopping("Mozzarella")
            .WithExtraCheese()
            .WithSpicyLevel(2)
            .Build();
    }

    public static Pizza CreateVeggie()
    {
        return new PizzaBuilder()
            .WithSize(PizzaSize.Medium)
            .WithDough(DoughType.Thin)
            .WithSauce(SauceType.Pesto)
            .AddTopping("Bell Peppers")
            .AddTopping("Mushrooms")
            .AddTopping("Olives")
            .AddTopping("Onions")
            .Build();
    }

    public static Pizza CreateMeatLovers()
    {
        return new PizzaBuilder()
            .WithSize(PizzaSize.ExtraLarge)
            .WithDough(DoughType.Thick)
            .WithSauce(SauceType.BBQ)
            .AddTopping("Pepperoni")
            .AddTopping("Sausage")
            .AddTopping("Bacon")
            .AddTopping("Ham")
            .WithExtraCheese()
            .WithSpicyLevel(3)
            .Build();
    }
}
```

### TODO 4: GlutenFreePizzaBuilder

```csharp
public class GlutenFreePizzaBuilder : PizzaBuilder
{
    public GlutenFreePizzaBuilder()
    {
        // Automatically set gluten-free dough
        WithDough(DoughType.GlutenFree);
    }

    public new Pizza Build()
    {
        // Could add additional gluten-free validation here
        return base.Build();
    }
}
```

## 📊 Pattern Explained

### Why Builder Pattern?

**Problem**: Creating complex objects with many optional parameters

**Bad Solution**:
```csharp
// Constructor hell!
var pizza = new Pizza(
    PizzaSize.Large,
    DoughType.Thin,
    SauceType.Tomato,
    new List<string> { "Cheese", "Pepperoni" },
    true,  // Extra cheese?
    2      // Spicy level?
);
```

**Good Solution** (Builder):
```csharp
var pizza = new PizzaBuilder()
    .WithSize(PizzaSize.Large)
    .WithDough(DoughType.Thin)
    .WithSauce(SauceType.Tomato)
    .AddTopping("Cheese")
    .AddTopping("Pepperoni")
    .WithExtraCheese()
    .WithSpicyLevel(2)
    .Build();
```

### Benefits

1. **Readable**: Self-documenting code
2. **Flexible**: Optional parameters are clear
3. **Immutable**: Build once, use many times
4. **Validation**: Centralized in Build()
5. **Fluent**: Natural method chaining

## 🎓 Interview Tips

### Q: When to use Builder Pattern?

**Use Builder when:**
- Object has many parameters (5+)
- Many parameters are optional
- Need different representations of same object
- Construction is complex

**Examples in .NET:**
- `StringBuilder`
- `UriBuilder`
- `DbConnectionStringBuilder`
- Entity Framework query builders

### Q: Builder vs Factory Pattern?

| Builder | Factory |
|---------|---------|
| Step-by-step construction | One-step creation |
| Same type, different configurations | Different types |
| Fluent interface | Simple method call |
| Example: Pizza with toppings | Example: Shape factory |

### Q: What is a "Fluent Interface"?

```csharp
// Fluent: Method chaining
var pizza = builder
    .WithSize(PizzaSize.Large)
    .WithDough(DoughType.Thin)
    .Build();

// Non-fluent: Individual calls
builder.WithSize(PizzaSize.Large);
builder.WithDough(DoughType.Thin);
var pizza = builder.Build();
```

**Key**: Each method returns `this` (the builder instance).

## ✅ Success Criteria

- [ ] All 16 tests pass
- [ ] Pizza class has all properties
- [ ] PizzaBuilder has fluent methods
- [ ] PizzaDirector creates 4 pizza types
- [ ] GlutenFreePizzaBuilder works correctly
- [ ] Method chaining works smoothly

## 📚 Real-World Examples

### 1. HTTP Request Builder
```csharp
var request = new HttpRequestBuilder()
    .WithUrl("https://api.example.com")
    .WithMethod(HttpMethod.Post)
    .AddHeader("Authorization", "Bearer token")
    .WithBody(jsonData)
    .WithTimeout(TimeSpan.FromSeconds(30))
    .Build();
```

### 2. SQL Query Builder
```csharp
var query = new QueryBuilder()
    .Select("Id", "Name", "Email")
    .From("Users")
    .Where("Age > 18")
    .OrderBy("Name")
    .Limit(10)
    .Build();
```

### 3. Email Builder
```csharp
var email = new EmailBuilder()
    .To("user@example.com")
    .Subject("Welcome!")
    .WithHtmlBody("<h1>Hello</h1>")
    .AddAttachment("document.pdf")
    .Send();
```

## 🚨 Common Mistakes

### Mistake 1: Forgetting `return this`
```csharp
// ❌ WRONG
public PizzaBuilder WithSize(PizzaSize size)
{
    _pizza.Size = size;
    // Forgot return this!
}

// ✅ CORRECT
public PizzaBuilder WithSize(PizzaSize size)
{
    _pizza.Size = size;
    return this;  // Enable chaining
}
```

### Mistake 2: Mutable Products
```csharp
// ❌ WRONG: Same pizza instance reused
private Pizza _pizza = new Pizza();

// ✅ CORRECT: New pizza each time
public PizzaBuilder()
{
    _pizza = new Pizza();
}
```

### Mistake 3: No Validation
```csharp
// ❌ WRONG: No validation
public Pizza Build() => _pizza;

// ✅ CORRECT: Validate before returning
public Pizza Build()
{
    if (_pizza.Toppings.Count == 0)
        throw new InvalidOperationException("Need toppings!");
    return _pizza;
}
```

---

**Good luck! 🎉** Check `SOLUTION.md` if stuck.
