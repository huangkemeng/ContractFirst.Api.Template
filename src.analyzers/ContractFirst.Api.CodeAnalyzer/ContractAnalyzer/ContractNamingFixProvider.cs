using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace ContractFirst.Api.CodeAnalyzer.ContractAnalyzer;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ContractNamingFixProvider))]
[Shared]
public class ContractNamingFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(ContractNamingAnalyzer.ContractNamingDiagnosticId);

    public override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var document = context.Document;
        var root = await document.GetSyntaxRootAsync(context.CancellationToken);
        foreach (var diagnostic in context.Diagnostics)
            if (diagnostic.Id == ContractNamingAnalyzer.ContractNamingDiagnosticId)
            {
                if (root != null)
                {
                    var token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "重命名为Contract结尾",
                            c => FixContractName(document, token, c),
                            ContractNamingAnalyzer.ContractNamingDiagnosticId),
                        diagnostic);
                }
            }
    }

    private static async Task<Document> FixContractName(Document document, SyntaxToken token,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        if (root != null)
        {
            root = root.ReplaceToken(token, SyntaxFactory.Identifier(token.ValueText + "Contract"));
            return document.WithSyntaxRoot(root);
        }

        return document;
    }
}