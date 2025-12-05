# 🔧 Extension Methods - Utility Extensions

## 📚 Overview

**ExtensionMethods** demonstrates C# extension methods with comprehensive utility functions. Learn how to extend existing types (string, collections, enums, DateTime, numerics) without modifying their source code using static classes and the `this` keyword.

**Lines of Code**: 909
**Build Status**: ✅ 0 errors
**C# Version**: 3.0+ (Extension methods introduced in C# 3.0)

## 🎯 Key Features

### Extension Method Categories
1. **String Extensions** - Email validation, title case, slug generation, truncation
2. **Collection Extensions** - Batching, shuffling, chunking, ForEach, DistinctBy
3. **Enum Extensions** - Description attributes, display names, flag checking
4. **DateTime Extensions** - Weekend checks, week/month boundaries, age calculation
5. **Numeric Extensions** - Even/odd, clamping, percentages, ordinal numbers
6. **LINQ Extensions** - Conditional filtering, conditional ordering, TakeUntil

## 💻 Quick Start

```bash
cd samples/02-Intermediate/ExtensionMethods
dotnet build
dotnet run
```

## 🎓 Core Concepts

### What Are Extension Methods?

**Extension methods** allow you to add new methods to existing types without:
- Creating a derived type
- Recompiling the original type
- Modifying the original type's source code

**Syntax**:
```csharp
public static class MyExtensions
{
    // 'this' keyword makes it an extension method
    public static ReturnType MethodName(this TypeToExtend obj, parameters...)
    {
        // Implementation
    }
}
```

**Requirements**:
1. Must be in a **static class**
2. Must be a **static method**
3. First parameter must have **`this` keyword**
4. Class must be in scope (using directive)

### 1. String Extensions

**Purpose**: Extend string type with utility methods

```csharp
public static class StringExtensions
{
    /// <summary>
    /// Validates if a string is a valid email address
    /// </summary>
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    /// <summary>
    /// Converts string to Title Case
    /// </summary>
    public static string ToTitleCase(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var textInfo = CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(text.ToLower());
    }

    /// <summary>
    /// Converts string to URL-friendly slug
    /// </summary>
    public static string ToSlug(this string text)
    {
        text = text.ToLowerInvariant();
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
        text = Regex.Replace(text, @"\s+", "-");
        return text.Trim('-');
    }
}
```

**Usage**:
```csharp
var email = "user@example.com";
bool isValid = email.IsValidEmail();  // true

var title = "hello world";
var titleCase = title.ToTitleCase();  // "Hello World"

var slug = "C# Programming Tips".ToSlug();  // "c-programming-tips"
```

**When to use**: Common string operations used across the application

### 2. Collection Extensions

**Purpose**: Extend IEnumerable<T> with collection operations

```csharp
public static class CollectionExtensions
{
    /// <summary>
    /// Splits collection into batches of specified size
    /// </summary>
    public static IEnumerable<IEnumerable<T>> Batch<T>(
        this IEnumerable<T> source,
        int batchSize)
    {
        var batch = new List<T>(batchSize);

        foreach (var item in source)
        {
            batch.Add(item);

            if (batch.Count == batchSize)
            {
                yield return batch;
                batch = new List<T>(batchSize);
            }
        }

        if (batch.Count > 0)
            yield return batch;
    }

    /// <summary>
    /// Shuffles collection randomly
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        var random = new Random();
        var list = source.ToList();

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }

        return list;
    }

    /// <summary>
    /// Returns distinct elements using a key selector
    /// </summary>
    public static IEnumerable<T> DistinctBy<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();

        foreach (var item in source)
        {
            if (seenKeys.Add(keySelector(item)))
                yield return item;
        }
    }
}
```

**Usage**:
```csharp
var numbers = Enumerable.Range(1, 15);

// Batch processing
var batches = numbers.Batch(5);
// [[1,2,3,4,5], [6,7,8,9,10], [11,12,13,14,15]]

// Shuffle
var shuffled = numbers.Shuffle();
// [3, 7, 1, 9, 2, ...] (random order)

// Distinct by property
var products = new[] {
    new { Id = 1, Category = "Electronics" },
    new { Id = 2, Category = "Electronics" },
    new { Id = 3, Category = "Furniture" }
};
var distinct = products.DistinctBy(p => p.Category);
// Only first Electronics and Furniture
```

**When to use**: Batch processing, randomization, custom LINQ operations

### 3. Enum Extensions

**Purpose**: Extend enums with metadata access

