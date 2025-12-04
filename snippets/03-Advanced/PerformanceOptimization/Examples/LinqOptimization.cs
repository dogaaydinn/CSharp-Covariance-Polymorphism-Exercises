using System;
using System.Collections.Generic;
using System.Linq;

namespace PerformanceOptimization.Examples;

/// <summary>
/// Demonstrates LINQ performance pitfalls and optimization techniques
/// </summary>
public static class LinqOptimization
{
    /// <summary>
    /// Example 1: LINQ vs for loop performance
    /// </summary>
    public static void LinqVsForLoop()
    {
        Console.WriteLine("\n=== LINQ vs For Loop ===");
        
        int[] numbers = Enumerable.Range(1, 1000).ToArray();
        
        // LINQ - elegant but with overhead
        var linqSum = numbers.Where(n => n % 2 == 0).Sum();
        Console.WriteLine($"LINQ sum of evens: {linqSum}");
        
        // For loop - faster
        int forSum = 0;
        for (int i = 0; i < numbers.Length; i++)
        {
            if (numbers[i] % 2 == 0)
                forSum += numbers[i];
        }
        Console.WriteLine($"For loop sum of evens: {forSum}");
        
        Console.WriteLine("\nRule:");
        Console.WriteLine("  ✓ LINQ: Readable, safe, good for most cases");
        Console.WriteLine("  ✓ For loop: Hot paths, performance-critical code");
        Console.WriteLine("  ✗ LINQ: Nested loops, very large datasets");
    }

    /// <summary>
    /// Example 2: Multiple enumeration - silent performance killer
    /// </summary>
    public static void MultipleEnumeration()
    {
        Console.WriteLine("\n=== Multiple Enumeration Problem ===");
        
        // ❌ Bad: Query is re-executed each time
        IEnumerable<int> query = GetExpensiveQuery();
        
        Console.WriteLine($"Count: {query.Count()}");      // Executes query
        Console.WriteLine($"First: {query.First()}");      // Executes query AGAIN
        Console.WriteLine($"Last: {query.Last()}");        // Executes query AGAIN
        Console.WriteLine("❌ Query executed 3 times!");
        
        // ✅ Good: Materialize once
        List<int> materialized = GetExpensiveQuery().ToList();
        Console.WriteLine($"\nCount: {materialized.Count}");   // From list
        Console.WriteLine($"First: {materialized.First()}");    // From list
        Console.WriteLine($"Last: {materialized.Last()}");      // From list
        Console.WriteLine("✅ Query executed once, cached in list");
        
        Console.WriteLine("\nRule: If using result multiple times, call .ToList() or .ToArray()");
    }

    private static IEnumerable<int> GetExpensiveQuery()
    {
        Console.Write("  [Executing expensive query...] ");
        for (int i = 1; i <= 10; i++)
        {
            yield return i;
        }
    }

    /// <summary>
    /// Example 3: Any() vs Count() - common mistake
    /// </summary>
    public static void AnyVsCount()
    {
        Console.WriteLine("\n=== Any() vs Count() ===");
        
        List<int> numbers = Enumerable.Range(1, 1000000).ToList();
        
        // ❌ Bad: Count() iterates entire sequence
        bool hasItems1 = numbers.Count() > 0;
        Console.WriteLine($"Count() > 0: {hasItems1} (checked all 1M items)");
        
        // ✅ Good: Any() stops at first item
        bool hasItems2 = numbers.Any();
        Console.WriteLine($"Any(): {hasItems2} (stopped after 1 item)");
        
        // ❌ Bad: Count entire sequence just to check threshold
        bool moreThan100_Bad = numbers.Count() > 100;
        
        // ✅ Good: Skip + Any
        bool moreThan100_Good = numbers.Skip(100).Any();
        Console.WriteLine($"More than 100 items: {moreThan100_Good}");
        
        Console.WriteLine("\nRules:");
        Console.WriteLine("  ✓ Any(): Check if collection has items");
        Console.WriteLine("  ✓ Count: When you need exact count AND it's cached");
        Console.WriteLine("  ✓ Skip(n).Any(): Check if more than N items");
    }

