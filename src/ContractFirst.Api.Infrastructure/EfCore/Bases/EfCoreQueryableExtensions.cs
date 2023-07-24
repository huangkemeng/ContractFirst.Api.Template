using System.Linq.Expressions;
using ContractFirst.Api.Primary.Contracts.Bases;
using Microsoft.EntityFrameworkCore;

namespace ContractFirst.Api.Infrastructure.EfCore.Bases;

public static class EfCoreQueryableExtensions
{
    public static IQueryable<T> Sort<T>(this IQueryable<T> query, ISortable sortable) where T : class
    {
        if (sortable != null && sortable.Sort != null)
        {
            var type = typeof(T);
            var prop = type.GetProperties().FirstOrDefault(e =>
                e.Name.Equals(sortable.Sort.FieldName, StringComparison.OrdinalIgnoreCase));
            if (prop != null)
            {
                var parameterExpression = Expression.Parameter(type, "x");
                var memberExpression = Expression.MakeMemberAccess(parameterExpression, prop);
                var filterExpress =
                    Expression.Lambda<Func<T, object>>(Expression.Convert(memberExpression, typeof(object)),
                        parameterExpression);
                if (filterExpress != null)
                {
                    if (sortable.Sort.Direction == SortDirectionEnum.Descending)
                        query = query.OrderByDescending(filterExpress);
                    else
                        query = query.OrderBy(filterExpress);
                }
            }
        }

        return query;
    }

    public static async Task<PaginatedResult<T>> PaginateAsync<T>(this IQueryable<T> query, IPaginable paginable,
        CancellationToken cancellationToken) where T : class
    {
        PaginatedResult<T> paginatedResult = new()
        {
            Total = await query.CountAsync(cancellationToken)
        };
        if (paginable != null)
            query = query.Skip((paginable.Offset - 1) * paginable.PageSize)
                .Take(paginable.PageSize);
        paginatedResult.List = await query.ToListAsync(cancellationToken);
        return paginatedResult;
    }

    public static IQueryable<T> WhereWhile<T>(this IQueryable<T> query, bool predicate,
        Expression<Func<T, bool>> expression) where T : class
    {
        if (predicate) return query.Where(expression);
        return query;
    }

    public static async Task MergeAsync<T>(this DbSet<T> db, IQueryable<T> existedEntitiesQueryable,
        IEnumerable<T> mergingObjects, Func<T, T, bool> filterToUpdate, Action<T, T>? updateAction,
        CancellationToken cancellationToken) where T : class
    {
        var existedEntities = await existedEntitiesQueryable.ToListAsync(cancellationToken);
        var addingEntities = new List<T>();
        var updatingEntities = new List<T>();
        foreach (var mergingObject in mergingObjects)
        {
            var existingEntity = existedEntities.FirstOrDefault(e => filterToUpdate(e, mergingObject));
            if (existingEntity != null)
            {
                updateAction?.Invoke(existingEntity, mergingObject);
                updatingEntities.Add(existingEntity);
            }
            else
            {
                addingEntities.Add(mergingObject);
            }
        }

        var deletingEntites = existedEntities.Where(e => !updatingEntities.Any(x => filterToUpdate(e, x))).ToList();
        if (deletingEntites.Any()) db.RemoveRange(deletingEntites);
        if (addingEntities.Any()) await db.AddRangeAsync(addingEntities, cancellationToken);
    }
}

public class PaginatedResult<T>
{
    public List<T> List { get; set; }

    public int Total { get; set; }
}