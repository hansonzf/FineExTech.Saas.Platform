using Shipment.Domain.Test.TestFixture;
using Shippment.Domain.AggregateModels;
using Shippment.Domain.AggregateModels.EquipmentAggregate;
using Shippment.Domain.AggregateModels.LocationAggregate;
using Shippment.Domain.AggregateModels.RouterAggregate;
using Shippment.Domain.AggregateModels.ScheduleAggregate;
using Shippment.Domain.AggregateModels.TransportOrderAggregate;
using Shippment.Domain.Events;

namespace Shipment.Domain.Test
{
    public class OperationCenterManageScheduleTest : IClassFixture<ScheduleTestFixture>
    {
        private IRouteRepository _routeRepository;
        private IEquipmentRepository _equipmentRepository;
        private ITransportScheduleRepository _scheduleRepository;
        private TransportOrderTestFixture _transportOrderTestFixture;

        public OperationCenterManageScheduleTest(ScheduleTestFixture fixture)
        {
            _routeRepository = fixture.RouteRepository;
            _equipmentRepository = fixture.EquipmentRepository;
            _scheduleRepository = fixture.ScheduleRepository;
            _transportOrderTestFixture = fixture.OrderTestFixture;
        }

        private async Task<TransportSchedule> GenerateScheduleWithStandbyStatus()
        {
            long routeId = 1; int legOfRoute = 0;
            DateTime setoutTime = new DateTime(2022, 11, 11, 12, 0, 0);
            var route = await _routeRepository.GetAsync(routeId);
            var equipments = await _equipmentRepository.GetAvailableEquipmentAsync(1);
            var mycar = equipments.First();
            var leg = route.GetRouteLeg(legOfRoute);
            var schedule = new TransportSchedule(leg, setoutTime, 10);
            schedule.DispatchingEquipment(mycar.Description);

            return schedule;
        }

        [Fact]
        public async Task Make_transport_schedule_should_success()
        {
            long routeId = 1; int legOfRoute = 0;
            DateTime setoutTime = new DateTime(2022, 11, 2, 12, 0, 0);
            var route = await _routeRepository.GetAsync(routeId);
            var leg = route.GetRouteLeg(legOfRoute);

            var schedule = new TransportSchedule(leg, setoutTime, 10);

            Assert.NotNull(schedule);
            Assert.Equal(1, schedule.From.LocationId);
            Assert.Equal(4, schedule.To.LocationId);
            Assert.Equal(ScheduleStatus.Created, schedule.Status);
        }

        [Fact]
        public async Task Dispatch_equipment_for_schedule_should_success()
        {
            long routeId = 1; int legOfRoute = 0;
            DateTime setoutTime = new DateTime(2022, 11, 2, 12, 0, 0);
            double interval = 10;
            var route = await _routeRepository.GetAsync(routeId);
            var leg = route.GetRouteLeg(legOfRoute);
            var equipments = await _equipmentRepository.GetAvailableEquipmentAsync(1);
            var equipment = equipments.First();

            var schedule = new TransportSchedule(leg, setoutTime, interval);
            schedule.DispatchingEquipment(equipment.Description);

            Assert.NotNull(schedule);
            Assert.Equal("鄂A62FD1", schedule.Equipment.Identifier);
            Assert.Equal(ScheduleStatus.Standby, schedule.Status);
            Assert.Collection(schedule.DomainEvents, (e) => {
                var evt = (e as ScheduleDispatchedEquipmentDomainEvent);
                Assert.NotNull(evt);
                Assert.Equal(equipment.Description, schedule.Equipment);
                Assert.Equal(setoutTime, evt.EstimateSetoutTime);
                Assert.Equal(interval, evt.EstimateTripInterval);
            });
        }

        [Fact]
        public async Task Loading_transport_cargo_to_schedule_should_success()
        {
            string trackingNumber = "TRS999999999";
            string barcode = "123456";
            Cargo cargo = new("测试货物", new Cube(0.2, 0.5, 0.4), new Weight(0.2), 5);
            var schedule = await GenerateScheduleWithStandbyStatus();
            // 计划中的体积装载率计算是，装载的货物总体积 / 运输车辆的最大容积
            // (本测试中，已装载的货物容积是0.2*0.5*0.4=0.04立方米 / 车辆的最大容积是0.6立方米)
            double expectVolumeRate = 6.67;
            // (本测试中，已装载货的总重量是0.2吨 / 车辆的最大载重是1吨)
            double expectWeightRate = 20;

            bool result = schedule.TakeOnCargo(trackingNumber, barcode, cargo);

            Assert.True(result);
            Assert.Equal(expectVolumeRate, schedule.VolumeLoadRate);
            Assert.Equal(expectWeightRate, schedule.WeightLoadRate);
        }

