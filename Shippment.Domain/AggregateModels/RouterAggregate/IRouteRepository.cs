using DomainBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shippment.Domain.AggregateModels.RouterAggregate
{
    public interface IRouteRepository : IRepository<Route>
    {
        Task<Route> GetAsync(long id);
        Task<bool> DeleteAsync(long id);
    }
}
