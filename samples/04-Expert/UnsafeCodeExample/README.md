# Unsafe Code Example

> **Expert C# Pattern** - Pointer arithmetic and unsafe context for maximum performance.

## What This Demonstrates

- **Pointers** - Direct memory access with `int*`, `byte*`
- **Fixed Statement** - Pin objects in memory
- **Pointer Arithmetic** - Navigate memory manually
- **stackalloc** - Stack-based allocations (zero GC pressure)

## Quick Start

```bash
cd samples/04-Expert/UnsafeCodeExample
dotnet run
```

**Output:**
```
=== Unsafe Code Example ===

1. Pointer Arithmetic
   Value: 42
   Pointer address: 0x...
   Dereferenced: 42
   After modification: 100

2. High-Performance Array Sum
   Sum: 55

3. Stack Allocation (stackalloc)
   Stack-allocated array:
   [0] = 10
   [1] = 20
   ...
   ✅ Zero heap allocations!

✅ Unsafe code demonstration complete!
```

## When to Use Unsafe Code

**✅ Good For:**
- Image processing (pixel manipulation)
- Network protocols (binary parsing)
- Interop with C/C++ libraries
- Performance-critical inner loops

**❌ Avoid For:**
- General application code
- When Span<T> works (safer alternative)
- Maintainability matters
- Cross-platform compatibility concerns

## Safety Considerations

- **Buffer overruns** - Pointer arithmetic can access invalid memory
- **Garbage collector** - Use `fixed` to pin objects
- **Platform specific** - Pointer sizes vary (32-bit vs 64-bit)
- **No bounds checking** - Crashes instead of exceptions

## Modern Alternatives

**Prefer Span<T> when possible:**
```csharp
// ❌ Unsafe (verbose, risky)
unsafe {
    fixed (int* ptr = array) { ... }
}

// ✅ Span<T> (safe, fast)
Span<int> span = array;
```

Span<T> gives 90% of unsafe performance with 100% safety!
