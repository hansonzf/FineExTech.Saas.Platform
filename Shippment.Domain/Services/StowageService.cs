using Shippment.Domain.AggregateModels;
using Shippment.Domain.AggregateModels.ScheduleAggregate;
using Shippment.Domain.AggregateModels.TransportOrderAggregate;

namespace Shippment.Domain.Services
{
    public class StowageService : IStowageService
    {
        private readonly IStowageRepository _repository;

        public StowageService(IStowageRepository repository)
        {
            _repository = repository;
        }

        public async Task<StowageResult> Stowage(TransportSchedule schedule, TransportOrder order)
        {
            bool result = true;
            if (schedule is null)
                return new StowageResult { Result = false, Message = "schedule is required" };
            if (order is null)
                return new StowageResult { Result = false, Message = "order is required" };
            if (schedule.Status != ScheduleStatus.Standby)
                return new StowageResult { Result = false, Message = "schedule is not prepared" };
            if (order.Status != OrderStatus.Standby)
                return new StowageResult { Result = false, Message = "order is not prepared" };
            if (!order.CargoList.Any())
                return new StowageResult { Result = false, Message = "order's cargo are not prepared" };
            DateTime timeBar = schedule.Efficiency.EstimateSetoutTime.AddMinutes(-30);
            if (DateTime.Now > timeBar)
                return new StowageResult { Result = false, Message = "schedule has finished preparing yet" };
            if (schedule.Equipment.MaxLoadWeight < order.CargoList.Sum(c => c.CargoInfo.Weight.Number) ||
                schedule.Equipment.MaxLoadVolume < order.CargoList.Sum(c => c.CargoInfo.Volume.Volume))
                return new StowageResult { Result = false, Message = "The equipment can not load any more" };

            foreach (var cargo in order.CargoList)
            {
                if (!result)
                    break;

                result &= schedule.TakeOnCargo(order.TrackingNumber, cargo.BarCode, cargo.CargoInfo);
            }
            if (result)
            {
                order.FollowSchedule(schedule.Id);
                result = await _repository.SaveAsync(schedule, order);
            }

            return new StowageResult { Result = result, Message = "stowage success" };
        }
    }
}
