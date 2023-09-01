using DomainBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shippment.Domain.AggregateModels.LocationAggregate
{
    public class ILocationRepository : IRepository<Location>
    {
        public IUnitOfWork UnitOfWork => throw new NotImplementedException();
    }
}
