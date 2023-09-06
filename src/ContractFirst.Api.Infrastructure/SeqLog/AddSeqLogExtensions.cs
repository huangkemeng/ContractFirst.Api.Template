using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ContractFirst.Api.Infrastructure.SeqLog;

public static class AddSeqLogExtensions
{
    public static void AddSeqLog(this ILoggingBuilder loggingBuilder)
    {
        var serviceProvider = loggingBuilder.Services.BuildServiceProvider();
        var seqSetting = serviceProvider.GetRequiredService<SeqSetting>();
        var applicationName = "ContractFirst.Api";
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", applicationName)
            .Enrich.WithCorrelationIdHeader()
            .Enrich.WithClientIp()
            .WriteTo.Console()
            .WriteTo.Seq(seqSetting.ServerUrl, apiKey: seqSetting.ApiKey)
            .CreateLogger();
        loggingBuilder.AddSerilog(Log.Logger);
        loggingBuilder.Services.AddSingleton(Log.Logger);
    }
}