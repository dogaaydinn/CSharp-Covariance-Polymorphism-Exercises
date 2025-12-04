# Casting Examples - C# Type Conversions Mastery

**Level:** Beginner
**Topics:** Implicit/Explicit Casting, is/as Operators, Pattern Matching
**Estimated Time:** 45 minutes

## 🎯 Learning Objectives

After completing this tutorial, you will understand:
- ✅ The difference between implicit and explicit casting
- ✅ When to use `is` vs `as` operators
- ✅ Modern C# pattern matching for type checking
- ✅ Boxing and unboxing performance implications
- ✅ Common casting pitfalls and how to avoid them

## 📚 What You'll Learn

### 1. **Implicit Casting** (Automatic, Safe)
- Numeric conversions: `byte → short → int → long → float → double`
- Derived to base class (upcasting)
- Nullable type conversions
- **Rule:** No data loss possible

### 2. **Explicit Casting** (Manual, Potentially Unsafe)
- Narrowing conversions with data loss
- Checked vs unchecked overflow
- Downcasting (base → derived)
- Boxing/unboxing value types
- String to numeric conversions

### 3. **The `is` Operator** (Type Checking)
- Safe runtime type checking
- Pattern matching with type patterns
- Property patterns
- Performance benefits over try-catch

### 4. **The `as` Operator** (Safe Casting)
- Returns `null` on failure (no exceptions)
- When to use `as` vs direct cast
- Combining with null-coalescing (`??`)
- Interface casting

### 5. **Modern Pattern Matching**
- Switch expressions with types
- Property and positional patterns
- Relational patterns (`<`, `>`, `>=`, `<=`)
- Logical patterns (`and`, `or`, `not`)
- List patterns (C# 11+)

## 🚀 Quick Start

```bash
cd samples/01-Beginner/CastingExamples
dotnet run
```

The interactive menu lets you explore 36 different casting scenarios!

## 📖 Key Concepts

### Casting Safety Hierarchy

```
Safest  → Implicit Casting  (automatic, compile-time checked)
          ↓
        → 'is' Operator      (type check only, no cast)
          ↓
        → 'as' Operator      (returns null on failure)
          ↓
Riskiest → Direct Cast       (throws exception on failure)
```

### When to Use Each Approach

| Scenario | Recommended Approach | Why |
|----------|---------------------|-----|
| Widening conversion | Implicit cast | Automatic, safe |
| Type check only | `is` operator | No cast overhead |
| Optional cast | `as` + null check | No exception |
| Multiple types | Switch expression | Clean, readable |
| Guaranteed type | Direct cast | Fastest (if certain) |

## 💡 Common Pitfalls

### ❌ **Pitfall 1: Unchecked Narrowing**
```csharp
long bigNumber = 5_000_000_000;
int overflow = (int)bigNumber;  // Silent overflow!
```

✅ **Solution: Use checked keyword**
```csharp
checked
{
    int safeConversion = (int)bigNumber;  // Throws OverflowException
}
```

### ❌ **Pitfall 2: Unboxing Wrong Type**
```csharp
object boxed = 42;  // Boxed as int
long wrong = (long)boxed;  // ❌ InvalidCastException!
```

✅ **Solution: Unbox to exact type**
```csharp
int correct = (int)boxed;  // ✅ Matches boxed type
long converted = correct;   // Then convert if needed
```

### ❌ **Pitfall 3: Unsafe Downcasting**
```csharp
Animal animal = GetAnimal();
Dog dog = (Dog)animal;  // ❌ Might throw!
```

✅ **Solution: Use pattern matching**
```csharp
if (animal is Dog dog)
{
    dog.Bark();  // ✅ Safe
}
```

## 🎓 Best Practices

### 1. **Prefer Pattern Matching**
```csharp
// Old way
if (obj is Dog)
{
    Dog dog = (Dog)obj;
    dog.Bark();
}

// Modern way
if (obj is Dog dog)
{
    dog.Bark();  // Type check + cast + variable declaration
}
```

### 2. **Use Switch Expressions for Multiple Types**
```csharp
string description = animal switch
{
    Dog { Breed: "Labrador" } => "Friendly Labrador",
    Dog d => $"Dog: {d.Breed}",
    Cat c => $"Cat: {c.Color}",
    _ => "Unknown animal"
};
```

### 3. **Avoid Boxing in Hot Paths**
```csharp
// ❌ Bad: Boxing in loop
for (int i = 0; i < 1000000; i++)
{
    object boxed = i;  // Allocates on heap!
    ProcessObject(boxed);
}

// ✅ Good: Use generics
for (int i = 0; i < 1000000; i++)
{
    ProcessValue(i);  // No boxing
}

void ProcessValue<T>(T value) { /* ... */ }
```

## 🔬 Performance Insights

### Pattern Matching Performance
```
Direct cast (unsafe):       Fastest (if type is certain)
'is' with pattern:          ~5% slower than direct cast
'as' with null check:       ~5% slower than direct cast
try-catch on cast:          ~100x slower (exception path)
```

**Conclusion:** Use `is` or `as` - the safety is worth the tiny overhead!

## 📊 Code Statistics

- **Total Lines:** ~3,500
- **Example Methods:** 36
- **Topics Covered:** 5 major areas
- **Interactive Demos:** 36 runnable scenarios

## 🔗 Related Topics

- **Polymorphism:** See `PolymorphismExamples` sample
- **Covariance/Contravariance:** See `CovarianceContravariance` sample
- **Boxing Performance:** See `BoxingPerformance` sample

## 📝 Exercises

Try these challenges to test your understanding:

1. **Casting Chain:** Create a chain: `byte → short → int → long → double`
2. **Safe Downcast:** Write a method that safely downcasts and returns default on failure
3. **Type Filter:** Filter a list to get only specific types using pattern matching
4. **Performance Test:** Compare `as` vs `is` vs direct cast in a loop

## 🎉 Summary

You've learned:
- ✅ **5 types of casting** from basic to advanced
- ✅ **When to use** each casting approach
- ✅ **Common pitfalls** and how to avoid them
- ✅ **Modern patterns** for clean, safe type checking
- ✅ **Performance implications** of different approaches

**Next Steps:** Explore the `OverrideVirtual` sample to see how casting works with polymorphism!

---

**Created:** 2025-12-01
**C# Version:** 12.0 (.NET 8.0)
**Difficulty:** ⭐⭐☆☆☆

