namespace ContractFirst.Api.Primary.Bases;

public interface ICombineQuery<T>
{
    Task<T?> QueryAsync(T model, CancellationToken cancellationToken);
}