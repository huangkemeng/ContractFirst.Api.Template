﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ContractFirst.Api.CodeAnalyzer.ContractAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ContractNamingAnalyzer : DiagnosticAnalyzer
{
    public const string ContractNamingDiagnosticId = "ContractNamingAnalyzer";

    private static readonly DiagnosticDescriptor ContractNamingRule = new(ContractNamingDiagnosticId,
        "ContractNamingAnalyzer", "继承IContract<T>的接口必须以Contract结尾", "Naming", DiagnosticSeverity.Error, true);

    private readonly Regex contractMatchRegex = new(@"\.Bases.IContract\<[0-9a-zA-Z_\.]+\>$");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(ContractNamingRule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(AnalysisContractInterfaceName, SyntaxKind.InterfaceDeclaration);
    }

    private void AnalysisContractInterfaceName(SyntaxNodeAnalysisContext context)
    {
        var model = context.SemanticModel;
        if (context.Node is InterfaceDeclarationSyntax interfaceDeclaration)
        {
            var baseList = interfaceDeclaration.BaseList;
            if (baseList != null)
                if (baseList.Types.Any(e =>
                        HasIContractType(new[] { model.GetTypeInfo(e.Type).Type }) &&
                        !interfaceDeclaration.Identifier.ValueText.EndsWith("Contract")))
                {
                    var diagnostic =
                        Diagnostic.Create(ContractNamingRule, interfaceDeclaration.Identifier.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
        }
    }

    private bool HasIContractType(IEnumerable<ISymbol> symbols)
    {
        var symbolArray = symbols.ToArray();
        if (symbolArray is { Length: > 0 })
        {
            foreach (var symbol in symbolArray)
            {
                if (symbol is ITypeSymbol typeSymbol)
                {
                    var match = contractMatchRegex.Match(typeSymbol.ToDisplayString());
                    if (match.Success) return true;
                    return HasIContractType(typeSymbol.Interfaces);
                }
            }
        }
        return false;
    }
}