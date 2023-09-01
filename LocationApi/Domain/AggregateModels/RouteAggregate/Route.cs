using DomainBase;
using LocationApi.Domain.AggregateModels.LocationAggregate;
using LocationApi.Domain.Events;
using System.Collections.ObjectModel;

namespace LocationApi.Domain.AggregateModels.RouteAggregate
{
    public class Route : Entity, IAggregateRoot
    {
        private Segment[] _segments = new Segment[0];
        private double _distance = 0;

        protected Route()
        { }

        public Route(long owner, string nameOfRoute, LocationDescription origin, LocationDescription destination, Segment[] segments = default)
        {
            OwnerId = owner;
            RouteName = nameOfRoute;
            Origin = origin;
            Destination = destination;
            segments = segments ?? new Segment[0];

            if (segments.Any() && IsValid(segments))
                SetSegments(segments);
        }

        public long OwnerId { get; protected set; }
        public string RouteName { get; protected set; }
        public LocationDescription Origin { get; protected set; }
        public LocationDescription Destination { get; protected set; }
        public double Distance
        {
            get => _distance;
            protected set => _distance = value;
        }
        public ReadOnlyCollection<Segment> Segments
        {
            get
            {
                return _segments.ToList().AsReadOnly();
            }
            protected set
            {
                _segments = value.OrderBy(s => s.Index).ToArray();
            }
        }

        public List<Leg> Legs
        {
            get
            {
                int segmentCount = _segments.Count();
                return _segments.Select(
                    s => new Leg(Id, RouteName, segmentCount, Distance, s.Index, s.From, s.To, s.Distance)).ToList();
            }
        }

        protected void SetSegments(Segment[] segments)
        {
            _segments = new Segment[segments.Length];
            for (int i = 0; i < segments.Length; i++)
            {
                _segments[i] = segments[i].Bind(Id, i);
            }
            _distance = _segments.Sum(s => s.Distance);
        }

        public bool IsValid(Segment[] segments)
        {
            if (!segments.Any())
                return true;

            for (int i = 0; i < segments.Length; i++)
            {
                if (i == segments.Length - 1)
                    break;

                if (segments[i].To != segments[i + 1].From)
                    return false;
            }

            if (segments.First().From != Origin || segments.Last().To != Destination)
                return false;

            return true;
        }

        public bool AddSegment(params Segment[] segments)
        {
            if (!segments.Any())
                return false;

            if (IsValid(segments))
            {
                var segmentChangedEvent = new RouteChangedDomainEvent
                {
                    RouteId = this.Id,
                    OriginalSegments = _segments,
                    NewSegments = segments,
                };
                SetSegments(segments);
                AddDomainEvent(segmentChangedEvent);

                return true;
            }

            return false;
        }

        public bool ReplaceSegment(int index, params Segment[] newSegments)
        {
            if (!newSegments.Any())
                return false;

            int newLength = _segments.Length + newSegments.Length - 1;
            Segment[] copy = new Segment[newLength];
            var firstSegment = _segments.Take(index);
            var lastSegment = _segments.Skip(index + 1).Take(newLength - index - 1);
            copy = firstSegment.Concat(newSegments).Concat(lastSegment).ToArray();

            if (IsValid(copy))
            {
                var segmentChangedEvent = new RouteChangedDomainEvent
                {
                    RouteId = this.Id,
                    OriginalSegments = _segments,
                    NewSegments = copy,
                };
                SetSegments(copy);
                AddDomainEvent(segmentChangedEvent);

                return true;
            }

            return false;
        }

        public Leg GetRouteLeg(int index)
        {
            Leg leg;
            Segment segment;
            int segmentCount = Segments.Count;

            if (index < 0 || index > segmentCount)
                throw new IndexOutOfRangeException($"index must in range {0} and {segmentCount + 1}");

            if (segmentCount == 0 || index == 0)
            {
                leg = new Leg(Id, RouteName, 0, Distance, 0, Origin, Destination, Distance);
            }
            else
            {
                segment = _segments[index - 1];
                leg = new Leg(Id, RouteName, segmentCount, Distance, index, segment.From, segment.To, segment.Distance);
            }

            return leg;
        }
    }
}
