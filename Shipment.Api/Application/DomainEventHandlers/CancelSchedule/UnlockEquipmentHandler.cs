using MediatR;
using Shippment.Domain.AggregateModels.EquipmentAggregate;
using Shippment.Domain.Events;

namespace Shipment.Api.Application.DomainEventHandlers.CancelSchedule
{
    public class UnlockEquipmentHandler : INotificationHandler<CancelScheduleDomainEvent>
    {
        private readonly IEquipmentRepository _repository;
        private ILogger<UnlockEquipmentHandler> _logger;

        public UnlockEquipmentHandler(IEquipmentRepository repository, ILogger<UnlockEquipmentHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Handle(CancelScheduleDomainEvent notification, CancellationToken cancellationToken)
        {
            var eq = notification.Equipment;

            if (eq == null)
                return;

            var equipment = await _repository.GetAsync(eq.EquipmentId);
            if (equipment.CancelTask())
                _ = await _repository.SaveAsync(equipment);
            else
                _logger.LogError($"Unlock equipment which ( id: {equipment.Id} - identify: {equipment.Identifier} ) failed");
        }
    }
}
