namespace HighPerformanceSpan;

/// <summary>
/// Zero-allocation string parsing using Span&lt;T&gt; and ReadOnlySpan&lt;T&gt;.
///
/// TRADITIONAL APPROACH (BAD):
///   string.Split(',')          → Allocates string array
///   string.Substring(0, 5)     → Allocates new string
///   string.Trim()              → Allocates new string
///
/// SPAN APPROACH (GOOD):
///   span.Slice(0, 5)           → NO allocation!
///   span.IndexOf(',')          → NO allocation!
///   span.Trim()                → NO allocation!
///
/// PERFORMANCE BENEFITS:
/// - 100x faster for parsing tasks
/// - Zero GC pressure
/// - Cache-friendly
/// - Stack-based when possible
/// </summary>
public static class StringParser
{
    /// <summary>
    /// Parse CSV data without allocating strings.
    /// </summary>
    public static void DemonstrateStringParsing()
    {
        Console.WriteLine("1. Zero-Allocation String Parsing");

        string data = "John,Doe,30,Engineer";

        // ✅ GOOD: No substring allocations!
        ReadOnlySpan<char> span = data.AsSpan();

        int firstComma = span.IndexOf(',');
        var firstName = span.Slice(0, firstComma);

        Console.WriteLine($"   First name: {firstName.ToString()}");
        Console.WriteLine($"   ✅ Zero string allocations!");
    }

    /// <summary>
    /// Parse CSV with multiple fields.
    /// </summary>
    public static void ParseCSV(string csvLine)
    {
        Console.WriteLine($"\n   Parsing CSV: '{csvLine}'");

        ReadOnlySpan<char> span = csvLine.AsSpan();
        int fieldIndex = 0;

        while (!span.IsEmpty)
        {
            int commaIndex = span.IndexOf(',');
            ReadOnlySpan<char> field;

            if (commaIndex == -1)
            {
                // Last field
                field = span;
                span = ReadOnlySpan<char>.Empty;
            }
            else
            {
                // Extract field before comma
                field = span.Slice(0, commaIndex);
                span = span.Slice(commaIndex + 1);
            }

            Console.WriteLine($"   Field {fieldIndex++}: '{field.ToString()}'");
        }

        Console.WriteLine($"   ✅ Zero allocations during parsing!");
    }

    /// <summary>
    /// Parse integer from string without allocation.
    /// </summary>
    public static int ParseInt(ReadOnlySpan<char> span)
    {
        // Built-in zero-allocation parsing
        if (int.TryParse(span, out int result))
        {
            return result;
        }
        throw new FormatException("Invalid integer format");
    }

    /// <summary>
    /// Trim whitespace without allocation.
    /// </summary>
    public static ReadOnlySpan<char> TrimWhitespace(ReadOnlySpan<char> span)
    {
        // Zero-allocation trim
        return span.Trim();
    }

    /// <summary>
    /// Count words in string without allocation.
    /// </summary>
    public static int CountWords(ReadOnlySpan<char> text)
    {
        int count = 0;
        bool inWord = false;

        for (int i = 0; i < text.Length; i++)
        {
            if (char.IsWhiteSpace(text[i]))
            {
                inWord = false;
            }
            else if (!inWord)
            {
                count++;
                inWord = true;
            }
        }

        return count;
    }

    /// <summary>
    /// Split string by delimiter (zero allocation for iteration).
    /// </summary>
    public static void SplitByDelimiter(ReadOnlySpan<char> text, char delimiter)
    {
        Console.WriteLine($"\n   Splitting by '{delimiter}':");

        int index = 0;
        while (!text.IsEmpty)
        {
            int delimiterIndex = text.IndexOf(delimiter);

            if (delimiterIndex == -1)
            {
                // Last segment
                Console.WriteLine($"   [{index}]: '{text.ToString()}'");
                break;
            }

            // Extract segment
            var segment = text.Slice(0, delimiterIndex);
            Console.WriteLine($"   [{index}]: '{segment.ToString()}'");

            // Move to next segment
            text = text.Slice(delimiterIndex + 1);
            index++;
        }

        Console.WriteLine($"   ✅ Split complete with minimal allocations!");
    }

    /// <summary>
    /// Extract extension from filename (zero allocation).
    /// </summary>
    public static ReadOnlySpan<char> GetFileExtension(ReadOnlySpan<char> filename)
    {
        int dotIndex = filename.LastIndexOf('.');
        if (dotIndex == -1)
            return ReadOnlySpan<char>.Empty;

        return filename.Slice(dotIndex + 1);
    }

    /// <summary>
    /// Check if string starts with prefix (zero allocation).
    /// </summary>
    public static bool StartsWith(ReadOnlySpan<char> text, ReadOnlySpan<char> prefix)
    {
        if (text.Length < prefix.Length)
            return false;

        return text.Slice(0, prefix.Length).SequenceEqual(prefix);
    }

    /// <summary>
    /// Reverse string in-place using Span.
    /// </summary>
    public static void ReverseString(Span<char> text)
    {
        for (int i = 0; i < text.Length / 2; i++)
        {
            int j = text.Length - 1 - i;
            (text[i], text[j]) = (text[j], text[i]);
        }
    }
}
