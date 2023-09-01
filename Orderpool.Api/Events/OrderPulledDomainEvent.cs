using MediatR;

namespace Orderpool.Api.Events
{
    public class OrderPulledDomainEvent : INotification
    {
        public long WatcherId { get; set; }
        public OrderPulledDomainEvent(long watcherId)
        {
            WatcherId = watcherId;
        }
    }
}
