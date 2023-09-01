using DomainBase;

namespace Shippment.Domain.AggregateModels.LocationAggregate
{
    public class LocationInfo : ValueObject
    {
        public LocationInfo(long locationId, string locationName)
        {
            LocationId = locationId;
            LocationName = locationName;
        }

        public long LocationId { get; private set; }
        public string LocationName { get; private set; }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return LocationId;
            yield return LocationName;
        }
    }
}
