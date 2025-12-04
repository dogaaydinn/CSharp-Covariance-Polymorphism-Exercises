using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AdvancedConcepts.Analyzers.Performance;

/// <summary>
/// Analyzer that detects common LINQ performance issues like Count() vs Any() and multiple enumerations.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class LinqPerformanceAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticIdCountVsAny = "AC1003";
    public const string DiagnosticIdMultipleEnumeration = "AC1004";

    private static readonly DiagnosticDescriptor CountVsAnyRule = new DiagnosticDescriptor(
        DiagnosticIdCountVsAny,
        "Use Any() instead of Count() > 0",
        "Use Any() instead of Count() {0} 0 for better performance",
        "Performance",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Any() is more efficient than Count() when checking for existence because it stops after finding the first element.",
        helpLinkUri: $"https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/docs/analyzers/{DiagnosticIdCountVsAny}.md");

    private static readonly DiagnosticDescriptor MultipleEnumerationRule = new DiagnosticDescriptor(
        DiagnosticIdMultipleEnumeration,
        "Possible multiple enumeration of IEnumerable",
        "Enumerable '{0}' might be enumerated multiple times - consider calling ToList() or ToArray()",
        "Performance",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Enumerating an IEnumerable multiple times can cause performance issues and unexpected behavior if the source is a query.",
        helpLinkUri: $"https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/docs/analyzers/{DiagnosticIdMultipleEnumeration}.md");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(CountVsAnyRule, MultipleEnumerationRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeCountUsage, SyntaxKind.GreaterThanExpression, SyntaxKind.NotEqualsExpression);
        context.RegisterSyntaxNodeAction(AnalyzeMethodBody, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeCountUsage(SyntaxNodeAnalysisContext context)
    {
        var binaryExpression = (BinaryExpressionSyntax)context.Node;

        // Check for patterns like Count() > 0 or Count() != 0
        InvocationExpressionSyntax? countInvocation = null;
        LiteralExpressionSyntax? zeroLiteral = null;
        string operatorSymbol = "";

        if (binaryExpression.Left is InvocationExpressionSyntax leftInvocation &&
            binaryExpression.Right is LiteralExpressionSyntax rightLiteral)
        {
            countInvocation = leftInvocation;
            zeroLiteral = rightLiteral;
            operatorSymbol = binaryExpression.OperatorToken.Text;
        }
        else if (binaryExpression.Right is InvocationExpressionSyntax rightInvocation &&
                 binaryExpression.Left is LiteralExpressionSyntax leftLiteral)
        {
            countInvocation = rightInvocation;
            zeroLiteral = leftLiteral;
            operatorSymbol = ReverseOperator(binaryExpression.OperatorToken.Text);
        }

        if (countInvocation == null || zeroLiteral == null)
            return;

        // Check if it's Count() method
        if (countInvocation.Expression is not MemberAccessExpressionSyntax memberAccess ||
            memberAccess.Name.Identifier.Text != "Count")
            return;

        // Check if compared to zero
        if (zeroLiteral.Token.ValueText != "0")
            return;

        // Check if it's > or !=
        if (operatorSymbol != ">" && operatorSymbol != "!=")
            return;

        var diagnostic = Diagnostic.Create(CountVsAnyRule, binaryExpression.GetLocation(), operatorSymbol);
        context.ReportDiagnostic(diagnostic);
    }

    private static void AnalyzeMethodBody(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        // Find all local variables that are IEnumerable
        var enumerableVariables = methodDeclaration.DescendantNodes()
            .OfType<VariableDeclaratorSyntax>()
            .Where(v => IsEnumerableType(v, context.SemanticModel))
            .ToList();

        foreach (var variable in enumerableVariables)
        {
            var variableName = variable.Identifier.Text;

            // Count how many times it's enumerated
            var enumerationCount = CountEnumerations(methodDeclaration, variableName);

            if (enumerationCount > 1)
            {
                var diagnostic = Diagnostic.Create(
                    MultipleEnumerationRule,
                    variable.GetLocation(),
                    variableName);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private static bool IsEnumerableType(VariableDeclaratorSyntax variable, SemanticModel semanticModel)
    {
        var symbol = semanticModel.GetDeclaredSymbol(variable);
        if (symbol is not ILocalSymbol localSymbol)
            return false;

        var type = localSymbol.Type;
        var typeString = type.ToDisplayString();

        return typeString.Contains("IEnumerable") &&
               !typeString.Contains("IEnumerable<char>") && // String is IEnumerable<char>
               !type.TypeKind.HasFlag(TypeKind.Array);
    }

    private static int CountEnumerations(MethodDeclarationSyntax method, string variableName)
    {
        // Count foreach loops, LINQ operations, ToList/ToArray calls
        var count = 0;

        // Count foreach loops
        count += method.DescendantNodes()
            .OfType<ForEachStatementSyntax>()
            .Count(f => f.Expression is IdentifierNameSyntax id && id.Identifier.Text == variableName);

        // Count LINQ operations that enumerate (Where, Select, OrderBy, etc.)
        var enumeratingMethods = new[] { "Where", "Select", "OrderBy", "OrderByDescending", "ToList", "ToArray", "First", "FirstOrDefault", "Count", "Any" };

        count += method.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Count(invocation =>
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                    memberAccess.Expression is IdentifierNameSyntax id &&
                    id.Identifier.Text == variableName &&
                    enumeratingMethods.Contains(memberAccess.Name.Identifier.Text))
                {
                    return true;
                }
                return false;
            });

        return count;
    }

    private static string ReverseOperator(string op)
    {
        return op switch
        {
            ">" => "<",
            "<" => ">",
            ">=" => "<=",
            "<=" => ">=",
            _ => op
        };
    }
}
