using DomainBase;
using LocationApi.Infrastructure.Repositories;

namespace LocationApi.Domain.AggregateModels.LocationAggregate
{
    public interface ILocationRepository : IRepository<Location>
    {
        Task<Location> GetAsync(long locationId);
        Task<PaginatedResult<Location>> GetByOwnerAsync(long ownerId, int pageIndex = 1, int pageSize = 20);
        Task<Location> AddAsync(Location location);
        Task<bool> DeleteAsync(long locationId);
    }
}
