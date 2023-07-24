using ContractFirst.Api.Primary.Contracts.Bases;
using Mediator.Net.Contracts;

namespace ContractFirst.Api.Primary.Contracts;

public interface ILoginContract : ICommandContract<LoginCommand,LoginResponse>
{
}

public class LoginCommand : ICommand
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }
}

public class LoginResponse : IResponse
{
    /// <summary>
    ///  访问凭证
    /// </summary>
    public string AccessToken { get; set; }
} 