namespace HighPerformanceSpan;

/// <summary>
/// Stack-based processing using stackalloc and Span&lt;T&gt;.
///
/// STACK vs HEAP ALLOCATION:
///
/// HEAP (new byte[256]):
/// - Allocates on managed heap
/// - GC tracking overhead
/// - Slower allocation (~100ns)
/// - Can cause GC pauses
///
/// STACK (stackalloc byte[256]):
/// - Allocates on thread stack
/// - Zero GC overhead
/// - Instant allocation (~1ns)
/// - Auto cleanup on scope exit
///
/// LIMITS:
/// - Stack size ~1MB (avoid large allocations)
/// - Cannot escape method scope
/// - Stack overflow risk if too large
///
/// BEST PRACTICES:
/// - Use for temporary buffers < 1KB
/// - Perfect for hot paths
/// - Combine with Span<T> for safety
/// </summary>
public static class StackProcessor
{
    /// <summary>
    /// Basic stack allocation demonstration.
    /// </summary>
    public static void DemonstrateStackAllocation()
    {
        Console.WriteLine("\n3. Stack-Based Processing");

        // Stack allocation (no GC pressure!)
        Span<byte> buffer = stackalloc byte[256];

        // Fill with pattern
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = (byte)(i % 16);
        }

        Console.WriteLine($"   Processed {buffer.Length} bytes on stack");
        Console.WriteLine($"   ✅ Zero heap allocations!");
    }

    /// <summary>
    /// Process data with stack buffer (zero heap allocation).
    /// </summary>
    public static int ProcessWithStackBuffer(ReadOnlySpan<int> data)
    {
        // Allocate working buffer on stack
        Span<int> workBuffer = stackalloc int[10];

        // Process data in chunks
        int sum = 0;
        for (int i = 0; i < data.Length; i += workBuffer.Length)
        {
            int chunkSize = Math.Min(workBuffer.Length, data.Length - i);
            var chunk = data.Slice(i, chunkSize);

            // Copy to work buffer
            chunk.CopyTo(workBuffer);

            // Process
            for (int j = 0; j < chunkSize; j++)
            {
                sum += workBuffer[j];
            }
        }

        return sum;
    }

    /// <summary>
    /// Build string using stack buffer (minimal allocations).
    /// </summary>
    public static string BuildStringWithStack()
    {
        // Stack-allocate char buffer
        Span<char> buffer = stackalloc char[100];

        int pos = 0;

        // Append "Hello"
        "Hello".AsSpan().CopyTo(buffer.Slice(pos));
        pos += 5;

        // Append ", "
        ", ".AsSpan().CopyTo(buffer.Slice(pos));
        pos += 2;

        // Append "World!"
        "World!".AsSpan().CopyTo(buffer.Slice(pos));
        pos += 6;

        // Convert to string (only allocation!)
        return new string(buffer.Slice(0, pos));
    }

    /// <summary>
    /// Format integer without allocation (using stack buffer).
    /// </summary>
    public static void FormatIntegerToStack(int value, Span<char> destination)
    {
        if (!value.TryFormat(destination, out int charsWritten))
        {
            throw new ArgumentException("Destination buffer too small");
        }

        Console.WriteLine($"   Formatted: '{new string(destination.Slice(0, charsWritten))}'");
    }

    /// <summary>
    /// Compute hash using stack workspace.
    /// </summary>
    public static int ComputeSimpleHash(ReadOnlySpan<char> text)
    {
        // Use stack for intermediate computation
        Span<int> hashes = stackalloc int[4];
        hashes.Fill(0);

        // Simple hash algorithm
        for (int i = 0; i < text.Length; i++)
        {
            hashes[i % 4] ^= text[i] << (i % 16);
        }

        // Combine hashes
        int result = 0;
        for (int i = 0; i < hashes.Length; i++)
        {
            result ^= hashes[i];
        }

        return result;
    }

    /// <summary>
    /// Binary search using stack-allocated workspace.
    /// </summary>
    public static int BinarySearchWithStack(ReadOnlySpan<int> sortedArray, int target)
    {
        // Could use stack for iterative search state if needed
        int left = 0;
        int right = sortedArray.Length - 1;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;

            if (sortedArray[mid] == target)
                return mid;

            if (sortedArray[mid] < target)
                left = mid + 1;
            else
                right = mid - 1;
        }

        return -1;
    }

    /// <summary>
    /// Convert UTF-8 bytes to string using stack buffer.
    /// </summary>
    public static string Utf8ToString(ReadOnlySpan<byte> utf8Bytes)
    {
        // Stack-allocate char buffer (assume 1 byte = 1 char worst case)
        Span<char> chars = utf8Bytes.Length < 256
            ? stackalloc char[utf8Bytes.Length]
            : new char[utf8Bytes.Length];

        int charCount = System.Text.Encoding.UTF8.GetChars(utf8Bytes, chars);

        return new string(chars.Slice(0, charCount));
    }

    /// <summary>
    /// Demonstrate safe stack allocation with size check.
    /// </summary>
    public static void SafeStackAllocation(int size)
    {
        const int MaxStackSize = 512;  // Safe limit

        if (size <= MaxStackSize)
        {
            // Safe to use stack
            Span<byte> buffer = stackalloc byte[size];
            Console.WriteLine($"   Allocated {size} bytes on stack");
        }
        else
        {
            // Fall back to heap
            Span<byte> buffer = new byte[size];
            Console.WriteLine($"   Allocated {size} bytes on heap (too large for stack)");
        }
    }
}
