using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AdvancedConcepts.Analyzers.Performance;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AsyncAwaitAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AC006";

    private static readonly LocalizableString Title = "Async/await pattern issue detected";
    private static readonly LocalizableString MessageFormat = "{0}";
    private static readonly LocalizableString Description = "Detects incorrect or inefficient async/await patterns.";
    private const string Category = "Performance";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeAsyncMethod, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeAwaitExpression, SyntaxKind.AwaitExpression);
    }

    private static void AnalyzeAsyncMethod(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        if (!methodDeclaration.Modifiers.Any(SyntaxKind.AsyncKeyword))
            return;

        // Check if method has no await keyword
        var awaitExpressions = methodDeclaration.DescendantNodes()
            .OfType<AwaitExpressionSyntax>()
            .ToList();

        if (!awaitExpressions.Any())
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                methodDeclaration.Identifier.GetLocation(),
                "Async method has no await expressions. Remove async modifier or add await.");
            context.ReportDiagnostic(diagnostic);
        }

        // Check for async void (should be avoided except for event handlers)
        if (methodDeclaration.ReturnType is PredefinedTypeSyntax predefinedType &&
            predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword))
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                methodDeclaration.ReturnType.GetLocation(),
                "Async void methods should be avoided. Use async Task instead.");
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static void AnalyzeAwaitExpression(SyntaxNodeAnalysisContext context)
    {
        var awaitExpression = (AwaitExpressionSyntax)context.Node;

        // Check for await Task.Run(() => synchronous_method())
        if (awaitExpression.Expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            if (IsTaskRun(memberAccess, context.SemanticModel))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    awaitExpression.GetLocation(),
                    "Avoid using Task.Run for CPU-bound work in ASP.NET. It doesn't improve scalability.");
                context.ReportDiagnostic(diagnostic);
            }
        }

        // Check for sequential awaits that could be parallel
        CheckForSequentialAwaits(context, awaitExpression);
    }

    private static void CheckForSequentialAwaits(SyntaxNodeAnalysisContext context, AwaitExpressionSyntax awaitExpression)
    {
        var parent = awaitExpression.Parent;
        if (parent is not ExpressionStatementSyntax expressionStatement)
            return;

        var block = expressionStatement.Parent as BlockSyntax;
        if (block == null)
            return;

        var statements = block.Statements;
        var currentIndex = statements.IndexOf(expressionStatement);

        if (currentIndex < statements.Count - 1 &&
            statements[currentIndex + 1] is ExpressionStatementSyntax nextStatement &&
            nextStatement.Expression is AwaitExpressionSyntax)
        {
            // Check if the two await expressions are independent
            var diagnostic = Diagnostic.Create(
                Rule,
                awaitExpression.GetLocation(),
                "Sequential independent awaits detected. Consider using Task.WhenAll for parallel execution.");
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool IsTaskRun(MemberAccessExpressionSyntax memberAccess, SemanticModel semanticModel)
    {
        var symbolInfo = semanticModel.GetSymbolInfo(memberAccess);
        var method = symbolInfo.Symbol as IMethodSymbol;

        return method?.ContainingType?.Name == "Task" &&
               method?.Name == "Run";
    }
}
