using ContractFirst.Api.Engines.SwaggerEngines;
using Microsoft.AspNetCore.Mvc;

namespace ContractFirst.Api.Controllers.Bases;

[Route("api/web/[controller]")]
[SwaggerApiGroup(SwaggerApiGroupNames.Web)]
public class WebBaseController : BaseController
{
}