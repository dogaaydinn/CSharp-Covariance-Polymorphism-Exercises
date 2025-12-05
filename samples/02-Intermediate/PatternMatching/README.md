# 🔍 Pattern Matching - Invoice Processing System

## 📚 Overview

**PatternMatching** demonstrates C# 8.0+ pattern matching features with a complete invoice processing system. Learn modern switch expressions, property/type/relational/list patterns, and complex nested pattern matching for expressive, type-safe code.

**Lines of Code**: 621
**Build Status**: ✅ 0 errors
**C# Version**: 8.0+ (Switch expressions, property patterns, relational patterns C# 9.0+, list patterns C# 11.0+)

## 🎯 Key Features

### Pattern Types Demonstrated
1. **Switch Expressions (C# 8.0)** - Modern concise switch syntax with `=>`
2. **Property Patterns** - Match on property values: `{ IsPaid: true }`
3. **Type Patterns** - Match type and capture: `StandardInvoice std => ...`
4. **Relational Patterns (C# 9.0)** - Use `<`, `>`, `<=`, `>=` in patterns
5. **Logical Patterns** - Combine with `and`, `or`, `not` keywords
6. **Positional Patterns** - Deconstruct records: `(name, priority, _, _)`
7. **List Patterns (C# 11.0)** - Match array elements: `[first, .., last]`
8. **Var Patterns** - Capture matched values: `{ Amount: var amt }`
9. **Discard Patterns** - Ignore values with `_`
10. **Combined/Recursive Patterns** - Complex nested property matching

## 💻 Quick Start

```bash
cd samples/02-Intermediate/PatternMatching
dotnet build
dotnet run
```

## 🎓 Core Concepts

### 1. Switch Expressions (C# 8.0)

**Purpose**: Modern, concise alternative to traditional switch statements

```csharp
// ❌ Old way (switch statement)
string invoiceType;
switch (invoice)
{
    case StandardInvoice:
        invoiceType = "Standard";
        break;
    case PremiumInvoice:
        invoiceType = "Premium";
        break;
    default:
        invoiceType = "Unknown";
        break;
}

// ✅ New way (switch expression - C# 8.0)
var invoiceType = invoice switch
{
    StandardInvoice => "Standard",
    PremiumInvoice => "Premium",
    WholesaleInvoice => "Wholesale",
    SubscriptionInvoice => "Subscription",
    _ => "Unknown"
};

// Multiple branches based on value
var processingFee = invoice.Amount switch
{
    < 100 => 5.00m,
    < 500 => 10.00m,
    < 1000 => 15.00m,
    _ => 20.00m
};
```

**Benefits**:
- Expression (returns a value) vs statement
- No `break` needed - each arm is an expression
- Compiler ensures exhaustive matching (all cases covered)
- More concise and readable

### 2. Property Patterns

**Purpose**: Match based on object property values

```csharp
// Match on single property
var status = invoice switch
{
    { IsPaid: true } => "✅ Paid",
    { IsPaid: false } => "⚠️ Unpaid",
    _ => "Unknown"
};

// Combine multiple properties
var urgency = invoice switch
{
    { IsPaid: false, Amount: > 1000 } => "⚠️ Unpaid (Large)",
    { IsPaid: false, Amount: > 500 } => "⚠️ Unpaid (Medium)",
    { IsPaid: false } => "⚠️ Unpaid (Small)",
    _ => "Unknown"
};

// Nested property access
var priority = invoice switch
{
    { Customer.Priority: Priority.VIP } => "🔴 URGENT",
    { Customer.Priority: Priority.High } => "🟠 High",
    { Customer.Priority: Priority.Normal } => "🟡 Normal",
    _ => "🟢 Low"
};
```

**When to use**: Matching based on object state (property values)

### 3. Type Patterns

**Purpose**: Match on type and capture the value

```csharp
var description = invoice switch
{
    // Type pattern: Match type AND capture variable
    StandardInvoice std => $"Standard (Payment term: {std.PaymentTermDays} days)",

    PremiumInvoice prm =>
        $"Premium (Discount: {prm.DiscountPercent}% - Final: ${invoice.Amount * (1 - prm.DiscountPercent / 100):F2})",

    WholesaleInvoice whl =>
        $"Wholesale (Qty: {whl.Quantity} - Discount: {(whl.Quantity > 100 ? 20 : 10)}%)",

    SubscriptionInvoice sub =>
        $"Subscription (Cycle: {sub.BillingCycleMonths} months - Next: {invoice.IssuedDate.AddMonths(sub.BillingCycleMonths):dd.MM.yyyy})",

    _ => "Unknown type"
};
```

**Benefits**:
- Type-safe casting - no need for explicit `as` or `is`
- Captured variable is already correctly typed
- Compiler verifies properties exist

### 4. Relational Patterns (C# 9.0)

**Purpose**: Use comparison operators in patterns

```csharp
// Use <, >, <=, >= directly in patterns
var tier = invoice.Amount switch
{
    < 100 => "Micro",
    >= 100 and < 500 => "Small",
    >= 500 and < 1000 => "Medium",
    >= 1000 and < 5000 => "Large",
    >= 5000 => "Enterprise"
};

// Tax brackets with relational patterns
var taxRate = invoice.Amount switch
{
    <= 100 => 0.05m,           // 5% for small amounts
    > 100 and <= 500 => 0.08m, // 8% for medium
    > 500 and <= 1000 => 0.10m,// 10% for large
    > 1000 => 0.12m            // 12% for very large
};
```

**When to use**: Range-based categorization, tier systems, tax brackets

### 5. Logical Patterns (and, or, not)

**Purpose**: Combine patterns with logical operators

```csharp
var risk = invoice switch
{
    // OR pattern: Match either condition
    { IsPaid: false, Amount: > 1000 } or
    { IsPaid: false, Customer.Priority: Priority.VIP }
        => "🔴 High Risk",

    // AND pattern: Both conditions must match
    { IsPaid: false, Amount: > 500 and < 1000 }
        => "🟠 Medium Risk",

    // NOT pattern: Negate a condition
    not { IsPaid: false } => "🟢 Low Risk",

    _ => "⚪ Unknown"
};

// Combine multiple logical operators
var followup = invoice switch
{
    { IsPaid: false } and ({ Amount: > 1000 } or { Customer.Priority: Priority.VIP })
        => "📞 Requires Followup",

    _ => ""
};
```

**When to use**: Complex business rules with multiple conditions

### 6. Positional Patterns (Records)

**Purpose**: Deconstruct and match record components

```csharp
// Record with positional parameters
public record Customer(string Name, Priority Priority, bool IsCorporate, int YearsActive);

// Positional pattern matching
var category = customer switch
{
    // Match on position: (name, priority, isCorporate, yearsActive)
    (_, Priority.VIP, true, > 3) => "🌟 Platinum Partner",
    (_, Priority.VIP, _, _) => "⭐ VIP Customer",
    (_, Priority.High, true, > 2) => "💼 Premium Corporate",
    (_, _, true, _) => "🏢 Corporate",
    (_, _, _, 0) => "🆕 New Customer",
    _ => "👤 Regular Customer"
};

// First parameter is name (ignored with _)
// Second is Priority (matched specifically)
// Third is IsCorporate (matched specifically)
// Fourth is YearsActive (matched with relational pattern)
```

**When to use**: Records with multiple properties, tuple deconstruction

### 7. List Patterns (C# 11.0)

**Purpose**: Match on array/list elements

```csharp
var batch = new[] { 100m, 200m, 300m };

var analysis = batch switch
{
    [] => "Empty batch",                              // Empty array
    [var single] => $"Single invoice: ${single:F2}",  // Single element
    [var first, var second] => $"Two invoices",       // Exactly two
    [var first, .., var last] => $"Multiple",         // First, middle (any), last
    _ => "Unknown"
};

// Advanced list patterns
var batchType = batch switch
{
    [< 100, < 100, ..] => "🟢 Starts with small invoices",
    [.., > 1000] => "🔴 Ends with large invoice",
    [_, _, _] => "Exactly 3 invoices",
    _ => "⚪ Mixed"
};
```

**When to use**: Validating array structure, batch processing, pattern detection

### 8. Var Patterns

**Purpose**: Capture matched values for further processing

```csharp
var message = invoice switch
{
    // Capture Amount as 'amt' variable
    { Amount: var amt } when amt > 1000 =>
        $"Large invoice of ${amt:F2} requires manager approval",

    // Capture multiple values
    WholesaleInvoice { Quantity: var qty, Amount: var amt } =>
        $"Bulk order: {qty} units at ${amt / qty:F2} each",

    // Capture and use in guard clause
    { Customer.Priority: var priority } when priority == Priority.VIP =>
        $"VIP customer invoice: ${invoice.Amount:F2}",

    _ => "Standard processing"
};
```

**When to use**: Need to use matched value in result expression

### 9. Discard Patterns (_)

**Purpose**: Ignore values you don't need

```csharp
// Discard in positional patterns
var segment = customer switch
{
    (_, Priority.VIP, _, _) => "VIP",      // Only care about priority
    (_, _, true, _) => "Corporate",        // Only care about IsCorporate
    (_, _, _, > 5) => "Long-term",         // Only care about years
    _ => "Regular"                         // Default case
};

// Discard in property patterns
var action = invoice switch
{
    StandardInvoice { PaymentTermDays: _ } => "Process as standard",  // Just check type
    PremiumInvoice _ => "Process with discount",                      // Type only
    _ => "Default processing"
};
```

**When to use**: Type checking without needing the value, positional matching where some positions are irrelevant

### 10. Combined/Recursive Patterns

**Purpose**: Complex nested pattern matching

```csharp
var action = invoice switch
{
    // Complex nested pattern
    PremiumInvoice
    {
        IsPaid: false,
        DiscountPercent: > 15,
        Amount: > 500 and < 2000,
        Customer: { Priority: Priority.VIP or Priority.High, IsCorporate: true }
    } => "🎁 Send thank you gift + Apply extra 5% discount",

    WholesaleInvoice
    {
        Quantity: > 100,
        Amount: > 5000,
        Customer: { IsCorporate: true, YearsActive: > 3 }
    } => "💼 Assign dedicated account manager",

    SubscriptionInvoice
    {
        BillingCycleMonths: >= 12,
        Customer.Priority: not Priority.Low
    } => "⭐ Offer annual discount",

    { IsPaid: true, Amount: > 1000 } => "✅ Send receipt + Request testimonial",

    { IsPaid: false, Amount: > 1000 } and
    { Customer.Priority: Priority.VIP or Priority.High }
        => "📞 Contact for payment plan options",

    _ => "📋 Standard processing"
};
```

**When to use**: Complex business rules, multi-factor decision making

## 📊 10 Demonstrations

### 1. Switch Expressions
```csharp
// Modern switch syntax
var invoiceType = invoice switch
{
    StandardInvoice => "Standard",
    PremiumInvoice => "Premium",
    WholesaleInvoice => "Wholesale",
    SubscriptionInvoice => "Subscription",
    _ => "Unknown"
};

// Returns a value directly (expression, not statement)
var fee = amount switch { < 100 => 5m, < 500 => 10m, _ => 20m };
```

### 2. Property Patterns
```csharp
var status = invoice switch
{
    { IsPaid: true } => "✅ Paid",
    { IsPaid: false, Amount: > 1000 } => "⚠️ Unpaid (Large)",
    { IsPaid: false } => "⚠️ Unpaid (Small)",
    _ => "Unknown"
};
```

### 3. Type Patterns
```csharp
var description = invoice switch
{
    StandardInvoice std => $"Standard (Term: {std.PaymentTermDays} days)",
    PremiumInvoice prm => $"Premium (Discount: {prm.DiscountPercent}%)",
    _ => "Unknown"
};
```

### 4. Relational Patterns
```csharp
var tier = amount switch
{
    < 100 => "Micro",
    >= 100 and < 500 => "Small",
    >= 500 and < 1000 => "Medium",
    >= 1000 => "Large"
};
```

### 5. Logical Patterns
```csharp
var risk = invoice switch
{
    { IsPaid: false, Amount: > 1000 } or { Customer.Priority: Priority.VIP }
        => "🔴 High Risk",
    { IsPaid: false, Amount: > 500 and < 1000 } => "🟠 Medium Risk",
    _ => "🟢 Low Risk"
};
```

### 6. Positional Patterns
```csharp
// Customer record: (Name, Priority, IsCorporate, YearsActive)
var category = customer switch
{
    (_, Priority.VIP, true, > 3) => "🌟 Platinum Partner",
    (_, Priority.VIP, _, _) => "⭐ VIP Customer",
    _ => "👤 Regular"
};
```

### 7. List Patterns
```csharp
var analysis = batch switch
{
    [] => "Empty",
    [var single] => $"Single: ${single}",
    [var first, .., var last] => $"Multiple: ${first} ... ${last}",
    _ => "Unknown"
};
```

### 8. Var Patterns
```csharp
var message = invoice switch
{
    { Amount: var amt } when amt > 1000 =>
        $"Large invoice of ${amt:F2} requires approval",
    _ => "Standard"
};
```

### 9. Discard Patterns
```csharp
var action = invoice switch
{
    StandardInvoice _ => "Process standard",
    PremiumInvoice _ => "Apply discount",
    _ => "Default"
};
```

### 10. Combined Patterns
```csharp
var action = invoice switch
{
    PremiumInvoice { IsPaid: false, Amount: > 500,
                     Customer: { Priority: Priority.VIP } }
        => "🎁 Special offer",
    _ => "📋 Standard"
};
```

## 💡 Best Practices

### DO ✅

1. **Use Switch Expressions Over Statements**
   ```csharp
   // ✅ GOOD: Expression (returns value)
   var result = value switch
   {
       1 => "One",
       2 => "Two",
       _ => "Other"
   };

   // ❌ BAD: Statement (verbose)
   string result;
   switch (value)
   {
       case 1: result = "One"; break;
       case 2: result = "Two"; break;
       default: result = "Other"; break;
   }
   ```

2. **Order Patterns from Specific to General**
   ```csharp
   // ✅ GOOD: Specific cases first
   var category = amount switch
   {
       0 => "Zero",              // Most specific
       < 100 => "Small",
       < 1000 => "Medium",
       _ => "Large"              // Most general (default)
   };

   // ❌ BAD: General case first (unreachable patterns)
   var category = amount switch
   {
       _ => "Large",    // ⚠️ This catches everything!
       < 100 => "Small" // ⚠️ Unreachable - compiler error
   };
   ```

3. **Use Property Patterns for Clarity**
   ```csharp
   // ✅ GOOD: Clear what you're matching
   var status = invoice switch
   {
       { IsPaid: true, Amount: > 1000 } => "Large paid",
       { IsPaid: true } => "Paid",
       _ => "Unpaid"
   };
   ```

4. **Combine Logical Patterns for Complex Rules**
   ```csharp
   // ✅ GOOD: Clear business logic
   var priority = invoice switch
   {
       { IsPaid: false } and ({ Amount: > 5000 } or { Customer.Priority: Priority.VIP })
           => "Urgent",
       _ => "Normal"
   };
   ```

5. **Use Type Patterns with Captured Variables**
   ```csharp
   // ✅ GOOD: Capture and use typed variable
   var info = invoice switch
   {
       StandardInvoice std => $"Due in {std.PaymentTermDays} days",
       PremiumInvoice prm => $"{prm.DiscountPercent}% discount",
       _ => "Unknown"
   };
   ```

6. **Leverage List Patterns for Collections**
   ```csharp
   // ✅ GOOD: Validate array structure
   var validation = items switch
   {
       [] => "Empty - invalid",
       [_] => "Single item - valid",
       [.., var last] when last < 0 => "Negative last - invalid",
       _ => "Valid"
   };
   ```

### DON'T ❌

1. **Don't Make Patterns Unreachable**
   ```csharp
   // ❌ BAD: Second pattern is unreachable
   var result = amount switch
   {
       > 100 => "Large",
       > 50 => "Medium",   // Unreachable! Already caught by > 100
       _ => "Small"
   };

   // ✅ GOOD: Correct order
   var result = amount switch
   {
       > 100 => "Large",
       > 50 => "Medium",
       _ => "Small"
   };
   ```

2. **Don't Over-Nest Patterns**
   ```csharp
   // ❌ BAD: Too deeply nested, hard to read
   var result = invoice switch
   {
       { Customer: { Address: { City: { PostalCode: var code } } } }
           when code.StartsWith("12") => "Region 12",
       _ => "Other"
   };

   // ✅ GOOD: Extract to method or simplify
   var postalCode = invoice.Customer?.Address?.City?.PostalCode;
   var result = postalCode switch
   {
       string code when code.StartsWith("12") => "Region 12",
       _ => "Other"
   };
   ```

3. **Don't Forget the Default Case**
   ```csharp
   // ❌ BAD: No default - throws if no match
   var result = invoice switch
   {
       StandardInvoice => "Standard",
       PremiumInvoice => "Premium"
       // ⚠️ What if WholesaleInvoice? Runtime exception!
   };

   // ✅ GOOD: Always handle default
   var result = invoice switch
   {
       StandardInvoice => "Standard",
       PremiumInvoice => "Premium",
       _ => "Other"  // Safe fallback
   };
   ```

4. **Don't Mix Old and New Syntax**
   ```csharp
   // ❌ BAD: Inconsistent style
   var type = invoice switch
   {
       StandardInvoice => "Standard",
       _ => "Other"
   };

   string status;
   switch (invoice.IsPaid)
   {
       case true: status = "Paid"; break;
       default: status = "Unpaid"; break;
   }

   // ✅ GOOD: Use switch expressions consistently
   var type = invoice switch { StandardInvoice => "Standard", _ => "Other" };
   var status = invoice.IsPaid switch { true => "Paid", false => "Unpaid" };
   ```

## 🔍 Common Patterns

### Pattern 1: Tiered Pricing/Categorization
```csharp
var tier = amount switch
{
    < 100 => "Micro",
    >= 100 and < 500 => "Small",
    >= 500 and < 1000 => "Medium",
    >= 1000 and < 5000 => "Large",
    >= 5000 => "Enterprise"
};
```

### Pattern 2: Multi-Factor Decision Making
```csharp
var action = (isPaid, amount, customerPriority) switch
{
    (false, > 1000, Priority.VIP) => "Urgent followup",
    (false, > 1000, _) => "Payment reminder",
    (true, > 5000, _) => "Send thank you",
    _ => "Standard processing"
};
```

### Pattern 3: Type-Based Polymorphism
```csharp
var processing = shape switch
{
    Circle c => c.Radius * c.Radius * Math.PI,
    Rectangle r => r.Width * r.Height,
    Triangle t => t.Base * t.Height / 2,
    _ => throw new ArgumentException("Unknown shape")
};
```

### Pattern 4: Validation with List Patterns
```csharp
var validation = coordinates switch
{
    [] => "Empty - invalid",
    [_] => "Single point - invalid",
    [_, _] => "Two points - valid line",
    [_, _, _] => "Three points - valid triangle",
    _ => "Polygon"
};
```

### Pattern 5: State Machine Transitions
```csharp
var nextState = (currentState, input) switch
{
    (State.Idle, "start") => State.Running,
    (State.Running, "pause") => State.Paused,
    (State.Paused, "resume") => State.Running,
    (State.Running or State.Paused, "stop") => State.Idle,
    _ => currentState  // No transition
};
```

## 🎯 Use Cases

1. **Invoice/Order Processing** - Categorize by type, amount, status
2. **State Machines** - Transition logic based on current state and input
3. **Tiered Systems** - Pricing tiers, tax brackets, shipping costs
4. **Validation Logic** - Complex validation rules with multiple factors
5. **Polymorphic Dispatch** - Type-based behavior without virtual methods
6. **Data Parsing** - Pattern-based parsing of structured data
7. **API Response Handling** - Match on status codes and response types
8. **Business Rules Engine** - Complex decision trees
9. **Game AI** - Behavior selection based on multiple conditions
10. **Configuration Management** - Select behavior based on settings

## 📈 Benefits

### Readability
```csharp
// Pattern matching is self-documenting
var tier = amount switch
{
    < 100 => "Micro",      // Clear: amounts under 100
    < 500 => "Small",      // Clear: 100-499
    _ => "Large"           // Clear: 500+
};
```

### Type Safety
```csharp
// Compiler verifies pattern completeness
var result = shape switch
{
    Circle c => c.Radius,      // Typed as Circle
    Rectangle r => r.Width,    // Typed as Rectangle
    // ⚠️ Compiler warning if Triangle not handled
};
```

### Conciseness
```csharp
// 3 lines vs 15+ with traditional if/else
var fee = amount switch { < 100 => 5m, < 500 => 10m, _ => 20m };
```

### Exhaustiveness Checking
```csharp
// Compiler ensures all cases handled
var status = priority switch
{
    Priority.Low => "🟢",
    Priority.Normal => "🟡",
    Priority.High => "🟠",
    Priority.VIP => "🔴"
    // No _ needed - all enum values covered
};
```

## 🔗 Related Patterns

- **Strategy Pattern** - Switch expressions can replace strategy selection
- **Visitor Pattern** - Type patterns provide similar functionality
- **State Pattern** - State transitions with tuple patterns
- **Guard Clauses** - `when` clauses act as guards
- **Discriminated Unions** - Type patterns simulate discriminated unions

**See**: [WHY_THIS_PATTERN.md](WHY_THIS_PATTERN.md) for detailed explanation

## 📚 Pattern Types Summary

| Pattern Type | Syntax | Example | Use Case |
|-------------|--------|---------|----------|
| Switch Expression | `value switch { }` | `x switch { 1 => "one", _ => "other" }` | Replace switch statements |
| Property Pattern | `{ Prop: value }` | `{ IsPaid: true }` | Match object properties |
| Type Pattern | `Type var => ` | `Circle c => c.Radius` | Type checking + capture |
| Relational Pattern | `< > <= >=` | `< 100 => "small"` | Range matching |
| Logical Pattern | `and or not` | `> 100 and < 200` | Combine conditions |
| Positional Pattern | `(p1, p2, ...)` | `(_, VIP, true, _)` | Deconstruct records |
| List Pattern | `[elem, .., last]` | `[first, .., last]` | Array structure |
| Var Pattern | `var name` | `{ Amount: var amt }` | Capture values |
| Discard Pattern | `_` | `(_, _, value)` | Ignore values |
| Recursive Pattern | Nested `{ }` | `{ Customer: { Priority: VIP } }` | Nested matching |

## ✨ Key Takeaways

1. ✅ **Switch expressions** replace switch statements with concise, expression-based syntax
2. ✅ **Property patterns** match on object state: `{ IsPaid: true }`
3. ✅ **Type patterns** combine type checking and variable capture
4. ✅ **Relational patterns** use `<`, `>`, `<=`, `>=` for ranges
5. ✅ **Logical patterns** combine with `and`, `or`, `not`
6. ✅ **Positional patterns** deconstruct records/tuples
7. ✅ **List patterns** (C# 11+) match array structure: `[first, .., last]`
8. ✅ **Var patterns** capture values for use in expressions
9. ✅ **Order matters** - specific patterns before general ones
10. ✅ **Always include default** (`_`) to handle all cases

---

**Remember**: Pattern matching makes code more expressive, type-safe, and concise - perfect for complex decision logic! 🔍
