using Shippment.Domain.AggregateModels;
using Shippment.Domain.AggregateModels.EquipmentAggregate;
using Shippment.Domain.AggregateModels.LocationAggregate;
using Shippment.Domain.AggregateModels.RouterAggregate;
using Shippment.Domain.AggregateModels.ScheduleAggregate;
using Shippment.Domain.AggregateModels.TransportOrderAggregate;
using System.Linq.Expressions;
using System.Reflection;

namespace Shipment.Domain.Test.MockAggregate
{
    public class ScheduleProxy : TransportSchedule
    {
        public ScheduleProxy(Leg routeLeg, DateTime estimateSetoutTime, float transportInterval = 0)
            : base(routeLeg, estimateSetoutTime, transportInterval)
        { }

        private static List<Route> routes = RouteProxy.SeedTestData();
        private static List<TransportOrder> orders = TransportOrderProxy.SeedTestData();

        public static List<TransportSchedule> SeedTestData()
        {
            DateTime setoutTime = new DateTime(2022, 10, 27, 18, 0, 0);
            EquipmentDescription equipment = new EquipmentDescription(1, "鄂AM73Z7", EquipmentType.Vehicle);
            EquipmentDescription equipment2 = new EquipmentDescription(1, "鄂A62FD1", EquipmentType.Vehicle);
            TimeManagement eff;
            List<TransportSchedule> schedules = new List<TransportSchedule>();

            schedules.Add(
                new ScheduleProxy(routes[1].GetRouteLeg(1), setoutTime) 
                { 
                    Id = 1,
                    Equipment = equipment
                });

            schedules.Add(
                new ScheduleProxy(routes[1].GetRouteLeg(1), setoutTime) 
                { 
                    Id = 2,
                    Equipment = equipment2
                });

            schedules.Add(
                new ScheduleProxy(routes[1].GetRouteLeg(1), setoutTime) 
                { 
                    Id = 3,
                    Status = ScheduleStatus.Standby,
                    Equipment = equipment
                });

            schedules.Add(
                new ScheduleProxy(routes[1].GetRouteLeg(1), setoutTime)
                {
                    Id = 4,
                    Status = ScheduleStatus.Standby,
                    Equipment = equipment2
                });

            var s1 = new ScheduleProxy(routes[1].GetRouteLeg(1),  setoutTime);
            s1.Id = 5;
            s1.Equipment = equipment;
            s1.Status = ScheduleStatus.Standby;
            eff = s1.Efficiency;
            s1.Status = ScheduleStatus.Executed;
            s1.Efficiency = eff.Leave();
            schedules.Add(s1);

            var s2 = new ScheduleProxy(routes[1].GetRouteLeg(1),  setoutTime);
            s2.Id = 6;
            s2.Equipment = equipment2;
            s2.Status = ScheduleStatus.Standby;
            eff = s2.Efficiency;
            s2.Status = ScheduleStatus.Executed;
            s2.Efficiency = eff.Leave();
            schedules.Add(s2);


            //var dispatching_Created_Status_Schedule_1 = new PickupSchedule(equipment, setoutTime, orders[5].Goal.Origin, orders[5].PickupCargoInfo);
            //dispatching_Created_Status_Schedule_1.SetProperty(s => s.Id, 7);
            //schedules.Add(dispatching_Created_Status_Schedule_1);

            //var dispatching_Created_Status_Schedule_2 = new PickupSchedule(equipment2, setoutTime, orders[5].Goal.Origin, orders[5].PickupCargoInfo);
            //dispatching_Created_Status_Schedule_2.SetProperty(s => s.Id, 8);
            //schedules.Add(dispatching_Created_Status_Schedule_2);

            //var dispatching_Standby_Status_Schedule_1 = new PickupSchedule(equipment, setoutTime, orders[5].Goal.Origin, orders[5].PickupCargoInfo);
            //dispatching_Standby_Status_Schedule_1.SetProperty(s => s.Id, 9);
            //dispatching_Standby_Status_Schedule_1.SetProperty(s => s.Status, ScheduleStatus.Standby);
            //schedules.Add(dispatching_Standby_Status_Schedule_1);

            //var dispatching_Standby_Status_Schedule_2 = new PickupSchedule(equipment2, setoutTime, orders[5].Goal.Origin, orders[5].PickupCargoInfo);
            //dispatching_Standby_Status_Schedule_2.SetProperty(s => s.Id, 10);
            //dispatching_Standby_Status_Schedule_1.SetProperty(s => s.Status, ScheduleStatus.Standby);
            //schedules.Add(dispatching_Standby_Status_Schedule_2);

            return schedules;
        }
    }

    public static class PropertyExtension
    {
        public static void SetProperty<T, TValue>(this T target, Expression<Func<T, TValue>> memberLambda, TValue value)
        {
            if (memberLambda.Body is MemberExpression memberSelectorExpression)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    property.SetValue(target, value);
                }
            }
        }

        public static TValue GetPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> memberLambda, TValue @default)
        {
            if (memberLambda.Body is MemberExpression memberSelectorExpression)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    return (TValue)property.GetValue(target);
                }
            }
            return @default;
        }
    }
}
