using System.Buffers;
using System.Runtime.InteropServices;

namespace AdvancedCsharpConcepts.Advanced.HighPerformance;

/// <summary>
/// Span&lt;T&gt; and Memory&lt;T&gt; - Zero-allocation, high-performance patterns.
/// NVIDIA developer mindset: minimize memory allocations for maximum throughput.
/// Silicon Valley best practice: use stack allocation when possible.
/// </summary>
public class SpanMemoryExamples
{
    /// <summary>
    /// Traditional string parsing with allocations.
    /// Creates substring objects on the heap.
    /// </summary>
    public static int[] ParseNumbersTraditional(string input)
    {
        var parts = input.Split(',');
        var numbers = new int[parts.Length];
        for (var i = 0; i < parts.Length; i++)
        {
            numbers[i] = int.Parse(parts[i].Trim());
        }
        return numbers;
    }

    /// <summary>
    /// Modern zero-allocation parsing with Span&lt;T&gt;.
    /// No heap allocations for intermediate strings.
    /// Expected: 5-10x faster with zero GC pressure.
    /// </summary>
    public static int[] ParseNumbersModern(string input)
    {
        var span = input.AsSpan();
        var numbers = new List<int>();

        var start = 0;
        for (var i = 0; i < span.Length; i++)
        {
            if (span[i] == ',')
            {
                var slice = span.Slice(start, i - start).Trim();
                if (int.TryParse(slice, out var number))
                {
                    numbers.Add(number);
                }
                start = i + 1;
            }
        }

        // Don't forget the last number
        if (start < span.Length)
        {
            var slice = span.Slice(start).Trim();
            if (int.TryParse(slice, out var number))
            {
                numbers.Add(number);
            }
        }

        return numbers.ToArray();
    }

    /// <summary>
    /// Stack allocation with Span&lt;T&gt; for temporary buffers.
    /// No heap allocation at all - perfect for hot paths.
    /// </summary>
    public static void StackAllocationExample()
    {
        // Allocate 256 bytes on the stack (not heap!)
        Span<byte> buffer = stackalloc byte[256];

        // Use the buffer for temporary work
        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = (byte)(i % 256);
        }

        // Calculate checksum
        int checksum = 0;
        foreach (var b in buffer)
        {
            checksum += b;
        }

