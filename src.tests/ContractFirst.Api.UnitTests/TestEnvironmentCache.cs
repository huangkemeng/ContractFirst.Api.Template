﻿using Autofac;

namespace ContractFirst.Api.UnitTests;

public static class TestEnvironmentCache
{
    public static ILifetimeScope? LifetimeScope { get; set; }
}