using MediatR;
using Shippment.Domain.AggregateModels.EquipmentAggregate;
using Shippment.Domain.AggregateModels.LocationAggregate;

namespace Shippment.Domain.Events
{
    public record ScheduleDispatchedEquipmentDomainEvent : INotification
    {
        public long ScheduleId { get; init; }
        public DateTime EstimateSetoutTime { get; init; }
        public double EstimateTripInterval { get; init; }
        public LocationDescription Destination { get; init; }
        public EquipmentDescription Equipment { get; init; }
    }
}
