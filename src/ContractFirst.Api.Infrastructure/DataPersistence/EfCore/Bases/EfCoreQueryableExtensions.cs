using System.Linq.Expressions;
using ContractFirst.Api.Infrastructure.DataPersistence.DataEntityBases;
using Microsoft.EntityFrameworkCore;

namespace ContractFirst.Api.Infrastructure.DataPersistence.EfCore.Bases;

public static class EfCoreQueryableExtensions
{
    public static IQueryable<T> Sort<T>(this IQueryable<T> query, ISortable? sortable) where T : class
    {
        if (sortable is { Sort: not null })
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
                query = sortable.Sort.Direction == SortDirectionEnum.Descending
                    ? query.OrderByDescending(filterExpress)
                    : query.OrderBy(filterExpress);
            }
        }

        return query;
    }

    public static async Task<PaginatedResult<T>> PaginateAsync<T>(this IQueryable<T> query, IPageable? pageable,
        CancellationToken cancellationToken) where T : class
    {
        PaginatedResult<T> paginatedResult = new()
        {
            Total = await query.CountAsync(cancellationToken)
        };
        if (pageable != null)
            query = query.Skip((pageable.Offset - 1) * pageable.PageSize)
                .Take(pageable.PageSize);
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
        IEnumerable<T>? mergingObjects, Func<T, T, bool> filterToUpdate, Action<T, T>? updateAction,
        CancellationToken cancellationToken) where T : class
    {
        var existedEntities = await existedEntitiesQueryable.ToListAsync(cancellationToken);
        var addingEntities = new List<T>();
        var updatingEntities = new List<T>();
        if (mergingObjects != null)
        {
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
        }

        var deletingEntities = existedEntities.Where(e => !updatingEntities.Any(x => filterToUpdate(e, x))).ToList();
        if (deletingEntities.Any()) db.RemoveRange(deletingEntities);
        if (addingEntities.Any()) await db.AddRangeAsync(addingEntities, cancellationToken);
    }

    public static async Task MergeAsync<T>(this DbSet<T> db, Expression<Func<T, bool>> existedEntitiesFilter,
        IEnumerable<T>? mergingObjects, Func<T, T, bool> filterToUpdate, Action<T, T>? updateAction,
        CancellationToken cancellationToken) where T : class
    {
        await db.MergeAsync(db.Where(existedEntitiesFilter), mergingObjects, filterToUpdate, updateAction,
            cancellationToken);
    }
}

public class PaginatedResult<T>
{
    public List<T> List { get; set; }

    public int Total { get; set; }
}