using MediatR;
using Shippment.Domain.AggregateModels.LocationAggregate;
using Shippment.Domain.AggregateModels.ScheduleAggregate;
using Shippment.Domain.AggregateModels.TransportOrderAggregate;
using Shippment.Domain.Events;

namespace Shipment.Api.Application.DomainEventHandlers.AcceptTransportOrder
{
    public class PickupCargoHandler
        : INotificationHandler<AcceptTransportOrderDomainEvent>
    {
        private readonly ILogger<PickupCargoHandler> _logger;
        private readonly ITransportScheduleRepository _transportScheduleRepository;
        private readonly ITransportOrderRepository _transportOrderRepository;

        public PickupCargoHandler(ILogger<PickupCargoHandler> logger, ITransportScheduleRepository transportScheduleRepository, ITransportOrderRepository transportOrderRepository)
        {
            _logger = logger;
            _transportScheduleRepository = transportScheduleRepository;
            _transportOrderRepository = transportOrderRepository;
        }

        public async Task Handle(AcceptTransportOrderDomainEvent notification, CancellationToken cancellationToken)
        {
            if (notification.AssignedPickupEquipment is not null)
            {
                long orderId = notification.TransportOrderId;
                var assignedEquipment = notification.AssignedPickupEquipment;
                var order = await _transportOrderRepository.GetAsync(orderId);

                var schedule = new PickupSchedule(
                    assignedEquipment,
                    order.PickupCargoInfo.PickupTime.Value,
                    order.Goal.Origin,
                    order.PickupCargoInfo);

                bool result = await _transportScheduleRepository.CreateNewScheduleAsync(schedule);

                if (!result)
                {
                    _logger.LogError($"The transport order which Id {orderId} generate pickup cargo dispatching failed!");
                    return;
                }

                var pickupCode = schedule.PickupInfo.PickupCode;
                order.SchedulePickupService(schedule.Id, pickupCode, assignedEquipment);

                if (!await _transportOrderRepository.UpdateTransportOrderAsync(order))
                {
                    _logger.LogError($"The transport order which Id {orderId} save pickup cargo dispatching failed!");
                }
            }
        }
    }
}
