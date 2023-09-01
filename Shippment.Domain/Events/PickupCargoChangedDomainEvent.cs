using MediatR;
using Shippment.Domain.AggregateModels.TransportOrderAggregate;

namespace Shippment.Domain.Events
{
    public class PickupCargoChangedDomainEvent : INotification
    {
        public PickupDescription PickupInformation { get; set; }
    }
}
