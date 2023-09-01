using DomainBase;
using LocationApi.Domain.AggregateModels.LocationAggregate;

namespace LocationApi.Domain.AggregateModels.RouteAggregate
{
    public class Leg : ValueObject
    {
        public Leg(long routeId, string routeName, int segmentCount, double totalDistance, int legIndex, LocationDescription from, LocationDescription to, double legDistance)
        {
            RouteId = routeId;
            RouteName = routeName;
            SegmentCount = segmentCount;
            TotalDistance = totalDistance;
            LegIndex = legIndex;
            From = from;
            To = to;
            LegDistance = legDistance;
        }

        public long RouteId { get; private set; }
        public string RouteName { get; private set; }
        public int SegmentCount { get; private set; }
        public double TotalDistance { get; private set; }
        public int LegIndex { get; private set; }
        public LocationDescription From { get; private set; }
        public LocationDescription To { get; private set; }
        public double LegDistance { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return RouteId;
            yield return RouteName;
            yield return SegmentCount;
            yield return TotalDistance;
            yield return LegIndex;
            yield return From;
            yield return To;
            yield return LegDistance;
        }
    }
}
