using ContractFirst.Api.Primary.Bases;

namespace ContractFirst.Api.Realization.Handlers.ReusableHandlers.Bases;

public static class ReusableExtensions
{
    public static async Task HandleAsync<TParameter>(this TParameter parameter,
        CancellationToken cancellationToken = default)
        where TParameter : IReusableHandlerParameter
    {
        if (CurrentApplication.TryContextResolve<IReusableHandler<TParameter>>(out var handler) && handler is not null)
        {
            await handler.Handle(parameter, cancellationToken);
            return;
        }

        throw new InvalidOperationException(
            $"Current reusable parameter {typeof(TParameter).Name} does not have a corresponding handler");
    }

    public static async Task<TReturn> HandleAsync<TParameter, TReturn>(this TParameter parameter,
        CancellationToken cancellationToken = default)
        where TParameter : IReusableHandlerParameter where TReturn : IReusableHandlerReturn
    {
        if (CurrentApplication.TryContextResolve<IReusableHandler<TParameter, TReturn>>(out var handler) &&
            handler is not null)
        {
            return await handler.Handle(parameter, cancellationToken);
        }

        throw new InvalidOperationException(
            $"Current reusable parameter '{typeof(TParameter).Name} + {typeof(TReturn).Name}' does not have a corresponding handler");
    }
}