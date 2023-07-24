using ContractFirst.Api.Engines.Bases;
using ContractFirst.Api.FilterAndMiddlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(options => { options.Filters.Add<GlobalExceptionFilter>(); });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
var app = builder.BuildWithEngines();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();