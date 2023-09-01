using DomainBase;

namespace LocationApi.Domain.AggregateModels.LocationAggregate
{
    public class LocationDescription : ValueObject
    {
        public long LocationId { get; private set; }
        public string LocationName { get; private set; }
        public string City { get; private set; }

        public LocationDescription(long locationId, string locationName)
        {
            LocationId = locationId;
            LocationName = locationName;
        }

        public LocationDescription(long locationId, string locationName, string city)
            : this(locationId, locationName)
        {
            City = city;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return LocationId;
            yield return LocationName;
            yield return City;
        }
    }
}
