using Shipment.Domain.Test.MockAggregate;
using Shipment.Domain.Test.TestFixture;
using Shippment.Domain;
using Shippment.Domain.AggregateModels;
using Shippment.Domain.AggregateModels.EquipmentAggregate;
using Shippment.Domain.AggregateModels.LocationAggregate;

namespace Shipment.Domain.Test
{
    public class LocationNodeManageEquipmentTest : IClassFixture<EquipmentTestFixture>
    {
        private readonly IEquipmentRepository _equipmentRepository;

        public LocationNodeManageEquipmentTest(EquipmentTestFixture fixture)
        {
            _equipmentRepository = fixture.EquipmentRepository;
        }

        [Fact]
        public void Create_equipment_should_success()
        {
            LocationDescription expectLocation = new LocationDescription(100, "武汉发网科技");
            string expectIdentity = "鄂A00001";
            bool expectIsSelfSupport = true;

            var equipment = new EquipmentProxy(expectIdentity, expectIsSelfSupport, expectLocation);

            Assert.NotNull(equipment);
            Assert.Equal(expectIdentity, equipment.Description.Identifier);
            Assert.True(equipment.IsSelfSupport);
            Assert.Equal(expectLocation.LocationId, equipment.CurrentLocation.LocationId);
        }

        [Fact]
        public void New_equipment_should_available()
        {
            LocationDescription expectLocation = new LocationDescription(100, "武汉发网科技");
            string expectIdentity = "鄂A00001";
            DateTime maxDateTime = DateTimeConstant.MaxDateTime;

            var equipment = new EquipmentProxy(expectIdentity, true, expectLocation);

            Assert.NotNull(equipment);
            Assert.False(equipment.IsInuse);
            Assert.Equal(maxDateTime, equipment.EstimateReleaseTime);
            Assert.Null(equipment.Destination);
        }

        [Fact]
        public async Task Add_Description_for_equipment_should_success()
        {
            LocationDescription expectLocation = new LocationDescription(100, "武汉发网科技");
            string expectIdentity = "鄂AFZ52M";
            var expectDescription = new EquipmentDescription(3, expectIdentity, EquipmentType.Vehicle, 0.6, 1);

            var equipment = await _equipmentRepository.GetAsync(3);
            equipment.AddEquipmentDescription(EquipmentType.Vehicle, 0.6, 1);

            Assert.Equal(expectDescription, equipment.Description);
            Assert.Equal(EquipmentType.Vehicle, equipment.Type);
        }

        [Fact]
        public async Task Assign_task_to_equiment_should_success()
        {
            var equipment = await _equipmentRepository.GetAsync(1);
            DateTime setoutTime = new DateTime(2022, 11, 1, 12, 0, 0);
            double interval = 8;
            DateTime expectReleaseTime = setoutTime.AddHours(interval);
            LocationDescription destination = new LocationDescription(200, "武汉网点一部");

            equipment.AssignTask(setoutTime, interval, destination);

            Assert.True(equipment.IsInuse);
            Assert.Equal(destination.LocationId, equipment.Destination.LocationId);
            Assert.Equal(expectReleaseTime, equipment.EstimateReleaseTime);
        }

        [Fact]
        public async Task Assign_task_to_equipment_which_already_has_task_should_return_false()
        {
            var equipment = await _equipmentRepository.GetAsync(2);
            DateTime setoutTime = new DateTime(2022, 11, 1, 12, 0, 0);
            double interval = 8;
            DateTime expectReleaseTime = setoutTime.AddHours(interval);
            LocationDescription destination = new LocationDescription(200, "武汉网点一部");

            bool assignResult = equipment.AssignTask(setoutTime, interval, destination);

            Assert.False(assignResult);
        }

        [Fact]
        public void Cancel_available_equipment_task_should_get_false_result()
        {
            LocationDescription setoutLocation = new LocationDescription(100, "武汉发网科技");
            string expectIdentity = "鄂A00001";
            var equipment = new EquipmentProxy(expectIdentity, true, setoutLocation);

            var cancelResult = equipment.CancelTask();

            Assert.False(cancelResult);
        }

        [Fact]
        public async Task Cancel_equipment_task_should_success()
        {
            LocationDescription expectLocation = new LocationDescription(100, "武汉发网科技");
            string expectIdentity = "鄂A00001";
            DateTime setoutTime = new DateTime(2022, 11, 1, 12, 0, 0);
            double interval = 8;
            LocationDescription destination = new LocationDescription(200, "武汉网点一部");

            var equipment = new EquipmentProxy(expectIdentity, true, expectLocation);
            equipment.AssignTask(setoutTime, interval, destination);
            equipment.CancelTask();

            Assert.False(equipment.IsInuse);
            Assert.Null(equipment.Destination);
            Assert.Equal(DateTimeConstant.MinDateTime, equipment.EstimateReleaseTime);
        }

        [Fact]
        public void Complete_task_equipment_should_arrived_another_location()
        {
            LocationDescription setoutLocation = new LocationDescription(100, "武汉发网科技");
            string expectIdentity = "鄂A00001";
            DateTime setoutTime = new DateTime(2022, 11, 1, 12, 0, 0);
            double interval = 8;
            DateTime expectReleaseTime = setoutTime.AddHours(interval);
            LocationDescription destination = new LocationDescription(200, "武汉网点一部");

            var equipment = new EquipmentProxy(expectIdentity, true, setoutLocation);
            bool actualAssginedTaskResult = equipment.AssignTask(setoutTime, interval, destination);
            equipment.Arrived(destination);

            Assert.True(actualAssginedTaskResult);
            Assert.Equal(destination.LocationId, equipment.CurrentLocation.LocationId);
            Assert.Null(equipment.Destination);
            Assert.False(equipment.IsInuse);
        }
    }
}
