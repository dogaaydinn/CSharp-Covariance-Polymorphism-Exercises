using System.Threading.Tasks;

namespace RoslynAnalyzerDemo.Consumer;

/// <summary>
/// Examples of properly named async methods.
/// No analyzer warnings will be generated for this file.
/// </summary>
public class GoodExample
{
    // ✅ CORRECT: Returns Task and ends with "Async"
    public async Task FetchUserAsync()
    {
        await Task.Delay(100);
    }

    // ✅ CORRECT: Returns Task<T> and ends with "Async"
    public async Task<string> GetDataAsync()
    {
        await Task.Delay(100);
        return "data";
    }

    // ✅ CORRECT: Returns ValueTask and ends with "Async"
    public async ValueTask SaveRecordAsync()
    {
        await Task.Delay(100);
    }

    // ✅ CORRECT: Returns ValueTask<T> and ends with "Async"
    public async ValueTask<bool> LoadConfigAsync()
    {
        await Task.Delay(100);
        return true;
    }

    // ✅ CORRECT: Synchronous method, no "Async" suffix needed
    public void ProcessData()
    {
        // Synchronous processing
    }

    // ✅ CORRECT: Returns int (not Task), no "Async" suffix needed
    public int Calculate()
    {
        return 42;
    }
}