```csharp
public enum Priority
{
    [Description("Low priority - can wait")]
    Low = 1,

    [Description("High priority - urgent")]
    High = 3
}

public static class EnumExtensions
{
    /// <summary>
    /// Gets the Description attribute value from enum
    /// </summary>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(
            field,
            typeof(DescriptionAttribute)
        );

        return attribute?.Description ?? value.ToString();
    }

    /// <summary>
    /// Converts enum to list of all values
    /// </summary>
    public static List<T> ToList<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T)).Cast<T>().ToList();
    }
}
```

**Usage**:
```csharp
var priority = Priority.High;
var description = priority.GetDescription();
// "High priority - urgent"

var allPriorities = EnumExtensions.ToList<Priority>();
// [Low, High]
```

**When to use**: Display user-friendly enum names, enum metadata

### 4. DateTime Extensions

**Purpose**: Extend DateTime with common date operations

```csharp
public static class DateTimeExtensions
{
    /// <summary>
    /// Checks if date is weekend
    /// </summary>
    public static bool IsWeekend(this DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday ||
               date.DayOfWeek == DayOfWeek.Sunday;
    }

    /// <summary>
    /// Gets the start of the week (Monday)
    /// </summary>
    public static DateTime StartOfWeek(this DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// Calculates age from birth date
    /// </summary>
    public static int Age(this DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age))
            age--;

        return age;
    }
}
```

**Usage**:
```csharp
var today = DateTime.Today;

bool isWeekend = today.IsWeekend();  // true/false

var weekStart = today.StartOfWeek();
// Monday of current week

var birthDate = new DateTime(1990, 5, 15);
int age = birthDate.Age();  // 34 (or current age)
```

**When to use**: Date calculations, business logic with dates

### 5. Numeric Extensions

**Purpose**: Extend numeric types with utility methods

```csharp
public static class NumericExtensions
{
    /// <summary>
    /// Checks if number is even
    /// </summary>
    public static bool IsEven(this int number)
    {
        return number % 2 == 0;
    }

    /// <summary>
    /// Clamps number between min and max values
    /// </summary>
    public static T Clamp<T>(this T value, T min, T max)
        where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0) return min;
        if (value.CompareTo(max) > 0) return max;
        return value;
    }

    /// <summary>
    /// Converts number to ordinal string (1st, 2nd, 3rd)
    /// </summary>
    public static string ToOrdinal(this int number)
    {
        return (number % 10) switch
        {
            1 => $"{number}st",
            2 => $"{number}nd",
            3 => $"{number}rd",
            _ => $"{number}th"
        };
    }
}
```

**Usage**:
```csharp
int num = 4;
bool even = num.IsEven();  // true

int value = 15;
int clamped = value.Clamp(0, 10);  // 10

int position = 3;
string ordinal = position.ToOrdinal();  // "3rd"
```

**When to use**: Math operations, formatting, range validation

### 6. LINQ Extensions

**Purpose**: Extend LINQ with conditional and advanced operations

```csharp
public static class LinqExtensions
{
    /// <summary>
    /// Conditionally filters sequence
    /// </summary>
    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }

    /// <summary>
    /// Takes elements until condition is met
    /// </summary>
    public static IEnumerable<T> TakeUntil<T>(
        this IEnumerable<T> source,
        Func<T, bool> predicate)
    {
        foreach (var item in source)
        {
            yield return item;
            if (predicate(item))
                yield break;
        }
    }

    /// <summary>
    /// Iterates with index
    /// </summary>
    public static void ForEachWithIndex<T>(
        this IEnumerable<T> source,
        Action<T, int> action)
    {
        int index = 0;
        foreach (var item in source)
        {
            action(item, index++);
        }
    }
}
```

**Usage**:
```csharp
var products = GetProducts();

// Conditional filter
bool filterExpensive = true;
var filtered = products.WhereIf(filterExpensive, p => p.Price > 100);

// Take until
var numbers = Enumerable.Range(1, 20);
var upTo10 = numbers.TakeUntil(n => n == 10);
// [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]

// ForEach with index
products.ForEachWithIndex((product, index) =>
{
    Console.WriteLine($"[{index}] {product.Name}");
});
```

**When to use**: Dynamic queries, advanced LINQ operations

## 📊 Complete Examples

### Example 1: String Extensions

