using CollectionSystem.Api.Engines.SwaggerEngines;
using Microsoft.AspNetCore.Mvc;

namespace CollectionSystem.Api.Controllers.Bases;

[Route("api/app/[controller]")]
[SwaggerApiGroup(SwaggerApiGroupNames.App)]
public class AppBaseController : BaseController
{
}