namespace ContractFirst.Api.Primary.Entities.Bases;

/// <summary>
///     主表请基于本接口
/// </summary>
public interface IMainEntity : IUpdatableEntity
{
    Guid? CreatedBy { get; set; }
}