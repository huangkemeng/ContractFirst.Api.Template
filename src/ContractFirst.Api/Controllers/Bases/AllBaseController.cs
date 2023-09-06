﻿using ContractFirst.Api.Engines.SwaggerEngines;
using Microsoft.AspNetCore.Mvc;

namespace ContractFirst.Api.Controllers.Bases;

[Route("api/app/[controller]")]
[Route("api/web/[controller]")]
[SwaggerApiGroup(true)]
public class AllBaseController : BaseController
{
}