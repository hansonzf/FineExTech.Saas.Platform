using DomainBase;
using Shippment.Domain.AggregateModels.LocationAggregate;

namespace Shippment.Domain.AggregateModels.RouterAggregate
{
    public class Segment : Entity
    {
        public long RouteId { get; protected set; }
        public int Index { get; protected set; }
        public LocationDescription From { get; protected set; }
        public LocationDescription To { get; protected set; }
        public double Distance { get; protected set; }

        public Segment(LocationDescription from, LocationDescription to, double distance)
        {
            From = from;
            To = to;
            Distance = distance;
        }

        internal Segment Bind(long routeId, int index)
        {
            Index = index; 
            RouteId = routeId;
            
            return this;
        }
    }
}
