using MediatR;
using Shippment.Domain.AggregateModels.ItineraryAggregate;
using Shippment.Domain.AggregateModels.RouterAggregate;
using Shippment.Domain.AggregateModels.ScheduleAggregate;
using Shippment.Domain.Events;

namespace Shipment.Api.Application.DomainEventHandlers.AssignedScheduleForOrder
{
    public class UpdateItineraryTrackTargetHandler 
        : INotificationHandler<AssignedScheduleForOrderDomainEvent>
    {
        private readonly ILogger<UpdateItineraryTrackTargetHandler> _logger;
        private readonly IItineraryRepository _itineraryRepository;
        private readonly ITransportScheduleRepository _scheduleRepository;
        private readonly IRouteRepository _routeRepository;

        public UpdateItineraryTrackTargetHandler(
            ILogger<UpdateItineraryTrackTargetHandler> logger,
            IItineraryRepository itineraryRepository,
            ITransportScheduleRepository scheduleRepository,
            IRouteRepository routeRepository)
        {
            _logger = logger;
            _itineraryRepository = itineraryRepository;
            _scheduleRepository = scheduleRepository;
            _routeRepository = routeRepository;
        }



        public async Task Handle(AssignedScheduleForOrderDomainEvent notification, CancellationToken cancellationToken)
        {
            string trackingNumber = notification.TrackingNumber;
            var schedule = await _scheduleRepository.GetAsync(notification.AssignedSchedule);
            if (string.IsNullOrEmpty(trackingNumber))
                _logger.LogError("log some error");
            if (schedule is null)
                _logger.LogError($"Can not find schedule which have Id: {notification.AssignedSchedule}");

            long routeId = schedule.RouteLeg.RouteId;
            var route = await _routeRepository.GetAsync(routeId);
            var itinerary = await _itineraryRepository.GetAsync(trackingNumber);
            itinerary.TrackRoute(route.Legs);

            await _itineraryRepository.SaveItineraryAsync(itinerary);
        }
    }
}
