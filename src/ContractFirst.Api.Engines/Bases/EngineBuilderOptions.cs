namespace ContractFirst.Api.Engines.Bases;

public class EngineBuilderOptions
{
    public bool EnableJwt { get; set; }
    public bool EnableSwagger { get; set; }
    public bool EnableCors { get; set; }
    public bool EnableEfCore { get; set; }
    public bool EnableValidator { get; set; }
    public bool EnableFakerRealization { get; set; }
    public bool EnableGlobalExceptionFilter { get; set; }
    public bool EnableAutoResolve { get; set; }
    public bool EnableTimezoneHandler { get; set; }
}