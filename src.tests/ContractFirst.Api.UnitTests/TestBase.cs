﻿using Autofac;
using ContractFirst.Api.Engines.Bases;
using Mediator.Net;

namespace ContractFirst.Api.UnitTests;

public class TestBase : IClassFixture<TestBase>, IDisposable
{
    public TestBase()
    {
        Build();
    }

    protected void Build(Action<ContainerBuilder>? builderAction = null)
    {
        if (TestEnvironmentCache.LifetimeScope == null)
        {
            var containerBuilder = new ContainerBuilder();
            TestEnvironmentCache.LifetimeScope = containerBuilder.TestBuildWithEngines();
        }

        TestLifetimeScope = builderAction != null
            ? TestEnvironmentCache.LifetimeScope.BeginLifetimeScope(builderAction)
            : TestEnvironmentCache.LifetimeScope;
        TestMediator = TestLifetimeScope.Resolve<IMediator>();
    }

    public ILifetimeScope TestLifetimeScope { get; private set; }

    protected IMediator TestMediator { get; private set; }

    public void Dispose()
    {
        TestLifetimeScope = null!;
        TestMediator = null!;
    }
}