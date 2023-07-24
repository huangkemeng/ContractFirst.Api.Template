using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContractFirst.Api.CodeGenerator;

[Generator]
public class BusinessDbContextGenerator : ISourceGenerator
{
    private readonly Regex entityMatchRegex = new(@"\.Bases.IEntityPrimary$");

    public async void Execute(GeneratorExecutionContext context)
    {
        StringBuilder stringBuilder = new();
        List<string> namespaces = new();
        foreach (var tree in context.Compilation.SyntaxTrees)
        {
            var root = await tree.GetRootAsync(context.CancellationToken);
            var model = context.Compilation.GetSemanticModel(tree);
            var classNodes = root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>();
            var ns = root.DescendantNodesAndSelf().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
            foreach (var classNode in classNodes)
            {
                var baseTypeList = classNode.BaseList;
                if (baseTypeList != null)
                    if (baseTypeList.Types.Any(e => IsEfCoreEntityType(new[] { model.GetTypeInfo(e.Type).Type })))
                    {
                        stringBuilder.Append(
                            $"     public virtual DbSet<{classNode.Identifier.ValueText}> {classNode.Identifier.ValueText}s {{ get; set; }}\n");
                        if (ns != null && !namespaces.Contains(ns.Name.ToString())) namespaces.Add(ns.Name.ToString());
                    }
            }
        }

        var finalSource = $@"
using Microsoft.EntityFrameworkCore;
{string.Join("\n", namespaces.Select(n => $"using {n};"))}
namespace {(namespaces.Any() ? $"{namespaces.First()}" : "BusinessDbContextNamespace")}
{{
    public class BusinessDbContext : DbContext
    {{
{stringBuilder}
    }}
}}
";
        context.AddSource("BusinessDbContextDbSet.cs", finalSource);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }

    private bool IsEfCoreEntityType(IEnumerable<ISymbol> symbols)
    {
        var symbolArray = symbols?.ToArray();
        if (symbolArray is { Length: > 0 })
        {
            foreach (var symbol in symbolArray)
                if (symbol is ITypeSymbol typeSymbol)
                {
                    if (entityMatchRegex.Match(typeSymbol.ToDisplayString()).Success) return true;
                    return IsEfCoreEntityType(typeSymbol.Interfaces);
                }
        }

        return false;
    }
}