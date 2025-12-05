# Common Mistakes with Native AOT

## Mistake #1: Using Reflection

### ❌ Problem

```csharp
// This compiles but FAILS at runtime!
var type = Type.GetType("MyNamespace.MyClass");
var instance = Activator.CreateInstance(type);

// Runtime error:
// MissingMetadataException: Cannot create instance of 'MyClass'
```

### ✅ Solution 1: Direct Instantiation

```csharp
var instance = new MyClass();  // Works perfectly!
```

### ✅ Solution 2: DynamicDependency Attribute

```csharp
using System.Diagnostics.CodeAnalysis;

[DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof(MyClass))]
void CreateInstance()
{
    var instance = Activator.CreateInstance<MyClass>();  // Now works!
}
```

## Mistake #2: Ignoring Trim Warnings

### ❌ Problem

```bash
dotnet publish -c Release
# Warning IL2026: Method uses reflection
# Warning IL3050: May not work with AOT

# You: "It's just warnings, ship it!"
# Production: 💥 MissingMetadataException
```

### ✅ Solution

```bash
# Treat warnings as errors
dotnet publish -c Release /p:TreatWarningsAsErrors=true

# Fix all IL2XXX and IL3XXX warnings before shipping!
```

## Mistake #3: Not Using JSON Source Generators

### ❌ Problem

```csharp
// Uses reflection internally (fails with AOT!)
var json = JsonSerializer.Serialize(person);

// Runtime: System.InvalidOperationException
```

### ✅ Solution

```csharp
[JsonSerializable(typeof(Person))]
internal partial class AppJsonContext : JsonSerializerContext { }

// AOT-compatible!
var json = JsonSerializer.Serialize(person, AppJsonContext.Default.Person);
```

## Mistake #4: Incompatible NuGet Packages

### ❌ Problem

```xml
<PackageReference Include="SomeOldLibrary" Version="1.0.0" />
<!-- This library uses heavy reflection -->

dotnet publish
# 50+ trim warnings!
# Runtime failures!
```

### ✅ Solution

```bash
# Check package AOT compatibility
# Look for these in package description:
# ✅ "Native AOT compatible"
# ✅ "Trim-friendly"
# ❌ "Uses reflection extensively"

# Or test it:
dotnet publish -c Release
# If you see IL2XXX warnings, find an alternative
```

### Popular AOT-Compatible Libraries

✅ **Works Great:**
- `System.Text.Json` (with source generators)
- `Microsoft.Extensions.Logging`
- `Dapper` (micro-ORM)
- `BenchmarkDotNet`

❌ **Problematic:**
- Most DI containers (use minimal API instead)
- Some serializers (use System.Text.Json)
- Reflection-heavy frameworks

## Mistake #5: Not Testing on Target Platform

### ❌ Problem

```bash
# Develop on macOS
dotnet publish -r linux-x64 -c Release

# Deploy to Linux
./myapp
# Error: Illegal instruction (incompatible CPU features)
```

### ✅ Solution

```bash
# Always test on actual target!
docker run --rm -v $(pwd):/app mcr.microsoft.com/dotnet/runtime:8.0 /app/myapp

# Or use CI/CD with target platform
```

## Mistake #6: Forgetting InvariantGlobalization

### ❌ Problem

```csharp
var date = DateTime.Now.ToString("D");  // Long date format
// Crashes: CultureNotFoundException
```

**Why:** `InvariantGlobalization=true` removes culture data (saves 15MB!)

### ✅ Solution 1: Use Invariant Culture

```csharp
var date = DateTime.Now.ToString("D", CultureInfo.InvariantCulture);
```

### ✅ Solution 2: Disable InvariantGlobalization

```xml
<PropertyGroup>
  <InvariantGlobalization>false</InvariantGlobalization>
  <!-- Binary size: 6MB → 21MB -->
</PropertyGroup>
```

## Mistake #7: Dynamic Code Generation

### ❌ Problem

```csharp
// Expression compilation fails with AOT
Expression<Func<int, int>> expr = x => x * 2;
var compiled = expr.Compile();  // ❌ Not supported!
```

