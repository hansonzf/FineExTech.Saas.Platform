using Shippment.Domain.AggregateModels.LocationAggregate;
using Shippment.Domain.AggregateModels.RouterAggregate;

namespace Shipment.Domain.Test.MockAggregate
{
    public class RouteProxy : Route
    {
        public RouteProxy(string nameOfRoute, LocationDescription origin, LocationDescription destination, Segment[] segments = default)
            : base(nameOfRoute, origin, destination, segments)
        { }

        public static List<Route> SeedTestData()
        {
            List<Route> store = new List<Route>();

            LocationDescription wh = new LocationDescription(1, "武汉");
            LocationDescription hf = new LocationDescription(2, "合肥");
            LocationDescription nj = new LocationDescription(3, "南京");
            LocationDescription sh = new LocationDescription(4, "上海");
            LocationDescription gz = new LocationDescription(5, "广州");
            LocationDescription gzg = new LocationDescription(6, "广州港出海口岸分部");
            LocationDescription singapore_hub = new LocationDescription(7, "新加坡港");
            LocationDescription singapore = new LocationDescription(8, "新加坡");
            LocationDescription singapore_proxy = new LocationDescription(9, "新加坡派件代理分部");


            var route1 = new RouteProxy("武汉 -> 上海 干线直达", wh, sh);
            route1.Id = 1;
            store.Add(route1);

            var route2 = new RouteProxy("武汉 -> 上海 干线中转（1）", wh, sh, 
                new Segment[2] { 
                    new Segment(wh, nj, 680),
                    new Segment(nj, sh, 240)
                });
            route2.Id = 2;
            store.Add(route2);

            var route3 = new RouteProxy("武汉 -> 上海 干线中转（2）", wh, sh,
                new Segment[3] {
                    new Segment(wh, hf, 350),
                    new Segment(hf, nj, 300),
                    new Segment(nj, sh, 240)
                });
            route3.Id = 3;
            store.Add(route3);

            var route4 = new RouteProxy("武汉 -> 新加坡 国际专线（4）", wh, singapore_proxy,
                new Segment[5] {
                    new Segment(wh, gz, 1009),
                    new Segment(gz, gzg, 87),
                    new Segment(gzg, singapore_hub, 2598),
                    new Segment(singapore_hub, singapore, 24),
                    new Segment(singapore, singapore_proxy, 62)
                });
            route4.Id = 4;
            store.Add(route4);

            var route5 = new RouteProxy("南京 -> 武汉 干线中转（1）", nj, wh, 
                new Segment[2] {
                    new Segment(nj, hf, 350),
                    new Segment(hf, wh, 300)
                });
            route5.Id = 5;
            store.Add(route5);

            return store;
        }
    }
}