    /// <summary>
    /// Example 4: FirstOrDefault vs SingleOrDefault
    /// </summary>
    public static void FirstVsSingle()
    {
        Console.WriteLine("\n=== FirstOrDefault vs SingleOrDefault ===");
        
        var users = new[]
        {
            new { Id = 1, Name = "Alice" },
            new { Id = 2, Name = "Bob" },
            new { Id = 3, Name = "Charlie" }
        };
        
        // FirstOrDefault - stops at first match (fast)
        var user1 = users.FirstOrDefault(u => u.Id > 0);
        Console.WriteLine($"FirstOrDefault: {user1?.Name} (stopped after 1 check)");
        
        // SingleOrDefault - checks entire sequence to ensure only one match (slow)
        var user2 = users.SingleOrDefault(u => u.Id == 2);
        Console.WriteLine($"SingleOrDefault: {user2?.Name} (checked all 3 items)");
        
        Console.WriteLine("\nRules:");
        Console.WriteLine("  ✓ FirstOrDefault(): Most cases, want first match");
        Console.WriteLine("  ✓ SingleOrDefault(): Ensure uniqueness (but slower)");
        Console.WriteLine("  ✗ Single(): Throws if 0 or >1 matches");
    }

    /// <summary>
    /// Example 5: Where + Select vs Select + Where
    /// </summary>
    public static void QueryOrdering()
    {
        Console.WriteLine("\n=== Query Ordering Optimization ===");
        
        var numbers = Enumerable.Range(1, 100).ToArray();
        
        // ❌ Less efficient: Select (100 calls) then Where
        var result1 = numbers
            .Select(n => n * n)           // 100 multiplications
            .Where(n => n > 50)           // 100 comparisons
            .ToList();
        Console.WriteLine($"Select then Where: {result1.Count} items");
        
        // ✅ More efficient: Where (100 calls) then Select (fewer calls)
        var result2 = numbers
            .Where(n => n * n > 50)       // 100 comparisons
            .Select(n => n * n)           // Only ~93 multiplications
            .ToList();
        Console.WriteLine($"Where then Select: {result2.Count} items (same result, fewer operations)");
        
        Console.WriteLine("\nRule: Filter early, transform late (Where before Select)");
    }

    /// <summary>
    /// Example 6: ToList vs ToArray - which to use?
    /// </summary>
    public static void ToListVsToArray()
    {
        Console.WriteLine("\n=== ToList() vs ToArray() ===");
        
        var query = Enumerable.Range(1, 100);
        
        // ToList - grows dynamically
        List<int> list = query.ToList();
        Console.WriteLine($"ToList: {list.Count} items (can add more)");
        list.Add(101);
        Console.WriteLine($"After add: {list.Count} items");
        
        // ToArray - fixed size, slightly faster
        int[] array = query.ToArray();
        Console.WriteLine($"ToArray: {array.Length} items (fixed size)");
        
        Console.WriteLine("\nWhen to use:");
        Console.WriteLine("  ToList(): Need to modify collection");
        Console.WriteLine("  ToArray(): Fixed data, slight perf advantage, indexing");
        Console.WriteLine("  Neither: If you only enumerate once (use IEnumerable)");
    }

    /// <summary>
    /// Example 7: Avoid LINQ in tight loops
    /// </summary>
    public static void AvoidLinqInLoops()
    {
        Console.WriteLine("\n=== LINQ in Loops - Performance Killer ===");
        
        var items = Enumerable.Range(1, 1000).ToList();
        
        // ❌ Very bad: LINQ in loop
        Console.WriteLine("❌ Bad approach:");
        for (int i = 0; i < 10; i++)
        {
            var _ = items.Max();  // Scans entire list every iteration!
        }
        Console.WriteLine("  Called Max() 10 times = 10,000 comparisons");
        
        // ✅ Good: Calculate once
        Console.WriteLine("\n✅ Good approach:");
        var max = items.Max();
        for (int i = 0; i < 10; i++)
        {
            // Use cached max
            var result = max * i;
        }
        Console.WriteLine("  Called Max() 1 time = 1,000 comparisons");
        
        Console.WriteLine("\nRule: Hoist invariant LINQ queries outside loops");
    }

