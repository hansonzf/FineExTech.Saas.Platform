using Shippment.Domain.AggregateModels.ScheduleAggregate;
using Shippment.Domain.AggregateModels.TransportOrderAggregate;

namespace Shippment.Domain.Services
{
    public interface IStowageService
    {
        Task<StowageResult> Stowage(TransportSchedule schedule, TransportOrder order);
    }

    public interface IStowageRepository
    {
        Task<bool> SaveAsync(TransportSchedule schedule, TransportOrder order);
    }

    public class StowageResult
    {
        public bool Result { get; set; }
        public string Message { get; set; }
    }
}
