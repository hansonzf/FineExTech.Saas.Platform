using MediatR;
using Orderpool.Api.Events;

namespace Orderpool.Api.Application.DomainEventHandlers.OrderPulled
{
    public class GetTransportRouteHandler
        : INotificationHandler<OrderPulledDomainEvent>
    {
        public Task Handle(OrderPulledDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
