using System;
using System.Text;

namespace PerformanceOptimization.Examples;

/// <summary>
/// Demonstrates string optimization techniques to reduce allocations
/// </summary>
public static class StringOptimization
{
    /// <summary>
    /// Example 1: StringBuilder vs string concatenation
    /// </summary>
    public static void StringBuilderVsConcatenation()
    {
        Console.WriteLine("\n=== StringBuilder vs String Concatenation ===");
        
        // ❌ Bad: String concatenation in loop - creates new string each time
        string result1 = "";
        for (int i = 0; i < 5; i++)
        {
            result1 += "Item" + i + " ";  // 5 iterations = 5+ allocations
        }
        Console.WriteLine($"Concatenation result: '{result1.Trim()}'");
        
        // ✅ Good: StringBuilder - reuses buffer
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 5; i++)
        {
            sb.Append("Item").Append(i).Append(' ');
        }
        string result2 = sb.ToString();
        Console.WriteLine($"StringBuilder result: '{result2.Trim()}'");
        
        Console.WriteLine("\nRule of thumb:");
        Console.WriteLine("  < 5 concatenations: + operator is fine");
        Console.WriteLine("  > 5 concatenations: Use StringBuilder");
        Console.WriteLine("  Loop concatenation: ALWAYS use StringBuilder");
    }

    /// <summary>
    /// Example 2: String interning - reuse identical strings
    /// </summary>
    public static void StringInterning()
    {
        Console.WriteLine("\n=== String Interning ===");
        
        // Literal strings are automatically interned
        string s1 = "Hello";
        string s2 = "Hello";
        Console.WriteLine($"s1 == s2: {object.ReferenceEquals(s1, s2)} (literals interned automatically)");
        
        // Runtime strings are not interned by default
        string s3 = new string("Hello".ToCharArray());
        string s4 = new string("Hello".ToCharArray());
        Console.WriteLine($"s3 == s4: {object.ReferenceEquals(s3, s4)} (runtime strings not interned)");
        
        // Manual interning
        string s5 = string.Intern(s3);
        string s6 = string.Intern(s4);
        Console.WriteLine($"Interned s5 == s6: {object.ReferenceEquals(s5, s6)} (manually interned)");
        
        Console.WriteLine("\nWhen to use Intern():");
        Console.WriteLine("  ✓ Many duplicate strings (e.g., enum names, config keys)");
        Console.WriteLine("  ✓ Long-lived strings");
        Console.WriteLine("  ✗ Temporary strings (wastes memory)");
        Console.WriteLine("  ✗ Unique strings (no benefit)");
    }

    /// <summary>
    /// Example 3: String.Create for efficient string building
    /// </summary>
    public static void StringCreate()
    {
        Console.WriteLine("\n=== String.Create (C# 7.2+) ===");
        
        int id = 42;
        string prefix = "User_";
        
        // Traditional: Multiple allocations
        string result1 = prefix + id.ToString();
        Console.WriteLine($"Traditional: '{result1}'");
        
        // String.Create: Single allocation, no intermediate strings
        string result2 = string.Create(prefix.Length + 10, (prefix, id), (span, state) =>
        {
            state.prefix.AsSpan().CopyTo(span);
            state.id.TryFormat(span.Slice(state.prefix.Length), out int written);
        });
        Console.WriteLine($"String.Create: '{result2}'");
        
        Console.WriteLine("\nBenefits:");
        Console.WriteLine("  ✓ Single allocation");
        Console.WriteLine("  ✓ No intermediate strings");
        Console.WriteLine("  ✓ Stack-based formatting");
    }

    /// <summary>
    /// Example 4: AsSpan for string slicing without allocation
    /// </summary>
    public static void StringAsSpan()
    {
        Console.WriteLine("\n=== String.AsSpan() ===");
        
        string path = "/home/user/documents/file.txt";
        
        // ❌ Bad: Substring allocates new string
        string filename1 = path.Substring(path.LastIndexOf('/') + 1);
        Console.WriteLine($"Substring: '{filename1}' (new allocation)");
        
        // ✅ Good: AsSpan returns view, no allocation
        ReadOnlySpan<char> span = path.AsSpan();
        int lastSlash = path.LastIndexOf('/');
        ReadOnlySpan<char> filename2 = span.Slice(lastSlash + 1);
        Console.WriteLine($"AsSpan: '{filename2.ToString()}' (zero allocation until ToString)");
        
        // Best for comparisons - no allocation at all
        ReadOnlySpan<char> extension = span.Slice(span.LastIndexOf('.') + 1);
        bool isTxt = extension.SequenceEqual("txt".AsSpan());
        Console.WriteLine($"Is .txt file? {isTxt} (compared without allocation!)");
    }

    /// <summary>
    /// Example 5: String pooling with ArrayPool
    /// </summary>
    public static void StringPooling()
    {
        Console.WriteLine("\n=== String Pooling Pattern ===");
        
        // Scenario: Building temporary strings frequently
        char[] buffer = System.Buffers.ArrayPool<char>.Shared.Rent(100);
        
        try
        {
            // Build string in rented buffer
            int length = 0;
            "Order_".AsSpan().CopyTo(buffer);
            length += 6;
            
            int orderId = 12345;
            orderId.TryFormat(buffer.AsSpan(length), out int written);
            length += written;
            
            // Create final string only once
            string result = new string(buffer, 0, length);
            Console.WriteLine($"Pooled result: '{result}'");
            Console.WriteLine("✓ Buffer reused, minimal allocations");
        }
        finally
        {
            System.Buffers.ArrayPool<char>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// Example 6: Implicit vs explicit string formatting
    /// </summary>
    public static void StringFormatting()
    {
        Console.WriteLine("\n=== String Formatting Performance ===");
        
        int id = 123;
        string name = "John";
        decimal price = 99.99m;
        
        // ❌ Slowest: String concatenation
        string result1 = "ID: " + id + ", Name: " + name + ", Price: $" + price;
        Console.WriteLine($"Concatenation: {result1}");
        
        // ⚠️ Medium: String.Format
        string result2 = string.Format("ID: {0}, Name: {1}, Price: ${2}", id, name, price);
        Console.WriteLine($"String.Format: {result2}");
        
        // ✅ Fast: String interpolation (C# 10+)
        string result3 = $"ID: {id}, Name: {name}, Price: ${price}";
        Console.WriteLine($"Interpolation: {result3}");
        
        // ✅ Fastest: StringBuilder
        var sb = new StringBuilder()
            .Append("ID: ").Append(id)
            .Append(", Name: ").Append(name)
            .Append(", Price: $").Append(price);
        string result4 = sb.ToString();
        Console.WriteLine($"StringBuilder: {result4}");
        
        Console.WriteLine("\nPerformance ranking (fastest to slowest):");
        Console.WriteLine("  1. StringBuilder (best for loops)");
        Console.WriteLine("  2. String interpolation (best readability)");
        Console.WriteLine("  3. String.Format");
        Console.WriteLine("  4. String concatenation (avoid in loops!)");
    }

    /// <summary>
    /// Example 7: String comparison optimization
    /// </summary>
    public static void StringComparisonOptimization()
    {
        Console.WriteLine("\n=== String Comparison Optimization ===");
        
        string s1 = "Hello World";
        string s2 = "hello world";
        
        // ❌ Slow: Case-insensitive comparison via ToLower
        bool equal1 = s1.ToLower() == s2.ToLower();  // Allocates 2 new strings!
        Console.WriteLine($"ToLower: {equal1} (2 allocations)");
        
        // ✅ Fast: StringComparison enum
        bool equal2 = string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        Console.WriteLine($"StringComparison: {equal2} (0 allocations)");
        
        // ✅ Even faster: Span comparison
        bool equal3 = s1.AsSpan().Equals(s2.AsSpan(), StringComparison.OrdinalIgnoreCase);
        Console.WriteLine($"Span comparison: {equal3} (0 allocations, fastest)");
        
        Console.WriteLine("\nAlways use:");
        Console.WriteLine("  StringComparison.Ordinal (exact match, fastest)");
        Console.WriteLine("  StringComparison.OrdinalIgnoreCase (case-insensitive)");
        Console.WriteLine("  StringComparison.CurrentCulture (locale-aware)");
    }

    /// <summary>
    /// Example 8: Real-world optimization - building JSON manually
    /// </summary>
    public static void BuildJsonEfficiently()
    {
        Console.WriteLine("\n=== Efficient JSON Building ===");
        
        int userId = 42;
        string userName = "John Doe";
        bool isActive = true;
        
        // ❌ Bad: String concatenation
        string json1 = "{" +
            "\"id\":" + userId + "," +
            "\"name\":\"" + userName + "\"," +
            "\"active\":" + isActive.ToString().ToLower() +
            "}";
        Console.WriteLine($"Concatenation: {json1}");
        
        // ✅ Good: StringBuilder
        var sb = new StringBuilder(128);  // Pre-allocate capacity
        sb.Append("{\"id\":").Append(userId)
          .Append(",\"name\":\"").Append(userName)
          .Append("\",\"active\":").Append(isActive ? "true" : "false")
          .Append('}');
        string json2 = sb.ToString();
        Console.WriteLine($"StringBuilder: {json2}");
        
        // ✅ Best: Use System.Text.Json for real scenarios
        Console.WriteLine("\n💡 For production: Use System.Text.Json.JsonSerializer");
    }

    /// <summary>
    /// Example 9: Common pitfalls
    /// </summary>
    public static void CommonPitfalls()
    {
        Console.WriteLine("\n=== Common String Performance Pitfalls ===");
        
        Console.WriteLine("\n❌ Pitfall 1: String concatenation in logs");
        Console.WriteLine("  Bad:  logger.LogDebug(\"User \" + userId + \" logged in\");");
        Console.WriteLine("  Good: logger.LogDebug(\"User {UserId} logged in\", userId);");
        
        Console.WriteLine("\n❌ Pitfall 2: Substring in tight loops");
        Console.WriteLine("  Bad:  for (...) { var sub = text.Substring(i, 10); }");
        Console.WriteLine("  Good: var span = text.AsSpan(); for (...) { var sub = span.Slice(i, 10); }");
        
        Console.WriteLine("\n❌ Pitfall 3: Implicit boxing");
        Console.WriteLine("  Bad:  string.Format(\"{0}\", 123);  // boxes int");
        Console.WriteLine("  Good: $\"{123}\";  // no boxing in C# 10+");
        
        Console.WriteLine("\n❌ Pitfall 4: Unnecessary ToString()");
        Console.WriteLine("  Bad:  var s = \"Value: \" + intValue.ToString();");
        Console.WriteLine("  Good: var s = $\"Value: {intValue}\";");
    }
}
