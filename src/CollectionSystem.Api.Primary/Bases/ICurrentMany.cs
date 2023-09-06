namespace CollectionSystem.Api.Primary.Bases;

public interface ICurrentMany<T> : ICurrent<T>
{
    Task<List<T>> QueryManyAsync(CancellationToken cancellationToken = default);
}