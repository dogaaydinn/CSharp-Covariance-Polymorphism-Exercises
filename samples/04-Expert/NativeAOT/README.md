# Native AOT in .NET - Expert Tutorial

> **Level:** Expert  
> **Prerequisites:** Advanced C# knowledge, understanding of compilation, .NET 8.0+  
> **Estimated Time:** 2-3 hours

## 📚 Overview

Native AOT (Ahead-of-Time compilation) is a .NET 7+ feature that compiles C# directly to native machine code at build time, eliminating the need for the .NET runtime. This tutorial covers Native AOT capabilities, limitations, and best practices for production use.

## 🎯 Learning Objectives

After completing this tutorial, you will be able to:

- ✅ Understand Native AOT vs traditional .NET compilation
- ✅ Configure projects for Native AOT publication
- ✅ Handle reflection limitations with source generators
- ✅ Use JsonSerializerContext for AOT-compatible JSON
- ✅ Analyze and fix trim warnings
- ✅ Optimize binary size and startup performance
- ✅ Decide when Native AOT is appropriate

## 🚀 Quick Start

```bash
cd samples/04-Expert/NativeAOT

# Regular run (with runtime)
dotnet run

# Build for Native AOT
dotnet publish -c Release -r linux-x64

# Run native binary (no .NET runtime needed!)
./bin/Release/net8.0/linux-x64/publish/NativeAOT
```

## 📊 What is Native AOT?

### Traditional .NET Compilation

```
C# Source Code
    ↓ (C# Compiler)
IL (Intermediate Language)
    ↓ (Deployed to production)
Runtime: JIT Compiler → Native Code
    ↓ (At startup, every time)
Execution
```

**Characteristics:**
- ✅ Flexible (reflection, dynamic code)
- ❌ Slow startup (~500ms)
- ❌ Large deployment (~200MB with runtime)
- ❌ Higher memory usage (~30-50MB)

### Native AOT Compilation

```
C# Source Code
    ↓ (AOT Compiler at build time)
Native Machine Code
    ↓ (Single executable)
Execution (instant!)
```

**Characteristics:**
- ✅ Fast startup (~5ms) - **100x faster!**
- ✅ Small deployment (~5-15MB) - **10-40x smaller!**
- ✅ Low memory (~5-10MB) - **5x less!**
- ❌ No runtime reflection
- ❌ No dynamic code generation

---

## 🔧 Configuration

### Project File Setup

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    
    <!-- Enable Native AOT -->
    <PublishAot>true</PublishAot>
    
    <!-- Optional: Optimization settings -->
    <IlcOptimizationPreference>Speed</IlcOptimizationPreference>
    <IlcGenerateStackTraceData>false</IlcGenerateStackTraceData>
    <InvariantGlobalization>false</InvariantGlobalization>
  </PropertyGroup>
</Project>
```

### Build Commands

```bash
# Windows x64
dotnet publish -c Release -r win-x64

# Linux x64
dotnet publish -c Release -r linux-x64

# Linux ARM64 (Raspberry Pi, AWS Graviton)
dotnet publish -c Release -r linux-arm64

# macOS ARM64 (Apple Silicon)
dotnet publish -c Release -r osx-arm64
```

**Result:** Single executable file, no .NET runtime required!

---

## ⚠️ Reflection Limitations

### What Doesn't Work

```csharp
// ❌ FAILS with Native AOT
var typeName = "MyNamespace.MyClass";
var type = Type.GetType(typeName);              // Returns null or fails
var instance = Activator.CreateInstance(type);  // Fails

// ❌ FAILS - Method reflection
var method = type.GetMethod("MyMethod");
method.Invoke(instance, new object[] { });

// ❌ FAILS - Assembly loading
var assembly = Assembly.Load("MyAssembly.dll");

// ❌ FAILS - Dynamic code generation
var dynamicMethod = new DynamicMethod(...);
```

**Why?** Native AOT needs to know all types at compile time. Reflection discovers types at runtime, so the compiler can't include them.

### What Works

```csharp
// ✅ WORKS - Compile-time known types
var person = new Person { Name = "Alice" };
Console.WriteLine(person.Name);

// ✅ WORKS - typeof() with known types
var type = typeof(Person);
Console.WriteLine(type.Name);

// ✅ WORKS - is/as operators
if (obj is Person person)
{
    Console.WriteLine(person.Name);
}
```

---

## 🔄 JSON Serialization

### Problem: System.Text.Json Uses Reflection

```csharp
// ❌ Default serialization (uses reflection, doesn't work with AOT)
var person = new Person { Name = "Alice", Age = 30 };
string json = JsonSerializer.Serialize(person);  // FAILS with AOT!
```

### Solution: Source Generators

```csharp
// 1. Define your models
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

// 2. Create a JsonSerializerContext with [JsonSerializable]
[JsonSerializable(typeof(Person))]
[JsonSerializable(typeof(List<Person>))]  // For collections
partial class MyJsonContext : JsonSerializerContext
{
}

