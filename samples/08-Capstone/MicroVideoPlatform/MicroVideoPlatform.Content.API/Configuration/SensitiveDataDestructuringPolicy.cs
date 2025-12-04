using Serilog.Core;
using Serilog.Events;
using System.Text.RegularExpressions;

namespace MicroVideoPlatform.Content.API.Configuration;

/// <summary>
/// Serilog destructuring policy that masks sensitive data in logs.
/// Prevents accidental logging of passwords, tokens, credit cards, etc.
/// </summary>
public class SensitiveDataDestructuringPolicy : IDestructuringPolicy
{
    // Property names that should be masked (case-insensitive)
    private static readonly HashSet<string> SensitivePropertyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "password",
        "pwd",
        "secret",
        "token",
        "apikey",
        "api_key",
        "authorization",
        "auth",
        "creditcard",
        "credit_card",
        "cvv",
        "ssn",
        "social_security",
        "privatekey",
        "private_key"
    };

    // Regex patterns for sensitive data
    private static readonly Regex CreditCardPattern = new(@"\b\d{4}[\s-]?\d{4}[\s-]?\d{4}[\s-]?\d{4}\b", RegexOptions.Compiled);
    private static readonly Regex SsnPattern = new(@"\b\d{3}-\d{2}-\d{4}\b", RegexOptions.Compiled);
    private static readonly Regex EmailPattern = new(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", RegexOptions.Compiled);

    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue? result)
    {
        result = null;

        // Only process objects and dictionaries
        if (value == null)
            return false;

        var type = value.GetType();

        // Handle strings - check for sensitive patterns
        if (type == typeof(string))
        {
            var stringValue = (string)value;
            if (ContainsSensitiveData(stringValue))
            {
                result = new ScalarValue(MaskString(stringValue));
                return true;
            }
            return false;
        }

        // Handle objects with properties
        if (!type.IsPrimitive && type != typeof(string))
        {
            var properties = type.GetProperties()
                .Where(p => p.CanRead)
                .Select(p =>
                {
                    var propValue = p.GetValue(value);
                    var propName = p.Name;

                    // Mask sensitive properties
                    if (SensitivePropertyNames.Contains(propName))
                    {
                        return new LogEventProperty(propName, new ScalarValue("***MASKED***"));
                    }

                    // Recursively process nested objects
                    if (propValue != null && propValue is string stringPropValue && ContainsSensitiveData(stringPropValue))
                    {
                        return new LogEventProperty(propName, new ScalarValue(MaskString(stringPropValue)));
                    }

                    return new LogEventProperty(propName, propertyValueFactory.CreatePropertyValue(propValue, destructureObjects: true));
                })
                .ToList();

            result = new StructureValue(properties);
            return true;
        }

        return false;
    }

    private static bool ContainsSensitiveData(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        // Check for credit card numbers
        if (CreditCardPattern.IsMatch(value))
            return true;

        // Check for SSN
        if (SsnPattern.IsMatch(value))
            return true;

        // Check if it looks like a token (long alphanumeric string)
        if (value.Length > 30 && value.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
            return true;

        return false;
    }

    private static string MaskString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        // Mask credit cards
        value = CreditCardPattern.Replace(value, "****-****-****-****");

        // Mask SSN
        value = SsnPattern.Replace(value, "***-**-****");

        // Mask emails (keep domain)
        value = EmailPattern.Replace(value, m =>
        {
            var parts = m.Value.Split('@');
            if (parts.Length == 2)
            {
                var localPart = parts[0];
                var domain = parts[1];
                var maskedLocal = localPart.Length > 2
                    ? localPart[0] + new string('*', localPart.Length - 2) + localPart[^1]
                    : new string('*', localPart.Length);
                return $"{maskedLocal}@{domain}";
            }
            return m.Value;
        });

        // If it looks like a token, show only first/last 4 chars
        if (value.Length > 30 && value.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
        {
            return $"{value[..4]}...{value[^4..]}";
        }

        return value;
    }
}

/// <summary>
/// Extension methods for adding sensitive data masking to Serilog
/// </summary>
public static class SerilogSensitiveDataExtensions
{
    public static LoggerConfiguration MaskSensitiveData(this LoggerConfiguration loggerConfiguration)
    {
        return loggerConfiguration.Destructure.With<SensitiveDataDestructuringPolicy>();
    }
}