        Console.WriteLine($"Checksum: {checksum} (zero heap allocations!)");
    }

    /// <summary>
    /// ArrayPool&lt;T&gt; - Rent/return pattern for reusable buffers.
    /// NVIDIA-style memory management: reuse instead of allocate.
    /// </summary>
    public static void ArrayPoolExample()
    {
        const int bufferSize = 1024;
        var pool = ArrayPool<int>.Shared;

        // Rent a buffer from the pool
        var buffer = pool.Rent(bufferSize);
        try
        {
            // Use the buffer
            var span = buffer.AsSpan(0, bufferSize);
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = i * 2;
            }

            var sum = 0;
            foreach (var value in span)
            {
                sum += value;
            }

            Console.WriteLine($"ArrayPool sum: {sum}");
        }
        finally
        {
            // Always return the buffer to the pool
            pool.Return(buffer);
        }
    }

    /// <summary>
    /// Slicing and dicing with Span&lt;T&gt; - zero-copy operations.
    /// Perfect for parsing, tokenizing, and data processing.
    /// </summary>
    public static void SpanSlicingExample()
    {
        int[] data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var span = data.AsSpan();

        // Get first half (no allocation)
        var firstHalf = span[..5];
        Console.WriteLine($"First half: {string.Join(", ", firstHalf.ToArray())}");

        // Get second half (no allocation)
        var secondHalf = span[5..];
        Console.WriteLine($"Second half: {string.Join(", ", secondHalf.ToArray())}");

        // Get middle elements (no allocation)
        var middle = span[3..7];
        Console.WriteLine($"Middle: {string.Join(", ", middle.ToArray())}");

        // Reverse in place (modifies original array)
        middle.Reverse();
        Console.WriteLine($"After reversing middle: {string.Join(", ", data)}");
    }

    /// <summary>
    /// Memory&lt;T&gt; for async operations.
    /// Span&lt;T&gt; is a ref struct and can't be used in async methods.
    /// Memory&lt;T&gt; provides similar benefits with async support.
    /// </summary>
    public static async Task<int> AsyncMemoryExample()
    {
        var memory = new Memory<int>(new int[1000]);

        // Simulate async operation
        await Task.Delay(10);

        // Fill the memory (work with span outside async state machine)
        FillMemory(memory);

        // Another async operation
        await Task.Delay(10);

        // Calculate sum (work with span outside async state machine)
        var sum = CalculateSum(memory);

        return sum;
    }

    private static void FillMemory(Memory<int> memory)
    {
        var span = memory.Span;
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = i;
        }
    }

    private static int CalculateSum(Memory<int> memory)
    {
        var span = memory.Span;
        var sum = 0;
        foreach (var value in span)
        {
            sum += value;
        }
        return sum;
    }

    /// <summary>
    /// Advanced: Custom ref struct for zero-allocation parsing.
    /// Mimics how high-performance parsers work.
    /// </summary>
    public ref struct SpanTokenizer
    {
        private ReadOnlySpan<char> _remaining;
        private readonly char _separator;

        public SpanTokenizer(ReadOnlySpan<char> input, char separator)
        {
            _remaining = input;
            _separator = separator;
        }

        public bool MoveNext(out ReadOnlySpan<char> token)
        {
            if (_remaining.IsEmpty)
            {
                token = default;
                return false;
            }

            var index = _remaining.IndexOf(_separator);
            if (index >= 0)
            {
                token = _remaining[..index];
                _remaining = _remaining[(index + 1)..];
            }
            else
            {
                token = _remaining;
                _remaining = default;
            }

            return true;
        }
    }

    /// <summary>
    /// Using the custom tokenizer - zero allocations!
    /// </summary>
    public static void CustomTokenizerExample()
    {
        const string input = "apple,banana,cherry,date,elderberry";
        var tokenizer = new SpanTokenizer(input.AsSpan(), ',');

        Console.WriteLine("Tokens (zero allocations):");
        while (tokenizer.MoveNext(out var token))
        {
            Console.WriteLine($"  - {token.ToString()}");
        }
    }

    /// <summary>
    /// Performance comparison: Traditional vs Span-based parsing.
    /// </summary>
    public static void BenchmarkParsing()
    {
        const string input = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20";
        const int iterations = 100_000;

        // Warmup
        _ = ParseNumbersTraditional(input);
        _ = ParseNumbersModern(input);

        // Traditional
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (var i = 0; i < iterations; i++)
        {
            _ = ParseNumbersTraditional(input);
        }
        var traditionalTime = sw.Elapsed;

        // Modern
        sw.Restart();
        for (var i = 0; i < iterations; i++)
        {
            _ = ParseNumbersModern(input);
        }
        var modernTime = sw.Elapsed;

        Console.WriteLine($"Traditional: {traditionalTime.TotalMilliseconds:F2} ms");
        Console.WriteLine($"Modern:      {modernTime.TotalMilliseconds:F2} ms");
        Console.WriteLine($"Speedup:     {traditionalTime.TotalMilliseconds / modernTime.TotalMilliseconds:F2}x");
    }

    /// <summary>
    /// Demonstrates all Span&lt;T&gt; and Memory&lt;T&gt; features.
    /// </summary>
    public static void RunExample()
    {
        Console.WriteLine("\n=== Span<T> and Memory<T> - Zero-Allocation Patterns ===\n");

        Console.WriteLine("1. Stack Allocation:");
        StackAllocationExample();

        Console.WriteLine("\n2. ArrayPool Rental:");
        ArrayPoolExample();

        Console.WriteLine("\n3. Span Slicing:");
        SpanSlicingExample();

        Console.WriteLine("\n4. Async Memory:");
        var asyncTask = AsyncMemoryExample();
        asyncTask.Wait();
        Console.WriteLine($"Async sum: {asyncTask.Result}");

        Console.WriteLine("\n5. Custom Tokenizer:");
        CustomTokenizerExample();

        Console.WriteLine("\n6. Parsing Benchmark:");
        BenchmarkParsing();
    }
}
