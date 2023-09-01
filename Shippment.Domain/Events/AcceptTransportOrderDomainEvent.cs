using MediatR;
using Shippment.Domain.AggregateModels.EquipmentAggregate;

namespace Shippment.Domain.Events
{
    public record AcceptTransportOrderDomainEvent : INotification
    {
        public long TransportOrderId { get; init; }
        public long ScheduleId { get; init; }
        public EquipmentDescription? AssignedPickupEquipment { get; init; }
    }
}
