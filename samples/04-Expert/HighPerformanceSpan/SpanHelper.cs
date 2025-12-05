namespace HighPerformanceSpan;

/// <summary>
/// Helper utilities for Span&lt;T&gt; and ReadOnlySpan&lt;T&gt;.
///
/// SPAN TYPES:
/// - Span<T>:         Mutable, stack-only, can modify
/// - ReadOnlySpan<T>: Immutable, stack-only, read-only
/// - Memory<T>:       Heap-friendly Span (can store in fields)
/// - ReadOnlyMemory<T>: Heap-friendly ReadOnlySpan
///
/// KEY RULES:
/// - Span<T> cannot be stored in fields (ref struct)
/// - Use Memory<T> for async methods
/// - Span is faster, Memory is more flexible
/// </summary>
public static class SpanHelper
{
    /// <summary>
    /// Demonstrate ReadOnlySpan for immutability.
    /// </summary>
    public static void DemonstrateReadOnlySpan()
    {
        Console.WriteLine("\n4. ReadOnlySpan for Immutability");

        string text = "Hello, World!";
        ReadOnlySpan<char> readOnly = text.AsSpan();

        // ✅ Can read
        Console.WriteLine($"   Length: {readOnly.Length}");
        Console.WriteLine($"   First char: {readOnly[0]}");

        // ❌ Cannot write (compile error!)
        // readOnly[0] = 'h';  // Error: ReadOnlySpan is immutable

        Console.WriteLine($"   ✅ Type-safe immutability!");
    }

    /// <summary>
    /// Convert Span to ReadOnlySpan (safe).
    /// </summary>
    public static ReadOnlySpan<T> ToReadOnly<T>(Span<T> span)
    {
        return span;  // Implicit conversion
    }

    /// <summary>
    /// Check if span is empty or whitespace.
    /// </summary>
    public static bool IsEmptyOrWhitespace(ReadOnlySpan<char> span)
    {
        if (span.IsEmpty)
            return true;

        for (int i = 0; i < span.Length; i++)
        {
            if (!char.IsWhiteSpace(span[i]))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Compare spans for equality (case-insensitive).
    /// </summary>
    public static bool EqualsIgnoreCase(ReadOnlySpan<char> a, ReadOnlySpan<char> b)
    {
        if (a.Length != b.Length)
            return false;

        for (int i = 0; i < a.Length; i++)
        {
            if (char.ToLowerInvariant(a[i]) != char.ToLowerInvariant(b[i]))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Count occurrences of character in span.
    /// </summary>
    public static int CountChar(ReadOnlySpan<char> span, char target)
    {
        int count = 0;
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == target)
                count++;
        }
        return count;
    }

    /// <summary>
    /// Replace character in-place (using Span).
    /// </summary>
    public static void ReplaceChar(Span<char> span, char oldChar, char newChar)
    {
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == oldChar)
                span[i] = newChar;
        }
    }

    /// <summary>
    /// Demonstrate Memory<T> for async scenarios.
    /// </summary>
    public static async Task DemonstrateMemoryAsync()
    {
        Console.WriteLine("\n   Memory<T> for async methods:");

        // Memory<T> can be stored and used across await
        Memory<byte> buffer = new byte[1024];

        await Task.Delay(10);

        // Can still use buffer after await
        buffer.Span.Fill(42);

        Console.WriteLine($"   Filled {buffer.Length} bytes");
        Console.WriteLine($"   ✅ Memory<T> works with async!");
    }

    /// <summary>
    /// Slice string into lines without allocation.
    /// Note: Custom delegate needed because ReadOnlySpan cannot be used in Action<T>
    /// </summary>
    public delegate void LineHandler(ReadOnlySpan<char> line);

    public static void ForEachLine(ReadOnlySpan<char> text, LineHandler lineHandler)
    {
        while (!text.IsEmpty)
        {
            int newlineIndex = text.IndexOfAny('\r', '\n');

            if (newlineIndex == -1)
            {
                // Last line
                lineHandler(text);
                break;
            }

            // Extract line
            var line = text.Slice(0, newlineIndex);
            lineHandler(line);

            // Skip newline characters
            text = text.Slice(newlineIndex + 1);
            if (!text.IsEmpty && text[0] == '\n')
                text = text.Slice(1);
        }
    }

    /// <summary>
    /// Clear sensitive data from span (security).
    /// </summary>
    public static void ClearSensitiveData(Span<char> sensitiveData)
    {
        sensitiveData.Clear();  // Zero out memory
        Console.WriteLine("   ✅ Sensitive data cleared!");
    }

    /// <summary>
    /// Demonstrate span initialization patterns.
    /// </summary>
    public static void DemonstrateSpanPatterns()
    {
        Console.WriteLine("\n   Span initialization patterns:");

        // From array
        int[] array = { 1, 2, 3, 4, 5 };
        Span<int> span1 = array;
        Console.WriteLine($"   From array: {span1.Length} elements");

        // From stackalloc
        Span<int> span2 = stackalloc int[] { 10, 20, 30 };
        Console.WriteLine($"   From stackalloc: {span2.Length} elements");

        // From string
        ReadOnlySpan<char> span3 = "Hello".AsSpan();
        Console.WriteLine($"   From string: {span3.Length} characters");

        // Empty span
        Span<int> span4 = Span<int>.Empty;
        Console.WriteLine($"   Empty span: {span4.Length} elements");

        Console.WriteLine($"   ✅ Multiple initialization options!");
    }
}
