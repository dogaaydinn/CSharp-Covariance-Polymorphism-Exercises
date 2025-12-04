using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AdvancedConcepts.Analyzers.Design;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SolidViolationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AC008";

    private static readonly LocalizableString Title = "SOLID principle violation detected";
    private static readonly LocalizableString MessageFormat = "{0}";
    private static readonly LocalizableString Description = "Detects violations of SOLID principles.";
    private const string Category = "Design";

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

        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeInterfaceDeclaration, SyntaxKind.InterfaceDeclaration);
    }

    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        // Single Responsibility Principle: Check for God classes
        CheckSingleResponsibility(context, classDeclaration);

        // Open/Closed Principle: Check for switch statements on types
        CheckOpenClosed(context, classDeclaration);

        // Dependency Inversion: Check for concrete dependencies
        CheckDependencyInversion(context, classDeclaration);
    }

    private static void CheckSingleResponsibility(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclaration)
    {
        var methods = classDeclaration.Members.OfType<MethodDeclarationSyntax>().ToList();
        var properties = classDeclaration.Members.OfType<PropertyDeclarationSyntax>().ToList();

        // Simple heuristic: Too many members suggests multiple responsibilities
        var totalMembers = methods.Count + properties.Count;

        if (totalMembers > 20)
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                classDeclaration.Identifier.GetLocation(),
                $"Class '{classDeclaration.Identifier.Text}' has {totalMembers} members. " +
                "Consider splitting into smaller classes (Single Responsibility Principle).");
            context.ReportDiagnostic(diagnostic);
        }

        // Check for mixed concerns (e.g., database + business logic)
        var hasDataAccess = methods.Any(m => m.Identifier.Text.Contains("Query") ||
                                             m.Identifier.Text.Contains("Execute") ||
                                             m.Identifier.Text.Contains("Save"));

        var hasBusinessLogic = methods.Any(m => m.Identifier.Text.Contains("Calculate") ||
                                                m.Identifier.Text.Contains("Validate") ||
                                                m.Identifier.Text.Contains("Process"));

        if (hasDataAccess && hasBusinessLogic)
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                classDeclaration.Identifier.GetLocation(),
                $"Class '{classDeclaration.Identifier.Text}' mixes data access with business logic. " +
                "Violates Single Responsibility Principle.");
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static void CheckOpenClosed(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclaration)
    {
        var switchStatements = classDeclaration.DescendantNodes()
            .OfType<SwitchStatementSyntax>()
            .ToList();

        foreach (var switchStatement in switchStatements)
        {
            // Check if switch is on a type pattern (potential OCP violation)
            if (switchStatement.Expression is BinaryExpressionSyntax ||
                switchStatement.Sections.Any(s => s.Labels.OfType<CasePatternSwitchLabelSyntax>().Any()))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    switchStatement.GetLocation(),
                    "Type-based switch statement detected. Consider using polymorphism instead (Open/Closed Principle).");
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private static void CheckDependencyInversion(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclaration)
    {
        var constructor = classDeclaration.Members
            .OfType<ConstructorDeclarationSyntax>()
            .FirstOrDefault();

        if (constructor == null)
            return;

        // Check for 'new' keyword in constructor (creating concrete dependencies)
        var objectCreations = constructor.DescendantNodes()
            .OfType<ObjectCreationExpressionSyntax>()
            .ToList();

        foreach (var creation in objectCreations)
        {
            var typeInfo = context.SemanticModel.GetTypeInfo(creation);
            var type = typeInfo.Type;

            if (type != null && type.TypeKind == TypeKind.Class && !type.IsAbstract)
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    creation.GetLocation(),
                    $"Constructor creates concrete dependency '{type.Name}'. " +
                    "Inject dependencies through constructor instead (Dependency Inversion Principle).");
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private static void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
    {
        var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;

        // Interface Segregation Principle: Check for fat interfaces
        var methods = interfaceDeclaration.Members.OfType<MethodDeclarationSyntax>().ToList();
        var properties = interfaceDeclaration.Members.OfType<PropertyDeclarationSyntax>().ToList();

        var totalMembers = methods.Count + properties.Count;

        if (totalMembers > 10)
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                interfaceDeclaration.Identifier.GetLocation(),
                $"Interface '{interfaceDeclaration.Identifier.Text}' has {totalMembers} members. " +
                "Consider splitting into smaller, more focused interfaces (Interface Segregation Principle).");
            context.ReportDiagnostic(diagnostic);
        }
    }
}
