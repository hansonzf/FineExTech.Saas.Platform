using MediatR;

namespace Shippment.Domain.Events
{
    public record AssignedScheduleForOrderDomainEvent : INotification
    {
        public string TrackingNumber { get; init; }
        public long AssignedSchedule { get; init; }
        public DateTime OccuredTime { get; init; }
    }
}
