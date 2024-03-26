namespace ContractFirst.Api.Primary.Bases;

public interface ICurrent
{
    Task<Guid> GetCurrentUserIdAsync();
}