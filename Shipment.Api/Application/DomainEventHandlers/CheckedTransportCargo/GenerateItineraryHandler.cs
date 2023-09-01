using MediatR;
using Shippment.Domain.AggregateModels;
using Shippment.Domain.AggregateModels.ItineraryAggregate;
using Shippment.Domain.Events;

namespace Shipment.Api.Application.DomainEventHandlers.CheckedTransportCargo
{
    public class GenerateItineraryHandler
        : INotificationHandler<CheckedTransportCargoDomainEvent>
    {
        private readonly IItineraryRepository _repository;
        private readonly ILogger<GenerateItineraryHandler> _logger;

        public GenerateItineraryHandler(IItineraryRepository repository, ILogger<GenerateItineraryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Handle(CheckedTransportCargoDomainEvent notification, CancellationToken cancellationToken)
        {
            string trackingNumber = notification.TrackingNumber;
            long orderId = notification.OrderId;
            OrderStatus status = notification.CurrentStatus;

            if (string.IsNullOrEmpty(trackingNumber))
            {
                _logger.LogError($"The transport order which Id {orderId} contains empty tracking number!");
                return;
            }

            if (status == OrderStatus.Standby)
            {
                var itinerary = new Itinerary(trackingNumber);
                bool result = await _repository.CreateNewItineraryAsync(itinerary);
                if (!result)
                {
                    _logger.LogError($"The order which tracking number {trackingNumber} generate intinerary occured error!");
                }
            }
        }
    }
}
