using Shipment.Domain.Test.MockAggregate;
using Shippment.Domain.AggregateModels;
using Shippment.Domain.AggregateModels.EquipmentAggregate;
using Shippment.Domain.AggregateModels.LocationAggregate;
using Shippment.Domain.AggregateModels.RouterAggregate;
using Shippment.Domain.AggregateModels.ScheduleAggregate;
using Shippment.Domain.AggregateModels.TransportOrderAggregate;

namespace Shipment.Domain.Test.TestFixture
{
    public class LocationNodeTestFixture
    {
        private readonly ScheduleTestFixture scheduleTestFixture;
        private readonly RouteTestFixture routeTestFixture;
        private readonly EquipmentTestFixture equipmentTestFixture;
        private readonly List<Route> _routeTestStore;

        public LocationNodeTestFixture()
        {
            scheduleTestFixture = new ScheduleTestFixture();
            routeTestFixture = new RouteTestFixture();
            equipmentTestFixture = new EquipmentTestFixture();
            _routeTestStore = RouteProxy.SeedTestData();
        }

        public IRouteRepository RouteRepository => routeTestFixture.RouteRepository;
        public ITransportScheduleRepository ScheduleRepository => scheduleTestFixture.ScheduleRepository;
        public IEquipmentRepository EquipmentRepository => equipmentTestFixture.EquipmentRepository;
        public List<Route> RouteTestStore => _routeTestStore;
        public LocationDescription WUHAN => new LocationDescription(1, "武汉");
        public LocationDescription SHANGHAI => new LocationDescription(2, "上海");
        public PickupDescription PickupInfo => new PickupDescription(
                true,
                EquipmentType.Vehicle,
                "武汉市洪山区东港科技园2栋5层",
                "张峰",
                "18571855277",
                new DateTime(2022, 10, 28, 20, 0, 0),
                "");
        public IEnumerable<Cargo> Cargos = new List<Cargo>
        {
            new Cargo("20个iphone 14 pro", new Cube(0.5, 0.4, 0.8), new Weight(12, UnitOfWeight.KiloGram), 4),
            new Cargo("20个iphone 14 pro max", new Cube(0.6, 0.5, 1), new Weight(15, UnitOfWeight.KiloGram), 4),
        };
    }
}
