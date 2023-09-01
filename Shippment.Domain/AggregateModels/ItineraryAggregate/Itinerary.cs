using DomainBase;
using Shippment.Domain.AggregateModels.LocationAggregate;
using Shippment.Domain.AggregateModels.RouterAggregate;
using Shippment.Domain.AggregateModels.TransportOrderAggregate;
using System.Collections.ObjectModel;
using System.Text;

namespace Shippment.Domain.AggregateModels.ItineraryAggregate
{
    public class Itinerary : Entity, IAggregateRoot
    {
        private List<Handing> _handings;
        private List<LocationDescription> _tracker;

        protected Itinerary()
        {
            _handings = new List<Handing>();
            _tracker = new List<LocationDescription>();
            CurrentLegIndex = 0;
        }

        public Itinerary(string trackingNumber)
            : this()
        {
            TrackingNumber = trackingNumber;
        }

        public string TrackingNumber { get; protected set; }
        public int CurrentLegIndex { get; protected set; }

        public ReadOnlyCollection<Handing> Handings
        {
            get => _handings.OrderBy(h => h.OperationTime).ToList().AsReadOnly();
            protected set => _handings = value.ToList();
        }
        public List<LocationDescription> Next
        {
            get
            {
                int pointCount = _tracker.Count;
                if (pointCount == CurrentLegIndex)
                    return new List<LocationDescription>();
                else
                    return _tracker.Skip(CurrentLegIndex).Take(pointCount - CurrentLegIndex).ToList();
            }
        }

        public void TrackRoute(List<Leg> legs)
        {
            if (legs is null)
                return;

            if (legs.Count > 1)
            {
                if (_tracker.Any())
                    HandleRedirectTrack(legs);
                else
                    HandleNewTrack(legs);
            }
        }

        private void HandleNewTrack(List<Leg> legs)
        {
            var orderedLegs = legs.OrderBy(l => l.LegIndex);
            foreach (var item in orderedLegs)
            {
                if (_tracker.Any() && _tracker.Last() == item.From)
                    continue;

                _tracker.Add(item.From);
            }
            _tracker.Add(orderedLegs.Last().To);
        }
        private void HandleRedirectTrack(List<Leg> legs)
        {
            int startTestMatchIndex = CurrentLegIndex;
            var currentLocation = _tracker[CurrentLegIndex];
            var locationHandings = Handings.Where(h => h.Location.LocationId == currentLocation.LocationId)
                .OrderBy(h => h.OperationTime)
                .AsEnumerable();
            if (locationHandings.Any(h => h.HandingType == Handing.Departing))
                startTestMatchIndex = CurrentLegIndex + 1;

            int totalLegs = _tracker.Count;
            bool routeHasIntersectPoint = false;
            LocationDescription newStartLocation = default;
            for (int i = startTestMatchIndex; i < totalLegs; i++)
            {
                var match = legs.FirstOrDefault(l => l.From.LocationId == _tracker[i].LocationId);
                if (match is not null)
                {
                    routeHasIntersectPoint = true;
                    newStartLocation = match.From;
                    break;
                }    
            }
            if (!routeHasIntersectPoint)
                throw new InvalidOperationException("The cargo future route will not intersect with specific route");

            int reserveIndex = _tracker.IndexOf(newStartLocation) + 1; int discardCount = 0; 
            if (locationHandings.Any(h => h.HandingType == Handing.Departing))
                reserveIndex = CurrentLegIndex + 2;
            else
                reserveIndex = CurrentLegIndex + 1;
            discardCount = totalLegs - reserveIndex;
            _tracker.RemoveRange(reserveIndex, discardCount);

            HandleNewTrack(legs);
        }

        public bool IsMatchDeliveryGoal(DeliverySpecification goal)
        {
            if (!_tracker.Any())
                return false;

            var from = _tracker.First();
            var to = _tracker.ElementAt(CurrentLegIndex);

            if (from.LocationId == goal.Origin.LocationId && to.LocationId == goal.Destination.LocationId)
                return true;
            else
                return false;
        }

        public void Log(Handing handingEvent)
        {
            if (handingEvent.HandingType > 30 && !_tracker.Any())
                return;

            if (handingEvent.HandingType == Handing.Arraiving)
                CurrentLegIndex = _tracker.IndexOf(handingEvent.Location);

            _handings.Add(handingEvent);
        }

        public string FlushLog()
        {
            if (!_handings.Any())
                return string.Empty;

            StringBuilder sb = new();
            var orderedHandings = _handings.OrderBy(h => h.OperationTime);
            foreach (var evt in orderedHandings)
            {
                sb.AppendLine(evt.HandingDescription);
            }

            return sb.ToString();
        }
    }
}
