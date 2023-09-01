using Shippment.Domain;
using Shippment.Domain.AggregateModels;
using Shippment.Domain.AggregateModels.EquipmentAggregate;
using Shippment.Domain.AggregateModels.LocationAggregate;

namespace Shipment.Domain.Test.MockAggregate
{
    public class EquipmentProxy : Equipment
    {
        public EquipmentProxy() { }

        public EquipmentProxy(string identifier, bool isSelfSupport, LocationDescription parkingLocation)
            : base(identifier, isSelfSupport, parkingLocation)
        {
            Id = new Random(1).NextInt64(1, 9999999);
        }

        public static List<Equipment> SeedTestData()
        {
            List<Equipment> equipmentList = new List<Equipment>();
            var car1 = new EquipmentProxy
            {
                Id = 1,
                Identifier = "鄂A62FD1",
                Type = EquipmentType.Vehicle,
                MaxLoadVolume = 0.6,
                MaxLoadWeight = 1,
                CurrentLocation = new LocationDescription(1, "武汉公司"),
                IsInuse = false,
                IsSelfSupport = true,
                EstimateReleaseTime = DateTimeConstant.MaxDateTime
            };
            equipmentList.Add(car1);

            var car2 = new EquipmentProxy
            {
                Id = 2,
                Identifier = "鄂AM73Z7",
                Type = EquipmentType.Vehicle,
                MaxLoadVolume = 0.5,
                MaxLoadWeight = 0.7,
                CurrentLocation = new LocationDescription(1, "武汉公司"),
                IsInuse = true,
                IsSelfSupport = true,
                Destination = new LocationDescription(200, "武汉网点一部"),
                EstimateReleaseTime = new DateTime(2022, 11, 2)
            };
            equipmentList.Add(car2);

            var car3 = new EquipmentProxy
            {
                Id = 3,
                Identifier = "鄂AFZ52M",
                CurrentLocation = new LocationDescription(1, "武汉公司"),
                IsInuse = false,
                IsSelfSupport = true,
                EstimateReleaseTime = DateTimeConstant.MaxDateTime
            };
            equipmentList.Add(car3);

            return equipmentList;
        }
    }
}
