using Microsoft.VisualStudio.TestPlatform.Utilities;
using Shipment.Domain.Test.TestFixture;
using Shippment.Domain.AggregateModels;
using Shippment.Domain.AggregateModels.ItineraryAggregate;
using Shippment.Domain.AggregateModels.LocationAggregate;
using Shippment.Domain.AggregateModels.TransportOrderAggregate;
using Shippment.Domain.Events;

namespace Shipment.Domain.Test
{
    public class CustomerBookingServiceTest : IClassFixture<CustomerTestFixture>
    {
        private readonly CustomerTestFixture _fixture;
        private readonly ITransportOrderRepository _repository;

        public CustomerBookingServiceTest(CustomerTestFixture fixture)
        {
            _fixture = fixture;
            _repository = fixture.TransportOrderRepository;
        }

        [Fact]
        public void Create_transport_order_should_be_draft_status()
        {
            DeliverySpecification specification = new DeliverySpecification(_fixture.WUHAN, _fixture.SHANGHAI);

            var order = new TransportOrder(1, specification, _fixture.Cargos);

            Assert.Equal(OrderStatus.Draft, order.Status);
            Assert.Null(order.OrderTime);
            Assert.Equal("武汉", order.Goal.Origin.LocationName);
            Assert.Equal("上海", order.Goal.Destination.LocationName);
            Assert.NotEmpty(order.CargoList);
        }

        [Fact]
        public void Create_transport_order_with_pickup_cargo_service()
        {
            DeliverySpecification specification = new DeliverySpecification(_fixture.WUHAN, _fixture.SHANGHAI);

            var order = new TransportOrder(1, specification, _fixture.PickupInfo, _fixture.Cargos);

            Assert.Equal(OrderStatus.Draft, order.Status);
            Assert.Null(order.OrderTime);
            Assert.True(order.PickupCargoInfo.NeedPickupService);
            Assert.False(order.IsPickupDispatched());
        }

        [Fact]
        public void Submit_transport_order_without_pickup_service()
        {
            DeliverySpecification specification = new DeliverySpecification(_fixture.WUHAN, _fixture.SHANGHAI);

            var order = new TransportOrder(1, specification, _fixture.Cargos);
            order.Submit();

            Assert.Equal(OrderStatus.Ordered, order.Status);
            Assert.NotNull(order.OrderTime);
            Assert.Equal("武汉", order.Goal.Origin.LocationName);
            Assert.Equal("上海", order.Goal.Destination.LocationName);
        }

        [Fact]
        public void Repeat_submit_order_should_throw_exception()
        {
            DeliverySpecification specification = new DeliverySpecification(_fixture.WUHAN, _fixture.SHANGHAI);

            var order = new TransportOrder(1, specification, _fixture.Cargos);
            order.Submit();

            Assert.Throws<InvalidOperationException>(() => {
                order.Submit();
            });
        }

        [Fact]
        public async Task Customer_change_pickup_cargo_address_and_remark_when_order_in_accept_status_should_success()
        {
            var order = await _repository.GetAsync(310);
            string expectAddr = "武汉市洪山区光谷现代世贸中心A栋";
            string expectContact = order.PickupCargoInfo.ContactName;
            string expectPhone = order.PickupCargoInfo.Phone;
            string expectRemark = "我改变了备注信息";

            order.ChangePickupCargoLocation(expectAddr, "", "", expectRemark);

            Assert.True(order.PickupCargoInfo.NeedPickupService);
            Assert.Equal(OrderStatus.Ordered, order.Status);
            Assert.Equal(expectAddr, order.PickupCargoInfo.DetailAddress);
            Assert.Equal(expectContact, order.PickupCargoInfo.ContactName);
            Assert.Equal(expectPhone, order.PickupCargoInfo.Phone);
            Assert.Equal(expectRemark, order.PickupCargoInfo.Remark);
        }

        [Fact]
        public async Task Customer_change_pickup_cargo_contact_and_phone_when_order_in_accept_status_should_success()
        {
            var order = await _repository.GetAsync(311);
            string expectAddr = order.PickupCargoInfo.DetailAddress;
            string expectContact = "Hanson";
            string expectPhone = "18666666666";
            string expectRemark = order.PickupCargoInfo.Remark;

            order.ChangePickupCargoLocation("", expectContact, expectPhone, "");

            Assert.True(order.PickupCargoInfo.NeedPickupService);
            Assert.Equal(OrderStatus.Ordered, order.Status);
            Assert.Equal(expectAddr, order.PickupCargoInfo.DetailAddress);
            Assert.Equal(expectContact, order.PickupCargoInfo.ContactName);
            Assert.Equal(expectPhone, order.PickupCargoInfo.Phone);
            Assert.Equal(expectRemark, order.PickupCargoInfo.Remark);
        }

        [Fact]
        public async Task Standby_transport_order_could_not_modify_pickup_information()
        {
            string expectContact = "Hanson";
            string expectPhone = "17007123810";
            var order = await _repository.GetAsync(420);

            bool actual = order.ChangePickupCargoLocation("", expectContact, expectPhone, "");

            Assert.False(actual);
        }

        [Fact]
        public async Task Draft_status_transport_order_modify_pickup_should_success()
        {
            string expectContact = "Hanson";
            string expectPhone = "17007123810";
            var order = await _repository.GetAsync(110);

            bool actual = order.ChangePickupCargoLocation("", expectContact, expectPhone, "");

            Assert.True(actual);
            Assert.Equal(OrderStatus.Draft, order.Status);
        }

        [Fact]
        public void Generate_itinerary_with_tracking_number_should_success()
        {
            string trackingNumber = "TRS-1-00001";

            var itinerary = new Itinerary(trackingNumber);

            Assert.NotNull(itinerary);
            Assert.Equal(trackingNumber, itinerary.TrackingNumber);
            Assert.Equal(0, itinerary.CurrentLegIndex);
        }

        

        [Fact]
        public void Assigned_schedule_executed_but_not_complete_then_changed_the_destination()
        {
            string trackingNumber = "TRS-1-00001";
            // route WH -> HF -> NJ -SH
            var route = _fixture.RouteTestStore[2];
            var backRoute = _fixture.RouteTestStore[4];

            var itinerary = new Itinerary(trackingNumber);
            itinerary.TrackRoute(route.Legs);
            var handings = GetHandingSteps(6);
            handings.ForEach(handing => {
                handing.Process(trackingNumber);
                itinerary.Log(handing);
            });
            itinerary.TrackRoute(backRoute.Legs);

            Assert.Equal("合肥", itinerary.Next[0].LocationName);
            Assert.Equal("南京", itinerary.Next[1].LocationName);
            Assert.Equal("合肥", itinerary.Next[2].LocationName);
            Assert.Equal("武汉", itinerary.Next[3].LocationName);
        }

        private List<Handing> GetHandingSteps(int step)
        {
            LocationDescription wh = new LocationDescription(1, "武汉");
            LocationDescription hf = new LocationDescription(2, "合肥");
            LocationDescription nj = new LocationDescription(3, "南京");


            List<Handing> handings = new List<Handing>
            {
                new LoadHanding(wh),
                new DepartureHanding(wh),
                new ArrivalHanding(hf),
                new UnloadHanding(hf),
                new LoadHanding(hf),
                new DepartureHanding(hf),
                new ArrivalHanding(nj),
                new UnloadHanding(nj)
        };

            return handings.Take(step).ToList();
        }
    }
}
