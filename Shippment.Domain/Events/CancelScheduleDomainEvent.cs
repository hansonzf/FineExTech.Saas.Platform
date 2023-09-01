using MediatR;
using Shippment.Domain.AggregateModels.EquipmentAggregate;

namespace Shippment.Domain.Events
{
    public record CancelScheduleDomainEvent : INotification
    {
        public EquipmentDescription Equipment { get; init; }
    }
}
