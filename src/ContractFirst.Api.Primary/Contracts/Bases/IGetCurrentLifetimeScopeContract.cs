﻿using Autofac;
using Mediator.Net.Contracts;

namespace ContractFirst.Api.Primary.Contracts.Bases;

public interface
    IGetCurrentLifetimeScopeContract : IRequestContract<GetCurrentLifetimeScopeRequest, GetCurrentLifetimeScopeResponse>
{
}

public class GetCurrentLifetimeScopeRequest : IRequest
{
}

public class GetCurrentLifetimeScopeResponse : IResponse
{
    public ILifetimeScope LifetimeScope { get; set; }
}