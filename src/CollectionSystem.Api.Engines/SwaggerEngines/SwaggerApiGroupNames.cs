namespace CollectionSystem.Api.Engines.SwaggerEngines;

public enum SwaggerApiGroupNames
{
    [SwaggerGroupInfo(Title = "调查收集系统Web端接口文档", Description = "Web端", Version = "v1", MatchRule = "api/web")]
    Web,

    [SwaggerGroupInfo(Title = "调查收集系统App端接口文档", Description = "App端", Version = "v1", MatchRule = "api/app")]
    App
}