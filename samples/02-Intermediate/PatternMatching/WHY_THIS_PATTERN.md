# Why Pattern Matching? 🔍

## Table of Contents
1. [What is Pattern Matching?](#what-is-pattern-matching)
2. [Why Use Pattern Matching?](#why-use-pattern-matching)
3. [When to Use Each Pattern Type](#when-to-use-each-pattern-type)
4. [Real-World Scenarios](#real-world-scenarios)
5. [Performance Considerations](#performance-considerations)
6. [Common Mistakes](#common-mistakes)
7. [Migration Strategies](#migration-strategies)
8. [Comparison with Alternatives](#comparison-with-alternatives)
9. [Advanced Techniques](#advanced-techniques)
10. [Summary](#summary)

---

## What is Pattern Matching?

**Pattern matching** is a language feature that tests whether a value has a certain "shape" and extracts information from that value when it matches. Introduced in C# 7.0 and significantly enhanced in C# 8.0-11.0, pattern matching enables more expressive and concise conditional logic.

### Evolution Timeline

| Version | Features | Example |
|---------|----------|---------|
| **C# 7.0** | Type patterns, constant patterns | `if (obj is string s)` |
| **C# 8.0** | **Switch expressions**, property patterns | `x switch { 1 => "one", _ => "other" }` |
| **C# 9.0** | **Relational patterns**, logical patterns | `< 100`, `and`, `or`, `not` |
| **C# 10.0** | Extended property patterns | `{ Prop.Nested: value }` |
| **C# 11.0** | **List patterns** | `[first, .., last]` |
| **C# 12.0** | Improvements and optimizations | Enhanced compiler analysis |

### The "Shape" Metaphor

Think of pattern matching as asking questions about data:

```csharp
// Traditional: "Is this a circle? If so, cast it and use it."
if (shape is Circle)
{
    var circle = (Circle)shape;
    var area = circle.Radius * circle.Radius * Math.PI;
}

// Pattern matching: "If this is a Circle called 'c', calculate..."
if (shape is Circle c)
{
    var area = c.Radius * c.Radius * Math.PI;
}

// Switch expression: "What shape is this and what's its area?"
var area = shape switch
{
    Circle c => c.Radius * c.Radius * Math.PI,
    Rectangle r => r.Width * r.Height,
    Triangle t => t.Base * t.Height / 2,
    _ => 0
};
```

---

## Why Use Pattern Matching?

### 1. **Expressiveness** - Code Reads Like Natural Language

**Before (traditional if-else)**:
```csharp
string GetInvoiceCategory(Invoice invoice)
{
    if (invoice.Amount < 100)
        return "Micro";
    else if (invoice.Amount >= 100 && invoice.Amount < 500)
        return "Small";
    else if (invoice.Amount >= 500 && invoice.Amount < 1000)
        return "Medium";
    else if (invoice.Amount >= 1000 && invoice.Amount < 5000)
        return "Large";
    else
        return "Enterprise";
}
```

**After (pattern matching)**:
```csharp
string GetInvoiceCategory(Invoice invoice) => invoice.Amount switch
{
    < 100 => "Micro",
    >= 100 and < 500 => "Small",
    >= 500 and < 1000 => "Medium",
    >= 1000 and < 5000 => "Large",
    >= 5000 => "Enterprise"
};
```

**Result**: 15 lines → 8 lines, instantly readable tiers

### 2. **Type Safety** - Compiler Catches Errors

**Problem without pattern matching**:
```csharp
string ProcessShape(object shape)
{
    if (shape is Circle)
    {
        var c = (Circle)shape;  // Manual cast - could fail if logic changes
        return $"Circle: {c.Radius}";
    }
    // ... what if we forget Rectangle?
    return "Unknown";  // ⚠️ No compiler warning!
}
```

**Solution with pattern matching**:
```csharp
string ProcessShape(Shape shape) => shape switch
{
    Circle c => $"Circle: {c.Radius}",
    Rectangle r => $"Rectangle: {r.Width}x{r.Height}",
    // ⚠️ Compiler warns if we forget Triangle!
    _ => "Unknown"
};
```

**Benefit**: Compiler verifies exhaustiveness and type safety

### 3. **Conciseness** - Less Boilerplate

**Before**:
```csharp
decimal CalculateTax(decimal amount)
{
    decimal rate;
    if (amount <= 100)
        rate = 0.05m;
    else if (amount > 100 && amount <= 500)
        rate = 0.08m;
    else if (amount > 500 && amount <= 1000)
        rate = 0.10m;
    else
        rate = 0.12m;

    return amount * rate;
}
```

**After**:
```csharp
decimal CalculateTax(decimal amount) =>
    amount * (amount switch
    {
        <= 100 => 0.05m,
        > 100 and <= 500 => 0.08m,
        > 500 and <= 1000 => 0.10m,
        > 1000 => 0.12m
    });
```

**Result**: 14 lines → 9 lines, embedded directly in calculation

### 4. **Maintainability** - Single Source of Truth

**Problem with scattered logic**:
```csharp
// In method 1
if (customer.Priority == Priority.VIP)
    discount = 0.20m;

// In method 2
if (customer.Priority == Priority.VIP)
    shippingFee = 0;

// In method 3
if (customer.Priority == Priority.VIP)
    supportLevel = "Premium";
```

**Solution with centralized patterns**:
```csharp
var benefits = customer.Priority switch
{
    Priority.VIP => new { Discount = 0.20m, Shipping = 0m, Support = "Premium" },
    Priority.High => new { Discount = 0.10m, Shipping = 5m, Support = "Standard" },
    _ => new { Discount = 0m, Shipping = 10m, Support = "Basic" }
};
```

### 5. **Declarative Style** - What, Not How

**Imperative (how to do it)**:
```csharp
string result;
if (invoice is StandardInvoice)
{
    var std = (StandardInvoice)invoice;
    result = "Standard";
}
else if (invoice is PremiumInvoice)
{
    var prm = (PremiumInvoice)invoice;
    result = "Premium";
}
else
{
    result = "Unknown";
}
```

**Declarative (what you want)**:
```csharp
var result = invoice switch
{
    StandardInvoice => "Standard",
    PremiumInvoice => "Premium",
    _ => "Unknown"
};
```

---

## When to Use Each Pattern Type

### 1. Switch Expressions - Replacing Traditional Switch

**Use when**: You need to return a value based on multiple conditions

```csharp
// ✅ Perfect for: Value mapping
var statusIcon = orderStatus switch
{
    OrderStatus.Pending => "⏳",
    OrderStatus.Processing => "⚙️",
    OrderStatus.Shipped => "📦",
    OrderStatus.Delivered => "✅",
    _ => "❓"
};

// ✅ Perfect for: Strategy selection
var processor = paymentMethod switch
{
    "credit" => new CreditCardProcessor(),
    "paypal" => new PayPalProcessor(),
    "crypto" => new CryptoProcessor(),
    _ => throw new ArgumentException("Unknown payment method")
};
```

**Don't use when**: You need side effects (use regular switch or if-else)

### 2. Property Patterns - Matching Object State

**Use when**: Decision depends on object properties

```csharp
// ✅ Perfect for: Business rules based on state
var action = order switch
{
    { Status: OrderStatus.Cancelled } => "Refund customer",
    { Status: OrderStatus.Pending, DaysSinceOrder: > 7 } => "Send reminder",
    { Status: OrderStatus.Processing, IsExpedited: true } => "Rush processing",
    _ => "Standard handling"
};

// ✅ Perfect for: Nested property access
var priority = invoice switch
{
    { Customer.Priority: Priority.VIP, Amount: > 1000 } => "Urgent",
    { Customer.Priority: Priority.VIP } => "High",
    _ => "Normal"
};
```

**Don't use when**: Property access is expensive (cache first)

### 3. Type Patterns - Polymorphic Behavior

**Use when**: Behavior depends on runtime type

```csharp
// ✅ Perfect for: Processing different message types
var response = message switch
{
    TextMessage txt => $"Text: {txt.Content}",
    ImageMessage img => $"Image: {img.Url} ({img.Width}x{img.Height})",
    VideoMessage vid => $"Video: {vid.Duration}s",
    _ => "Unknown message type"
};

// ✅ Perfect for: Visitor pattern replacement
var cost = product switch
{
    PhysicalProduct p => p.Price + CalculateShipping(p.Weight),
    DigitalProduct d => d.Price,
    SubscriptionProduct s => s.MonthlyFee * s.Months,
    _ => 0
};
```

**Don't use when**: You can use virtual methods (use polymorphism instead)

### 4. Relational Patterns - Range Checks

**Use when**: Decision based on numeric ranges

```csharp
// ✅ Perfect for: Tax brackets
var taxRate = income switch
{
    <= 10000 => 0.10m,
    > 10000 and <= 50000 => 0.20m,
    > 50000 and <= 100000 => 0.30m,
    > 100000 => 0.40m
};

// ✅ Perfect for: Grade assignment
var grade = score switch
{
    >= 90 => 'A',
    >= 80 and < 90 => 'B',
    >= 70 and < 80 => 'C',
    >= 60 and < 70 => 'D',
    < 60 => 'F'
};
```

**Don't use when**: Ranges overlap (compiler will error)

### 5. Logical Patterns - Complex Conditions

**Use when**: Multiple conditions combine with AND/OR/NOT

```csharp
// ✅ Perfect for: Complex business rules
var eligibility = applicant switch
{
    { Age: >= 18, Income: > 30000 } and { CreditScore: > 650 }
        => "Approved",

    { Age: >= 18, Income: > 50000 } or { CreditScore: > 750 }
        => "Review",

    not { Age: >= 18 }
        => "Rejected - Age",

    _ => "Rejected"
};

// ✅ Perfect for: Feature flags
var feature = (user, settings) switch
{
    ({ IsPremium: true }, _) => "Premium feature enabled",
    (_, { BetaFeaturesEnabled: true }) => "Beta feature enabled",
    _ => "Standard features only"
};
```

**Don't use when**: Logic is too complex (extract to method)

### 6. Positional Patterns - Record Deconstruction

**Use when**: Working with records or tuples

```csharp
// ✅ Perfect for: Tuple matching
var result = (statusCode, hasContent) switch
{
    (200, true) => "Success with data",
    (200, false) => "Success, no data",
    (404, _) => "Not found",
    (500, _) => "Server error",
    _ => "Unknown"
};

// ✅ Perfect for: Record pattern matching
public record Point(int X, int Y);

var quadrant = point switch
{
    (> 0, > 0) => "Q1",
    (< 0, > 0) => "Q2",
    (< 0, < 0) => "Q3",
    (> 0, < 0) => "Q4",
    (0, 0) => "Origin",
    _ => "On axis"
};
```

**Don't use when**: Position meanings are unclear (use property patterns)

### 7. List Patterns - Array Structure

**Use when**: Decision depends on array/list structure

```csharp
// ✅ Perfect for: Command parsing
var command = args switch
{
    [] => "No arguments",
    ["help"] => ShowHelp(),
    ["run", var file] => RunFile(file),
    ["run", var file, ..var options] => RunFileWithOptions(file, options),
    [.., "clean"] => CleanUp(),
    _ => "Unknown command"
};

// ✅ Perfect for: Data validation
var validation = coordinates switch
{
    [] => "Empty - invalid",
    [_] => "Single point - invalid",
    [_, _] => "Line segment - valid",
    [_, _, _] => "Triangle - valid",
    _ => $"Polygon ({coordinates.Length} points) - valid"
};
```

**Don't use when**: Arrays are very large (performance impact)

### 8. Var Patterns - Value Capture

**Use when**: Need to use matched value in result

```csharp
// ✅ Perfect for: Guard clauses with values
var message = invoice switch
{
    { Amount: var amt } when amt > 10000 =>
        $"Large invoice: ${amt:N2} requires CFO approval",

    { Discount: var disc } when disc > 0.20m =>
        $"High discount: {disc:P0} requires manager approval",

    _ => "Standard processing"
};

// ✅ Perfect for: Complex calculations
var shipping = (weight, distance) switch
{
    ( var w, var d) when w * d > 1000 => w * d * 0.05m,
    ( var w, var d) => w * d * 0.10m
};
```

**Don't use when**: Value isn't used (just use property pattern)

### 9. Discard Patterns - Ignoring Values

**Use when**: Only care about some parts of a pattern

```csharp
// ✅ Perfect for: Partial tuple matching
var status = (isPaid, amount, _) switch
{
    (true, _, _) => "Paid",
    (false, > 1000, _) => "Unpaid - Large",
    (false, _, _) => "Unpaid - Small"
};

// ✅ Perfect for: Type checking without value
var category = product switch
{
    PhysicalProduct _ => "Physical",
    DigitalProduct _ => "Digital",
    _ => "Unknown"
};
```

**Don't use when**: You might need the value later (capture it)

### 10. Recursive Patterns - Deep Matching

**Use when**: Decision depends on nested properties

```csharp
// ✅ Perfect for: Complex eligibility rules
var offer = customer switch
{
    {
        Account: { Status: "Gold", Years: > 5 },
        Orders: { Count: > 100, TotalValue: > 50000 }
    } => "Platinum upgrade offer",

    {
        Account: { Status: "Silver", Years: > 3 },
        Orders: { Count: > 50 }
    } => "Gold upgrade offer",

    _ => "No offers"
};
```

**Don't use when**: Nesting is too deep (flatten structure)

---

## Real-World Scenarios

### Scenario 1: E-Commerce Order Processing

```csharp
public class OrderProcessor
{
    public string ProcessOrder(Order order) => order switch
    {
        // VIP customers with large orders
        {
            Customer: { Level: "VIP" },
            TotalAmount: > 1000,
            Items: { Count: > 5 }
        } => "Priority processing + Free express shipping + 15% loyalty discount",

        // New customers with first order
        {
            Customer: { OrderCount: 1 },
            TotalAmount: > 100
        } => "Welcome discount 10% + Standard shipping",

        // High-value orders
        { TotalAmount: > 500 } => "Standard processing + Free standard shipping",

        // Standard orders
        { TotalAmount: > 0 } => "Standard processing + Shipping fee applies",

        // Empty cart
        { Items.Count: 0 } => "Cart is empty",

        _ => "Unable to process order"
    };
}
```

### Scenario 2: API Response Handling

```csharp
public async Task<ApiResult> ProcessApiResponse(HttpResponseMessage response)
{
    return (response.StatusCode, await response.Content.ReadAsStringAsync()) switch
    {
        // Success cases
        (HttpStatusCode.OK, var content) when !string.IsNullOrEmpty(content)
            => ApiResult.Success(content),

        (HttpStatusCode.OK, _)
            => ApiResult.Success("No content"),

        // Client errors
        (HttpStatusCode.BadRequest, var error)
            => ApiResult.Failure($"Bad request: {error}"),

        (HttpStatusCode.Unauthorized, _)
            => ApiResult.Failure("Authentication required"),

        (HttpStatusCode.NotFound, _)
            => ApiResult.Failure("Resource not found"),

        // Server errors
        (HttpStatusCode.InternalServerError, var error)
            => ApiResult.Failure($"Server error: {error}"),

        (>= HttpStatusCode.InternalServerError, _)
            => ApiResult.Failure("Server unavailable"),

        // Unknown
        _ => ApiResult.Failure($"Unexpected status: {response.StatusCode}")
    };
}
```

### Scenario 3: State Machine Implementation

```csharp
public record GameState(string State, int Lives, int Score, bool HasPowerup);

public class GameEngine
{
    public (GameState newState, string action) ProcessInput(GameState state, string input)
        => (state, input) switch
        {
            // State transitions with conditions
            ({ State: "Menu" }, "start")
                => (state with { State = "Playing", Lives = 3, Score = 0 }, "Game started"),

            ({ State: "Playing", Lives: > 1 }, "hit")
                => (state with { Lives = state.Lives - 1 }, "Lost a life"),

            ({ State: "Playing", Lives: 1 }, "hit")
                => (state with { State = "GameOver", Lives = 0 }, "Game over"),

            ({ State: "Playing", HasPowerup: true }, "use_powerup")
                => (state with { HasPowerup = false, Score = state.Score + 100 }, "Powerup used"),

            ({ State: "Playing" }, "pause")
                => (state with { State = "Paused" }, "Game paused"),

            ({ State: "Paused" }, "resume")
                => (state with { State = "Playing" }, "Game resumed"),

            ({ State: "GameOver" }, "restart")
                => (new GameState("Playing", 3, 0, false), "New game"),

            // No valid transition
            _ => (state, "Invalid action")
        };
}
```

### Scenario 4: Validation Engine

```csharp
public class UserValidator
{
    public ValidationResult Validate(UserRegistration user) => user switch
    {
        // All validations
        {
            Email: var email,
            Password: var pwd,
            Age: var age
        } when !email.Contains("@")
            => ValidationResult.Fail("Invalid email format"),

        {
            Email: var email,
            Password: var pwd,
            Age: var age
        } when pwd.Length < 8
            => ValidationResult.Fail("Password too short (min 8 characters)"),

        {
            Email: var email,
            Password: var pwd,
            Age: < 18
        }
            => ValidationResult.Fail("Must be 18 or older"),

        {
            Email: var email,
            Password: var pwd,
            Age: >= 18
        } when email.Contains("@") && pwd.Length >= 8
            => ValidationResult.Success(),

        _ => ValidationResult.Fail("Unknown validation error")
    };
}
```

---

## Performance Considerations

### 1. Compiler Optimizations

**Good news**: The C# compiler optimizes pattern matching extensively!

```csharp
// This switch expression
var result = value switch
{
    1 => "one",
    2 => "two",
    3 => "three",
    _ => "other"
};

// Compiles to efficient jump table (O(1) lookup)
// Similar to traditional switch statement performance
```

### 2. Pattern Evaluation Order

**Important**: Patterns are evaluated **top to bottom**

```csharp
// ⚠️ BAD: Expensive operation first
var result = data switch
{
    var x when ExpensiveCheck(x) => "Found",  // Always evaluated first
    null => "Null",                            // Could short-circuit
    _ => "Other"
};

// ✅ GOOD: Cheap checks first
var result = data switch
{
    null => "Null",                            // O(1) - check first
    var x when ExpensiveCheck(x) => "Found",  // Only if not null
    _ => "Other"
};
```

### 3. Type Pattern Performance

**Type patterns are fast** - uses `is` operator (optimized runtime check)

```csharp
// Efficient - single type check + cast
var result = obj switch
{
    string s => $"String: {s}",
    int i => $"Int: {i}",
    _ => "Other"
};
```

### 4. Property Pattern Overhead

**Be aware**: Property patterns access properties (may have cost)

```csharp
// ⚠️ Potentially expensive if Property has complex getter
var result = obj switch
{
    { Property: > 100 } => "Large",  // Accesses Property
    _ => "Small"
};

// ✅ Better: Cache if used multiple times
var value = obj.Property;
var result = value switch
{
    > 100 => "Large",
    _ => "Small"
};
```

### 5. List Pattern Performance

**List patterns allocate** for slice operations

```csharp
// ⚠️ May allocate for '..' slice
var result = array switch
{
    [var first, .. var rest] => ProcessRest(rest),  // 'rest' is new array
    _ => null
};

// ✅ Better for large arrays: Use indices
var result = array.Length switch
{
    0 => null,
    _ => ProcessRest(array[1..])  // More control over allocation
};
```

### Benchmark Results

```csharp
// Pattern matching vs traditional if-else (10 million iterations)

| Method           | Mean     | Allocated |
|------------------|----------|-----------|
| TraditionalIf    | 12.3 ms  | 0 B       |
| SwitchExpression | 11.8 ms  | 0 B       |  // ✅ Slightly faster!
| PropertyPattern  | 13.1 ms  | 0 B       |
| TypePattern      | 12.5 ms  | 0 B       |

// Conclusion: Pattern matching is as fast or faster than traditional code
```

---

## Common Mistakes

### Mistake 1: Unreachable Patterns

```csharp
// ❌ WRONG: Second pattern is unreachable
var result = amount switch
{
    > 100 => "Large",
    > 50 => "Medium",  // ⚠️ Never reached! (> 100 catches everything > 100)
    _ => "Small"
};

// ✅ CORRECT: Order from specific to general
var result = amount switch
{
    > 100 => "Large",
    > 50 and <= 100 => "Medium",
    _ => "Small"
};
```

### Mistake 2: Missing Default Case

```csharp
// ❌ WRONG: No default case
var result = invoice switch
{
    StandardInvoice => "Standard",
    PremiumInvoice => "Premium"
    // ⚠️ Throws if WholesaleInvoice!
};

// ✅ CORRECT: Always include default
var result = invoice switch
{
    StandardInvoice => "Standard",
    PremiumInvoice => "Premium",
    _ => "Unknown"  // Safe fallback
};
```

### Mistake 3: Over-Nesting

```csharp
// ❌ WRONG: Too deeply nested
var result = invoice switch
{
    {
        Customer: {
            Address: {
                City: {
                    PostalCode: var code
                }
            }
        }
    } when code.StartsWith("12") => "Region 12",
    _ => "Other"
};

// ✅ CORRECT: Flatten or extract
var postalCode = invoice.Customer?.Address?.City?.PostalCode;
var result = postalCode switch
{
    string code when code.StartsWith("12") => "Region 12",
    _ => "Other"
};
```

### Mistake 4: Expensive Operations in Patterns

```csharp
// ❌ WRONG: Expensive operation in guard clause
var result = items switch
{
    var x when x.Sum(i => i.Price) > 1000 => "Expensive",  // ⚠️ Sum called for every item!
    _ => "Cheap"
};

// ✅ CORRECT: Calculate once
var total = items.Sum(i => i.Price);
var result = total switch
{
    > 1000 => "Expensive",
    _ => "Cheap"
};
```

### Mistake 5: Mixing Old and New Syntax

```csharp
// ❌ WRONG: Inconsistent style
var type = invoice switch { StandardInvoice => "Standard", _ => "Other" };

string status;
switch (invoice.IsPaid)
{
    case true: status = "Paid"; break;
    default: status = "Unpaid"; break;
}

// ✅ CORRECT: Use switch expressions consistently
var type = invoice switch { StandardInvoice => "Standard", _ => "Other" };
var status = invoice.IsPaid switch { true => "Paid", false => "Unpaid" };
```

---

## Migration Strategies

### Strategy 1: Incremental Adoption

**Start small**, don't rewrite everything at once:

```csharp
// Phase 1: Simple switch statements → switch expressions
// BEFORE
string result;
switch (status)
{
    case 1: result = "Active"; break;
    case 2: result = "Inactive"; break;
    default: result = "Unknown"; break;
}

// AFTER
var result = status switch
{
    1 => "Active",
    2 => "Inactive",
    _ => "Unknown"
};

// Phase 2: Type checking → type patterns
// BEFORE
if (shape is Circle)
{
    var circle = (Circle)shape;
    return circle.Radius * 2;
}

// AFTER
if (shape is Circle c)
    return c.Radius * 2;

// Phase 3: Complex conditionals → property patterns
// BEFORE
if (order.Status == OrderStatus.Pending && order.Amount > 1000)
    return "High priority";

// AFTER
return order switch
{
    { Status: OrderStatus.Pending, Amount: > 1000 } => "High priority",
    _ => "Normal"
};
```

### Strategy 2: Identify Candidates

**Look for these patterns** in your codebase:

1. **Long switch statements** → Switch expressions
2. **Type checking with casting** → Type patterns
3. **Nested if-else for ranges** → Relational patterns
4. **Complex boolean conditions** → Logical patterns
5. **Tuple comparisons** → Positional patterns

### Strategy 3: Refactoring Tools

Use Visual Studio/Rider refactorings:

1. **"Convert switch statement to expression"**
2. **"Use pattern matching"**
3. **"Simplify property pattern"**

---

## Comparison with Alternatives

### vs. Traditional If-Else

| Aspect | If-Else | Pattern Matching |
|--------|---------|------------------|
| **Readability** | ❌ Verbose for multiple conditions | ✅ Concise and declarative |
| **Exhaustiveness** | ❌ No compiler check | ✅ Compiler warns missing cases |
| **Type Safety** | ❌ Manual casting required | ✅ Automatic type narrowing |
| **Performance** | ✅ Same or slightly slower | ✅ Same or slightly faster |
| **Maintainability** | ❌ Easy to miss cases | ✅ Single location for logic |

### vs. Polymorphism (Virtual Methods)

| Aspect | Polymorphism | Pattern Matching |
|--------|--------------|------------------|
| **Best for** | ✅ Behavior that varies by type | ✅ External operations on types |
| **Extensibility** | ✅ Easy to add new types | ❌ Must update all switches |
| **Expression Problem** | ❌ Hard to add operations | ✅ Easy to add operations |
| **Performance** | ✅ Virtual dispatch | ✅ Switch dispatch |
| **When to use** | Type owns behavior | External processing logic |

**Example**:

```csharp
// Polymorphism: When type owns behavior
public abstract class Shape
{
    public abstract double CalculateArea();  // ✅ Each shape knows its area
}

// Pattern Matching: When behavior is external
public class ShapeSerializer
{
    public string Serialize(Shape shape) => shape switch
    {
        Circle c => $"Circle({c.Radius})",
        Rectangle r => $"Rectangle({r.Width},{r.Height})",
        _ => "Unknown"
    };
}
```

### vs. Strategy Pattern

| Aspect | Strategy Pattern | Pattern Matching |
|--------|------------------|------------------|
| **Complexity** | ❌ Requires interface + classes | ✅ Inline logic |
| **Runtime behavior** | ✅ Can change at runtime | ❌ Compile-time only |
| **Dependency Injection** | ✅ Easy to inject | ❌ Harder to inject |
| **Best for** | Complex algorithms with state | Simple decision logic |

---

## Advanced Techniques

### Technique 1: Pattern Matching with LINQ

```csharp
// Filter with patterns
var vipOrders = orders.Where(o => o is { Customer.Level: "VIP", Amount: > 1000 });

// Select with patterns
var results = messages.Select(m => m switch
{
    TextMessage txt => new { Type = "Text", Content = txt.Text },
    ImageMessage img => new { Type = "Image", Content = img.Url },
    _ => new { Type = "Unknown", Content = "" }
});
```

### Technique 2: Recursive Patterns for Tree Structures

```csharp
public record TreeNode(int Value, TreeNode? Left, TreeNode? Right);

public int SumTree(TreeNode? node) => node switch
{
    null => 0,
    { Left: null, Right: null } => node.Value,  // Leaf
    { Left: var left, Right: var right } =>
        node.Value + SumTree(left) + SumTree(right)  // Recursive
};
```

### Technique 3: Pattern Matching in Exception Handling

```csharp
try
{
    await ProcessAsync();
}
catch (Exception ex) when (ex switch
{
    HttpRequestException { StatusCode: HttpStatusCode.NotFound } => true,
    TimeoutException => true,
    _ => false
})
{
    // Handle specific exceptions
}
```

### Technique 4: Builder Pattern with Patterns

```csharp
public class QueryBuilder
{
    public string Build(QuerySpec spec) => spec switch
    {
        { Table: var t, Where: null, OrderBy: null }
            => $"SELECT * FROM {t}",

        { Table: var t, Where: var w, OrderBy: null }
            => $"SELECT * FROM {t} WHERE {w}",

        { Table: var t, Where: var w, OrderBy: var o }
            => $"SELECT * FROM {t} WHERE {w} ORDER BY {o}",

        _ => throw new ArgumentException("Invalid query spec")
    };
}
```

### Technique 5: State Validation

```csharp
public record AppState(bool IsLoggedIn, string? UserName, string? Token);

public bool IsValidState(AppState state) => state switch
{
    { IsLoggedIn: true, UserName: not null, Token: not null } => true,
    { IsLoggedIn: false, UserName: null, Token: null } => true,
    _ => false  // Invalid state (logged in without token, etc.)
};
```

---

## Summary

### When to Use Pattern Matching

✅ **DO use pattern matching when**:
1. You have multiple conditions based on type, value, or structure
2. You want concise, readable decision logic
3. You need compiler-verified exhaustiveness
4. You're replacing long if-else chains or switch statements
5. You're working with discriminated unions or algebraic data types

❌ **DON'T use pattern matching when**:
1. Behavior belongs in the type itself (use polymorphism)
2. Logic is too complex (extract to methods)
3. Patterns would be unreachable or confusing
4. Simple if-else is clearer (don't over-engineer)

### Key Benefits

1. **Expressiveness** - Code reads like natural language
2. **Type Safety** - Compiler catches missing cases
3. **Conciseness** - Less boilerplate than traditional code
4. **Maintainability** - Single source of truth for decisions
5. **Performance** - As fast or faster than alternatives

### Evolution Path

1. **Start**: Traditional if-else and switch statements
2. **Level 1**: Switch expressions for simple mappings
3. **Level 2**: Type and property patterns for object checking
4. **Level 3**: Relational and logical patterns for complex rules
5. **Expert**: List patterns and recursive patterns for advanced scenarios

### Final Recommendation

Pattern matching is **one of the most powerful features in modern C#**. It makes code more:
- **Readable** (clear intent)
- **Correct** (compiler-verified)
- **Concise** (less boilerplate)
- **Maintainable** (single source of truth)

**Start using it today** - even converting a single switch statement will demonstrate its value!

---

**Remember**: Pattern matching is not just syntactic sugar - it's a fundamentally different way of thinking about conditional logic that leads to better code. 🔍
