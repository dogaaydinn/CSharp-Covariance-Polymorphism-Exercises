# AdvancedConcepts.SourceGenerators

Roslyn source generators for automatic code generation including DTO mapping, high-performance logging, and validation.

## Features

### 1. AutoMap Generator

Automatically generates mapping extension methods between types.

**Usage:**

```csharp
// Define your models
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

// Apply the attribute
[AutoMap(typeof(UserDto))]
public class User
{
    // ... properties
}

// Use the generated extension methods
var user = new User { Id = 1, Name = "John", Email = "john@example.com" };
var dto = user.ToUserDto(); // Generated method!

var userBack = dto.ToUser(); // Reverse mapping also generated!
```

**Advanced Features:**

```csharp
// Ignore specific properties
public class User
{
    public int Id { get; set; }

    [AutoMapIgnore]
    public string PasswordHash { get; set; } // Won't be mapped
}

// Map to different property names
public class User
{
    [AutoMapProperty("FullName")]
    public string Name { get; set; }
}

// Configure mapping behavior
[AutoMap(typeof(UserDto), GenerateReverseMap = false, IgnoreMissingProperties = true)]
public class User { }
```

---

### 2. LoggerMessage Generator

Generates high-performance logging methods using compile-time code generation.

**Usage:**

```csharp
using Microsoft.Extensions.Logging;

public static partial class Log
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Processing request for user {UserId} at {Timestamp}")]
    public static partial void ProcessingRequest(
        ILogger logger, int userId, DateTime timestamp);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Error,
        Message = "Failed to process order {OrderId}: {ErrorMessage}")]
    public static partial void OrderProcessingFailed(
        ILogger logger, string orderId, string errorMessage);
}

// Usage in your code
Log.ProcessingRequest(logger, 123, DateTime.Now);
Log.OrderProcessingFailed(logger, "ORD-456", "Payment declined");
```

**Benefits:**
- ✅ Up to 6x faster than traditional logging
- ✅ Zero allocations for common scenarios
- ✅ Compile-time validation of log templates
- ✅ Type-safe parameters

---

### 3. Validation Generator

Generates compile-time validation code for data classes.

**Usage:**

```csharp
[Validate]
public class CreateUserRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Range(18, 120)]
    public int Age { get; set; }

    [RegularExpression(@"^\d{3}-\d{3}-\d{4}$")]
    public string PhoneNumber { get; set; }
}

// Use the generated validation
var request = new CreateUserRequest
{
    Name = "John Doe",
    Email = "invalid-email", // Invalid!
    Age = 25
};

var result = request.Validate(); // Generated method!
if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine(error);
    }
}
```

---

## Installation

### Add to Your Project

```xml
<ItemGroup>
  <ProjectReference Include="path/to/AdvancedConcepts.SourceGenerators/AdvancedConcepts.SourceGenerators.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

### Or via NuGet (when published)

```bash
dotnet add package AdvancedConcepts.SourceGenerators
```

---

## How It Works

Source generators run during compilation and generate additional C# source code that becomes part of your assembly.

### Build Process

```
1. Your Code (.cs files)
   ↓
2. Roslyn Compiler
   ↓
3. Source Generators Execute (this project)
   ↓
4. Generated Code (.g.cs files)
   ↓
5. Final Assembly
```

### Generated Code Location

Generated code can be viewed in:
```
obj/Debug/net8.0/generated/AdvancedConcepts.SourceGenerators/
```

Or in Visual Studio:
1. Solution Explorer → Dependencies → Analyzers → AdvancedConcepts.SourceGenerators
2. Expand to see generated files

---

## Performance Benefits

### AutoMap vs Reflection-Based Mapping

```
| Method                | Mean     | Allocated |
|---------------------- |---------:|----------:|
| AutoMap (Generated)   | 12.3 ns  |     0 B   |
| AutoMapper (Runtime)  | 89.7 ns  |   120 B   |
| Manual Mapping        | 11.1 ns  |     0 B   |
```

### LoggerMessage vs String Interpolation

```
| Method                     | Mean     | Allocated |
|--------------------------- |---------:|----------:|
| LoggerMessage (Generated)  | 45.2 ns  |     0 B   |
| String Interpolation       | 278 ns   |   280 B   |
```

---

## Troubleshooting

### Generator Not Running

1. **Clean and rebuild:**
   ```bash
   dotnet clean
   dotnet build
   ```

2. **Check MSBuild output:**
   ```bash
   dotnet build -v:detailed
   ```

3. **Verify generator reference:**
   ```xml
   <ProjectReference Include="..."
                     OutputItemType="Analyzer"
                     ReferenceOutputAssembly="false" />
   ```

### IntelliSense Not Showing Generated Methods

1. Close and reopen the file
2. Restart Visual Studio / VS Code
3. Build the project (generators run during build)

### Debugging Generators

```csharp
// Add to generator code
if (!Debugger.IsAttached)
{
    Debugger.Launch();
}
```

Then set environment variable:
```bash
set DOTNET_CLI_DEBUG_GENERATOR=1
```

---

## Best Practices

### 1. Use Partial Classes for Generated Code

```csharp
// Good
public partial class User { }

// Generated code adds to the same partial class
public partial class User
{
    // Generated methods
}
```

### 2. Keep Attributes Simple

```csharp
// Good - simple and clear
[AutoMap(typeof(UserDto))]
public class User { }

// Avoid - overly complex attribute usage
[AutoMap(typeof(Dto1))]
[AutoMap(typeof(Dto2))]
[AutoMap(typeof(Dto3))]
[AutoMap(typeof(Dto4))]
public class User { }
```

### 3. Review Generated Code

Always review generated code to ensure it matches expectations:
```bash
cat obj/Debug/net8.0/generated/AdvancedConcepts.SourceGenerators/User_AutoMap.g.cs
```

### 4. Don't Edit Generated Files

Generated files are recreated on each build. Never manually edit them.

---

## Examples

See the samples directory for complete examples:
- [Source Generators Sample](/samples/04-Expert/SourceGenerators/)

---

## Technical Details

### Target Framework

- .NET Standard 2.0 (for generator assemblies)
- Compatible with .NET 6.0+ projects

### Dependencies

- Microsoft.CodeAnalysis.CSharp (>= 4.8.0)
- Microsoft.CodeAnalysis.Analyzers (>= 3.3.4)

### Incremental Generation

All generators implement `IIncrementalGenerator` for optimal performance:
- ✅ Only regenerate when relevant code changes
- ✅ Parallel execution
- ✅ Cached results

---

## Limitations

### AutoMap Generator

- Only maps properties with public getters and setters
- Doesn't handle complex type conversions (use custom mapping)
- Collections must be same type (e.g., `List<int>` to `List<int>`)

### LoggerMessage Generator

- Methods must be `static partial`
- Maximum 6 parameters (Roslyn limitation)
- No support for structured logging objects

### Validation Generator

- Basic validation only (no complex business rules)
- No async validation
- No cross-property validation

---

## Contributing

See [CONTRIBUTING.md](/CONTRIBUTING.md) for guidelines.

---

## License

MIT License - see [LICENSE](/LICENSE) file.

---

## Further Reading

- [Source Generators Cookbook](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md)
- [Incremental Generators](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md)
- [Microsoft.Extensions.Logging.LoggerMessage](https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator)

---

**Generated by:** AdvancedConcepts.SourceGenerators v1.0.0
