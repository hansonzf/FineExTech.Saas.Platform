using MediatR;
using Shippment.Domain.AggregateModels;
using Shippment.Domain.AggregateModels.LocationAggregate;

namespace Shippment.Domain.Events
{
    public record ScheduleExecutedDomainEvent : INotification
    {
        public long ScheduleId { get; init; }
        public ScheduleType Type { get; init; }
        public DateTime OccuredTime { get; init; }
        public LocationDescription Location { get; init; }
    }
}
