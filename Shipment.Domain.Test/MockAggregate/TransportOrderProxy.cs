using Shippment.Domain.AggregateModels;
using Shippment.Domain.AggregateModels.LocationAggregate;
using Shippment.Domain.AggregateModels.TransportOrderAggregate;

namespace Shipment.Domain.Test.MockAggregate
{
    internal class TransportOrderProxy : TransportOrder
    {
        public TransportOrderProxy()
        { }

        public TransportOrderProxy(long customerId, DeliverySpecification goal, IEnumerable<Cargo> cargoList)
            : base(customerId, goal, cargoList)
        { }

        public TransportOrderProxy(long customerId, DeliverySpecification goal, PickupDescription pickupCargo, IEnumerable<Cargo> cargoList)
            : base(customerId, goal, pickupCargo, cargoList)
        { }

        public static List<TransportOrder> SeedTestData()
        {
            List<TransportOrder> _orders = new List<TransportOrder>();

            LocationDescription wh = new LocationDescription(1, "武汉");
            LocationDescription hf = new LocationDescription(2, "合肥");
            LocationDescription nj = new LocationDescription(3, "南京");
            LocationDescription sh = new LocationDescription(4, "上海");

            TransportOrder order;
            PickupDescription pickupDesc = new PickupDescription(
                true,
                EquipmentType.Vehicle,
                "武汉市洪山区东港科技园2栋5楼",
                "张峰", "18571855277",
                new DateTime(2022, 10, 28, 18, 0, 0),
                "货大约5个方，需要携带起重设备");

            Dictionary<string, Cargo> additionalCargo = new Dictionary<string, Cargo>();
            additionalCargo.Add("12345",
                new Cargo(
                    "1立方米的水",
                    new Cube(new Line(1), new Line(1), new Line(1)),
                    new Weight(1),
                    9
                ));

            IEnumerable<Cargo> cargos = new List<Cargo>
            {
                new Cargo("水泥", new Cube(2, 3, 1.2), new Weight(12, UnitOfWeight.Tonne), 9),
                new Cargo("砖头", new Cube(4, 5, 2), new Weight(25, UnitOfWeight.Tonne), 9),
                new Cargo("电冰箱10台", new Cube(8.2, 7.8, 1.76), new Weight(1596, UnitOfWeight.KiloGram), 4),
            };

            order = new TransportOrderProxy(1, new DeliverySpecification(wh, sh), cargos) 
            { 
                Id = 100 
            };
            _orders.Add(order);

            order = new TransportOrderProxy(1, new DeliverySpecification(wh, nj), pickupDesc, cargos) 
            { 
                Id = 110 
            };
            _orders.Add(order);

            order = new TransportOrderProxy(1, new DeliverySpecification(wh, nj), cargos) 
            { 
                Id = 200,
                OrderTime = DateTime.Now,
                Status = OrderStatus.Ordered
            };
            _orders.Add(order);

            order = new TransportOrderProxy(1, new DeliverySpecification(wh, nj), pickupDesc, cargos)
            {
                Id = 210,
                OrderTime = DateTime.Now,
                Status = OrderStatus.Ordered
            };
            _orders.Add(order);

            order = new TransportOrderProxy(1, new DeliverySpecification(wh, nj), cargos)
            { 
                Id = 300,
                Status = OrderStatus.Accepted,
                ScheduleId = 1,
            };
            _orders.Add(order);

            order = new TransportOrderProxy(1, new DeliverySpecification(wh, hf), pickupDesc, cargos)
            {
                Id = 310,
                Status = OrderStatus.Accepted,
                ScheduleId = 2,
            };
            _orders.Add(order);

            order = new TransportOrderProxy(1, new DeliverySpecification(wh, hf), pickupDesc, cargos)
            {
                Id = 311,
                Status = OrderStatus.Accepted,
                ScheduleId = 3,
            };
            _orders.Add(order);

            order = new TransportOrderProxy(1, new DeliverySpecification(wh, hf), pickupDesc, cargos)
            {
                Id = 420,
                Status = OrderStatus.Accepted,
                ScheduleId = 2,
            };
            order.CheckCargo(additionalCargo);
            _orders.Add(order);
            
            return _orders;
        }
    }
}
