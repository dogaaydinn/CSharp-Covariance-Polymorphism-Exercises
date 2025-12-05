# High-Performance Span&lt;T&gt;

> **Expert C# Pattern** - Zero-allocation string and array processing using Span&lt;T&gt; and Memory&lt;T&gt;.

## What This Demonstrates

- **Span&lt;T&gt;** - Stack-only type for array slices
- **ReadOnlySpan&lt;T&gt;** - Immutable spans
- **stackalloc** - Stack-based allocations
- **Zero-copy slicing** - No data duplication

## Quick Start

```bash
cd samples/04-Expert/HighPerformanceSpan
dotnet run
```

**Output:**
```
=== High-Performance Span<T> Example ===

1. Zero-Allocation String Parsing
   First name: John
   ✅ Zero string allocations!

2. Array Slicing (No Copying)
   Middle 3 elements: 5 6 7
   Original array[4]: 999 (modified!)

3. Stack-Based Processing
   Processed 256 bytes on stack
   ✅ Zero heap allocations!

4. ReadOnlySpan for Immutability
   Length: 13
   First char: H
   ✅ Type-safe immutability!

✅ All operations completed with zero allocations!
```

## Why Span&lt;T&gt;?

### Traditional Approach (Allocates Memory)

```csharp
string data = "John,Doe,30";
string firstName = data.Substring(0, 4);  // ❌ Allocates new string!
```

### Span&lt;T&gt; Approach (Zero Allocations)

```csharp
ReadOnlySpan<char> data = "John,Doe,30".AsSpan();
ReadOnlySpan<char> firstName = data.Slice(0, 4);  // ✅ No allocation!
```

## Performance Benefits

| Operation | Traditional | Span&lt;T&gt; | Improvement |
|-----------|------------|---------|-------------|
| String slice | 500ns + allocation | 5ns + zero allocation | **100x faster** |
| Array slice | Copy entire array | Reference existing | **Zero-copy** |
| Parse CSV line | 10 allocations | 0 allocations | **Infinite** |

## Use Cases

**✅ Perfect For:**
- **String parsing** - CSV, JSON, log files
- **Binary protocols** - Network packets, file formats
- **High-throughput APIs** - Process millions of requests
- **Game loops** - 60 FPS with zero GC pauses

**❌ Not Suitable For:**
- **Storing in fields** - Span is stack-only (use Memory&lt;T&gt; instead)
- **Async methods** - Can't cross await boundaries
- **LINQ queries** - Use arrays/lists for LINQ

## Key Differences

| Type | Heap/Stack | Mutable | Use Case |
|------|-----------|---------|----------|
| **Span&lt;T&gt;** | Stack | Yes | Local processing |
| **ReadOnlySpan&lt;T&gt;** | Stack | No | Immutable views |
| **Memory&lt;T&gt;** | Heap | Yes | Async/storage |
| **ReadOnlyMemory&lt;T&gt;** | Heap | No | Async immutable |

## Best Practices

1. **Use ReadOnlySpan&lt;T&gt; for inputs** - Prevents accidental modifications
2. **stackalloc for small buffers** - < 1KB is safe
3. **Slice instead of Substring** - Zero allocations
4. **Avoid Span in async** - Use Memory&lt;T&gt; instead

## Common Patterns

### CSV Parsing (Zero Allocations)

```csharp
ReadOnlySpan<char> line = "Alice,30,Engineer".AsSpan();
int comma1 = line.IndexOf(',');
int comma2 = line.Slice(comma1 + 1).IndexOf(',') + comma1 + 1;

var name = line.Slice(0, comma1);
var age = int.Parse(line.Slice(comma1 + 1, comma2 - comma1 - 1));
var role = line.Slice(comma2 + 1);
```

### Binary Protocol Parsing

```csharp
Span<byte> buffer = stackalloc byte[1024];
socket.Receive(buffer);

int messageType = buffer[0];
int length = BitConverter.ToInt32(buffer.Slice(1, 4));
Span<byte> payload = buffer.Slice(5, length);
```

## Modern C# Features

**C# 12 Collection Expressions:**
```csharp
Span<int> numbers = [1, 2, 3, 4, 5];  // ✨ New in C# 12!
```

**Inline Arrays:**
```csharp
Span<byte> buffer = stackalloc byte[] { 0x01, 0x02, 0x03 };
```

---

**📝 Summary:** Span&lt;T&gt; enables zero-allocation, high-performance code while maintaining type safety. Essential for performance-critical applications.
