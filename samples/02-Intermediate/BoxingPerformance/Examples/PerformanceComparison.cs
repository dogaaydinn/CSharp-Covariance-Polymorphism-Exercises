using System.Collections;
using System.Diagnostics;
using System.Runtime;

namespace BoxingPerformance.Examples;

/// <summary>
/// Comprehensive performance comparison between collections that cause boxing
/// (ArrayList, Hashtable) and generic collections that avoid boxing (List&lt;T&gt;, Dictionary&lt;K,V&gt;).
/// </summary>
/// <remarks>
/// This class demonstrates real-world performance differences through benchmarks:
/// - ArrayList vs List&lt;T&gt;: Up to 10x performance difference
/// - Hashtable vs Dictionary&lt;K,V&gt;: Up to 5x performance difference
/// - Memory allocation differences: Up to 6x memory overhead with boxing
/// - GC pressure comparison: Boxing creates significantly more Gen0 collections
/// </remarks>
public static class PerformanceComparison
{
    /// <summary>
    /// Compares ArrayList (with boxing) vs List&lt;int&gt; (without boxing) performance.
    /// Measures add, iterate, and search operations.
    /// </summary>
    public static void CompareArrayListVsListPerformance()
    {
        Console.WriteLine("=== ArrayList vs List<int> Performance ===");
        Console.WriteLine();

        const int iterations = 1_000_000;
        Console.WriteLine($"Operations: {iterations:N0} integers");
        Console.WriteLine();

        // Force garbage collection before tests
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Track GC stats
        int gen0Before = GC.CollectionCount(0);
        int gen1Before = GC.CollectionCount(1);
        int gen2Before = GC.CollectionCount(2);
        long memoryBefore = GC.GetTotalMemory(true);

        // Test 1: ArrayList (with boxing)
        Console.WriteLine("1. ArrayList (non-generic, WITH boxing):");
        var sw1 = Stopwatch.StartNew();

        var arrayList = new ArrayList(iterations);
        for (int i = 0; i < iterations; i++)
        {
            arrayList.Add(i);  // Boxing: int -> object
        }
        sw1.Stop();
        long addTime1 = sw1.ElapsedMilliseconds;

        sw1.Restart();
        long sum1 = 0;
        foreach (object obj in arrayList)
        {
            sum1 += (int)obj;  // Unboxing: object -> int
        }
        sw1.Stop();
        long iterateTime1 = sw1.ElapsedMilliseconds;

        sw1.Restart();
        int searchValue = iterations / 2;
        bool found1 = arrayList.Contains(searchValue);  // Boxing for comparison
        sw1.Stop();
        long searchTime1 = sw1.ElapsedMilliseconds;

        long memoryAfterArrayList = GC.GetTotalMemory(false);
        int gen0AfterArrayList = GC.CollectionCount(0);
        int gen1AfterArrayList = GC.CollectionCount(1);
        int gen2AfterArrayList = GC.CollectionCount(2);

        Console.WriteLine($"   Add:      {addTime1,6} ms");
        Console.WriteLine($"   Iterate:  {iterateTime1,6} ms");
        Console.WriteLine($"   Search:   {searchTime1,6} ms");
        Console.WriteLine($"   Sum:      {sum1:N0}");
        Console.WriteLine($"   Memory:   {(memoryAfterArrayList - memoryBefore) / 1024 / 1024:F2} MB");
        Console.WriteLine($"   GC Gen0:  {gen0AfterArrayList - gen0Before} collections");
        Console.WriteLine();

        // Clear and collect before next test
        arrayList.Clear();
        arrayList = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        memoryBefore = GC.GetTotalMemory(true);
        gen0Before = GC.CollectionCount(0);
        gen1Before = GC.CollectionCount(1);
        gen2Before = GC.CollectionCount(2);

        // Test 2: List<int> (without boxing)
        Console.WriteLine("2. List<int> (generic, WITHOUT boxing):");
        var sw2 = Stopwatch.StartNew();

        var genericList = new List<int>(iterations);
        for (int i = 0; i < iterations; i++)
        {
            genericList.Add(i);  // No boxing
        }
        sw2.Stop();
        long addTime2 = sw2.ElapsedMilliseconds;

        sw2.Restart();
        long sum2 = 0;
        foreach (int value in genericList)
        {
            sum2 += value;  // No unboxing
        }
        sw2.Stop();
        long iterateTime2 = sw2.ElapsedMilliseconds;

        sw2.Restart();
        bool found2 = genericList.Contains(searchValue);  // No boxing
        sw2.Stop();
        long searchTime2 = sw2.ElapsedMilliseconds;

        long memoryAfterList = GC.GetTotalMemory(false);
        int gen0AfterList = GC.CollectionCount(0);
        int gen1AfterList = GC.CollectionCount(1);
        int gen2AfterList = GC.CollectionCount(2);

        Console.WriteLine($"   Add:      {addTime2,6} ms");
        Console.WriteLine($"   Iterate:  {iterateTime2,6} ms");
        Console.WriteLine($"   Search:   {searchTime2,6} ms");
        Console.WriteLine($"   Sum:      {sum2:N0}");
        Console.WriteLine($"   Memory:   {(memoryAfterList - memoryBefore) / 1024 / 1024:F2} MB");
        Console.WriteLine($"   GC Gen0:  {gen0AfterList - gen0Before} collections");
        Console.WriteLine();

        // Calculate improvements
        Console.WriteLine("PERFORMANCE IMPROVEMENT:");
        Console.WriteLine($"   Add:      {(double)addTime1 / Math.Max(addTime2, 1):F1}x faster");
        Console.WriteLine($"   Iterate:  {(double)iterateTime1 / Math.Max(iterateTime2, 1):F1}x faster");
        Console.WriteLine($"   Search:   {(double)searchTime1 / Math.Max(searchTime2, 1):F1}x faster");
        Console.WriteLine();

        Console.WriteLine("MEMORY SAVINGS:");
        long arrayListMemory = memoryAfterArrayList - memoryBefore;
        long listMemory = memoryAfterList - memoryBefore;
        if (listMemory > 0)
        {
            Console.WriteLine($"   ArrayList uses {(double)arrayListMemory / listMemory:F1}x more memory");
        }
        Console.WriteLine($"   Saved: ~{(arrayListMemory - listMemory) / 1024 / 1024:F2} MB");
        Console.WriteLine();

        Console.WriteLine("GC IMPACT:");
        int gcDifference = (gen0AfterArrayList - gen0Before) - (gen0AfterList - gen0Before);
        Console.WriteLine($"   ArrayList caused {gcDifference} more Gen0 collections");
        Console.WriteLine($"   More GC = More pause time = Lower throughput");
        Console.WriteLine();
    }

