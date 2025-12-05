using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace RoslynAnalyzerDemo;

/// <summary>
/// Code fix provider that automatically adds "Async" suffix to method names.
/// Renames all references to the method throughout the project.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AsyncNamingCodeFixProvider)), Shared]
public class AsyncNamingCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(AsyncNamingAnalyzer.DiagnosticId);

    public sealed override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
            .ConfigureAwait(false);

        if (root is null)
            return;

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the method declaration
        var methodDeclaration = root.FindToken(diagnosticSpan.Start)
            .Parent?
            .AncestorsAndSelf()
            .OfType<MethodDeclarationSyntax>()
            .First();

        if (methodDeclaration is null)
            return;

        // Register code action to add "Async" suffix
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Add 'Async' suffix",
                createChangedSolution: c => AddAsyncSuffixAsync(context.Document, methodDeclaration, c),
                equivalenceKey: nameof(AsyncNamingCodeFixProvider)),
            diagnostic);
    }

    private static async Task<Solution> AddAsyncSuffixAsync(
        Document document,
        MethodDeclarationSyntax methodDeclaration,
        CancellationToken cancellationToken)
    {
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken)
            .ConfigureAwait(false);

        if (semanticModel is null)
            return document.Project.Solution;

        var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

        if (methodSymbol is null)
            return document.Project.Solution;

        // New name with "Async" suffix
        var newName = methodSymbol.Name + "Async";

        // Rename symbol and all references
        var solution = document.Project.Solution;
        var optionSet = solution.Options;

        var newSolution = await Renamer.RenameSymbolAsync(
            solution,
            methodSymbol,
            new SymbolRenameOptions(),
            newName,
            cancellationToken).ConfigureAwait(false);

        return newSolution;
    }
}
