# Async-Await

## Problem
Implement async methods for parallel data fetching.

## Example
```csharp
// Fetch 3 APIs in parallel
var task1 = FetchUserAsync(1);
var task2 = FetchPostsAsync(1);
var task3 = FetchCommentsAsync(1);

await Task.WhenAll(task1, task2, task3);
```

## Key Concepts
- async/await keywords
- Task<T> return type
- Task.WhenAll, Task.WhenAny
- ConfigureAwait(false)
- Error handling with try/catch

## Common Mistakes
- Blocking with .Result or .Wait()
- Not awaiting tasks
- Async void (should be async Task)
