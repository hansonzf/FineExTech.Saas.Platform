using MediatR;
using Orderpool.Api.Models;
using Orderpool.Api.Models.OrderWatcherAggregate;

namespace Orderpool.Api.Events
{
    public class ProcessingOrderDomainEvent
        : INotification
    {
        public OrderDigest OrderDescription { get; set; }
        public ProcessHandler Handlers { get; set; }
    }
}
