using MediatR;
using Shippment.Domain.AggregateModels;
using Shippment.Domain.AggregateModels.LocationAggregate;

namespace Shippment.Domain.Events
{
    public class ArrivedTransportDestinationDomainEvent : INotification
    {
        public long ScheduleId { get; set; }
        public ScheduleType ScheduleType { get; set; }
        public LocationDescription Setout { get; set; }
        public LocationDescription Destination { get; set; }
    }
}
