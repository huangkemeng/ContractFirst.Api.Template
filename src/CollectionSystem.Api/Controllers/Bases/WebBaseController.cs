using CollectionSystem.Api.Engines.SwaggerEngines;
using Microsoft.AspNetCore.Mvc;

namespace CollectionSystem.Api.Controllers.Bases;

[Route("api/web/[controller]")]
[SwaggerApiGroup(SwaggerApiGroupNames.Web)]
public class WebBaseController : BaseController
{
}