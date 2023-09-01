using Shipment.Domain.Test.TestFixture;
using Shippment.Domain.AggregateModels;
using Shippment.Domain.AggregateModels.EquipmentAggregate;
using Shippment.Domain.AggregateModels.ItineraryAggregate;
using Shippment.Domain.AggregateModels.LocationAggregate;
using Shippment.Domain.AggregateModels.RouterAggregate;
using Shippment.Domain.AggregateModels.ScheduleAggregate;
using Shippment.Domain.AggregateModels.TransportOrderAggregate;
using Shippment.Domain.Events;
using Xunit.Abstractions;

namespace Shipment.Domain.Test
{
    public class LocationNodeOperationTest : IClassFixture<LocationNodeTestFixture>
    {
        private readonly ITestOutputHelper _output;
        private ITransportScheduleRepository _scheduleRepository;
        private IRouteRepository _routeRepository;
        private IEquipmentRepository _equipmentRepository;
        private LocationNodeTestFixture _fixture;

        public LocationNodeOperationTest(LocationNodeTestFixture fixture, ITestOutputHelper output)
        {
            _scheduleRepository = fixture.ScheduleRepository;
            _routeRepository = fixture.RouteRepository;
            _equipmentRepository = fixture.EquipmentRepository;
            _fixture = fixture;
            _output = output;
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
        public async Task Executing_schedule_from_correct_setout_location_should_success()
        {
            LocationDescription expectOrigin = new LocationDescription(1, "武汉");
            // 该测试数据的始发地是武汉，目的地是上海
            var schedule = await GenerateScheduleWithStandbyStatus();

            bool result = schedule.Execute(expectOrigin);

            Assert.True(result);
            Assert.NotNull(schedule.Efficiency.FactSetoutTime);
            Assert.Equal(ScheduleStatus.Executed, schedule.Status);
            Assert.Contains(schedule.DomainEvents, e => e is ScheduleExecutedDomainEvent);
        }

        [Fact]
        public async Task Executing_schedule_from_wrong_departure_should_failed()
        {
            LocationDescription wrongDeparture = new LocationDescription(4, "上海");
            // 该测试数据的始发地是武汉，目的地是上海
            var schedule = await GenerateScheduleWithStandbyStatus();

            bool result = schedule.Execute(wrongDeparture);

            Assert.False(result);
            Assert.Null(schedule.Efficiency.FactSetoutTime);
            Assert.Equal(ScheduleStatus.Standby, schedule.Status);
        }

        [Fact]
        public async Task When_carrier_get_ready_to_departure_but_with_wrong_status_of_schedule()
        {
            LocationDescription departure = new LocationDescription(2, "合肥");
            var schedule = await _scheduleRepository.GetTransportScheduleByEquipmentAsync("鄂A62FD1");

            Assert.Throws<InvalidOperationException>(() => {
                bool result = schedule.Execute(departure);
            });
        }

        [Fact]
        public async Task When_carrier_arrived_location_should_check_in()
        {
            LocationDescription destination = new LocationDescription(3, "南京");
            var schedule = await _scheduleRepository.GetTransportScheduleByEquipmentAsync("鄂A62FD1");

            bool result = schedule.CheckIn(destination);

            Assert.NotNull(schedule);
            Assert.True(result);
            Assert.Equal(ScheduleStatus.Completed, schedule.Status);
        }

        [Fact]
        public async Task When_carrier_arrived_location_check_in_with_wrong_location()
        {
            LocationDescription destination = new LocationDescription(4, "上海");
            var schedule = await _scheduleRepository.GetTransportScheduleByEquipmentAsync("鄂A62FD1");

            bool result = schedule.CheckIn(destination);

            Assert.NotNull(schedule);
            Assert.False(result);
        }

        private Route GenerateTestRoute()
        {
            LocationDescription origin = new LocationDescription(1, "发网武汉临空港一仓", "武汉");
            LocationDescription destination = new LocationDescription(2, "发网总公司", "上海");
            LocationDescription step1 = new LocationDescription(3, "发网南京中转站", "南京");

            Segment[] segments = new Segment[2]
            {
                new Segment(origin, step1, 500),
                new Segment(step1, destination, 400)
            };

            return new Route("测试路由", origin, destination, segments);
        }

        private Itinerary GenerateTestItinerary()
        {
            string trackingNumber = "TRS-1-00001";
            var route = GenerateTestRoute();
            var itinerary = new Itinerary(trackingNumber);
            itinerary.TrackRoute(route.Legs);

            return itinerary;
        }

        private List<Handing> GetHandingSteps(int stepCount)
        {
            LocationDescription origin = new LocationDescription(1, "发网武汉临空港一仓", "武汉");
            LocationDescription destination = new LocationDescription(2, "发网总公司", "上海");
            LocationDescription step1 = new LocationDescription(3, "发网南京中转站", "南京");

            List<Handing> handings = new List<Handing>
            {
                new LoadHanding(origin),
                new DepartureHanding(origin),
                new ArrivalHanding(step1),
                new UnloadHanding(step1),
                new LoadHanding(step1),
                new DepartureHanding(step1),
                new ArrivalHanding(destination),
                new UnloadHanding(destination)
        };

            return handings.Take(stepCount).ToList();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void Worker_scan_tracking_number_will_get_the_guid_of_cargo_future_path(int step)
        {
            string trackingNumber = "TRS-1-00001";
            var itinerary = GenerateTestItinerary();

            if (step > 0)
            {
                var handings = GetHandingSteps(step);
                handings.ForEach(handing => {
                    handing.Process(trackingNumber);
                    itinerary.Log(handing);
                });
            }

            if (step < 3)
            {
                Assert.Equal(3, itinerary.Next.Count);
                Assert.Equal("发网武汉临空港一仓", itinerary.Next[0].LocationName);
                Assert.Equal("发网南京中转站", itinerary.Next[1].LocationName);
                Assert.Equal("发网总公司", itinerary.Next[2].LocationName);
            }
            else if (step < 7)
            {
                Assert.Equal(2, itinerary.Next.Count);
                Assert.Equal("发网南京中转站", itinerary.Next[0].LocationName);
                Assert.Equal("发网总公司", itinerary.Next[1].LocationName);
            }
            else
            {
                Assert.Single(itinerary.Next);
                Assert.Equal("发网总公司", itinerary.Next[0].LocationName);
            }
            
        }

        [Fact]
        public void After_assign_order_to_schedule_should_update_itinerary_track_target()
        {
            string trackingNumber = "TRS-1-00001";
            // route WH -> HF -> NJ -SH
            var route = _fixture.RouteTestStore[2];

            var itinerary = new Itinerary(trackingNumber);
            itinerary.TrackRoute(route.Legs);

            Assert.Equal(4, itinerary.Next.Count);
            Assert.Equal("武汉", itinerary.Next[0].LocationName);
            Assert.Equal("合肥", itinerary.Next[1].LocationName);
            Assert.Equal("南京", itinerary.Next[2].LocationName);
            Assert.Equal("上海", itinerary.Next[3].LocationName);
        }

        [Fact]
        public void Worker_scan_tracking_number_and_loading_cargo()
        {
            string trackingNumber = "TRS-1-00001";
            LocationDescription wh = new LocationDescription(1, "发网武汉临空港一仓", "武汉");
            var itinerary = GenerateTestItinerary();

            var handing = new LoadHanding(wh);
            handing.Process(trackingNumber);
            itinerary.Log(handing);
            _output.WriteLine(itinerary.FlushLog());

            Assert.NotEmpty(itinerary.Handings);
            Assert.Equal(0, itinerary.CurrentLegIndex);
        }

        [Fact]
        public void Worker_scan_tracking_number_then_the_truck_depart()
        {
            string trackingNumber = "TRS-1-00001";
            var itinerary = GenerateTestItinerary();

            var handings = GetHandingSteps(2);
            handings.ForEach(handing => {
                handing.Process(trackingNumber);
                itinerary.Log(handing);
            });
            _output.WriteLine(itinerary.FlushLog());

            Assert.NotEmpty(itinerary.Handings);
        }

        [Fact]
        public void Truck_driver_arrived_destination_should_check_in()
        {
            string trackingNumber = "TRS-1-00001";
            var itinerary = GenerateTestItinerary();

            var handings = GetHandingSteps(3);
            handings.ForEach(handing => {
                handing.Process(trackingNumber);
                itinerary.Log(handing);
            });
            _output.WriteLine(itinerary.FlushLog());

            Assert.NotEmpty(itinerary.Handings);
        }

        [Fact]
        public void Worker_scan_tracking_number_will_get_cargo_future_path()
        {
            string trackingNumber = "TRS-1-00001";
            var itinerary = GenerateTestItinerary();

            var handings = GetHandingSteps(3);
            handings.ForEach(handing => {
                handing.Process(trackingNumber);
                itinerary.Log(handing);
            });
            _output.WriteLine(itinerary.FlushLog());

            Assert.Equal("发网南京中转站", itinerary.Next[0].LocationName);
            Assert.Equal("发网总公司", itinerary.Next[1].LocationName);
        }

        [Fact]
        public void After_truck_driver_checked_in_worker_should_unloading_cargos()
        {
            string trackingNumber = "TRS-1-00001";
            var itinerary = GenerateTestItinerary();

            var handings = GetHandingSteps(4);
            handings.ForEach(handing => {
                handing.Process(trackingNumber);
                itinerary.Log(handing);
            });
            _output.WriteLine(itinerary.FlushLog());

            Assert.NotEmpty(itinerary.Handings);
        }

        [Fact]
        public void Truck_arrived_second_segment_of_route()
        {
            string trackingNumber = "TRS-1-00001";
            var itinerary = GenerateTestItinerary();

            var handings = GetHandingSteps(6);
            handings.ForEach(handing => {
                handing.Process(trackingNumber);
                itinerary.Log(handing);
            });
            _output.WriteLine(itinerary.FlushLog());

            Assert.NotEmpty(itinerary.Handings);
        }

        [Fact]
        public void Examine_cargo_whether_specified_goal_when_the_route_not_complete()
        {
            string trackingNumber = "TRS-1-00001";
            DeliverySpecification specification = new DeliverySpecification(
                new LocationDescription(1, "发网武汉临空港一仓", "武汉"),
                new LocationDescription(3, "发网南京中转站", "南京"));
            var itinerary = GenerateTestItinerary();

            var handings = GetHandingSteps(3);
            handings.ForEach(handing => {
                handing.Process(trackingNumber);
                itinerary.Log(handing);
            });
            bool isMatch = itinerary.IsMatchDeliveryGoal(specification);

            Assert.True(isMatch);
        }

        [Fact]
        public void Examine_cargo_whether_specified_goal()
        {
            string trackingNumber = "TRS-1-00001";
            DeliverySpecification specification = new DeliverySpecification(
                new LocationDescription(1, "发网武汉临空港一仓", "武汉"),
                new LocationDescription(2, "发网总公司", "上海"));
            var itinerary = GenerateTestItinerary();

            var handings = GetHandingSteps(3);
            handings.ForEach(handing => {
                handing.Process(trackingNumber);
                itinerary.Log(handing);
            });
            bool isMatch = itinerary.IsMatchDeliveryGoal(specification);

            Assert.False(isMatch);
        }
    }
}