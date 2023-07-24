namespace ContractFirst.Api.Primary.Entities.Bases;

/// <summary>
///     多对多关系表请基于本接口
/// </summary>
public interface IRelationEntity : IRelationEntity<Guid, Guid>
{
}

/// <summary>
///     多对多关系表请基于本接口
/// </summary>
public interface IRelationEntity<T1, T2> : IEntityPrimary
{
    T1 LeftId { get; set; }
    T2 RightId { get; set; }
}