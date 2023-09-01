using DomainBase;
using Orderpool.Api.Models;
using Orderpool.Api.Models.OrderWatcherAggregate;

namespace Orderpool.Api.Infrastructure
{
    public class OrderWatcherRepository : IOrderWatchRepository
    {
        public IUnitOfWork UnitOfWork => throw new NotImplementedException();

        public Task<bool> BulkInsertImportedDataAsync(IEnumerable<OrderWatcher> watcher, IEnumerable<OrderMetadata> orders)
        {
            throw new NotImplementedException();
        }

        public Task<OrderWatcher> GetNextWatcherAsync()
        {
            string fetchId = Guid.NewGuid().ToString();
            string sql = @"
UPDATE TOP 1 OrderWatcher 
SET IsProcessing = 1, FetchId = @FetchId 
WHERE IsProcessing = 0 
ORDER BY ProcessIndex";
            throw new NotImplementedException();
        }

        public Task<OrderWatcher> NextAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveAsync(OrderWatcher watcher)
        {
            throw new NotImplementedException();
        }
    }
}
