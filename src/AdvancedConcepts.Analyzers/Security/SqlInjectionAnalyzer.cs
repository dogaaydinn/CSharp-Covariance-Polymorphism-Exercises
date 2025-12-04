using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AdvancedConcepts.Analyzers.Security;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SqlInjectionAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AC009";

    private static readonly LocalizableString Title = "Potential SQL injection vulnerability";
    private static readonly LocalizableString MessageFormat = "{0}";
    private static readonly LocalizableString Description = "Detects potential SQL injection vulnerabilities from string concatenation.";
    private const string Category = "Security";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeBinaryExpression, SyntaxKind.AddExpression);
        context.RegisterSyntaxNodeAction(AnalyzeInterpolatedString, SyntaxKind.InterpolatedStringExpression);
    }

    private static void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
    {
        var binaryExpression = (BinaryExpressionSyntax)context.Node;

        // Check if this is string concatenation
        var leftType = context.SemanticModel.GetTypeInfo(binaryExpression.Left).Type;
        if (leftType?.SpecialType != SpecialType.System_String)
            return;

        // Check if left side contains SQL keywords
        var leftText = binaryExpression.Left.ToString();
        if (!ContainsSqlKeywords(leftText))
            return;

        // Check if right side is a variable (potential user input)
        if (binaryExpression.Right is IdentifierNameSyntax ||
            binaryExpression.Right is MemberAccessExpressionSyntax)
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                binaryExpression.GetLocation(),
                "Potential SQL injection vulnerability: SQL query built with string concatenation. " +
                "Use parameterized queries instead.");
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static void AnalyzeInterpolatedString(SyntaxNodeAnalysisContext context)
    {
        var interpolatedString = (InterpolatedStringExpressionSyntax)context.Node;

        // Check if the string contains SQL keywords
        var stringText = interpolatedString.ToString();
        if (!ContainsSqlKeywords(stringText))
            return;

        // Check if there are interpolations (user input)
        if (interpolatedString.Contents.OfType<InterpolationSyntax>().Any())
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                interpolatedString.GetLocation(),
                "Potential SQL injection vulnerability: SQL query built with string interpolation. " +
                "Use parameterized queries or ORM instead.");
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool ContainsSqlKeywords(string text)
    {
        var sqlKeywords = new[]
        {
            "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "CREATE",
            "ALTER", "EXEC", "EXECUTE", "FROM", "WHERE", "JOIN"
        };

        var upperText = text.ToUpperInvariant();
        return sqlKeywords.Any(keyword => upperText.Contains(keyword));
    }
}
