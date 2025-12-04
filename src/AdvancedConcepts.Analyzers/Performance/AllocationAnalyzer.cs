using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace AdvancedConcepts.Analyzers.Performance;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AllocationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AC005";

    private static readonly LocalizableString Title = "Unnecessary allocation detected";
    private static readonly LocalizableString MessageFormat = "{0}";
    private static readonly LocalizableString Description = "Detects unnecessary heap allocations that can be avoided.";
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

        context.RegisterSyntaxNodeAction(AnalyzeStringConcatenation, SyntaxKind.AddExpression);
        context.RegisterSyntaxNodeAction(AnalyzeEmptyArrayAllocation, SyntaxKind.ArrayCreationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeLinqToArray, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeStringConcatenation(SyntaxNodeAnalysisContext context)
    {
        var addExpression = (BinaryExpressionSyntax)context.Node;

        // Check if it's a string concatenation in a loop or multiple times
        if (!IsStringConcatenation(addExpression, context.SemanticModel))
            return;

        // Check if inside a loop
        if (IsInsideLoop(addExpression))
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                addExpression.GetLocation(),
                "String concatenation in loop causes multiple allocations. Use StringBuilder instead.");
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static void AnalyzeEmptyArrayAllocation(SyntaxNodeAnalysisContext context)
    {
        var arrayCreation = (ArrayCreationExpressionSyntax)context.Node;

        // Check for new T[0] or new T[] { }
        if (IsEmptyArrayCreation(arrayCreation))
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                arrayCreation.GetLocation(),
                "Empty array allocation detected. Use Array.Empty<T>() instead.");
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static void AnalyzeLinqToArray(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Name.Identifier.Text == "ToArray")
        {
            // Check if result is immediately enumerated
            var parent = invocation.Parent;
            if (parent is ForEachStatementSyntax)
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    invocation.GetLocation(),
                    "Unnecessary ToArray() before foreach. Remove ToArray() to avoid allocation.");
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private static bool IsStringConcatenation(BinaryExpressionSyntax expression, SemanticModel semanticModel)
    {
        var leftType = semanticModel.GetTypeInfo(expression.Left).Type;
        var rightType = semanticModel.GetTypeInfo(expression.Right).Type;

        return leftType?.SpecialType == SpecialType.System_String ||
               rightType?.SpecialType == SpecialType.System_String;
    }

    private static bool IsInsideLoop(SyntaxNode node)
    {
        var current = node.Parent;
        while (current != null)
        {
            if (current is ForStatementSyntax ||
                current is WhileStatementSyntax ||
                current is DoStatementSyntax ||
                current is ForEachStatementSyntax)
            {
                return true;
            }
            current = current.Parent;
        }
        return false;
    }

    private static bool IsEmptyArrayCreation(ArrayCreationExpressionSyntax arrayCreation)
    {
        // Check for new T[0]
        if (arrayCreation.Type.RankSpecifiers.Any())
        {
            var firstRankSpecifier = arrayCreation.Type.RankSpecifiers[0];
            if (firstRankSpecifier.Sizes.Count == 1 &&
                firstRankSpecifier.Sizes[0] is LiteralExpressionSyntax literal &&
                literal.Token.ValueText == "0")
            {
                return true;
            }
        }

        // Check for new T[] { }
        if (arrayCreation.Initializer != null &&
            !arrayCreation.Initializer.Expressions.Any())
        {
            return true;
        }

        return false;
    }
}
