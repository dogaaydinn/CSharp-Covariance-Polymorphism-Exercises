using System;

namespace AdvancedConcepts.SourceGenerators;

/// <summary>
/// Generates compile-time validation code for classes.
/// </summary>
/// <example>
/// <code>
/// [Validate]
/// public class CreateUserRequest
/// {
///     [Required]
///     [StringLength(100)]
///     public string Name { get; set; }
///
///     [Required]
///     [EmailAddress]
///     public string Email { get; set; }
///
///     [Range(18, 120)]
///     public int Age { get; set; }
/// }
///
/// // Generated code creates:
/// // - public ValidationResult Validate()
/// // - public bool IsValid()
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class ValidateAttribute : Attribute
{
    /// <summary>
    /// If true, generates validation method. Default is true.
    /// </summary>
    public bool GenerateValidationMethod { get; set; } = true;

    /// <summary>
    /// If true, throws exception on validation failure. Default is false (returns validation result).
    /// </summary>
    public bool ThrowOnValidationFailure { get; set; } = false;
}

/// <summary>
/// Marks a property as required.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class RequiredAttribute : Attribute
{
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Specifies string length constraints.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class StringLengthAttribute : Attribute
{
    public int MaximumLength { get; }
    public int MinimumLength { get; set; }
    public string? ErrorMessage { get; set; }

    public StringLengthAttribute(int maximumLength)
    {
        MaximumLength = maximumLength;
    }
}

/// <summary>
/// Validates that a string is a valid email address.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class EmailAddressAttribute : Attribute
{
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Specifies numeric range constraints.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class RangeAttribute : Attribute
{
    public object Minimum { get; }
    public object Maximum { get; }
    public string? ErrorMessage { get; set; }

    public RangeAttribute(int minimum, int maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    public RangeAttribute(double minimum, double maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
    }
}

/// <summary>
/// Validates using a regular expression pattern.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class RegularExpressionAttribute : Attribute
{
    public string Pattern { get; }
    public string? ErrorMessage { get; set; }

    public RegularExpressionAttribute(string pattern)
    {
        Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
    }
}
