﻿using ContractFirst.Api.Engines.SwaggerEngines;
using Microsoft.AspNetCore.Mvc;

namespace ContractFirst.Api.Controllers.Bases;

[Route("api/app/[controller]")]
[SwaggerApiGroup(SwaggerApiGroupNames.App)]
public class AppBaseController : BaseController
{
}