using DomainBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shippment.Domain.AggregateModels.TransportOrderAggregate
{
    public interface ITransportOrderRepository : IRepository<TransportOrder>
    {
        Task<TransportOrder> GetAsync(long orderId);
        Task<IEnumerable<TransportOrder>> GetWaitforAuditOrdersAsync();
        Task<IEnumerable<TransportOrder>> GetWaitforPickupOrdersAsync();
        Task<bool> UpdateTransportOrderAsync(TransportOrder transportOrder);
    }
}