    /// <summary>
    /// Example 8: Deferred execution gotchas
    /// </summary>
    public static void DeferredExecution()
    {
        Console.WriteLine("\n=== Deferred Execution ===");
        
        var numbers = new List<int> { 1, 2, 3, 4, 5 };
        
        // Query defined but not executed
        var query = numbers.Where(n => n > 2);
        Console.WriteLine("Query defined");
        
        // Modify source
        numbers.Add(6);
        numbers.Add(7);
        Console.WriteLine("Added 6 and 7 to source");
        
        // Now execute query - sees modified data
        var result = query.ToList();
        Console.WriteLine($"Query result: [{string.Join(", ", result)}]");
        Console.WriteLine("Query saw 6 and 7 because execution was deferred!");
        
        Console.WriteLine("\nDeferred operators: Where, Select, OrderBy, GroupBy, etc.");
        Console.WriteLine("Immediate operators: ToList, ToArray, Count, First, Any, etc.");
    }

    /// <summary>
    /// Example 9: HashSet for Contains checks
    /// </summary>
    public static void HashSetForContains()
    {
        Console.WriteLine("\n=== HashSet for Fast Lookups ===");
        
        var list = Enumerable.Range(1, 10000).ToList();
        var hashSet = Enumerable.Range(1, 10000).ToHashSet();
        
        // ❌ Bad: List.Contains is O(n)
        bool containsList = list.Contains(9999);  // Checks up to 9,999 items
        Console.WriteLine($"List.Contains(9999): {containsList} (slow - O(n))");
        
        // ✅ Good: HashSet.Contains is O(1)
        bool containsHashSet = hashSet.Contains(9999);  // Checks hash table
        Console.WriteLine($"HashSet.Contains(9999): {containsHashSet} (fast - O(1))");
        
        Console.WriteLine("\nRule: For frequent Contains checks, use HashSet");
        Console.WriteLine("  List.Contains: O(n) - linear scan");
        Console.WriteLine("  HashSet.Contains: O(1) - hash lookup");
    }

    /// <summary>
    /// Example 10: Real-world optimization
    /// </summary>
    public static void RealWorldOptimization()
    {
        Console.WriteLine("\n=== Real-World Example: User Processing ===");
        
        var users = GetUsers();
        
        // ❌ Bad: Multiple passes, multiple enumerations
        var activeCount = users.Count(u => u.IsActive);
        var adminCount = users.Count(u => u.IsAdmin);
        var avgAge = users.Where(u => u.IsActive).Average(u => u.Age);
        
        Console.WriteLine("❌ Bad approach: Enumerated collection 3 times");
        
        // ✅ Good: Single pass with aggregate
        var stats = users.Aggregate(
            new { ActiveCount = 0, AdminCount = 0, TotalAge = 0 },
            (acc, user) => new
            {
                ActiveCount = acc.ActiveCount + (user.IsActive ? 1 : 0),
                AdminCount = acc.AdminCount + (user.IsAdmin ? 1 : 0),
                TotalAge = acc.TotalAge + (user.IsActive ? user.Age : 0)
            });
        
        var avgAge2 = stats.ActiveCount > 0 ? (double)stats.TotalAge / stats.ActiveCount : 0;
        
        Console.WriteLine($"✅ Good approach: Single pass");
        Console.WriteLine($"  Active users: {stats.ActiveCount}");
        Console.WriteLine($"  Admin users: {stats.AdminCount}");
        Console.WriteLine($"  Average age of active users: {avgAge2:F1}");
    }

    private static List<User> GetUsers()
    {
        return new List<User>
        {
            new User { Id = 1, Name = "Alice", Age = 30, IsActive = true, IsAdmin = false },
            new User { Id = 2, Name = "Bob", Age = 25, IsActive = true, IsAdmin = true },
            new User { Id = 3, Name = "Charlie", Age = 35, IsActive = false, IsAdmin = false },
            new User { Id = 4, Name = "Diana", Age = 28, IsActive = true, IsAdmin = false },
            new User { Id = 5, Name = "Eve", Age = 32, IsActive = true, IsAdmin = true }
        };
    }

    private class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
    }
}
