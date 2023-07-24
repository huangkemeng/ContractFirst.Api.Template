namespace ContractFirst.Api.Primary.Entities.Bases;

public static class InitEntityExtensions
{
    public static void InitPropertyValues<T>(this T entity) where T : IEntityPrimary
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedOn = DateTimeOffset.Now;
    }
}