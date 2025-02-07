using ContractFirst.Api.Infrastructure.DataPersistence.EfCore.Entities.Bases;
using Microsoft.EntityFrameworkCore;

namespace ContractFirst.Api.Infrastructure.DataPersistence.EfCore;

public class DbAccessor<T>(ApplicationDbContext context) where T : class, IEfEntity<T>
{
    public DbSet<T> DbSet => context.Set<T>();
}