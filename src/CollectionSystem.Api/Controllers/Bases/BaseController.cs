using CollectionSystem.Api.FilterAndMiddlewares;
using Mediator.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectionSystem.Api.Controllers.Bases;

[ApiController]
[Authorize]
[TypeFilter(typeof(AutoResolveFilter))]
[TypeFilter(typeof(HandleTimezoneResultFilter))]
public class BaseController : ControllerBase
{
    [AutoResolve] public IMediator Mediator { get; set; }
}