using DomainBase;
using LocationApi.Domain.AggregateModels.LocationAggregate;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace LocationApi.Infrastructure.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly LocationDbContext _dbContext;

        public LocationRepository(LocationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IUnitOfWork UnitOfWork => (IUnitOfWork)_dbContext;

        public async Task<Location> AddAsync(Location location)
        {
            if (location.IsTransient())
                await _dbContext.Set<Location>().AddAsync(location);

            return location;
        }

        public async Task<bool> DeleteAsync(long locationId)
        {
            var toDel = await _dbContext.Set<Location>().FindAsync(locationId);
            var entry = _dbContext.Remove<Location>(toDel);
            return entry != null && entry.State == EntityState.Deleted;
        }

        public async Task<Location> GetAsync(long locationId)
        {
            if (locationId < 1)
                return null;

            return _dbContext.Set<Location>().FirstOrDefault(l => l.Id == locationId);
        }

        public async Task<PaginatedResult<Location>> GetByOwnerAsync(long ownerId, int pageIndex = 1, int pageSize = 20)
        {
            if (ownerId < 1)
                return null;

            PaginatedResult<Location> result = new PaginatedResult<Location>
            { 
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            result.TotalCount = await _dbContext.Set<Location>().CountAsync(l => l.Owner == ownerId);
            result.Data = _dbContext.Set<Location>().Where(l => l.Owner == ownerId)
                .OrderBy(l => l.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            return result;
        }
    }

    public class PaginatedResult<T> where T : class
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public static class PropertyExtension
    {
        public static void SetProperty<T, TValue>(this T target, Expression<Func<T, TValue>> memberLambda, TValue value)
        {
            if (memberLambda.Body is MemberExpression memberSelectorExpression)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    property.SetValue(target, value);
                }
            }
        }

        public static TValue GetPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> memberLambda, TValue @default)
        {
            if (memberLambda.Body is MemberExpression memberSelectorExpression)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    return (TValue)property.GetValue(target);
                }
            }
            return @default;
        }
    }
}
