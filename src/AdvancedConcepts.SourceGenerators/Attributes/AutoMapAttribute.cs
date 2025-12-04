using System;

namespace AdvancedConcepts.SourceGenerators;

/// <summary>
/// Generates automatic mapping code between source and target types.
/// </summary>
/// <example>
/// <code>
/// [AutoMap(typeof(UserDto))]
/// public class User
/// {
///     public int Id { get; set; }
///     public string Name { get; set; }
///     public string Email { get; set; }
/// }
///
/// // Generated code will create:
/// // - public static UserDto ToUserDto(this User source)
/// // - public static User ToUser(this UserDto source)
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class AutoMapAttribute : Attribute
{
    /// <summary>
    /// The type to map to/from.
    /// </summary>
    public Type TargetType { get; }

    /// <summary>
    /// Optional: Custom method name prefix. Default is "To{TargetTypeName}".
    /// </summary>
    public string? MethodNamePrefix { get; set; }

    /// <summary>
    /// If true, generates reverse mapping as well. Default is true.
    /// </summary>
    public bool GenerateReverseMap { get; set; } = true;

    /// <summary>
    /// If true, ignores properties that don't exist in target. Default is false (throws exception).
    /// </summary>
    public bool IgnoreMissingProperties { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the AutoMapAttribute.
    /// </summary>
    /// <param name="targetType">The type to generate mapping code for.</param>
    public AutoMapAttribute(Type targetType)
    {
        TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
    }
}

/// <summary>
/// Excludes a property from automatic mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class AutoMapIgnoreAttribute : Attribute
{
}

/// <summary>
/// Maps a property to a different property name in the target type.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class AutoMapPropertyAttribute : Attribute
{
    /// <summary>
    /// The target property name.
    /// </summary>
    public string TargetPropertyName { get; }

    /// <summary>
    /// Initializes a new instance of the AutoMapPropertyAttribute.
    /// </summary>
    /// <param name="targetPropertyName">The name of the property in the target type.</param>
    public AutoMapPropertyAttribute(string targetPropertyName)
    {
        TargetPropertyName = targetPropertyName ?? throw new ArgumentNullException(nameof(targetPropertyName));
    }
}
