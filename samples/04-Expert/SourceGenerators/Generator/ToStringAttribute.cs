using System;

namespace SourceGenerators;

/// <summary>
/// Marker attribute to trigger ToString() generation.
/// Roslyn source generator will detect this attribute and auto-generate ToString() method.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GenerateToStringAttribute : Attribute
{
    /// <summary>
    /// When true, includes property names in the output.
    /// Example: "Person { Name = John, Age = 30 }"
    /// </summary>
    public bool IncludePropertyNames { get; set; } = true;

    /// <summary>
    /// When true, includes private fields in the output.
    /// </summary>
    public bool IncludePrivateFields { get; set; } = false;
}
