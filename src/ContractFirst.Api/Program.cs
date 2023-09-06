using ContractFirst.Api.Engines.Bases;

var builder = WebApplication.CreateBuilder(args);
var app = builder.BuildWithEngines();
app.Run();