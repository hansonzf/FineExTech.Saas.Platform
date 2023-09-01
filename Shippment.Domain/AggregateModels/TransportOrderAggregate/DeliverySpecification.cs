using DomainBase;
using Shippment.Domain.AggregateModels.ItineraryAggregate;
using Shippment.Domain.AggregateModels.LocationAggregate;

namespace Shippment.Domain.AggregateModels.TransportOrderAggregate
{
    public class DeliverySpecification : ValueObject
    {
        public DeliverySpecification(LocationDescription origin, LocationDescription destination, DateTime? expectingArrivedTime = null, DateTime? expectingSetoutTime = null)
        {
            Origin = origin;
            Destination = destination;
            ExpectingSetoutTime = expectingSetoutTime;
            ExpectingArrivedTime = expectingArrivedTime;
        }

        public LocationDescription Origin { get; private set; }
        public LocationDescription Destination { get; private set; }
        public DateTime? ExpectingSetoutTime { get; private set; }
        public DateTime? ExpectingArrivedTime { get; private set; }

        public bool Satisfied(Itinerary itinerary)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Origin;
            yield return Destination;
            yield return ExpectingSetoutTime;
            yield return ExpectingArrivedTime;
        }
    }
}
