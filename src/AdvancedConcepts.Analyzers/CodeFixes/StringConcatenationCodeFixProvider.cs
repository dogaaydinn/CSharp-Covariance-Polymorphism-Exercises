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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringConcatenationCodeFixProvider)), Shared]
public class StringConcatenationCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create("AC001"); // StringConcatenationAnalyzer

    public sealed override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null)
            return;

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var binaryExpression = root.FindToken(diagnosticSpan.Start)
            .Parent?
            .AncestorsAndSelf()
            .OfType<BinaryExpressionSyntax>()
            .First();

        if (binaryExpression == null)
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Use string interpolation",
                createChangedDocument: c => UseStringInterpolationAsync(context.Document, binaryExpression, c),
                equivalenceKey: "UseStringInterpolation"),
            diagnostic);
    }

    private static async Task<Document> UseStringInterpolationAsync(
        Document document,
        BinaryExpressionSyntax binaryExpression,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null)
            return document;

        // Collect all parts of the concatenation
        var parts = CollectConcatenationParts(binaryExpression);

        // Build interpolated string
        var interpolations = parts.Select(part =>
        {
            if (part is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
            {
                var text = literal.Token.ValueText;
                return (InterpolatedStringContentSyntax)SyntaxFactory.InterpolatedStringText(
                    SyntaxFactory.Token(
                        SyntaxTriviaList.Empty,
                        SyntaxKind.InterpolatedStringTextToken,
                        text,
                        text,
                        SyntaxTriviaList.Empty));
            }
            else
            {
                return (InterpolatedStringContentSyntax)SyntaxFactory.Interpolation(part);
            }
        });

        var interpolatedString = SyntaxFactory.InterpolatedStringExpression(
            SyntaxFactory.Token(SyntaxKind.InterpolatedStringStartToken),
            SyntaxFactory.List(interpolations));

        var newRoot = root.ReplaceNode(binaryExpression, interpolatedString);
        return document.WithSyntaxRoot(newRoot);
    }

    private static List<ExpressionSyntax> CollectConcatenationParts(BinaryExpressionSyntax expression)
    {
        var parts = new List<ExpressionSyntax>();

        void CollectParts(ExpressionSyntax expr)
        {
            if (expr is BinaryExpressionSyntax binary && binary.IsKind(SyntaxKind.AddExpression))
            {
                CollectParts(binary.Left);
                CollectParts(binary.Right);
            }
            else
            {
                parts.Add(expr);
            }
        }

        CollectParts(expression);
        return parts;
    }
}
