using System.Text.Json.Serialization;

namespace NativeAOTExample;

/// <summary>
/// JSON serialization context for Native AOT compilation.
///
/// WHY: Native AOT cannot use reflection at runtime, so we use source generators
/// to generate serialization code at compile time.
///
/// PERFORMANCE BENEFITS:
/// - Zero reflection overhead
/// - Faster serialization (2-3x)
/// - Smaller binary size
/// - Predictable performance
///
/// Usage:
///   var json = JsonSerializer.Serialize(person, AppJsonContext.Default.Person);
/// </summary>
[JsonSerializable(typeof(Person))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(List<Person>))]
internal partial class AppJsonContext : JsonSerializerContext
{
    // Source generator will implement this at compile time
    // No reflection needed at runtime! ✅
}
