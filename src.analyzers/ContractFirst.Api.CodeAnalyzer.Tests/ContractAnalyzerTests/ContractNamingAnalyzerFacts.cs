using System.Threading;
using System.Threading.Tasks;
using ContractFirst.Api.CodeAnalyzer.ContractAnalyzers;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;

namespace ContractFirst.Api.CodeAnalyzer.Tests.ContractAnalyzerTests;

public class ContractNamingAnalyzerFacts : CSharpAnalyzerTest<ContractNamingAnalyzer, XUnitVerifier>
{
}

public class
    ContractNamingFixProviderFacts : CSharpCodeFixTest<ContractNamingAnalyzer, ContractNamingFixProvider, XUnitVerifier>
{
    [Fact]
    public async Task FixWithoutContractInterfaceName()
    {
        TestCode = "public interface IInterface : Test.Bases.IContract<string>{}";
        FixedCode = "public interface IInterfaceContract : Test.Bases.IContract<string>{}";
        BatchFixedCode = "public interface IInterfaceContract : Test.Bases.IContract<string>{}";
        NumberOfIncrementalIterations = 1;
        NumberOfFixAllIterations = 1;
        await VerifyFixAsync(TestState, FixedState, BatchFixedState, Verify, CancellationToken.None);
    }
}