```csharp
// Email validation
"user@example.com".IsValidEmail()     // true
"invalid.email".IsValidEmail()        // false

// Title case
"hello world".ToTitleCase()           // "Hello World"
"LOUD NOISES".ToTitleCase()           // "Loud Noises"

// Truncate
var text = "This is a very long text that needs to be truncated";
text.Truncate(20)                     // "This is a very long ..."

// URL slug
"C# Programming Tips".ToSlug()        // "c-programming-tips"
"  Spaces & Special@#$ ".ToSlug()     // "spaces-special"

// Remove whitespace
"  This   has   spaces  ".RemoveWhitespace()  // "Thishasspaces"

// Reverse
"Hello".Reverse()                     // "olleH"

// Contains any
"This is a test".ContainsAny("test", "hello")  // true
```

### Example 2: Collection Extensions

```csharp
var numbers = Enumerable.Range(1, 15);

// Batch
var batches = numbers.Batch(5);
// Batch 1: [1, 2, 3, 4, 5]
// Batch 2: [6, 7, 8, 9, 10]
// Batch 3: [11, 12, 13, 14, 15]

// Shuffle
var shuffled = numbers.Shuffle();
// [3, 7, 1, 9, 2, ...] (random)

// ForEach
numbers.Take(5).ForEach(n => Console.Write($"{n * n} "));
// Output: 1 4 9 16 25

// IsNullOrEmpty
List<int>? nullList = null;
nullList.IsNullOrEmpty()              // true
new List<int>().IsNullOrEmpty()       // true
new List<int> { 1 }.IsNullOrEmpty()   // false

// DistinctBy
var products = new[] {
    new { Id = 1, Category = "Electronics" },
    new { Id = 2, Category = "Electronics" },
    new { Id = 3, Category = "Furniture" }
};
var distinct = products.DistinctBy(p => p.Category);
// Only 2 items (first Electronics and Furniture)

// Chunk
var chunks = numbers.Chunk(4);
// Chunk 1: [1, 2, 3, 4]
// Chunk 2: [5, 6, 7, 8]
// Chunk 3: [9, 10, 11, 12]
// Chunk 4: [13, 14, 15]
```

### Example 3: Enum Extensions

```csharp
public enum Priority
{
    [Description("Low priority - can wait")]
    Low = 1,

    [Description("High priority - urgent")]
    High = 3
}

// Get description
Priority.Low.GetDescription()
// "Low priority - can wait"

Priority.High.GetDescription()
// "High priority - urgent"

// Get display name
OrderStatus.Shipped.GetDisplayName()  // "Shipped"

// Enum to list
var priorities = EnumExtensions.ToList<Priority>();
// [Low, High]

// Has flag
var current = Priority.High;
current.HasFlag(Priority.High)        // true
current.HasFlag(Priority.Low)         // true (bitwise)
```

### Example 4: DateTime Extensions

```csharp
var today = DateTime.Today;

// Is weekend
today.IsWeekend()                     // true/false

// Weekend check for week
for (int i = 0; i < 7; i++)
{
    var date = today.AddDays(i);
    Console.WriteLine($"{date:ddd} - {(date.IsWeekend() ? "Weekend" : "Weekday")}");
}

// Start of week
today.StartOfWeek()                   // Monday of this week

// End of month
today.EndOfMonth()                    // Last day of month

// Age calculation
var birthDate = new DateTime(1990, 5, 15);
birthDate.Age()                       // 34 (current age)

// Start/End of day
var now = DateTime.Now;
now.StartOfDay()                      // 2024-12-05 00:00:00
now.EndOfDay()                        // 2024-12-05 23:59:59

// Is today
today.IsToday()                       // true
today.AddDays(1).IsToday()            // false
```

### Example 5: Numeric Extensions

```csharp
// Even/Odd
1.IsEven()                            // false
2.IsEven()                            // true
3.IsOdd()                             // true

// Clamp
(-5).Clamp(0, 10)                     // 0
5.Clamp(0, 10)                        // 5
15.Clamp(0, 10)                       // 10

// Percentage
0.25m.ToPercentage()                  // "25.00%"
0.5m.ToPercentage(1)                  // "50.0%"
1.25m.ToPercentage()                  // "125.00%"

// In range
5.InRange(0, 10)                      // true
15.InRange(0, 10)                     // false

// Ordinal
1.ToOrdinal()                         // "1st"
2.ToOrdinal()                         // "2nd"
3.ToOrdinal()                         // "3rd"
11.ToOrdinal()                        // "11th"
21.ToOrdinal()                        // "21st"
```

### Example 6: LINQ Extensions

