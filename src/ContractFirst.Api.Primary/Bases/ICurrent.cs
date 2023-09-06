namespace ContractFirst.Api.Primary.Bases;

public interface ICurrent<TEntity>
{
    Task<TEntity?> QueryAsync(CancellationToken cancellationToken = default);
    Task<Guid?> GetCurrentUserIdAsync();
}