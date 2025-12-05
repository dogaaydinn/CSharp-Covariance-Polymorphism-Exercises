# Native AOT Example

> **Expert-Level C# Pattern** - Ahead-of-time compilation for faster startup, smaller binaries, and better performance.

## 📋 Quick Reference

**What:** Compile C# to native machine code instead of IL bytecode
**When:** CLI tools, containers, serverless functions, microservices
**Why:** 10-100x faster startup, 90% smaller binaries, lower memory usage
**Level:** Expert (requires understanding of AOT limitations and workarounds)

## 🎯 What This Example Demonstrates

### Benefits of Native AOT

1. **Faster Startup** - 5-10ms vs 100-300ms (JIT compilation)
2. **Smaller Binaries** - 5-10MB vs 100-150MB
3. **Lower Memory** - No JIT compiler overhead
4. **Better Predictability** - No runtime compilation pauses

### AOT-Compatible Patterns

✅ **What Works:**
- Direct method calls (no dynamic dispatch)
- Generic types known at compile time
- JSON source generators
- Span&lt;T&gt; and Memory&lt;T&gt;
- LINQ (compiled to native code)

❌ **What Doesn't Work:**
- Reflection (Type.GetType, Activator.CreateInstance)
- Dynamic code generation (Reflection.Emit)
- C# `dynamic` keyword
- Some serialization libraries (use source generators)

## 🚀 Getting Started

### Build and Run

```bash
# Standard build (JIT)
cd samples/04-Expert/NativeAOTExample
dotnet build
dotnet run

# Publish as Native AOT (creates native executable)
dotnet publish -c Release

# Run the native executable
./bin/Release/net8.0/osx-arm64/publish/NativeAOTExample
# (Path varies by OS: win-x64, linux-x64, osx-arm64, etc.)
```

### Expected Output

```
=== Native AOT Example ===

1. Basic Operations (AOT-Compatible)
   Sum: 15
   Product: 120

2. JSON Serialization (Source Generator)
   Serialized: {"Name":"Alice Johnson","Age":30,"Email":"alice@example.com"}
   Deserialized: Alice Johnson, Age 30

3. Collection Operations (Zero Allocations)
   Processing 5 items:
   - Apple (5 chars)
   - Banana (6 chars)
   - Cherry (6 chars)
   - Date (4 chars)
   - Elderberry (10 chars)

✅ Total execution time: 12ms
✅ Native AOT enables fast startup and small binary size!
```

### Size Comparison

```bash
# Standard .NET publish
dotnet publish -c Release /p:PublishSingleFile=true
# Result: ~65MB (includes .NET runtime)

# Native AOT publish
dotnet publish -c Release
# Result: ~3-8MB (native code only)

# Savings: 90% smaller!
```

### Startup Time Comparison

| Build Type | Cold Start | Warm Start |
|------------|-----------|-----------|
| **JIT (.NET)** | 150-300ms | 50-100ms |
| **Native AOT** | 5-15ms | 3-10ms |
| **Improvement** | **10-30x faster** | **5-10x faster** |

## 📖 How It Works

### 1. Enable AOT in .csproj

```xml
<PropertyGroup>
  <!-- Enable Native AOT -->
  <PublishAot>true</PublishAot>

  <!-- Reduce size further -->
  <InvariantGlobalization>true</InvariantGlobalization>

  <!-- Optimize for speed -->
  <IlcOptimizationPreference>Speed</IlcOptimizationPreference>

  <!-- Disable stack trace data (smaller binary) -->
  <IlcGenerateStackTraceData>false</IlcGenerateStackTraceData>
</PropertyGroup>
```

### 2. JSON Source Generator (No Reflection!)

```csharp
// ❌ BAD: Reflection-based (not AOT-compatible)
var json = JsonSerializer.Serialize(person);

// ✅ GOOD: Source generator (AOT-compatible)
[JsonSerializable(typeof(Person))]
internal partial class AppJsonContext : JsonSerializerContext { }

var json = JsonSerializer.Serialize(person, AppJsonContext.Default.Person);
```

**Why:** Source generators create serialization code at compile time, eliminating reflection.

### 3. Avoid Reflection

```csharp
// ❌ BAD: Not AOT-compatible
var type = Type.GetType("MyNamespace.MyClass");
var instance = Activator.CreateInstance(type);

// ✅ GOOD: Direct instantiation
var instance = new MyClass();

// ✅ GOOD: Generic factory
T Create<T>() where T : new() => new T();
```

## 💡 Key Concepts

### Trimming

Native AOT automatically **trims** unused code:

```csharp
// If you never call this method, it won't be in the binary
void UnusedMethod() { }  // Removed during publish!
```

**Result:** Only code you actually use is included.

### Size Optimizations

```xml
<!-- Minimal binary size -->
<PropertyGroup>
  <IlcOptimizationPreference>Size</IlcOptimizationPreference>
  <IlcGenerateStackTraceData>false</IlcGenerateStackTraceData>
  <IlcFoldIdenticalMethodBodies>true</IlcFoldIdenticalMethodBodies>
  <InvariantGlobalization>true</InvariantGlobalization>
</PropertyGroup>
```

**Trade-offs:**
- `Size` optimization → 30% smaller, slightly slower
- `Speed` optimization → 20% faster, slightly larger
- No stack traces → Harder debugging in production

