using Mediator.Net.Contracts;

namespace ContractFirst.Api.Primary.Bases;

public class BaseResponse<T> : IResponse
{
    public BaseResponse(T data)
    {
        Data = data;
    }
    
    public T Data { get; set; }
}