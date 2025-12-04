using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AdvancedConcepts.Analyzers.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConfigureAwaitCodeFixProvider)), Shared]
public class ConfigureAwaitCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create("AC003"); // ConfigureAwaitAnalyzer

    public sealed override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null)
            return;

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var awaitExpression = root.FindToken(diagnosticSpan.Start)
            .Parent?
            .AncestorsAndSelf()
            .OfType<AwaitExpressionSyntax>()
            .First();

        if (awaitExpression == null)
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Add ConfigureAwait(false)",
                createChangedDocument: c => AddConfigureAwaitAsync(context.Document, awaitExpression, c),
                equivalenceKey: "AddConfigureAwait"),
            diagnostic);
    }

    private static async Task<Document> AddConfigureAwaitAsync(
        Document document,
        AwaitExpressionSyntax awaitExpression,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null)
            return document;

        // Create ConfigureAwait(false) invocation
        var configureAwaitInvocation = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                awaitExpression.Expression,
                SyntaxFactory.IdentifierName("ConfigureAwait")),
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Argument(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.FalseLiteralExpression)))));

        // Replace the await expression
        var newAwaitExpression = awaitExpression.WithExpression(configureAwaitInvocation);
        var newRoot = root.ReplaceNode(awaitExpression, newAwaitExpression);

        return document.WithSyntaxRoot(newRoot);
    }
}
