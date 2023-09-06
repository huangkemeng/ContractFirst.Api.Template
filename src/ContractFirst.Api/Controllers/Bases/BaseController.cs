using ContractFirst.Api.FilterAndMiddlewares;
using ContractFirst.Api.Middlewares;
using Mediator.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractFirst.Api.Controllers.Bases;

[ApiController]
[Authorize]
[TypeFilter(typeof(AutoResolveFilter))]
[TypeFilter(typeof(HandleTimezoneResultFilter))]
public class BaseController : ControllerBase
{
    [AutoResolve] public IMediator Mediator { get; set; }
}