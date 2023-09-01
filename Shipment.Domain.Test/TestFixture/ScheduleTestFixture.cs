using Moq;
using Shipment.Domain.Test.MockAggregate;
using Shippment.Domain.AggregateModels;
using Shippment.Domain.AggregateModels.EquipmentAggregate;
using Shippment.Domain.AggregateModels.RouterAggregate;
using Shippment.Domain.AggregateModels.ScheduleAggregate;
using Shippment.Domain.AggregateModels.TransportOrderAggregate;

namespace Shipment.Domain.Test.TestFixture
{
    public class ScheduleTestFixture
    {
        private readonly List<TransportSchedule> _scheduleTestData;
        private readonly EquipmentTestFixture equipmentTestDataFixture;
        private readonly RouteTestFixture routeTestFixture;
        private readonly TransportOrderTestFixture orderTestFixture;
        private Mock<ITransportScheduleRepository> _mockRepository;

        public ScheduleTestFixture()
        {
            _scheduleTestData = ScheduleProxy.SeedTestData();
            InitializeMockRepository();

            equipmentTestDataFixture = new EquipmentTestFixture();
            routeTestFixture = new RouteTestFixture();
            orderTestFixture = new TransportOrderTestFixture();
        }

        private void InitializeMockRepository()
        {
            _mockRepository = new Mock<ITransportScheduleRepository>();

            _mockRepository.Setup(rp => rp.GetTransportScheduleByEquipmentAsync(
                    It.Is<string>(identity => !string.IsNullOrEmpty(identity))))
                .ReturnsAsync(
                    (string identity) => _scheduleTestData.Where(
                        s => s.Equipment.Identifier == identity && s.Status == ScheduleStatus.Executed).FirstOrDefault());

            _mockRepository.Setup
                (rp => rp.GetAsync(It.Is<long>(id => id > 0 && id <= 10)))
                .ReturnsAsync(
                    (long id) => _scheduleTestData.FirstOrDefault(s => s.Id == id));
        }


        public IRouteRepository RouteRepository => routeTestFixture.RouteRepository;
        public IEquipmentRepository EquipmentRepository => equipmentTestDataFixture.EquipmentRepository;
        public ITransportScheduleRepository ScheduleRepository => _mockRepository.Object;
        public ITransportOrderRepository TransportOrderRepository => orderTestFixture.TransportOrderRepository;
        public TransportOrderTestFixture OrderTestFixture => orderTestFixture;
    }
}
