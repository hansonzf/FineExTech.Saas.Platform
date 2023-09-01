using DomainBase;

namespace Orderpool.Api.Models.OrderWatcherAggregate
{
    [Flags]
    public enum ProcessHandler
    {
        Undefined = 0,
        WmsStockHandler = 1,
        WmsCombineHandler = 2,
        TmsRouteHandler = 128,
    }

    public class OrderWatcher : Entity, IAggregateRoot
    {
        public long OriginOrderId { get; protected set; }
        public bool IsProcessing { get; protected set; }
        public string FetchId { get; protected set; }
        public DateTime PulledDate { get; protected set; }
        public DateTime CreatedTime { get; protected set; }
        public int ProcessedCount { get; protected set; }
        public ProcessHandler Handlers { get; protected set; }
        public int HandleResult { get; protected set; }
        public OrderDigest OrderInfo { get; protected set; }
        public List<StockDigest> StockInfo { get; protected set; }
        public List<TransportDigest> TransportInfo { get; protected set; }

        protected OrderWatcher()
        { }

        public OrderWatcher(OrderMetadata orderInfo)
        {
            IsProcessing = false;
            PulledDate = DateTime.Now.Date;
            CreatedTime = DateTime.Now;
            ProcessedCount = 0;
            Handlers = 0;
            OrderInfo = orderInfo.Digest;
        }

        public void RequireHandler(ProcessHandler handler)
        {
            if (!Handlers.HasFlag(handler))
                Handlers |= handler;
        }

        public void RemoveHandler(ProcessHandler handler)
        {
            if (Handlers.HasFlag(handler))
                Handlers &= ~handler;
        }

        public void Processed(int result)
        {
            ProcessedCount++;
            HandleResult = result;
        }

        public void ProcessedInventoryCheck(string sku, Dictionary<string, int> stockInfo)
        {

        }
    }
}
