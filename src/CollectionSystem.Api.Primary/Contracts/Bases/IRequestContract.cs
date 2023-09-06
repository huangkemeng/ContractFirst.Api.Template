using Mediator.Net.Contracts;

namespace CollectionSystem.Api.Primary.Contracts.Bases;

public interface IRequestContract<TRequest, TResponse> : IContract<TRequest>, ITestable<TRequest, TResponse>,
    IRequestHandler<TRequest, TResponse> where TRequest : class, IRequest where TResponse : class, IResponse
{
}