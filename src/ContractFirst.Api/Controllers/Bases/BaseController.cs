using ContractFirst.Api.Engines.Bases;
using Mediator.Net;
using Microsoft.AspNetCore.Mvc;

namespace ContractFirst.Api.Controllers.Bases;

[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    [AutoResolve] public IMediator Mediator { get; set; }
}