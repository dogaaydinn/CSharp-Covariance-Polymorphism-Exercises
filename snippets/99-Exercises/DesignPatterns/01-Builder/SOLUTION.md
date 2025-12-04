# ⚠️ SPOILER WARNING ⚠️

**DO NOT READ UNTIL YOU'VE TRIED YOURSELF!**

---

# Builder Pattern - Complete Solution

All complete solutions are provided in `INSTRUCTIONS.md` hints section.

## Key Takeaways

1. **Builder Pattern Benefits**:
   - Readable, self-documenting code
   - Handles complex object construction
   - Fluent interface with method chaining
   - Optional parameters made clear

2. **Fluent Interface**:
   - Each method returns `this`
   - Enables natural method chaining
   - Example: `.WithSize().WithDough().Build()`

3. **Director Pattern**:
   - Pre-configured builders
   - Encapsulates common construction logic
   - Example: `PizzaDirector.CreateMargherita()`

4. **Inheritance**:
   - Specialized builders extend base
   - Example: `GlutenFreePizzaBuilder`
   - Override behavior as needed

## Complete Code Examples

See `INSTRUCTIONS.md` for full implementations.

## When to Use Builder Pattern

✅ **Use Builder when:**
- Object has 5+ parameters
- Many parameters are optional
- Need step-by-step construction
- Want immutable objects
- Different representations needed

❌ **Don't use Builder when:**
- Simple object with few fields
- All parameters are required
- No optional configuration

## Real-World .NET Examples

```csharp
// StringBuilder
var sb = new StringBuilder()
    .Append("Hello")
    .Append(" ")
    .Append("World")
    .ToString();

// UriBuilder
var uri = new UriBuilder()
{
    Scheme = "https",
    Host = "example.com",
    Path = "/api/users"
}.Uri;

// Entity Framework
var query = dbContext.Users
    .Where(u => u.Age > 18)
    .OrderBy(u => u.Name)
    .Take(10)
    .ToList();
```

## Interview Favorites

**Q: Builder vs Factory?**
- **Builder**: Step-by-step, same type, different configs
- **Factory**: One-step, different types

**Q: Why return `this`?**
- Enables method chaining (fluent interface)
- More readable than separate calls

**Q: Where is validation done?**
- In `Build()` method
- Centralized, consistent

**Congratulations! 🎉**

You've mastered the **Builder Pattern**!

**Next**: Observer Pattern (IObservable<T>, event-driven)
