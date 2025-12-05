using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RoslynAnalyzerDemo;

/// <summary>
/// Diagnostic analyzer that enforces async method naming conventions.
/// Ensures methods returning Task or Task&lt;T&gt; end with "Async" suffix.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AsyncNamingAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "ASYNC001";
    private const string Category = "Naming";

    private static readonly LocalizableString Title =
        "Async method should end with 'Async' suffix";

    private static readonly LocalizableString MessageFormat =
        "Method '{0}' returns Task but doesn't end with 'Async'";

    private static readonly LocalizableString Description =
        "Methods returning Task or Task<T> should have names ending with 'Async' to indicate asynchronous behavior.";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Register callback for method declarations
        context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        // Get the method symbol
        if (context.SemanticModel.GetDeclaredSymbol(methodDeclaration) is not IMethodSymbol methodSymbol)
            return;

        // Skip if already ends with "Async"
        if (methodSymbol.Name.EndsWith("Async"))
            return;

        // Check if return type is Task or Task<T>
        var returnType = methodSymbol.ReturnType;

        if (IsTaskType(returnType))
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                methodDeclaration.Identifier.GetLocation(),
                methodSymbol.Name);

            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool IsTaskType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Void)
            return false;

        var typeName = typeSymbol.OriginalDefinition.ToDisplayString();

        return typeName == "System.Threading.Tasks.Task" ||
               typeName == "System.Threading.Tasks.Task<TResult>" ||
               typeName == "System.Threading.Tasks.ValueTask" ||
               typeName == "System.Threading.Tasks.ValueTask<TResult>";
    }
}
