using System;
using System.Threading.Tasks;

namespace RoslynAnalyzerDemo.Consumer;

/// <summary>
/// Demonstrates the AsyncNamingAnalyzer in action.
/// Try uncommenting the "bad" examples to see analyzer warnings.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Roslyn Analyzer Demo ===\n");

        // ✅ CORRECT: Async method with proper naming
        await FetchDataAsync();
        await ProcessItemsAsync();

        // ❌ INCORRECT: These will trigger analyzer warnings (uncomment to test)
        // await FetchData();      // Warning ASYNC001: Should end with "Async"
        // await ProcessItems();   // Warning ASYNC001: Should end with "Async"

        Console.WriteLine("\n✅ All async methods follow naming conventions!");
    }

    // ✅ GOOD: Returns Task, ends with "Async"
    static async Task FetchDataAsync()
    {
        Console.WriteLine("Fetching data...");
        await Task.Delay(100);
        Console.WriteLine("✅ Data fetched successfully");
    }

    // ✅ GOOD: Returns Task<T>, ends with "Async"
    static async Task<int> ProcessItemsAsync()
    {
        Console.WriteLine("Processing items...");
        await Task.Delay(100);
        Console.WriteLine("✅ Processed 42 items");
        return 42;
    }

    // ❌ BAD: Returns Task but doesn't end with "Async"
    // Uncomment to see analyzer warning:
    /*
    static async Task FetchData()
    {
        await Task.Delay(100);
    }
    */

    // ❌ BAD: Returns Task<T> but doesn't end with "Async"
    // Uncomment to see analyzer warning:
    /*
    static async Task<string> GetUserName()
    {
        await Task.Delay(100);
        return "John Doe";
    }
    */

    // ✅ GOOD: Synchronous method (no Task), no "Async" suffix required
    static void PrintMessage(string message)
    {
        Console.WriteLine(message);
    }
}
