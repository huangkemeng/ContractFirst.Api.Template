using CollectionSystem.Api.Engines.SwaggerEngines;
using Microsoft.AspNetCore.Mvc;

namespace CollectionSystem.Api.Controllers.Bases;

[Route("api/app/[controller]")]
[Route("api/web/[controller]")]
[SwaggerApiGroup(true)]
public class AllBaseController : BaseController
{
}