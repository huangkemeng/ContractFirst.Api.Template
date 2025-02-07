namespace ContractFirst.Api.Infrastructure.DataPersistence.DataEntityBases;

public interface IDetailPaginated 
{
    bool HasPreviousPage { get;}

    bool HasNextPage { get;}
    
    int TotalPages { get; set; }
    
    int Offset { get; set; }
}