// 3. Use the context
var person = new Person { Name = "Alice", Age = 30 };

// ✅ Serialize with context
string json = JsonSerializer.Serialize(person, MyJsonContext.Default.Person);

// ✅ Deserialize with context
Person? result = JsonSerializer.Deserialize(json, MyJsonContext.Default.Person);
```

**How it works:**
- Source generator runs at compile time
- Generates serialization code for your types
- No reflection needed at runtime
- AOT-compatible!

---

## 🔍 Trimming Analysis

### What is Trimming?

Native AOT removes unused code to reduce binary size:

```
Your App: 10,000 methods
↓ (Trimmer analyzes from Main())
Reachable: 2,000 methods
↓ (Removes unreachable code)
Final Binary: 2,000 methods (80% smaller!)
```

### Common Trim Warnings

**IL2026: RequiresUnreferencedCode**
```csharp
// This method uses reflection
[RequiresUnreferencedCode("Uses reflection")]
public void LoadPlugin(string typeName)
{
    var type = Type.GetType(typeName);  // Unsafe for trimming
    // ...
}
```

**IL2067: DynamicallyAccessedMembers**
```csharp
// Need to tell trimmer which members to keep
public void ProcessType(
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
    Type type)
{
    var methods = type.GetMethods();  // Now safe
}
```

**IL2070: 'this' parameter**
```csharp
// Can't guarantee which members are needed
public void Process<T>()
{
    var methods = typeof(T).GetMethods();  // Warning!
}
```

### How to Fix Trim Warnings

1. **Use Source Generators** (best solution)
   ```csharp
   // Instead of reflection, use source generator
   [JsonSerializable(typeof(MyType))]
   partial class MyContext : JsonSerializerContext { }
   ```

2. **Add Attributes**
   ```csharp
   [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
   Type myType;
   ```

3. **Suppress if Safe**
   ```csharp
   [UnconditionalSuppressMessage("Trim", "IL2026")]
   public void MyMethod() { ... }
   ```

4. **Mark as Unsafe**
   ```csharp
   [RequiresUnreferencedCode("This uses reflection")]
   public void MyReflectionMethod() { ... }
   ```

---

## 📈 Performance Comparison

### Startup Time

| Metric | Traditional .NET | Native AOT | Improvement |
|--------|------------------|------------|-------------|
| Cold Start | ~500ms | ~5ms | **100x faster** |
| Warm Start | ~100ms | ~5ms | **20x faster** |

**Use Case:** Serverless functions, CLI tools

### Memory Usage

| Metric | Traditional .NET | Native AOT | Improvement |
|--------|------------------|------------|-------------|
| Base Memory | ~30-50 MB | ~5-10 MB | **5x less** |
| Working Set | Higher | Lower | Less GC pressure |

**Use Case:** Containers, microservices

### Binary Size

| Metric | Traditional .NET | Native AOT | Improvement |
|--------|------------------|------------|-------------|
| Single File | ~80-100 MB | ~5-15 MB | **10x smaller** |
| With Runtime | ~200+ MB | ~5-15 MB | **40x smaller** |
| Docker Image | ~200 MB | ~15 MB | **13x smaller** |

**Use Case:** Docker images, edge devices

### Execution Speed

| Metric | Traditional .NET | Native AOT | Difference |
|--------|------------------|------------|------------|
| Runtime | Baseline | ±10% | Similar |
| CPU-Bound | JIT optimizes | AOT optimized | Comparable |

**Note:** Native AOT is optimized at build time. Traditional .NET optimizes at runtime (JIT). Long-running apps end up similar.

---

## 🎯 When to Use Native AOT

### ✅ Perfect For:

**1. Microservices**
```
Traditional: 200MB image, 500ms startup
Native AOT: 15MB image, 5ms startup
Result: 13x smaller, 100x faster startup!
```

**2. Serverless / Cloud Functions**
```
Cold start penalty: 500ms → 5ms
Cost savings: Run 5x more instances
```

**3. CLI Tools**
```
User experience: Instant execution
Distribution: Single .exe file
```

**4. Containers / Docker**
```
Before: FROM mcr.microsoft.com/dotnet/runtime:8.0 (~200MB base)
After:  FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine (~15MB)
```

**5. IoT / Edge Devices**
```
Raspberry Pi: Limited memory, fast boot needed
Result: 5MB footprint, instant startup
```

### ❌ Not Recommended For:

**1. Heavy Reflection Usage**
```csharp
// Plugin systems that load types dynamically
var pluginType = Type.GetType(config.PluginTypeName);
// Doesn't work with AOT!
```

**2. Desktop Applications**
- Startup time less critical
- Reflection often used (WPF, WinForms)
- JIT can optimize long-running apps

**3. ASP.NET Core (Complex)**
- Many ASP.NET features use reflection
- MVC, Razor Pages need special handling
- Minimal APIs work better with AOT

**4. Entity Framework Core**
- EF Core uses heavy reflection
- Some EF Core features incompatible
- Consider Dapper or ADO.NET instead

**5. Dynamic Scenarios**
- Code generation at runtime
- Dynamic proxies
- Expression trees (limited support)

---

## 🛠️ Migration Strategy

### Step 1: Enable AOT and Test

```xml
<PublishAot>true</PublishAot>
```

```bash
dotnet publish -c Release -r linux-x64
```

**Check for:** Trim warnings, runtime errors

### Step 2: Fix JSON Serialization

```csharp
// Before
JsonSerializer.Serialize(obj);

// After
[JsonSerializable(typeof(MyType))]
partial class MyContext : JsonSerializerContext { }

JsonSerializer.Serialize(obj, MyContext.Default.MyType);
```

### Step 3: Replace Reflection

```csharp
// Before
var type = Type.GetType(typeName);
var instance = Activator.CreateInstance(type);

// After
var instance = typeName switch
{
    "TypeA" => new TypeA(),
    "TypeB" => new TypeB(),
    _ => throw new Exception("Unknown type")
};
```

### Step 4: Handle Trim Warnings

```csharp
// Add attributes or suppress warnings
[RequiresUnreferencedCode("Uses reflection")]
public void MyMethod() { ... }
```

### Step 5: Measure Performance

```bash
# Measure startup time
time ./MyApp

# Measure binary size
ls -lh MyApp

# Measure memory usage
/usr/bin/time -v ./MyApp
```

---

## 📦 Docker Integration

### Dockerfile for Native AOT

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -r linux-x64 -o /app

# Runtime stage (tiny!)
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./MyApp"]
```

**Result:**
- Final image: ~15MB (vs ~200MB traditional)
- Startup: ~5ms (vs ~500ms traditional)
- Memory: ~10MB (vs ~50MB traditional)

---

## 🎓 Best Practices Summary

### ✅ DO:

1. **Use Source Generators**
   - JsonSerializerContext for JSON
   - Replace reflection with code generation

2. **Test Early**
   - Enable `PublishAot=true` from day 1
   - Fix warnings immediately

3. **Use Compile-Time Types**
   - Avoid `Type.GetType(string)`
   - Prefer `typeof(T)` and generics

4. **Optimize for Size**
   ```xml
   <IlcOptimizationPreference>Size</IlcOptimizationPreference>
   ```

5. **Handle Trim Warnings**
   - Don't ignore IL2XXX warnings
   - Add attributes or fix code

### ❌ DON'T:

1. **Don't Use Runtime Reflection**
   ```csharp
   // BAD
   Activator.CreateInstance(Type.GetType(name));
   ```

2. **Don't Ignore Warnings**
   - Trim warnings indicate problems
   - Address them before production

3. **Don't Use Incompatible Libraries**
   - Check if library supports AOT
   - Entity Framework Core (limited support)
   - Newtonsoft.Json (not compatible)

4. **Don't Expect Magic**
   - AOT doesn't make slow code fast
   - Optimize algorithms first

---

## 🔗 Related Topics

- **Advanced:** [Performance Optimization](../03-Advanced/PerformanceOptimization/)
- **Advanced:** [Observability Patterns](../03-Advanced/ObservabilityPatterns/)
- **Expert:** [Advanced Performance](../04-Expert/AdvancedPerformance/)

---

## 📚 Further Reading

### Official Documentation
- [Native AOT Deployment](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
- [Trimming Options](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/trimming-options)
- [JsonSerializerContext](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation)

### Articles
- "Building Cloud Native Apps with Native AOT" - Microsoft Blog
- "Native AOT in Production: Lessons Learned" - .NET Blog
- "Docker Images: 200MB → 15MB with Native AOT"

### Tools
- **ILSpy** - Inspect trimmed IL code
- **dotnet-trace** - Profile Native AOT apps
- **BenchmarkDotNet** - Compare AOT vs JIT performance

---

## ✅ Learning Checklist

- [ ] Understand Native AOT vs traditional .NET compilation
- [ ] Configure `PublishAot=true` in project file
- [ ] Replace reflection with source generators
- [ ] Use JsonSerializerContext for JSON serialization
- [ ] Analyze and fix IL2XXX trim warnings
- [ ] Measure startup time, memory, and binary size
- [ ] Build for multiple platforms (win-x64, linux-x64, osx-arm64)
- [ ] Create Docker images with Native AOT
- [ ] Decide when AOT is appropriate for your project

---

## 🚀 Real-World Impact

### Case Study: Microservice Migration

**Before (Traditional .NET):**
- Docker image: 210 MB
- Cold start: 520ms
- Memory: 45 MB per instance
- Cost: $500/month (50 instances)

**After (Native AOT):**
- Docker image: 18 MB (**12x smaller**)
- Cold start: 6ms (**87x faster**)
- Memory: 8 MB per instance (**5.6x less**)
- Cost: $100/month (250 instances on same hardware)

**Result:** 80% cost reduction, 5x capacity increase! 🎉

---

**Happy Compiling! ⚡**

*"The best performance optimization is the one you don't have to do at runtime." - Native AOT Philosophy*
