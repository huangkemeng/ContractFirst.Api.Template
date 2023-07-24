using System.ComponentModel;
using System.Reflection;

namespace CollectionSystem.Api.Realization.Bases;

public class BusinessException : Exception
{
    public BusinessException(string msg,
        BusinessExceptionTypeEnum exceptionType = BusinessExceptionTypeEnum.NotSpecified) : base(
        GetFullExceptionMessage(msg))
    {
        Type = exceptionType;
    }

    public BusinessException(IEnumerable<string> msg,
        BusinessExceptionTypeEnum exceptionType = BusinessExceptionTypeEnum.NotSpecified) : base(
        GetFullExceptionMessage(msg.ToArray()))
    {
        Type = exceptionType;
    }

    public BusinessExceptionTypeEnum Type { get; set; }

    public string TypeName => GetTypeName(Type);


    private static string GetTypeName(BusinessExceptionTypeEnum type)
    {
        var businessExceptionTypeStateType = typeof(BusinessExceptionTypeEnum);
        var businessExceptionTypeStateTypeField = businessExceptionTypeStateType.GetField(type.ToString())!;
        var descriptionAttr = businessExceptionTypeStateTypeField.GetCustomAttribute(typeof(DescriptionAttribute));
        if (descriptionAttr is DescriptionAttribute description) return description.Description;
        return type.ToString();
    }

    private static string GetFullExceptionMessage(params string[] msg)
    {
        return string.Join(";", msg);
    }
}

public enum BusinessExceptionTypeEnum
{
    [Description("")] NotSpecified,

    /// <summary>
    ///     参数有误
    /// </summary>
    [Description("参数有误")] Validator,

    /// <summary>
    ///     配置有误
    /// </summary>
    [Description("配置有误")] Configuration,

    /// <summary>
    ///     身份验证
    /// </summary>
    [Description("身份异常")] UnauthorizedIdentity,

    /// <summary>
    ///     权限不足
    /// </summary>
    [Description("权限不足")] Forbidden,
}