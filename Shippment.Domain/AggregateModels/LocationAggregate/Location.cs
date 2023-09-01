using DomainBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shippment.Domain.AggregateModels.LocationAggregate
{
    public class Location : Entity, IAggregateRoot
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string City { get; private set; }

        public override string ToString()
        {
            return $"({Code}) {Name}";
        }
    }
}
