using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AdvancedConcepts.Analyzers.Design;

/// <summary>
/// Analyzer that detects classes that violate Single Responsibility Principle by having too many responsibilities.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ClassComplexityAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AC2001";

    private const int MaxMethodCount = 15;
    private const int MaxFieldCount = 10;

    private static readonly LocalizableString Title = "Class has too many responsibilities";
    private static readonly LocalizableString MessageFormat = "Class '{0}' has {1} methods and {2} fields - consider splitting into smaller classes following Single Responsibility Principle";
    private static readonly LocalizableString Description = "Classes with too many methods and fields likely have multiple responsibilities and violate the Single Responsibility Principle.";
    private const string Category = "Design";

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

        context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        // Skip test classes, generated code, and small classes
        if (IsTestClass(classDeclaration) || IsPartialClass(classDeclaration))
            return;

        // Count methods (excluding properties, constructors)
        var methodCount = classDeclaration.Members
            .OfType<MethodDeclarationSyntax>()
            .Count(m => !m.Modifiers.Any(SyntaxKind.OverrideKeyword));

        // Count fields
        var fieldCount = classDeclaration.Members
            .OfType<FieldDeclarationSyntax>()
            .Sum(f => f.Declaration.Variables.Count);

        // Report if exceeds thresholds
        if (methodCount > MaxMethodCount || fieldCount > MaxFieldCount)
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                classDeclaration.Identifier.GetLocation(),
                classDeclaration.Identifier.Text,
                methodCount,
                fieldCount);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool IsTestClass(ClassDeclarationSyntax classDeclaration)
    {
        // Check if class has test attributes or ends with "Tests"
        var className = classDeclaration.Identifier.Text;
        if (className.EndsWith("Tests") || className.EndsWith("Test"))
            return true;

        // Check for common test attributes
        foreach (var attributeList in classDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var name = attribute.Name.ToString();
                if (name.Contains("Test") || name.Contains("Fact"))
                    return true;
            }
        }

        return false;
    }

    private static bool IsPartialClass(ClassDeclarationSyntax classDeclaration)
    {
        return classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword);
    }
}
