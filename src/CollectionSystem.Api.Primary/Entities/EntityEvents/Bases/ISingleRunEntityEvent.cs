namespace CollectionSystem.Api.Primary.Entities.EntityEvents.Bases;

/// <summary>
///     同个请求有多个相同事件时  只运行一个
/// </summary>
public interface ISingleRunEntityEvent : IEntityEvent
{
}