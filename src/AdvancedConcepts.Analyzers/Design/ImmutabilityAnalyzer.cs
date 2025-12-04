using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AdvancedConcepts.Analyzers.Design;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ImmutabilityAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AC007";

    private static readonly LocalizableString Title = "Immutability violation detected";
    private static readonly LocalizableString MessageFormat = "{0}";
    private static readonly LocalizableString Description = "Detects violations of immutability principles.";
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

        context.RegisterSyntaxNodeAction(AnalyzeRecordDeclaration, SyntaxKind.RecordDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeRecordDeclaration(SyntaxNodeAnalysisContext context)
    {
        var recordDeclaration = (RecordDeclarationSyntax)context.Node;

        // Check for mutable properties in records
        foreach (var member in recordDeclaration.Members.OfType<PropertyDeclarationSyntax>())
        {
            if (HasSetter(member) && !IsInitOnlySetter(member))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    member.GetLocation(),
                    "Record property has mutable setter. Use 'init' or make property read-only.");
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        // Check if class has attributes indicating it should be immutable
        var hasImmutableAttribute = classDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .Any(attr => attr.Name.ToString().Contains("Immutable"));

        if (!hasImmutableAttribute)
            return;

        // Verify all properties are read-only or init-only
        foreach (var member in classDeclaration.Members.OfType<PropertyDeclarationSyntax>())
        {
            if (HasSetter(member) && !IsInitOnlySetter(member))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    member.GetLocation(),
                    $"Class marked as [Immutable] has mutable property '{member.Identifier.Text}'. Make property read-only or init-only.");
                context.ReportDiagnostic(diagnostic);
            }
        }

        // Check for mutable fields
        foreach (var member in classDeclaration.Members.OfType<FieldDeclarationSyntax>())
        {
            if (!member.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    member.GetLocation(),
                    "Class marked as [Immutable] has mutable field. Make field readonly.");
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private static bool HasSetter(PropertyDeclarationSyntax property)
    {
        return property.AccessorList?.Accessors
            .Any(a => a.IsKind(SyntaxKind.SetAccessorDeclaration) ||
                     a.IsKind(SyntaxKind.InitAccessorDeclaration)) ?? false;
    }

    private static bool IsInitOnlySetter(PropertyDeclarationSyntax property)
    {
        return property.AccessorList?.Accessors
            .Any(a => a.IsKind(SyntaxKind.InitAccessorDeclaration)) ?? false;
    }
}
