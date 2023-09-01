using MediatR;
using Orderpool.Api.Models;
using Orderpool.Api.Models.OrderWatcherAggregate;

namespace Orderpool.Api.Pipeline
{
    public class ProcessOrderContext : IRequest<OrderWatcher>
    {
        private OrderDigest _order;
        private ProcessHandler _handlers;
        private int _handleResult;
        private OrderWatcher _watcher;

        public ProcessOrderContext(OrderWatcher order)
        {
            _watcher = order;
            _handlers = ProcessHandler.Undefined;
            _handleResult = 0;
        }

        public bool IsCompleted => _handleResult == (int)_watcher.Handlers;
        public OrderWatcher Watcher => _watcher;
        public int HandleResult => _handleResult;

        public bool RequireCheckInventory
        {
            get
            {
                return _handlers.HasFlag(ProcessHandler.WmsStockHandler);
            }
        }

        public void CheckedInventory()
        {
            _handleResult = _handleResult | (int)ProcessHandler.WmsStockHandler;
        }
    }
}
