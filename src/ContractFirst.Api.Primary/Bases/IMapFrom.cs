using AutoMapper;

namespace ContractFirst.Api.Primary.Bases;

public interface IMapFrom<in TSource> where TSource : class
{
    virtual void ConfigureMapper(IMapperConfigurationExpression cfg, TSource? source)
    {
    }
}