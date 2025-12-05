# Dynamic Code Generation

> **Expert C# Pattern** - Generate types and methods at runtime using Reflection.Emit and IL opcodes.

## What This Demonstrates

- **TypeBuilder** - Create classes/structs at runtime
- **MethodBuilder** - Define methods dynamically
- **IL Generation** - Emit IL opcodes (add, mul, ldarg, ret)
- **DynamicMethod** - Lightweight method generation

## Quick Start

```bash
cd samples/04-Expert/DynamicCodeGeneration
dotnet run
```

**Output:**
```
=== Dynamic Code Generation Example ===

✅ Created dynamic type: Calculator
✅ Dynamic Add(10, 32) = 42
✅ Dynamic Multiply(6, 7) = 42

✅ Dynamic code generation complete!
```

## Use Cases

- **ORM Mappers** - Generate entity mappers at runtime (Dapper, EF Core)
- **Proxy Generation** - Create proxies for AOP/mocking (Castle DynamicProxy, Moq)
- **Expression Compilation** - LINQ to SQL query generation
- **Performance** - Faster than reflection (direct method calls)

## Key Concepts

**IL Opcodes:**
- `Ldarg_1` - Load argument 1
- `Add` - Add two values
- `Mul` - Multiply two values
- `Ret` - Return from method

**When to Use:**
- Performance-critical dynamic code
- Plugin systems
- Advanced metaprogramming

**When NOT to Use:**
- Native AOT (not supported!)
- Simple scenarios (use delegates/lambdas)
- Debugging required (IL is hard to debug)
