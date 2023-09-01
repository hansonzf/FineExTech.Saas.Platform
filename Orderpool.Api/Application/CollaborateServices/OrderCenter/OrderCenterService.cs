using Orderpool.Api.Models;
using Orderpool.Api.Services;

namespace Orderpool.Api.Application.CollaborateServices.OrderCenter
{
    public class OrderCenterService : IOrderCenterSerivce
    {
        private readonly OrderCenterHttpAdapter _adapter;

        public OrderCenterService(OrderCenterHttpAdapter adapter)
        {
            _adapter = adapter;
        }

        public async Task<IEnumerable<OrderDigest>> PullOrder(DateTime orderBeforeTime)
        {
            throw new NotImplementedException();
        }
    }
}
