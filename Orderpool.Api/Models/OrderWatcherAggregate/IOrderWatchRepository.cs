using DomainBase;

namespace Orderpool.Api.Models.OrderWatcherAggregate
{
    public interface IOrderWatchRepository : IRepository<OrderWatcher>
    {
        Task<OrderWatcher> NextAsync();
        Task<bool> SaveAsync(OrderWatcher watcher);
        Task<bool> BulkInsertImportedDataAsync(IEnumerable<OrderWatcher> watcher, IEnumerable<OrderMetadata> orders);
    }
}