### Cross-Platform Publishing

```bash
# Publish for specific platforms
dotnet publish -r win-x64 -c Release      # Windows x64
dotnet publish -r linux-x64 -c Release    # Linux x64
dotnet publish -r osx-arm64 -c Release    # macOS Apple Silicon
dotnet publish -r linux-arm64 -c Release  # Linux ARM (Raspberry Pi, etc.)
```

## 🎓 Learning Checklist

### Beginner Level
- [ ] Understand what AOT compilation means
- [ ] Build and run the example
- [ ] Compare binary sizes (JIT vs AOT)
- [ ] Measure startup time differences

### Intermediate Level
- [ ] Configure AOT properties in .csproj
- [ ] Use JSON source generators
- [ ] Understand trimming warnings
- [ ] Publish for multiple platforms

### Advanced Level
- [ ] Handle incompatible libraries
- [ ] Use `[DynamicDependency]` for reflection
- [ ] Optimize binary size
- [ ] Debug AOT-specific issues

### Expert Level
- [ ] Build AOT-compatible libraries
- [ ] Contribute AOT compatibility fixes to OSS
- [ ] Profile and optimize AOT performance
- [ ] Use custom IL compiler options

## 🛠️ Real-World Use Cases

### 1. CLI Tools

```bash
# Before (JIT): 150MB, 200ms startup
myapp.exe list --verbose

# After (AOT): 5MB, 10ms startup
myapp list --verbose  # Feels instant!
```

**Benefits:** Users love fast, responsive CLI tools.

### 2. AWS Lambda / Azure Functions

```csharp
// Cold start times:
// JIT: 500-1000ms (unacceptable for APIs)
// AOT: 50-100ms (acceptable!)

public string FunctionHandler(APIGatewayProxyRequest request)
{
    return JsonSerializer.Serialize(result, AppJsonContext.Default.Result);
}
```

**Benefits:** Lower costs (pay per ms), better UX.

### 3. Docker Containers

```dockerfile
FROM scratch  # No base image needed!
COPY ./publish/myapp /myapp
ENTRYPOINT ["/myapp"]

# Result: 8MB container (vs 200MB with .NET runtime)
```

**Benefits:** Faster deployments, lower storage costs.

### 4. Kubernetes Microservices

**Faster scaling:**
- JIT: 30 pods × 200ms = 6 seconds to scale
- AOT: 30 pods × 10ms = 300ms to scale

**Lower resource usage:**
- JIT: 100MB RAM per pod
- AOT: 20MB RAM per pod
- **Savings:** 80% reduction (5x more pods per node!)

## 📚 Related Patterns

| Pattern | Purpose | Relationship |
|---------|---------|--------------|
| **Source Generators** | Generate code at compile time | Required for AOT JSON, DI |
| **Span&lt;T&gt;** | Zero-allocation slicing | Perfect for AOT (no GC overhead) |
| **Trimming** | Remove unused code | Automatic with AOT |
| **ReadyToRun (R2R)** | Hybrid JIT/AOT | Middle ground option |

## 🔗 Limitations and Workarounds

### Limitation 1: Reflection

**Problem:**
```csharp
var type = Type.GetType("MyClass");  // ❌ Throws at runtime
```

**Workaround 1 - Direct Reference:**
```csharp
var instance = new MyClass();  // ✅
```

**Workaround 2 - DynamicDependency Attribute:**
```csharp
[DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods, typeof(MyClass))]
void ProcessClass()
{
    var type = typeof(MyClass);  // ✅ AOT preserves this
}
```

### Limitation 2: Dynamic Assembly Loading

**Problem:**
```csharp
Assembly.LoadFrom("plugin.dll");  // ❌ Not supported
```

**Workaround:**
```csharp
// Use static plugin discovery instead
var plugins = new IPlugin[] { new Plugin1(), new Plugin2() };
```

### Limitation 3: Some NuGet Packages

**Problem:** Not all libraries are AOT-compatible.

**Workaround:**
- Check package documentation for AOT support
- Use `dotnet publish` warnings to find issues
- Replace with AOT-compatible alternatives

## 📖 Additional Resources

### Official Documentation
- [Native AOT Deployment](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
- [AOT Warnings (IL2XXX, IL3XXX)](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/warnings/)
- [Source Generators](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)

### Blogs & Articles
- [.NET Native AOT in Action](https://devblogs.microsoft.com/dotnet/announcing-net-8/)
- [Building Cloud-Native with AOT](https://aws.amazon.com/blogs/developer/net-8-native-aot-for-aws-lambda/)

## 🎯 Next Steps

1. **Explore** `WHY_THIS_PATTERN.md` - Deep dive into AOT benefits
2. **Study** `CAREER_IMPACT.md` - AOT expertise and salaries
3. **Review** `PERFORMANCE_NOTES.md` - Benchmarking and optimization
4. **Avoid** `COMMON_MISTAKES.md` - AOT pitfalls and fixes

---

**📝 Summary:** Native AOT compiles C# to native machine code for 10x faster startup, 90% smaller binaries, and predictable performance. Perfect for CLI tools, serverless functions, and containers.

**🎓 Key Takeaway:** AOT trades flexibility (no reflection) for performance (fast startup, small size). Choose AOT when startup time and binary size matter more than dynamic capabilities.
