using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AdvancedConcepts.Analyzers.Performance;

/// <summary>
/// Analyzer that detects missing ConfigureAwait(false) calls in library code.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ConfigureAwaitAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AC1002";

    private static readonly LocalizableString Title = "Missing ConfigureAwait(false) in library code";
    private static readonly LocalizableString MessageFormat = "Add ConfigureAwait(false) to avoid deadlocks in library code";
    private static readonly LocalizableString Description = "Library code should use ConfigureAwait(false) to avoid capturing the synchronization context and prevent potential deadlocks.";
    private const string Category = "Performance";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: Description,
        helpLinkUri: $"https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/docs/analyzers/{DiagnosticId}.md");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeAwaitExpression, SyntaxKind.AwaitExpression);
    }

    private static void AnalyzeAwaitExpression(SyntaxNodeAnalysisContext context)
    {
        var awaitExpression = (AwaitExpressionSyntax)context.Node;

        // Check if already has ConfigureAwait
        if (HasConfigureAwait(awaitExpression))
        {
            return;
        }

        // Check if the awaited expression is a Task
        var typeInfo = context.SemanticModel.GetTypeInfo(awaitExpression.Expression);
        if (typeInfo.Type == null)
        {
            return;
        }

        var typeName = typeInfo.Type.ToDisplayString();
        if (!typeName.StartsWith("System.Threading.Tasks.Task"))
        {
            return;
        }

        // Report diagnostic
        var diagnostic = Diagnostic.Create(Rule, awaitExpression.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }

    private static bool HasConfigureAwait(AwaitExpressionSyntax awaitExpression)
    {
        // Check if expression is an invocation ending with ConfigureAwait
        if (awaitExpression.Expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Name.Identifier.Text == "ConfigureAwait")
        {
            return true;
        }

        return false;
    }
}
