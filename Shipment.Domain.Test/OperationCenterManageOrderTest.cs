using Shipment.Domain.Test.TestFixture;
using Shippment.Domain.AggregateModels;
using Shippment.Domain.AggregateModels.EquipmentAggregate;
using Shippment.Domain.AggregateModels.TransportOrderAggregate;
using Shippment.Domain.Events;

namespace Shipment.Domain.Test
{
    public class OperationCenterManageOrderTest : IClassFixture<TransportOrderTestFixture>
    {
        private readonly TransportOrderTestFixture _fixture;
        private readonly ITransportOrderRepository _transportOrderRepository;

        public OperationCenterManageOrderTest(TransportOrderTestFixture fixture)
        {
            _fixture = fixture;
            _transportOrderRepository = fixture.TransportOrderRepository;
        }

        [Fact]
        public void Submit_transport_order_should_be_ordered_status()
        {
            DeliverySpecification specification = new DeliverySpecification(_fixture.WUHAN, _fixture.SHANGHAI);

            var order = new TransportOrder(1, specification, _fixture.Cargos);
            order.Submit();

            Assert.Equal(OrderStatus.Ordered, order.Status);
        }

        [Fact]
        public async Task Audit_transport_order_without_pickup_service_should_be_accept_status()
        {
            var orders = await _transportOrderRepository.GetWaitforAuditOrdersAsync();

            var order = orders.First();
            order.Accept();

            Assert.Equal(OrderStatus.Accepted, order.Status);
            Assert.Collection(order.DomainEvents, e => {
                var evt = e as AcceptTransportOrderDomainEvent;
                Assert.NotNull(evt);
            });
        }

        [Fact]
        public async Task Audit_transport_order_with_pickup_service_should_be_pickup_status()
        {
            // if shipper choose pickup cargo service,
            //   then his order after accpet will on the pickup status
            // else
            //   his order will be accpet status
            EquipmentDescription pickupEquipment = new EquipmentDescription(1, "鄂A00001", EquipmentType.Vehicle);
            var orders = await _transportOrderRepository.GetWaitforAuditOrdersAsync();
            var order = orders.Last();

            order.Accept(pickupEquipment);

            Assert.Equal(OrderStatus.Pickup, order.Status);
            Assert.Collection(order.DomainEvents, e => {
                var evt = e as AcceptTransportOrderDomainEvent;
                Assert.NotNull(evt);
            });
        }
        
        [Fact]
        public async Task Check_only_on_pickup_and_accept_status_order_could_check_cargos()
        {
            DeliverySpecification specification = new DeliverySpecification(_fixture.WUHAN, _fixture.SHANGHAI);
            var order = new TransportOrder(1, specification, _fixture.Cargos);
            order.Submit();

            Assert.Throws<InvalidOperationException>(() => {
                order.CheckCargo(new Dictionary<string, Cargo>());
            });
        }
        
        [Fact]
        public async Task Check_transport_order_cargos_should_standby_status()
        {
            Cargo[] cargos = new Cargo[3]
            {
                new Cargo("货A", new Cube(new Line(1), new Line(1.5), new Line(0.8)), new Weight(1), 9),
                new Cargo("货B", new Cube(new Line(0.5), new Line(1.2), new Line(0.6)), new Weight(1.5), 6),
                new Cargo("货C", new Cube(new Line(2), new Line(1.3), new Line(0.9)), new Weight(0.6), 3)
            };
            Dictionary<string, Cargo> pickedCargos = cargos.ToDictionary(c => c.GetHashCode().ToString("000000"));
            DeliverySpecification specification = new DeliverySpecification(_fixture.WUHAN, _fixture.SHANGHAI);
            var order = new TransportOrder(1, specification, _fixture.Cargos);
            order.Submit();
            order.Accept();

            order.CheckCargo(pickedCargos);

            Assert.Equal(OrderStatus.Standby, order.Status);
            Assert.Equal(3, order.CargoList.Count);
            Assert.Contains(order.DomainEvents, e => e is CheckedTransportCargoDomainEvent);
        }
        
        [Fact]
        public async Task Check_transport_order_with_empty_cargos_should_throw_argument_null_exception()
        {
            var order = await _transportOrderRepository.GetAsync(310);

            Assert.Throws<ArgumentNullException>(() => { 
                order.CheckCargo(new Dictionary<string, Cargo>());
            });
        }

        [Fact]
        public async Task Schedule_pickup_should_failed_when_order_is_draft()
        {
            var order = await _transportOrderRepository.GetAsync(110);
            PickupDescription desc = order.PickupCargoInfo;
            string expectPickupCode = "1234";
            var expectEquip = new EquipmentDescription(1, "鄂A62FD1", EquipmentType.Vehicle);

            Assert.Throws<InvalidOperationException>(() => {
                order.SchedulePickupService(1, expectPickupCode, expectEquip);
            });
        }

        [Fact]
        public async Task Schedule_pickup_should_failed_when_order_is_ordered()
        {
            var order = await _transportOrderRepository.GetAsync(210);
            PickupDescription desc = order.PickupCargoInfo;
            string expectPickupCode = "1234";
            var expectEquip = new EquipmentDescription(1, "鄂A62FD1", EquipmentType.Vehicle);

            Assert.Throws<InvalidOperationException>(() => {
                order.SchedulePickupService(1, expectPickupCode, expectEquip);
            });
        }

        [Fact]
        public async Task Schedule_pickup_should_success_when_order_is_accept()
        {
            var order = await _transportOrderRepository.GetAsync(310);
            PickupDescription desc = order.PickupCargoInfo;
            string expectPickupCode = "1234";
            var expectEquip = new EquipmentDescription(1, "鄂A62FD1", EquipmentType.Vehicle);
            long scheduleId = 100;

            order.SchedulePickupService(scheduleId, expectPickupCode, expectEquip);

            Assert.Equal(expectPickupCode, order.PickupCargoInfo.PickupCode);
            Assert.Equal(scheduleId, order.PickupCargoInfo.DispatchingId);
            Assert.Equal("鄂A62FD1", order.PickupCargoInfo.DispatchedEquipment.Identifier);
        }
    }
}
