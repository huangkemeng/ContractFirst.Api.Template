using ContractFirst.Api.Engines.Bases;

var builder = WebApplication.CreateBuilder(args);
var app = builder
    .BuildWithEngines(options =>
    {
        options.EnableSwagger = true;
        options.EnableFakerRealization = true;
        options.EnableAutoResolve = true;
        options.EnableEfCore = false;
        options.EnableGlobalExceptionFilter = false;
        options.EnableTimezoneHandler = false;
        options.EnableValidator = false;
        options.EnableCors = false;
        options.EnableJwt = false;
    });
app.UseHttpsRedirection();
app.MapControllers();
app.Run();