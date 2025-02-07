using ContractFirst.Api.Engines.Bases;
using ContractFirst.Api.FilterAndMiddlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<AutoResolveFilter>();
var app = builder.BuildWithEngines();
app.Run();