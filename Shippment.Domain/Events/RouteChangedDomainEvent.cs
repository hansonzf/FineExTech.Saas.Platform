using MediatR;
using Shippment.Domain.AggregateModels.RouterAggregate;

namespace Shippment.Domain.Events
{
    public record RouteChangedDomainEvent : INotification
    {
        public Segment[] OriginalSegments { get; init; }
        public Segment[] NewSegments { get; init; }
        public long RouteId { get; init; }
    }
}
