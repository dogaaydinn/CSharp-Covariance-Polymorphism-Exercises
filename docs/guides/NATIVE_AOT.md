# Native AOT Compilation Guide

## Overview

Native Ahead-of-Time (AOT) compilation enables .NET applications to compile to native code ahead of time, rather than using Just-In-Time (JIT) compilation at runtime. This guide covers preparing your C# application for Native AOT compilation.

## Table of Contents

- [What is Native AOT?](#what-is-native-aot)
- [Benefits and Trade-offs](#benefits-and-trade-offs)
- [Getting Started](#getting-started)
- [Preparing Code for AOT](#preparing-code-for-aot)
- [Resolving Trim Warnings](#resolving-trim-warnings)
- [Eliminating Reflection](#eliminating-reflection)
- [Source Generator-Based Serialization](#source-generator-based-serialization)
- [Build Configuration](#build-configuration)
- [Performance Targets](#performance-targets)
- [Compatibility Checklist](#compatibility-checklist)
- [Troubleshooting](#troubleshooting)

## What is Native AOT?

Native AOT compiles your .NET application to native machine code during the build process, producing a self-contained executable with:
- No JIT compilation at runtime
- No dependency on .NET runtime
- Native OS executable format

### How It Works

```
C# Source Code → Roslyn Compiler → IL Code → AOT Compiler → Native Machine Code
```

Traditional .NET:
```
C# → IL → JIT (runtime) → Machine Code
```

Native AOT:
```
C# → IL → AOT (build time) → Machine Code
```

## Benefits and Trade-offs

### Benefits ✅
- **Fast Startup**: Application starts in <50ms (vs 500ms+ with JIT)
- **Small Memory Footprint**: <30MB typical (vs 100MB+ with runtime)
- **Self-Contained**: No .NET runtime installation required
- **Smaller Deployment Size**: Trimmed and optimized
- **Better Performance**: No JIT compilation overhead
- **Reduced Attack Surface**: No JIT code generation at runtime

### Trade-offs ⚠️
- **Limited Reflection**: Dynamic code generation not supported
- **Larger Executable**: Single-file includes all dependencies
- **Longer Build Time**: Compilation happens at build time
- **Platform-Specific**: Must build for each target platform
- **Library Compatibility**: Not all NuGet packages support AOT

## Getting Started

### 1. Project Configuration

Update your `.csproj` file:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <PublishAot>true</PublishAot>
    <InvariantGlobalization>false</InvariantGlobalization>

    <!-- Optional: Enable trim analyzers -->
    <EnableTrimAnalyzer>true</EnableTrimAnalyzer>
    <EnableSingleFileAnalyzer>true</EnableSingleFileAnalyzer>
    <EnableAotAnalyzer>true</EnableAotAnalyzer>

    <!-- Optional: Trim options -->
    <TrimMode>full</TrimMode>
    <IlcOptimizationPreference>Speed</IlcOptimizationPreference>
    <IlcGenerateStackTraceData>false</IlcGenerateStackTraceData>
  </PropertyGroup>
</Project>
```

### 2. Publish for Native AOT

```bash
# Publish for current platform
dotnet publish -c Release

# Publish for specific platform
dotnet publish -c Release -r win-x64
dotnet publish -c Release -r linux-x64
dotnet publish -c Release -r osx-arm64
```

### 3. Verify AOT Compatibility

```bash
# Run AOT compatibility analysis
dotnet publish -c Release /p:PublishAot=true /p:TrimmerSingleWarn=false

# Check for trim warnings
dotnet build /p:EnableTrimAnalyzer=true
```

## Preparing Code for AOT

### 1. Avoid Dynamic Code Generation

❌ **Not AOT-Compatible:**
```csharp
// Dynamic assembly generation
var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(...);

// Expression tree compilation
Expression<Func<int, int>> expr = x => x * 2;
var compiled = expr.Compile(); // Uses JIT

// Dynamic proxy generation
var proxy = DispatchProxy.Create<IService, ServiceProxy>();
```

✅ **AOT-Compatible Alternatives:**
```csharp
// Use source generators instead of dynamic generation
[AutoGenerate]
public partial class Service : IService
{
    // Generated at compile time
}

// Use static delegates instead of expression compilation
Func<int, int> func = static x => x * 2;

// Use compile-time proxies
public class ServiceProxy : IService
{
    private readonly IService _inner;
    public ServiceProxy(IService inner) => _inner = inner;

    public void DoWork() => _inner.DoWork();
}
```

### 2. Use Type-Safe Reflection

❌ **Not AOT-Compatible:**
```csharp
// Runtime type discovery
var types = Assembly.GetTypes();
foreach (var type in types)
{
    var instance = Activator.CreateInstance(type);
}

// String-based reflection
var method = type.GetMethod("MethodName");
method.Invoke(instance, parameters);
```

✅ **AOT-Compatible Alternatives:**
```csharp
// Use generic constraints
public T Create<T>() where T : new()
{
    return new T(); // Direct instantiation
}

// Use static factories
public interface IFactory<out T>
{
    T Create();
}

public class UserFactory : IFactory<User>
{
    public User Create() => new User();
}

// Use direct method calls
public void CallMethod(IService service)
{
    service.DoWork(); // Direct call through interface
}
```

## Resolving Trim Warnings

### Understanding Trim Warnings

Trim warnings indicate code that may not work correctly after trimming:

```
warning IL2026: Using member 'Type.GetMethod(String)' which has 'RequiresUnreferencedCodeAttribute'
can break functionality when trimming application code.
```

### Common Warning Categories

#### IL2026: RequiresUnreferencedCode
```csharp
// Code that requires unreferenced code
[RequiresUnreferencedCode("Uses reflection")]
public void ProcessType(Type type)
{
    var methods = type.GetMethods(); // May be trimmed
}
```

**Solution:** Use source generators or preserve with attributes
```csharp
// Option 1: Source generator
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
Type type = typeof(MyClass);

// Option 2: Preserve specific members
[DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods, typeof(MyClass))]
public void ProcessType()
{
    var methods = typeof(MyClass).GetMethods();
}
```

#### IL2070: Unrecognized Value Passed to Parameter

```csharp
public void Process(Type type) // Compiler doesn't know what type this is
{
    var properties = type.GetProperties();
}
```

**Solution:** Add annotations
```csharp
public void Process(
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    Type type)
{
    var properties = type.GetProperties(); // Preserved
}
```

### Preserving Types

```csharp
// Preserve entire assembly
[assembly: UnconditionalSuppressMessage("Trimming", "IL2026",
    Justification = "Legacy library")]

// Preserve specific type
[DynamicallyAccessedMembers(
    DynamicallyAccessedMemberTypes.PublicProperties |
    DynamicallyAccessedMemberTypes.PublicMethods)]
Type preservedType = typeof(MyClass);

// Preserve through dependency
[DynamicDependency(nameof(MyClass.MyMethod), typeof(MyClass))]
public void CallIndirectly()
{
    // MyClass.MyMethod won't be trimmed
}
```

## Eliminating Reflection

### 1. JSON Serialization

❌ **Reflection-Based (Not AOT-Compatible):**
```csharp
using System.Text.Json;

var user = new User { Id = 1, Name = "John" };
var json = JsonSerializer.Serialize(user); // Uses reflection
var deserialized = JsonSerializer.Deserialize<User>(json);
```

✅ **Source Generator-Based (AOT-Compatible):**
```csharp
using System.Text.Json;
using System.Text.Json.Serialization;

// Define source generation context
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(List<User>))]
[JsonSerializable(typeof(Dictionary<string, User>))]
[JsonSourceGenerationOptions(WriteIndented = true)]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}

// Use the generated context
var user = new User { Id = 1, Name = "John" };
var json = JsonSerializer.Serialize(user, AppJsonSerializerContext.Default.User);
var deserialized = JsonSerializer.Deserialize<User>(json, AppJsonSerializerContext.Default.User);
```

### 2. Dependency Injection

❌ **Reflection-Based:**
```csharp
// Runtime assembly scanning
services.AddControllers(); // Scans assemblies for controllers
services.AddAutoMapper(Assembly.GetExecutingAssembly());
```

✅ **Explicit Registration:**
```csharp
// Explicit service registration
services.AddSingleton<IUserService, UserService>();
services.AddScoped<IRepository<User>, UserRepository>();
services.AddTransient<IEmailService, EmailService>();

// Manual controller registration
services.AddControllers()
    .AddApplicationPart(typeof(UserController).Assembly)
    .AddControllersAsServices();

// Explicit mapping configuration
services.AddSingleton<IMapper>(sp =>
{
    var config = new MapperConfiguration(cfg =>
    {
        cfg.CreateMap<User, UserDto>();
        cfg.CreateMap<UserDto, User>();
    });
    return config.CreateMapper();
});
```

### 3. Configuration Binding

❌ **Reflection-Based:**
```csharp
var settings = configuration.GetSection("AppSettings").Get<AppSettings>();
```

✅ **Source Generator-Based:**
```csharp
// Use ConfigurationBinder source generator (NET 8+)
var settings = configuration.GetSection("AppSettings").Get<AppSettings>(
    options => options.BindNonPublicProperties = false);

// Or bind manually
var settings = new AppSettings
{
    ConnectionString = configuration["AppSettings:ConnectionString"],
    MaxRetries = int.Parse(configuration["AppSettings:MaxRetries"]),
    Timeout = TimeSpan.Parse(configuration["AppSettings:Timeout"])
};
```

## Source Generator-Based Serialization

### JSON Serialization Context

Complete example with multiple types:

```csharp
using System.Text.Json.Serialization;

// Domain models
public record User(int Id, string Name, string Email);
public record Product(int Id, string Name, decimal Price);
public record Order(int Id, int UserId, List<int> ProductIds);

// API response types
public record ApiResponse<T>(bool Success, T? Data, string? Error);
public record PaginatedResponse<T>(List<T> Items, int Total, int Page, int PageSize);

// Single source generation context for entire application
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(Order))]
[JsonSerializable(typeof(List<User>))]
[JsonSerializable(typeof(List<Product>))]
[JsonSerializable(typeof(List<Order>))]
[JsonSerializable(typeof(ApiResponse<User>))]
[JsonSerializable(typeof(ApiResponse<Product>))]
[JsonSerializable(typeof(ApiResponse<Order>))]
[JsonSerializable(typeof(PaginatedResponse<User>))]
[JsonSerializable(typeof(PaginatedResponse<Product>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Metadata | JsonSourceGenerationMode.Serialization)]
internal partial class AppJsonContext : JsonSerializerContext
{
}

// Usage in application
public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<User?> GetUserAsync(int id)
    {
        var response = await _httpClient.GetStringAsync($"/api/users/{id}");
        return JsonSerializer.Deserialize(response, AppJsonContext.Default.User);
    }

    public async Task<List<User>> GetUsersAsync()
    {
        var response = await _httpClient.GetStringAsync("/api/users");
        return JsonSerializer.Deserialize(response, AppJsonContext.Default.ListUser)
            ?? new List<User>();
    }

    public async Task CreateUserAsync(User user)
    {
        var json = JsonSerializer.Serialize(user, AppJsonContext.Default.User);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await _httpClient.PostAsync("/api/users", content);
    }
}
```

### ASP.NET Core Integration

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Configure JSON options with source generation
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonContext.Default);
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonContext.Default);
    });

var app = builder.Build();
app.MapControllers();
app.Run();

// Controller automatically uses the context
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public ActionResult<User> GetUser(int id)
    {
        // Serialization uses AppJsonContext automatically
        return Ok(new User(id, "John Doe", "john@example.com"));
    }
}
```

## Build Configuration

### Project File Configuration

Complete `.csproj` with all AOT options:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>

    <!-- Native AOT Configuration -->
    <PublishAot>true</PublishAot>

    <!-- Trimming Options -->
    <PublishTrimmed>true</PublishTrimmed>
    <TrimMode>full</TrimMode>

    <!-- Analyzers -->
    <EnableTrimAnalyzer>true</EnableTrimAnalyzer>
    <EnableSingleFileAnalyzer>true</EnableSingleFileAnalyzer>
    <EnableAotAnalyzer>true</EnableAotAnalyzer>

    <!-- Optimization -->
    <IlcOptimizationPreference>Speed</IlcOptimizationPreference>
    <IlcInstructionSet>native</IlcInstructionSet>

    <!-- Reduce size (optional) -->
    <IlcGenerateStackTraceData>false</IlcGenerateStackTraceData>
    <IlcFoldIdenticalMethodBodies>true</IlcFoldIdenticalMethodBodies>

    <!-- Globalization -->
    <InvariantGlobalization>false</InvariantGlobalization>

    <!-- Single file -->
    <PublishSingleFile>true</PublishSingleFile>
    <SelfContained>true</SelfContained>
  </PropertyGroup>

  <!-- AOT-compatible packages -->
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
    <PackageReference Include="System.Text.Json" Version="8.0.0" />
  </ItemGroup>

  <!-- Trim warnings as errors (strict mode) -->
  <PropertyGroup Condition="'$(Configuration)' == 'Release'">
    <TrimmerSingleWarn>false</TrimmerSingleWarn>
    <WarningsAsErrors>$(WarningsAsErrors);IL2026;IL2087;IL2091</WarningsAsErrors>
  </PropertyGroup>
</Project>
```

### Build Scripts

**build-aot.sh** (Linux/macOS):
```bash
#!/bin/bash
set -e

echo "Building Native AOT for multiple platforms..."

# Clean previous builds
dotnet clean -c Release

# Build for Linux x64
echo "Building for linux-x64..."
dotnet publish -c Release -r linux-x64 --self-contained \
  -p:PublishAot=true \
  -p:StripSymbols=true \
  -o ./publish/linux-x64

# Build for Windows x64
echo "Building for win-x64..."
dotnet publish -c Release -r win-x64 --self-contained \
  -p:PublishAot=true \
  -o ./publish/win-x64

# Build for macOS ARM64
echo "Building for osx-arm64..."
dotnet publish -c Release -r osx-arm64 --self-contained \
  -p:PublishAot=true \
  -o ./publish/osx-arm64

# Display sizes
echo ""
echo "Build sizes:"
du -h ./publish/*/AdvancedConcepts*

echo ""
echo "Build complete!"
```

**build-aot.ps1** (Windows):
```powershell
Write-Host "Building Native AOT for multiple platforms..." -ForegroundColor Green

# Clean previous builds
dotnet clean -c Release

# Build for Windows x64
Write-Host "Building for win-x64..." -ForegroundColor Cyan
dotnet publish -c Release -r win-x64 --self-contained `
  -p:PublishAot=true `
  -p:StripSymbols=true `
  -o ./publish/win-x64

# Build for Linux x64 (cross-compile)
Write-Host "Building for linux-x64..." -ForegroundColor Cyan
dotnet publish -c Release -r linux-x64 --self-contained `
  -p:PublishAot=true `
  -o ./publish/linux-x64

# Display sizes
Write-Host "`nBuild sizes:" -ForegroundColor Green
Get-ChildItem ./publish/*/AdvancedConcepts* | ForEach-Object {
    "{0:N2} MB - {1}" -f ($_.Length / 1MB), $_.FullName
}

Write-Host "`nBuild complete!" -ForegroundColor Green
```

## Performance Targets

### Startup Time: <50ms

**Measure startup time:**
```csharp
using System.Diagnostics;

public class Program
{
    private static readonly Stopwatch StartupTimer = Stopwatch.StartNew();

    public static void Main(string[] args)
    {
        // Application initialization
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/", () => "Hello World");

        StartupTimer.Stop();
        Console.WriteLine($"Startup time: {StartupTimer.ElapsedMilliseconds}ms");

        app.Run();
    }
}
```

**Optimization techniques:**
1. Minimize DI container registrations
2. Use source generators instead of reflection
3. Lazy-load non-critical services
4. Defer expensive initialization to background tasks

### Memory Footprint: <30MB

**Measure memory usage:**
```bash
# Linux
ps aux | grep AdvancedConcepts

# Windows
Get-Process AdvancedConcepts | Select-Object WorkingSet64

# Cross-platform in code
var process = Process.GetCurrentProcess();
Console.WriteLine($"Memory: {process.WorkingSet64 / 1024 / 1024} MB");
```

**Optimization techniques:**
1. Enable aggressive trimming
2. Disable stack trace data in production
3. Use `ArrayPool<T>` and `MemoryPool<T>`
4. Implement proper `IDisposable` patterns

### Binary Size

Typical sizes with Native AOT:

| Platform | Size | Notes |
|----------|------|-------|
| linux-x64 | 8-12 MB | With trimming and compression |
| win-x64 | 10-15 MB | Windows executable overhead |
| osx-arm64 | 8-12 MB | Apple Silicon optimized |

**Size optimization:**
```xml
<PropertyGroup>
  <!-- Aggressive size reduction -->
  <IlcGenerateStackTraceData>false</IlcGenerateStackTraceData>
  <IlcOptimizationPreference>Size</IlcOptimizationPreference>
  <IlcFoldIdenticalMethodBodies>true</IlcFoldIdenticalMethodBodies>
  <DebuggerSupport>false</DebuggerSupport>
  <EnableUnsafeBinaryFormatterSerialization>false</EnableUnsafeBinaryFormatterSerialization>
  <EventSourceSupport>false</EventSourceSupport>
  <UseSystemResourceKeys>true</UseSystemResourceKeys>
  <InvariantGlobalization>true</InvariantGlobalization>
</PropertyGroup>
```

## Compatibility Checklist

### ✅ AOT-Compatible Patterns

- [x] Direct type instantiation (`new T()` with constraints)
- [x] Interface-based polymorphism
- [x] Generic methods with constraints
- [x] LINQ (most operations)
- [x] Async/await
- [x] Source generators
- [x] Static methods and properties
- [x] Value types (structs)
- [x] Records
- [x] Pattern matching

### ❌ AOT-Incompatible Patterns

- [ ] `Activator.CreateInstance(Type)`
- [ ] `Assembly.Load` / `Assembly.LoadFrom`
- [ ] Dynamic code generation (Emit, DynamicMethod)
- [ ] Expression tree compilation
- [ ] MakeGenericType / MakeGenericMethod
- [ ] Type.GetType(string)
- [ ] Reflection.Emit
- [ ] COM interop
- [ ] Built-in serialization (BinaryFormatter, SoapFormatter)

### Package Compatibility

**AOT-Compatible:**
- ✅ System.Text.Json (with source generators)
- ✅ Microsoft.Extensions.* (most packages)
- ✅ Dapper
- ✅ NLog
- ✅ Serilog
- ✅ BenchmarkDotNet (with limitations)

**Not AOT-Compatible:**
- ❌ Entity Framework Core (full version)
- ❌ Newtonsoft.Json
- ❌ AutoMapper (runtime version)
- ❌ Moq
- ❌ Castle.DynamicProxy

**Partially Compatible:**
- ⚠️ ASP.NET Core (Minimal APIs: yes, MVC: limited)
- ⚠️ gRPC (with source generation)
- ⚠️ SignalR (limited)

## Troubleshooting

### Common Issues

#### Issue: TrimWarning IL2026

**Error:**
```
warning IL2026: Using member 'JsonSerializer.Deserialize<T>(String, JsonSerializerOptions)'
which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming.
```

**Solution:**
Use source-generated serialization context:
```csharp
// Instead of:
var obj = JsonSerializer.Deserialize<MyClass>(json);

// Use:
var obj = JsonSerializer.Deserialize(json, AppJsonContext.Default.MyClass);
```

#### Issue: Runtime TypeLoadException

**Error:**
```
System.TypeLoadException: Could not load type 'MyNamespace.MyClass'
```

**Solution:**
Add type preservation:
```csharp
[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(MyClass))]
```

#### Issue: Method Not Found at Runtime

**Error:**
```
System.MissingMethodException: Method not found: 'MyMethod'
```

**Solution:**
Ensure method is referenced or preserved:
```csharp
[DynamicDependency(nameof(MyMethod), typeof(MyClass))]
```

#### Issue: Large Binary Size

**Problem:** Published binary is >100MB

**Solution:**
1. Enable trimming: `<PublishTrimmed>true</PublishTrimmed>`
2. Use invariant globalization if possible
3. Disable debug symbols: `<DebugType>None</DebugType>`
4. Review dependencies - remove unused packages

#### Issue: Slow Startup Despite AOT

**Problem:** Startup time >100ms

**Solution:**
1. Profile with `dotnet-trace`: `dotnet trace collect --name AdvancedConcepts`
2. Lazy-load services
3. Minimize synchronous initialization
4. Use `IHostedService` for background startup tasks

### Debugging AOT Applications

```bash
# Enable debugging symbols
dotnet publish -c Release -r linux-x64 -p:PublishAot=true -p:StripSymbols=false

# Debug with GDB (Linux)
gdb ./AdvancedConcepts
(gdb) run
(gdb) backtrace

# Debug with LLDB (macOS)
lldb ./AdvancedConcepts
(lldb) run
(lldb) bt

# Performance profiling
dotnet-trace collect --name AdvancedConcepts --providers Microsoft-Windows-DotNETRuntime

# Memory profiling
dotnet-gcdump collect -p <PID>
```

## Best Practices

1. **Start Early**: Enable AOT analyzers from the beginning
2. **Incremental Migration**: Convert one module at a time
3. **Test Thoroughly**: AOT can expose runtime issues
4. **Use Source Generators**: Prefer compile-time over runtime generation
5. **Explicit Registration**: Avoid assembly scanning and auto-discovery
6. **Measure Performance**: Track startup time and memory usage
7. **CI/CD Integration**: Include AOT builds in pipelines
8. **Documentation**: Document AOT-incompatible APIs

## Resources

- [Native AOT Deployment](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
- [Prepare .NET Libraries for Trimming](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/prepare-libraries-for-trimming)
- [Introduction to AOT Warnings](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/fixing-warnings)
- [System.Text.Json Source Generation](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation)

---

**Last Updated:** 2025-12-01
**Version:** 1.0
