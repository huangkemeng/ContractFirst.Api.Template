using Autofac;
using ContractFirst.Api.Engines.Bases;

namespace ContractFirst.Api.UnitTests;

public class TestBase : IClassFixture<TestBase>, IDisposable
{
    public TestBase()
    {
        var builder = new ContainerBuilder();
        Container = builder.TestBuildWithEngines();
    }

    public IContainer Container { get; }

    public void Dispose()
    {
        Container.Dispose();
    }
}