        [Fact]
        public async Task Loading_transport_cargo_but_without_available_tracking_number_should_failed()
        {
            string trackingNumber = "";
            string barcode = "123456";
            Cargo cargo = new("测试货物", new Cube(0.2, 0.5, 0.4), new Weight(0.2), 5);
            var schedule = await GenerateScheduleWithStandbyStatus();

            bool result = schedule.TakeOnCargo(trackingNumber, barcode, cargo);

            Assert.False(result);
        }

        [Fact]
        public async Task Unloading_transport_cargo_should_success()
        {
            string trackingNumber = "TRS999999999";
            Cargo cargo = new("测试货物", new Cube(0.2, 0.5, 0.4), new Weight(0.2), 5);
            Cargo willUnloadCargo = new("将被移除的货物", new Cube(0.4, 0.3, 0.6), new Weight(0.5), 5);
            var schedule = await GenerateScheduleWithStandbyStatus();
            double expectVolumeRate = 6.67;
            double expectWeightRate = 20;

            _ = schedule.TakeOnCargo(trackingNumber, "111111", cargo);
            _ = schedule.TakeOnCargo(trackingNumber, "222222", willUnloadCargo);
            bool result = schedule.TakeOffCargo(trackingNumber, "222222");

            Assert.True(result);
            Assert.Equal(expectVolumeRate, schedule.VolumeLoadRate);
            Assert.Equal(expectWeightRate, schedule.WeightLoadRate);
        }

        [Fact]
        public async Task Executing_transport_schedule_should_success()
        {
            LocationDescription departure = new LocationDescription(1, "武汉");
            // this mock schedule route is 武汉 -> 上海
            var schedule = await GenerateScheduleWithStandbyStatus();

            bool result = schedule.Execute(departure);

            Assert.True(result);
            Assert.Equal(ScheduleStatus.Executed, schedule.Status);
            Assert.Contains(schedule.DomainEvents, e => e is ScheduleExecutedDomainEvent);
        }

        [Fact]
        public async Task Cancel_unexecuted_schedule_should_success()
        {
            var schedule = await GenerateScheduleWithStandbyStatus();

            bool result = schedule.Cancel();

            Assert.True(result);
            Assert.Contains(schedule.DomainEvents, e => e is CancelScheduleDomainEvent);
        }

        [Fact]
        public async Task Cancel_executed_schedule_should_failed()
        {
            LocationDescription expectOrigin = new LocationDescription(1, "武汉");
            var schedule = await GenerateScheduleWithStandbyStatus();

            _ = schedule.Execute(expectOrigin);
            bool result = schedule.Cancel();

            Assert.False(result);
            Assert.DoesNotContain(schedule.DomainEvents, e => e is CancelScheduleDomainEvent);
        }

        [Fact]
        public async Task Change_unexecuted_schedule_setout_time_should_success()
        {
            DateTime expectSetoutTime = new DateTime(2022, 11, 11, 11, 11, 11);
            var schedule = await GenerateScheduleWithStandbyStatus();

            bool result = schedule.ChangeEstimateSetoutTime(expectSetoutTime);

            Assert.True(result);
            Assert.Equal(expectSetoutTime, schedule.Efficiency.EstimateSetoutTime);
            Assert.Contains(schedule.DomainEvents, e => e is ScheduleSetoutTimeChangedDomainEvent);
        }

        [Fact]
        public async Task Change_unexecuted_schedule_destination_should_success()
        {
            LocationDescription expectLocation = new LocationDescription(3, "南京");
            var schedule = await GenerateScheduleWithStandbyStatus();

            bool result = schedule.ChangeScheduleDestination(expectLocation);

            Assert.True(result);
            Assert.Equal(expectLocation, schedule.To);
            Assert.Contains(schedule.DomainEvents, e => e is ScheduleChangedDestinationDomainEvent);
        }

        [Fact]
        public async Task Delay_unexecuted_schedule_should_success()
        {
            double delayHours = 12;
            var schedule = await GenerateScheduleWithStandbyStatus();
            DateTime originalPlanSetoutTime = schedule.Efficiency.EstimateSetoutTime;
            DateTime expectingSetoutTime = originalPlanSetoutTime.AddHours(delayHours);

            bool result = schedule.DelayEstimateSetoutTimeByHours(delayHours);

            Assert.True(result);
            Assert.Equal(expectingSetoutTime, schedule.Efficiency.EstimateSetoutTime);
            Assert.Contains(schedule.DomainEvents, e => e is ScheduleSetoutTimeChangedDomainEvent);
        }