    /// <summary>
    /// Compares Hashtable (with boxing) vs Dictionary&lt;int, string&gt; (without boxing) performance.
    /// </summary>
    public static void CompareHashtableVsDictionaryPerformance()
    {
        Console.WriteLine("=== Hashtable vs Dictionary<int, string> Performance ===");
        Console.WriteLine();

        const int iterations = 500_000;
        Console.WriteLine($"Operations: {iterations:N0} key-value pairs");
        Console.WriteLine();

        // Prepare test data
        var keys = new int[iterations];
        var values = new string[iterations];
        for (int i = 0; i < iterations; i++)
        {
            keys[i] = i;
            values[i] = $"Value{i}";
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long memoryBefore = GC.GetTotalMemory(true);
        int gen0Before = GC.CollectionCount(0);

        // Test 1: Hashtable (with boxing for keys)
        Console.WriteLine("1. Hashtable (non-generic, WITH boxing on keys):");
        var sw1 = Stopwatch.StartNew();

        var hashtable = new Hashtable(iterations);
        for (int i = 0; i < iterations; i++)
        {
            hashtable.Add(keys[i], values[i]);  // Boxing: int key -> object
        }
        sw1.Stop();
        long addTime1 = sw1.ElapsedMilliseconds;

        sw1.Restart();
        int count1 = 0;
        foreach (DictionaryEntry entry in hashtable)
        {
            count1++;
            var key = (int)entry.Key;  // Unboxing
            var value = (string)entry.Value;
        }
        sw1.Stop();
        long iterateTime1 = sw1.ElapsedMilliseconds;

        sw1.Restart();
        int searchKey = iterations / 2;
        bool found1 = hashtable.ContainsKey(searchKey);  // Boxing for lookup
        object? value1 = hashtable[searchKey];  // Boxing for lookup
        sw1.Stop();
        long lookupTime1 = sw1.ElapsedMilliseconds;

        long memoryAfterHashtable = GC.GetTotalMemory(false);
        int gen0AfterHashtable = GC.CollectionCount(0);

        Console.WriteLine($"   Add:      {addTime1,6} ms");
        Console.WriteLine($"   Iterate:  {iterateTime1,6} ms");
        Console.WriteLine($"   Lookup:   {lookupTime1,6} ms");
        Console.WriteLine($"   Count:    {count1:N0}");
        Console.WriteLine($"   Memory:   {(memoryAfterHashtable - memoryBefore) / 1024 / 1024:F2} MB");
        Console.WriteLine($"   GC Gen0:  {gen0AfterHashtable - gen0Before} collections");
        Console.WriteLine();

        // Clear for next test
        hashtable.Clear();
        hashtable = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        memoryBefore = GC.GetTotalMemory(true);
        gen0Before = GC.CollectionCount(0);

        // Test 2: Dictionary<int, string> (without boxing)
        Console.WriteLine("2. Dictionary<int, string> (generic, WITHOUT boxing):");
        var sw2 = Stopwatch.StartNew();

        var dictionary = new Dictionary<int, string>(iterations);
        for (int i = 0; i < iterations; i++)
        {
            dictionary.Add(keys[i], values[i]);  // No boxing
        }
        sw2.Stop();
        long addTime2 = sw2.ElapsedMilliseconds;

        sw2.Restart();
        int count2 = 0;
        foreach (var kvp in dictionary)
        {
            count2++;
            var key = kvp.Key;  // No unboxing
            var value = kvp.Value;
        }
        sw2.Stop();
        long iterateTime2 = sw2.ElapsedMilliseconds;

        sw2.Restart();
        bool found2 = dictionary.ContainsKey(searchKey);  // No boxing
        string? value2 = dictionary[searchKey];  // No boxing
        sw2.Stop();
        long lookupTime2 = sw2.ElapsedMilliseconds;

        long memoryAfterDictionary = GC.GetTotalMemory(false);
        int gen0AfterDictionary = GC.CollectionCount(0);

        Console.WriteLine($"   Add:      {addTime2,6} ms");
        Console.WriteLine($"   Iterate:  {iterateTime2,6} ms");
        Console.WriteLine($"   Lookup:   {lookupTime2,6} ms");
        Console.WriteLine($"   Count:    {count2:N0}");
        Console.WriteLine($"   Memory:   {(memoryAfterDictionary - memoryBefore) / 1024 / 1024:F2} MB");
        Console.WriteLine($"   GC Gen0:  {gen0AfterDictionary - gen0Before} collections");
        Console.WriteLine();

        // Calculate improvements
        Console.WriteLine("PERFORMANCE IMPROVEMENT:");
        Console.WriteLine($"   Add:      {(double)addTime1 / Math.Max(addTime2, 1):F1}x faster");
        Console.WriteLine($"   Iterate:  {(double)iterateTime1 / Math.Max(iterateTime2, 1):F1}x faster");
        Console.WriteLine($"   Lookup:   {(double)lookupTime1 / Math.Max(lookupTime2, 1):F1}x faster");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates the performance impact of boxing in tight loops.
    /// Shows how boxing in hot paths dramatically affects performance.
    /// </summary>
    public static void DemonstrateHotPathBoxing()
    {
        Console.WriteLine("=== Hot Path Boxing Impact ===");
        Console.WriteLine();
        Console.WriteLine("Simulating tight loop with calculations (hot path)");
        Console.WriteLine();

        const int iterations = 10_000_000;

        // Scenario 1: Boxing in hot path (BAD)
        Console.WriteLine("1. WITH Boxing in hot path:");
        var sw1 = Stopwatch.StartNew();
        ArrayList list1 = new ArrayList();
        for (int i = 0; i < iterations; i++)
        {
            int value = i * 2;
            list1.Add(value);  // Boxing in every iteration!
            if (list1.Count > 1000)
            {
                list1.RemoveAt(0);  // Keep memory reasonable
            }
        }
        sw1.Stop();
        Console.WriteLine($"   Time: {sw1.ElapsedMilliseconds,6} ms");
        Console.WriteLine($"   Operations per second: {iterations / (sw1.ElapsedMilliseconds / 1000.0):N0}");
        Console.WriteLine();

        // Scenario 2: No boxing in hot path (GOOD)
        Console.WriteLine("2. WITHOUT Boxing in hot path:");
        var sw2 = Stopwatch.StartNew();
        List<int> list2 = new List<int>();
        for (int i = 0; i < iterations; i++)
        {
            int value = i * 2;
            list2.Add(value);  // No boxing
            if (list2.Count > 1000)
            {
                list2.RemoveAt(0);  // Keep memory reasonable
            }
        }
        sw2.Stop();
        Console.WriteLine($"   Time: {sw2.ElapsedMilliseconds,6} ms");
        Console.WriteLine($"   Operations per second: {iterations / (sw2.ElapsedMilliseconds / 1000.0):N0}");
        Console.WriteLine();

        Console.WriteLine("RESULT:");
        double speedup = (double)sw1.ElapsedMilliseconds / sw2.ElapsedMilliseconds;
        Console.WriteLine($"   Generic version is {speedup:F1}x FASTER");
        Console.WriteLine($"   Time saved: {sw1.ElapsedMilliseconds - sw2.ElapsedMilliseconds} ms");
        Console.WriteLine();

        Console.WriteLine("LESSON:");
        Console.WriteLine("   Boxing in hot paths (tight loops) has devastating performance impact");
        Console.WriteLine("   Always profile your hot paths and eliminate boxing");
        Console.WriteLine();
    }

    /// <summary>
    /// Compares struct boxing performance with different scenarios.
    /// </summary>
    public static void CompareStructBoxingScenarios()
    {
        Console.WriteLine("=== Struct Boxing Performance ===");
        Console.WriteLine();

        const int iterations = 1_000_000;
        var point = new Point3D(10, 20, 30);

        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Scenario 1: Boxing struct to object
        Console.WriteLine("1. Boxing struct to object:");
        var sw1 = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            object boxed = point;  // Boxing
            var unboxed = (Point3D)boxed;  // Unboxing
        }
        sw1.Stop();
        Console.WriteLine($"   Time: {sw1.ElapsedMilliseconds,6} ms");
        Console.WriteLine();

        // Scenario 2: Boxing struct to interface
        Console.WriteLine("2. Boxing struct to interface:");
        var sw2 = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            IFormattable formattable = point;  // Boxing
            string str = formattable.ToString("", null);
        }
        sw2.Stop();
        Console.WriteLine($"   Time: {sw2.ElapsedMilliseconds,6} ms");
        Console.WriteLine();

        // Scenario 3: No boxing - direct struct usage
        Console.WriteLine("3. No boxing - direct struct usage:");
        var sw3 = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            Point3D copy = point;  // Just copy, no boxing
            string str = copy.ToString();  // No boxing (overridden method)
        }
        sw3.Stop();
        Console.WriteLine($"   Time: {sw3.ElapsedMilliseconds,6} ms");
        Console.WriteLine();

