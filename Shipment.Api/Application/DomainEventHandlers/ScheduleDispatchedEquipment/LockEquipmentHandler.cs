using MediatR;
using Shippment.Domain.AggregateModels.EquipmentAggregate;
using Shippment.Domain.AggregateModels.LocationAggregate;
using Shippment.Domain.Events;

namespace Shipment.Api.Application.DomainEventHandlers.ScheduleDispatchedEquipment
{
    public class LockEquipmentHandler : INotificationHandler<ScheduleDispatchedEquipmentDomainEvent>
    {
        private readonly IEquipmentRepository _repository;
        private readonly ILogger<LockEquipmentHandler> _logger;

        public LockEquipmentHandler(IEquipmentRepository repository, ILogger<LockEquipmentHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Handle(ScheduleDispatchedEquipmentDomainEvent notification, CancellationToken cancellationToken)
        {
            var eq = notification.Equipment;
            DateTime setoutTime = notification.EstimateSetoutTime;
            double interval = notification.EstimateTripInterval;
            LocationDescription destination = notification.Destination;

            if (eq == null) 
                return;
            
            var equipment = await _repository.GetAsync(eq.EquipmentId);
            if (equipment.AssignTask(setoutTime, interval, destination))
                _ = await _repository.SaveAsync(equipment);
            else
                _logger.LogError(
                    $"Lock equipment which ( Id: {equipment.Id} - Identifier: {equipment.Identifier}) failed, this equipment is using in transport schedule Id is {notification.ScheduleId}");
        }
    }
}
