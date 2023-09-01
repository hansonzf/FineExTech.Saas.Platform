using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Orderpool.Api.Models;
using Orderpool.Api.Models.OrderWatcherAggregate;

namespace Orderpool.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderWatchRepository _repository;
        private readonly ILogger _logger;

        public OrdersController(IOrderWatchRepository repository, ILogger<OrdersController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ImportOrders(ImportOrdersPayload payload)
        {
            _logger.LogTrace("{Method} are called with payload\r\n{Payload}", nameof(ImportOrders), payload.ToString());
            if (payload.Orders is null || !payload.Orders.Any())
            {
                _logger.LogDebug("{Method} are called with empty Payload", nameof(ImportOrders));
                return NoContent();
            }

            int count = payload.Orders.Count;
            List<OrderWatcher> watchers = new(count);
            List<OrderMetadata> orders = new(count);
            foreach (var item in payload.Orders)
            {
                var orderMetadata = new OrderMetadata(item.ToString());
                var watcher = new OrderWatcher(orderMetadata);
                watchers.Add(watcher);
                orders.Add(orderMetadata);
            }
            await _repository.BulkInsertImportedDataAsync(watchers, orders);

            return Ok();
        }
    }

    public class ImportOrdersPayload
    {
        public List<RemoteOrder> Orders { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        public class RemoteOrder
        {
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }
    }
}