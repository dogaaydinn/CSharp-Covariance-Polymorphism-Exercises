using System.Threading.Tasks;

namespace RoslynAnalyzerDemo.Consumer;

/// <summary>
/// This file contains intentional violations to demonstrate the analyzer.
/// Build the project to see ASYNC001 warnings.
/// Use the code fix (Ctrl+. or Cmd+.) to automatically add "Async" suffix.
/// </summary>
public class BadExample
{
    // ❌ ASYNC001: Method 'FetchUser' returns Task but doesn't end with 'Async'
    public async Task FetchUser()
    {
        await Task.Delay(100);
    }

    // ❌ ASYNC001: Method 'GetData' returns Task<string> but doesn't end with 'Async'
    public async Task<string> GetData()
    {
        await Task.Delay(100);
        return "data";
    }

    // ❌ ASYNC001: Method 'SaveRecord' returns ValueTask but doesn't end with 'Async'
    public async ValueTask SaveRecord()
    {
        await Task.Delay(100);
    }

    // ❌ ASYNC001: Method 'LoadConfig' returns ValueTask<T> but doesn't end with 'Async'
    public async ValueTask<bool> LoadConfig()
    {
        await Task.Delay(100);
        return true;
    }
}