```csharp
var products = new[] {
    new Product { Name = "Laptop", Price = 1000 },
    new Product { Name = "Mouse", Price = 20 },
    new Product { Name = "Desk", Price = 500 }
};

// WhereIf (conditional filter)
bool filterExpensive = true;
var filtered = products.WhereIf(filterExpensive, p => p.Price > 100);
// Laptop, Desk

filterExpensive = false;
filtered = products.WhereIf(filterExpensive, p => p.Price > 100);
// All products (filter not applied)

// OrderByIf (conditional sort)
bool sortByPrice = true;
var sorted = products.OrderByIf(sortByPrice, p => p.Price);
// Mouse ($20), Desk ($500), Laptop ($1000)

// TakeUntil
var numbers = Enumerable.Range(1, 20);
var upTo10 = numbers.TakeUntil(n => n == 10);
// [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]

// ForEachWithIndex
products.ForEachWithIndex((product, index) =>
{
    Console.WriteLine($"[{index}] {product.Name} - ${product.Price}");
});
// [0] Laptop - $1000
// [1] Mouse - $20
// [2] Desk - $500

// DefaultIfEmpty
var emptyList = new List<Product>();
var defaultProduct = new[] { new Product { Name = "No products" } };

emptyList.DefaultIfEmpty(defaultProduct);
// [Product { Name = "No products" }]

products.DefaultIfEmpty(defaultProduct);
// Original products (not empty)
```

## 💡 Best Practices

### DO ✅

1. **Use Meaningful Names**
   ```csharp
   // ✅ GOOD: Clear what it does
   public static bool IsValidEmail(this string email)

   // ❌ BAD: Unclear
   public static bool Validate(this string s)
   ```

2. **Keep Methods Focused**
   ```csharp
   // ✅ GOOD: Single responsibility
   public static string ToSlug(this string text)
   public static string ToTitleCase(this string text)

   // ❌ BAD: Does too much
   public static string Format(this string text, string type)
   ```

3. **Null-Check Parameters**
   ```csharp
   // ✅ GOOD: Validates input
   public static IEnumerable<T> Batch<T>(this IEnumerable<T> source, int size)
   {
       ArgumentNullException.ThrowIfNull(source);
       // ...
   }
   ```

4. **Use Descriptive Parameter Names**
   ```csharp
   // ✅ GOOD
   public static string Truncate(this string text, int maxLength)

   // ❌ BAD
   public static string Truncate(this string s, int l)
   ```

5. **Group Related Extensions**
   ```csharp
   // ✅ GOOD: Organized by type
   public static class StringExtensions { }
   public static class CollectionExtensions { }
   public static class DateTimeExtensions { }
   ```

6. **Document with XML Comments**
   ```csharp
   /// <summary>
   /// Validates if a string is a valid email address
   /// </summary>
   /// <param name="email">The email string to validate</param>
   /// <returns>True if valid, false otherwise</returns>
   public static bool IsValidEmail(this string email)
   ```

### DON'T ❌

1. **Don't Extend with Instance Methods**
   ```csharp
   // ❌ WRONG: Not static
   public class StringExtensions
   {
       public bool IsValidEmail(this string email) { }
   }

   // ✅ CORRECT: Static class and method
   public static class StringExtensions
   {
       public static bool IsValidEmail(this string email) { }
   }
   ```

2. **Don't Forget `this` Keyword**
   ```csharp
   // ❌ WRONG: Missing 'this'
   public static bool IsEven(int number)

   // ✅ CORRECT: Has 'this'
   public static bool IsEven(this int number)
   ```

3. **Don't Overuse Extension Methods**
   ```csharp
   // ❌ BAD: Should be a regular class
   public static class Calculator
   {
       public static int Add(this int a, int b) => a + b;
   }

   // ✅ GOOD: Regular static class
   public static class Calculator
   {
       public static int Add(int a, int b) => a + b;
   }
   ```

4. **Don't Modify State**
   ```csharp
   // ❌ BAD: Modifies original (if mutable)
   public static void Clear(this List<int> list)
   {
       list.Clear();  // Side effect
   }

   // ✅ GOOD: Returns new instance
   public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
   {
       var list = source.ToList();  // New copy
       // Shuffle and return
   }
   ```

5. **Don't Create Ambiguous Extensions**
   ```csharp
   // ❌ BAD: Conflicts with LINQ
   public static IEnumerable<T> Where<T>(this IEnumerable<T> source, ...)

   // ✅ GOOD: Unique name
   public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, ...)
   ```

## 🔍 Common Patterns

### Pattern 1: Validation Extensions

