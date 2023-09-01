using MediatR;
using Shippment.Domain.AggregateModels.EquipmentAggregate;
using Shippment.Domain.AggregateModels.LocationAggregate;
using Shippment.Domain.AggregateModels.ScheduleAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shippment.Domain.Events
{
    /// <summary>
    /// this event will be triggered after made a transport schedule
    /// 
    /// 该事件目前应该响应车辆资源占用锁。比如，某站点计划车辆A执行一段运输任务
    /// 那么在计划制定后，车辆A在这个站点内是不可用状态。
    /// </summary>
    public record MakingScheduleDomainEvent : INotification
    {
        public long RouteId { get; init; }
        public EquipmentDescription Equipment { get; init; }
        public LocationDescription From { get; init;}
        public LocationDescription To { get; init; }
        public TimeManagement Efficiency { get; init; }

        public MakingScheduleDomainEvent(long routeId, EquipmentDescription equipment, LocationDescription from, LocationDescription to, TimeManagement efficiency)
        {
            RouteId = routeId;
            Equipment = equipment;
            From = from;
            To = to;
            Efficiency = efficiency;
        }
    }
}
