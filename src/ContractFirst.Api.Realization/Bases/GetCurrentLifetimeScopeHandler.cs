using Autofac;
using ContractFirst.Api.Primary.Contracts.Bases;
using Mediator.Net.Context;

namespace ContractFirst.Api.Realization.Bases;

public class GetCurrentLifetimeScopeHandler(ILifetimeScope lifetimeScope) : IGetCurrentLifetimeScopeContract
{
    public Task<GetCurrentLifetimeScopeResponse> Handle(IReceiveContext<GetCurrentLifetimeScopeRequest> context,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new GetCurrentLifetimeScopeResponse
        {
            LifetimeScope = lifetimeScope
        });
    }

    public void Validate(ContractValidator<GetCurrentLifetimeScopeRequest> validator)
    {
    }

    public void Test(TestContext<GetCurrentLifetimeScopeRequest, GetCurrentLifetimeScopeResponse> context)
    {
    }
}