        // Scenario 4: Generic list (no boxing)
        Console.WriteLine("4. Adding to List<Point3D> (no boxing):");
        var sw4 = Stopwatch.StartNew();
        List<Point3D> points = new List<Point3D>(iterations);
        for (int i = 0; i < iterations; i++)
        {
            points.Add(point);  // No boxing
        }
        sw4.Stop();
        Console.WriteLine($"   Time: {sw4.ElapsedMilliseconds,6} ms");
        Console.WriteLine();

        // Scenario 5: Non-generic list (boxing)
        Console.WriteLine("5. Adding to ArrayList (boxing):");
        var sw5 = Stopwatch.StartNew();
        ArrayList arrayList = new ArrayList(iterations);
        for (int i = 0; i < iterations; i++)
        {
            arrayList.Add(point);  // Boxing
        }
        sw5.Stop();
        Console.WriteLine($"   Time: {sw5.ElapsedMilliseconds,6} ms");
        Console.WriteLine();

        Console.WriteLine("COMPARISON:");
        Console.WriteLine($"   ArrayList is {(double)sw5.ElapsedMilliseconds / sw4.ElapsedMilliseconds:F1}x slower than List<T>");
        Console.WriteLine();
    }

    /// <summary>
    /// Runs all performance comparison tests.
    /// </summary>
    public static void RunAll()
    {
        CompareArrayListVsListPerformance();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        CompareHashtableVsDictionaryPerformance();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        DemonstrateHotPathBoxing();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        CompareStructBoxingScenarios();
    }
}

/// <summary>
/// Sample struct for demonstrating boxing scenarios.
/// </summary>
public struct Point3D : IFormattable
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public Point3D(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public override string ToString() => $"({X}, {Y}, {Z})";

    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();
}
