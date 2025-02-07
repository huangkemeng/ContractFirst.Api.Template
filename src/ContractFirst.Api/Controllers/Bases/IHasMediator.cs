using Mediator.Net;

namespace ContractFirst.Api.Controllers.Bases;

public interface IHasMediator
{
    IMediator Mediator { get; set; }
}