# Decorator Pattern Exercise

## 🎯 Learning Objectives
- **Decorator Pattern**: Add responsibilities to objects dynamically
- **Component Interface**: Define common interface for components and decorators
- **Wrapper Classes**: Decorators wrap components and add behavior
- **Flexible Composition**: Combine decorators in any order at runtime

## 📋 Exercise Tasks (Models.cs)

1. **FileDataSource** - Concrete component implementing IDataSource
2. **DataSourceDecorator** - Abstract base decorator
3. **EncryptionDecorator** - Adds Caesar cipher encryption
4. **CompressionDecorator** - Adds simple compression (run-length encoding)
5. **LoggingDecorator** - Adds operation logging

## 🚀 Getting Started

```bash
cd samples/99-Exercises/DesignPatterns/03-Decorator
dotnet test  # Should see 12 FAILED tests
```

## 💡 Implementation Hints

### FileDataSource
```csharp
public void WriteData(string data)
{
    _data = data;  // Store in memory (simulating file write)
}

public string ReadData()
{
    return _data ?? string.Empty;
}
```

### DataSourceDecorator
```csharp
public virtual void WriteData(string data)
{
    _wrappee.WriteData(data);  // Delegate to wrapped component
}

public virtual string ReadData()
{
    return _wrappee.ReadData();
}
```

### EncryptionDecorator
```csharp
public override void WriteData(string data)
{
    var encrypted = Encrypt(data);
    _wrappee.WriteData(encrypted);
}

public override string ReadData()
{
    var encrypted = _wrappee.ReadData();
    return Decrypt(encrypted);
}

private string Encrypt(string plainText)
{
    // Caesar cipher: shift each character by 3
    return new string(plainText.Select(c => (char)(c + 3)).ToArray());
}

private string Decrypt(string cipherText)
{
    return new string(cipherText.Select(c => (char)(c - 3)).ToArray());
}
```

### CompressionDecorator
```csharp
public override void WriteData(string data)
{
    var compressed = Compress(data);
    _wrappee.WriteData(compressed);
}

public override string ReadData()
{
    var compressed = _wrappee.ReadData();
    return Decompress(compressed);
}

private string Compress(string data)
{
    // Run-length encoding: "aaabbc" -> "a3b2c1"
    if (string.IsNullOrEmpty(data)) return data;

    var result = new System.Text.StringBuilder();
    var count = 1;

    for (int i = 0; i < data.Length; i++)
    {
        if (i + 1 < data.Length && data[i] == data[i + 1])
        {
            count++;
        }
        else
        {
            result.Append(data[i]);
            result.Append(count);
            count = 1;
        }
    }

    return result.ToString();
}

private string Decompress(string data)
{
    // Reverse run-length encoding: "a3b2c1" -> "aaabbc"
    if (string.IsNullOrEmpty(data)) return data;

    var result = new System.Text.StringBuilder();

    for (int i = 0; i < data.Length; i += 2)
    {
        if (i + 1 < data.Length)
        {
            var ch = data[i];
            var count = int.Parse(data[i + 1].ToString());
            result.Append(new string(ch, count));
        }
    }

    return result.ToString();
}
```

### LoggingDecorator
```csharp
public override void WriteData(string data)
{
    Log($"Writing data: {data.Length} characters");
    _wrappee.WriteData(data);
}

public override string ReadData()
{
    Log("Reading data");
    return _wrappee.ReadData();
}

private void Log(string message)
{
    var timestamp = DateTime.Now.ToString("HH:mm:ss");
    Logs.Add($"[{timestamp}] {message}");
}
```

## 🎓 Key Concepts

**Decorator Pattern Benefits:**
- Add behavior without modifying existing code
- Combine behaviors flexibly at runtime
- Single Responsibility Principle
- Open/Closed Principle

**Real-World Examples:**
- Java I/O Streams (BufferedReader wrapping FileReader)
- ASP.NET Core Middleware
- .NET Stream decorators (CryptoStream, GZipStream)
- HTTP request/response decorators

**Decorator vs Inheritance:**
- Decorator: Runtime composition, flexible
- Inheritance: Compile-time, rigid, class explosion problem

## ⚠️ Common Mistakes

1. **Not delegating to wrappee** - Decorators must call wrapped component
2. **Breaking decorator chain** - Each decorator must work with interface
3. **Order dependency** - Document if decorator order matters
4. **Over-decorating** - Too many layers can impact performance

## 🎯 Interview Tips

**Q: When to use Decorator Pattern?**
A: When you need to add responsibilities to objects dynamically without affecting other objects of the same class.

**Q: Decorator vs Adapter vs Proxy?**
- **Decorator**: Adds new behavior
- **Adapter**: Converts interface
- **Proxy**: Controls access

**Q: How does Decorator differ from inheritance?**
A: Decorators use composition for runtime flexibility, inheritance is static at compile-time.

## ✅ Success: All 12 tests pass!

**Next**: Strategy Pattern or Chain of Responsibility
