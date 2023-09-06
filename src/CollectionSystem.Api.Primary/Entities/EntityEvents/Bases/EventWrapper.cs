namespace CollectionSystem.Api.Primary.Entities.EntityEvents.Bases;

public class EventWrapper
{
    /// <summary>
    ///     事件的类型
    /// </summary>
    public Type EventType { get; set; }

    /// <summary>
    ///     事件执行的时机
    /// </summary>
    public EventTimingEnum EventTiming { get; set; }
}

/// <summary>
///     事件执行的时机
/// </summary>
public enum EventTimingEnum
{
    /// <summary>
    ///     实体新增时
    /// </summary>
    EntityAdded,

    /// <summary>
    ///     实体更新时
    /// </summary>
    EntityUpdated,

    /// <summary>
    ///     实体删除时
    /// </summary>
    EntityDeleted,

    /// <summary>
    ///     实体的属性更新时
    /// </summary>
    EntityPropertyUpdated
}