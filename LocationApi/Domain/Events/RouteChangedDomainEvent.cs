using LocationApi.Domain.AggregateModels.RouteAggregate;
using MediatR;

namespace LocationApi.Domain.Events
{
    public record RouteChangedDomainEvent : INotification
    {
        public Segment[] OriginalSegments { get; init; }
        public Segment[] NewSegments { get; init; }
        public long RouteId { get; init; }
    }
}
