namespace ContractFirst.Api.Primary.Bases;

public interface ICurrentMany<T> : ICurrent<T>
{
    Task<List<T>> ToListAsync(CancellationToken cancellationToken = default);
}