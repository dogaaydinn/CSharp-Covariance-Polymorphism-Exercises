using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace AdvancedConcepts.Analyzers.Performance;

/// <summary>
/// Analyzer that detects inefficient string concatenation in loops.
/// Suggests using StringBuilder for better performance.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class StringConcatenationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AC1001";

    private static readonly LocalizableString Title = "String concatenation in loop";
    private static readonly LocalizableString MessageFormat = "Consider using StringBuilder instead of string concatenation in loop for better performance";
    private static readonly LocalizableString Description = "String concatenation in loops creates multiple string objects, causing unnecessary allocations. Use StringBuilder for better performance.";
    private const string Category = "Performance";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: $"https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/docs/analyzers/{DiagnosticId}.md");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeAssignment, SyntaxKind.AddAssignmentExpression);
    }

    private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;

        // Check if left side is a string
        var leftSymbol = context.SemanticModel.GetSymbolInfo(assignment.Left).Symbol;
        if (leftSymbol is not ILocalSymbol localSymbol ||
            localSymbol.Type.SpecialType != SpecialType.System_String)
        {
            return;
        }

        // Check if we're inside a loop
        if (!IsInsideLoop(assignment))
        {
            return;
        }

        // Report diagnostic
        var diagnostic = Diagnostic.Create(Rule, assignment.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsInsideLoop(SyntaxNode node)
    {
        var current = node.Parent;
        while (current != null)
        {
            if (current is ForStatementSyntax ||
                current is ForEachStatementSyntax ||
                current is WhileStatementSyntax ||
                current is DoStatementSyntax)
            {
                return true;
            }

            current = current.Parent;
        }

        return false;
    }
}