```csharp
public static class ValidationExtensions
{
    public static bool IsValidEmail(this string email) { }
    public static bool IsValidUrl(this string url) { }
    public static bool IsValidPhone(this string phone) { }
}
```

### Pattern 2: Conversion Extensions

```csharp
public static class ConversionExtensions
{
    public static string ToSlug(this string text) { }
    public static string ToTitleCase(this string text) { }
    public static decimal ToPercentage(this decimal value) { }
    public static string ToOrdinal(this int number) { }
}
```

### Pattern 3: LINQ-Style Extensions

```csharp
public static class LinqExtensions
{
    public static IEnumerable<T> WhereIf<T>(...) { }
    public static IEnumerable<T> OrderByIf<T>(...) { }
    public static IEnumerable<T> DistinctBy<T, TKey>(...) { }
}
```

### Pattern 4: Utility Extensions

```csharp
public static class UtilityExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source) { }
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) { }
    public static T Clamp<T>(this T value, T min, T max) { }
}
```

### Pattern 5: Domain-Specific Extensions

```csharp
public static class DateExtensions
{
    public static bool IsWeekend(this DateTime date) { }
    public static bool IsBusinessDay(this DateTime date) { }
    public static int Age(this DateTime birthDate) { }
}
```

## 🎯 Use Cases

1. **String Utilities** - Email validation, formatting, slug generation
2. **Collection Processing** - Batching for pagination, shuffling, chunking
3. **Enum Metadata** - Display names from attributes for UI
4. **Date Calculations** - Business day logic, age calculations
5. **Numeric Operations** - Range validation, formatting, clamping
6. **LINQ Enhancements** - Conditional queries, custom operations
7. **Fluent APIs** - Method chaining for readable code
8. **Cross-Cutting Concerns** - Logging, validation, transformation

## 📈 Benefits

### Code Readability
```csharp
// ❌ Without extensions
bool isValid = EmailValidator.Validate(email);
string slug = SlugGenerator.Generate(title);

// ✅ With extensions
bool isValid = email.IsValidEmail();
string slug = title.ToSlug();
```

### Fluent Chaining
```csharp
// Extension methods enable fluent syntax
var result = text
    .ToLowerInvariant()
    .Trim()
    .RemoveWhitespace()
    .Truncate(50);
```

### Discoverability
```csharp
// IntelliSense shows extensions on the object
var email = "user@example.com";
email.  // IntelliSense shows IsValidEmail, ToSlug, etc.
```

### Backward Compatibility
```csharp
// Add methods to types you don't own (string, int, DateTime)
// without breaking existing code
```

## 🔗 Related Patterns

- **Helper/Utility Classes** - Alternative to extensions for static methods
- **Decorator Pattern** - Extensions provide similar functionality
- **Fluent Interface** - Extensions enable method chaining
- **LINQ** - Built using extension methods
- **Mixin Pattern** - Extensions simulate mixins in C#

**See**: [WHY_THIS_PATTERN.md](WHY_THIS_PATTERN.md) for detailed explanation

## 📚 Extension Categories Summary

| Category | Methods | Use Case |
|----------|---------|----------|
| **String** | IsValidEmail, ToTitleCase, ToSlug, Truncate, Reverse | Text processing, validation |
| **Collection** | Batch, Shuffle, Chunk, ForEach, DistinctBy | Collection operations |
| **Enum** | GetDescription, GetDisplayName, ToList, HasFlag | Enum metadata |
| **DateTime** | IsWeekend, StartOfWeek, EndOfMonth, Age | Date calculations |
| **Numeric** | IsEven, IsOdd, Clamp, ToPercentage, ToOrdinal | Math operations |
| **LINQ** | WhereIf, OrderByIf, TakeUntil, ForEachWithIndex | Advanced LINQ |

## ✨ Key Takeaways

1. ✅ **Extension methods** add functionality to existing types without modification
2. ✅ **Static class + static method + `this` keyword** = Extension method
3. ✅ **Group by category** - StringExtensions, CollectionExtensions, etc.
4. ✅ **Null-check parameters** - Prevent runtime errors
5. ✅ **Meaningful names** - Make intent clear
6. ✅ **Use for utilities** - Validation, conversion, LINQ operations
7. ✅ **Enable fluent syntax** - Method chaining for readability
8. ✅ **Document with XML** - Help IntelliSense users
9. ✅ **Don't overuse** - Not everything should be an extension
10. ✅ **Follow conventions** - Match existing patterns (LINQ-style)

---

**Remember**: Extension methods are syntactic sugar that makes code more readable and discoverable - use them wisely! 🔧
