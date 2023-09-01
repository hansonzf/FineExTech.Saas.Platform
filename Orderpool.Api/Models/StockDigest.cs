namespace Orderpool.Api.Models
{
    public class StockDigest
    {
        public Guid Id { get; set; }
        public long WatcherId { get; set; }
        public int OriginOrderId { get; set; }
        public string SKU { get; set; }
        public int OrderQty { get; set; }
        public long WarehouseId { get; set; }
        public int StockQty { get; set; }
        public double DistanceFromTarget { get; set; }
    }
}