### ✅ Solution

```csharp
// Use direct delegates instead
Func<int, int> func = x => x * 2;  // ✅ Works!
```

## Mistake #8: Missing RID (Runtime Identifier)

### ❌ Problem

```bash
dotnet publish -c Release
# Error: Please specify a runtime identifier (RID)
```

### ✅ Solution

```bash
# Specify target platform
dotnet publish -r linux-x64 -c Release      # Linux
dotnet publish -r win-x64 -c Release        # Windows
dotnet publish -r osx-arm64 -c Release      # macOS Apple Silicon
```

## Mistake #9: Not Handling Platform Differences

### ❌ Problem

```csharp
// Works on Windows, fails on Linux (case-sensitive!)
var file = File.Open("MyFile.txt", FileMode.Open);
// Actual filename: "myfile.txt"
```

### ✅ Solution

```csharp
// Use consistent casing or runtime checks
var file = File.Open(
    RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? "MyFile.txt"
        : "myfile.txt",
    FileMode.Open
);
```

## Mistake #10: Slow Build Times

### ❌ Problem

```bash
dotnet publish -c Release
# Takes 8 minutes! 😴
```

### ✅ Solution

```bash
# Use cached intermediate files
dotnet publish -c Release --no-restore

# Or publish incrementally (development)
dotnet publish -c Debug  # Much faster, but larger binary
```

## Debugging Checklist

### AOT App Crashes at Runtime?

1. ✅ Check for IL2XXX/IL3XXX warnings
2. ✅ Search codebase for `Type.GetType`
3. ✅ Look for `Activator.CreateInstance`
4. ✅ Verify all JSON uses source generators
5. ✅ Test with `InvariantGlobalization=false`
6. ✅ Run on actual target platform

### App Works in Debug but Not Release?

1. ✅ Trimming removed needed code
2. ✅ Check for `[DynamicDependency]` attributes
3. ✅ Review trim warnings carefully
4. ✅ Use `<TrimmerRootAssembly>` if needed

### Binary Size Too Large?

1. ✅ Enable `InvariantGlobalization=true`
2. ✅ Set `IlcOptimizationPreference=Size`
3. ✅ Disable stack traces: `IlcGenerateStackTraceData=false`
4. ✅ Remove unused NuGet packages
5. ✅ Use `IlcFoldIdenticalMethodBodies=true`

## Best Practices Summary

**✅ DO:**
- Use JSON source generators
- Fix all trim warnings
- Test on target platform
- Use `sealed` classes when possible
- Profile and measure performance

**❌ DON'T:**
- Use reflection (unless absolutely necessary)
- Ignore IL2XXX/IL3XXX warnings
- Assume libraries are AOT-compatible
- Skip testing on actual deployment target
- Use dynamic code generation

## Quick Reference

### AOT-Friendly Patterns

```csharp
// ✅ Direct instantiation
var obj = new MyClass();

// ✅ Generic constraints
T Create<T>() where T : new() => new T();

// ✅ Source generators
[JsonSerializable(typeof(MyClass))]
partial class JsonContext : JsonSerializerContext { }

// ✅ Sealed classes (better optimization)
public sealed class MyClass { }
```

### AOT-Unfriendly Patterns

```csharp
// ❌ Reflection
Type.GetType("MyClass");
Activator.CreateInstance(typeof(MyClass));

// ❌ Expression compilation
Expression<Func<int, int>> expr = x => x * 2;
expr.Compile();

// ❌ Dynamic keyword
dynamic obj = GetObject();

// ❌ Assembly loading
Assembly.LoadFrom("plugin.dll");
```

## Conclusion

Most AOT issues come from **reflection usage**. Follow these rules:

1. **Use source generators** for serialization
2. **Fix all warnings** before shipping
3. **Test on target platform** always
4. **Avoid reflection** when possible
5. **Check library compatibility** before adding dependencies

**Bottom Line:** AOT requires more upfront work but delivers 10x better runtime performance. Worth it for production deployments!
