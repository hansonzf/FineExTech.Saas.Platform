using DomainBase;

namespace Shippment.Domain.AggregateModels.TransportOrderAggregate
{
    public class TransportCargo : Entity
    {
        internal TransportCargo(long orderId, string barCode, Cargo cargoInfo)
        {
            OrderId = orderId;
            BarCode = barCode;
            CargoInfo = cargoInfo;
        }
    
        public long OrderId { get; protected set; }
        public string BarCode { get; protected set; }
        public Cargo CargoInfo { get; protected set; }
    }
}
