# Month 5 Capstone: Logging & Monitoring Pipeline

**Difficulty**: ⭐⭐⭐☆☆ (Advanced)
**Estimated Time**: 25-30 hours
**Prerequisites**: Completed Week 17-20 of Path 1 (Decorator Pattern & SOLID)

---

## 🎯 Project Overview

Build a flexible data processing pipeline using Decorator pattern with multiple decorators that can be chained dynamically, demonstrating all SOLID principles.

### Learning Objectives

- ✅ Decorator pattern implementation
- ✅ Dynamic behavior composition
- ✅ All SOLID principles applied
- ✅ Decorator chaining
- ✅ Pipeline pattern

---

## 📋 Requirements

### 1. Core Component Interface

```csharp
public interface IDataSource
{
    string Read();
    void Write(string data);
}
```

### 2. Base Implementation

```csharp
public class FileDataSource : IDataSource
{
    private readonly string _filePath;

    public string Read()
    {
        // Read from file
    }

    public void Write(string data)
    {
        // Write to file
    }
}
```

### 3. Decorators to Implement (5+)

1. **EncryptionDecorator**: Encrypts/decrypts data
2. **CompressionDecorator**: Compresses/decompresses data
3. **LoggingDecorator**: Logs all operations
4. **CachingDecorator**: Caches read results
5. **ValidationDecorator**: Validates data before write
6. **RetryDecorator**: Retries on failure
7. **PerformanceMonitorDecorator**: Tracks execution time

### 4. Decorator Chain Examples

```csharp
// Simple chain
IDataSource source = new FileDataSource("data.txt");
source = new EncryptionDecorator(source);
source = new LoggingDecorator(source);

// Complex chain
IDataSource source = new FileDataSource("data.txt");
source = new CompressionDecorator(source);
source = new EncryptionDecorator(source);
source = new CachingDecorator(source);
source = new ValidationDecorator(source);
source = new LoggingDecorator(source);
source = new PerformanceMonitorDecorator(source);
```

### 5. SOLID Principles Demonstration

**SRP**: Each decorator has single responsibility
**OCP**: Add new decorators without modifying existing
**LSP**: All decorators are substitutable for IDataSource
**ISP**: Single focused interface
**DIP**: Depend on IDataSource abstraction

---

## 🏗️ Project Structure

```
DecoratorPipeline/
├── Core/
│   ├── IDataSource.cs
│   └── FileDataSource.cs
├── Decorators/
│   ├── DataSourceDecorator.cs (base)
│   ├── EncryptionDecorator.cs
│   ├── CompressionDecorator.cs
│   ├── LoggingDecorator.cs
│   ├── CachingDecorator.cs
│   ├── ValidationDecorator.cs
│   ├── RetryDecorator.cs
│   └── PerformanceMonitorDecorator.cs
├── Builders/
│   └── DataSourcePipelineBuilder.cs
├── Configuration/
│   └── PipelineConfig.cs
└── Tests/
    ├── DecoratorTests.cs
    └── PipelineTests.cs
```

---

## 🚀 Implementation

### Step 1: Base Decorator

```csharp
public abstract class DataSourceDecorator : IDataSource
{
    protected readonly IDataSource _wrappedSource;

    protected DataSourceDecorator(IDataSource source)
    {
        _wrappedSource = source ?? throw new ArgumentNullException(nameof(source));
    }

    public virtual string Read()
    {
        return _wrappedSource.Read();
    }

    public virtual void Write(string data)
    {
        _wrappedSource.Write(data);
    }
}
```

### Step 2: Example Decorators

```csharp
public class EncryptionDecorator : DataSourceDecorator
{
    public EncryptionDecorator(IDataSource source) : base(source) { }

    public override string Read()
    {
        string data = base.Read();
        return Decrypt(data);
    }

    public override void Write(string data)
    {
        string encrypted = Encrypt(data);
        base.Write(encrypted);
    }

    private string Encrypt(string data)
    {
        // TODO: Implement encryption
    }

    private string Decrypt(string data)
    {
        // TODO: Implement decryption
    }
}

public class LoggingDecorator : DataSourceDecorator
{
    public LoggingDecorator(IDataSource source) : base(source) { }

    public override string Read()
    {
        Console.WriteLine($"[{DateTime.Now}] Reading data...");
        string data = base.Read();
        Console.WriteLine($"[{DateTime.Now}] Read {data.Length} characters");
        return data;
    }

    public override void Write(string data)
    {
        Console.WriteLine($"[{DateTime.Now}] Writing {data.Length} characters...");
        base.Write(data);
        Console.WriteLine($"[{DateTime.Now}] Write complete");
    }
}

// TODO: Implement remaining decorators
```

### Step 3: Pipeline Builder

```csharp
public class DataSourcePipelineBuilder
{
    private IDataSource _source;

    public DataSourcePipelineBuilder WithSource(IDataSource source)
    {
        _source = source;
        return this;
    }

    public DataSourcePipelineBuilder AddEncryption()
    {
        _source = new EncryptionDecorator(_source);
        return this;
    }

    public DataSourcePipelineBuilder AddCompression()
    {
        _source = new CompressionDecorator(_source);
        return this;
    }

    public DataSourcePipelineBuilder AddLogging()
    {
        _source = new LoggingDecorator(_source);
        return this;
    }

    // TODO: Add more fluent methods

    public IDataSource Build()
    {
        return _source;
    }
}

// Usage
var pipeline = new DataSourcePipelineBuilder()
    .WithSource(new FileDataSource("data.txt"))
    .AddCompression()
    .AddEncryption()
    .AddLogging()
    .Build();
```

---

## 🎯 Milestones

1. **Day 1-3**: Implement core + 3 decorators
2. **Day 4-6**: Implement remaining decorators
3. **Day 7-8**: Add pipeline builder
4. **Day 9-10**: Testing and documentation

---

## ✅ Evaluation Criteria

| Criteria | Points |
|----------|--------|
| Decorator Pattern | 30 |
| SOLID Principles | 25 |
| Pipeline Builder | 15 |
| 7+ Decorators | 20 |
| Tests | 10 |
| **TOTAL** | **100** |

---

## 📚 Resources

- `samples/99-Exercises/DesignPatterns/03-Decorator/`
- `src/AdvancedConcepts.Core/Advanced/SOLIDPrinciples/`

---

*Template Version: 1.0*
