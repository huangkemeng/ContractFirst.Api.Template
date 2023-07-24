namespace ContractFirst.Api.Primary.Entities.EntityEvents.Bases;

/// <summary>
///     同个请求有多个相同事件时  会运行多个
/// </summary>
public interface IMultipleRunEntityEvent : IEntityEvent
{
}