        [Fact]
        public async Task Ahead_unexecuted_schedule_should_success()
        {
            double aheadHours = 12;
            var schedule = await GenerateScheduleWithStandbyStatus();
            DateTime originalPlanSetoutTime = schedule.Efficiency.EstimateSetoutTime;
            DateTime expectingSetoutTime = originalPlanSetoutTime.AddHours(0 - aheadHours);

            bool result = schedule.AheadEstimateSetoutTimeByHours(aheadHours);

            Assert.True(result);
            Assert.Equal(expectingSetoutTime, schedule.Efficiency.EstimateSetoutTime);
            Assert.Contains(schedule.DomainEvents, e => e is ScheduleSetoutTimeChangedDomainEvent);
        }

        [Fact]
        public async Task When_carrier_arrived_destination_do_check_in_should_success()
        {
            LocationDescription originLocation = new LocationDescription(1, "武汉");
            LocationDescription expectDestination = new LocationDescription(4, "上海");
            // 该测试数据的始发地是武汉，目的地是上海
            var schedule = await GenerateScheduleWithStandbyStatus();

            _ = schedule.Execute(originLocation);
            bool result = schedule.CheckIn(expectDestination);

            Assert.True(result);
            Assert.NotNull(schedule.Efficiency.FactArrivedTime);
            Assert.Equal(ScheduleStatus.Completed, schedule.Status);
            Assert.Contains(schedule.DomainEvents, e => e is ArrivedTransportDestinationDomainEvent);
        }




        [Fact]
        public void Generate_pickup_schedule_should_success()
        {
            PickupDescription pickupDesc = new PickupDescription(
                true,
                EquipmentType.Vehicle,
                "武汉市洪山区东港科技园2栋5楼",
                "张峰", "18571855277",
                new DateTime(2022, 10, 28, 18, 0, 0),
                "货大约5个方，需要携带起重设备");
            var equipment = new EquipmentDescription(1, "鄂A62FD1", EquipmentType.Vehicle);
            var setoutTime = new DateTime(2022, 10, 31, 9, 0, 0);
            var from = new LocationDescription(1, "发网武汉仓");

            PickupSchedule actualSchedule = new PickupSchedule(equipment, setoutTime, from, pickupDesc);                

            Assert.NotNull(actualSchedule);
            Assert.Equal(ScheduleStatus.Standby, actualSchedule.Status);
            Assert.Equal(equipment, actualSchedule.Equipment);
            Assert.Equal(1, actualSchedule.From.LocationId);
            Assert.Equal(pickupDesc.DetailAddress, actualSchedule.To.LocationName);
            Assert.Equal(pickupDesc.ContactName, actualSchedule.PickupInfo.ContactName);
            Assert.Equal(pickupDesc.Phone, actualSchedule.PickupInfo.Phone);
            Assert.Equal(pickupDesc.PickupCode, actualSchedule.PickupInfo.PickupCode);
        }

        [Fact]
        public void Generate_pickup_schedule_with_invalid_pickup_information()
        {
            PickupDescription pickupDesc = new PickupDescription(
                true,
                EquipmentType.Vehicle,
                "武汉市洪山区东港科技园2栋5楼",
                "张峰", "56598952",
                new DateTime(2022, 10, 28, 18, 0, 0),
                "货大约5个方，需要携带起重设备");
            var equipment = new EquipmentDescription(1, "鄂A62FD1", EquipmentType.Vehicle);
            var setoutTime = new DateTime(2022, 10, 31, 9, 0, 0);
            var from = new LocationDescription(1, "发网武汉仓");

            Assert.Throws<InvalidOperationException>(() => {
                PickupSchedule actualSchedule = new PickupSchedule(equipment, setoutTime, from, pickupDesc);
            });
        }

        [Fact]
        public async Task Executing_dispatch_schedule_at_correct_setout_place_should_success()
        {
            var equipment = new EquipmentDescription(1, "鄂A62FD1", EquipmentType.Vehicle);
            var setoutTime = new DateTime(2022, 11, 11, 9, 0, 0);
            var from = new LocationDescription(1, "发网武汉仓");

            PickupSchedule actualSchedule = new (equipment, setoutTime, from, _transportOrderTestFixture.PickupInfo);
            bool result = actualSchedule.Execute(from);

            Assert.True(result);
            Assert.Equal(ScheduleStatus.Executed, actualSchedule.Status);
            Assert.Collection(actualSchedule.DomainEvents, e => {
                var evt = e as ScheduleExecutedDomainEvent;
                Assert.NotNull(evt);
                Assert.Equal(ScheduleType.Pickup, evt.Type);
            });
        }
    }
}
