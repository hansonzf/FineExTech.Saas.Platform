using MediatR;
using Shippment.Domain.AggregateModels;

namespace Shippment.Domain.Events
{
    public record CheckedTransportCargoDomainEvent : INotification
    {
        public CheckedTransportCargoDomainEvent(long orderId, string trackingNumber, OrderStatus status)
        { 
            OrderId = orderId;
            TrackingNumber = trackingNumber;
            CurrentStatus = status;
        }

        public long OrderId { get; init; }
        public OrderStatus CurrentStatus { get; init; }
        public string TrackingNumber { get; init; }
    }
}
