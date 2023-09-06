namespace ContractFirst.Api.Primary.Bases;

/// <summary>
///     组合查询，当多个实体同属于一种类型的时候，
///     需要一次查询所有同种类型的数据，比如查询组织架构的时候，需要将用户、部门，公司等全部都需要查出来
///     那么就需要继承这个类去实现
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICombineQueryMany<T>
{
    Task<List<T>> QueryManyAsync(IEnumerable<T> model, CancellationToken cancellationToken);
}