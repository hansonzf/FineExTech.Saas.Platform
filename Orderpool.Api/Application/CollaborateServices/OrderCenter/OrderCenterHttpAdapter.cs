using Orderpool.Api.Services;

namespace Orderpool.Api.Application.CollaborateServices.OrderCenter
{
    public class OrderCenterHttpAdapter
    {
        private readonly OrderCenterTranslator _translator;
        private readonly HttpClient _client;

        public OrderCenterHttpAdapter(OrderCenterTranslator translator, OrderCenterHttpAdapterOptions options)
        {
            _translator = translator;
            _client = new HttpClient
            {
                BaseAddress = new Uri(options.BaseUrl),
            };
        }

        public async Task QueryOrders(DateTime orderBeforeTime)
        {
            string queryOrderUrl = "";
            var resp = await _client.GetAsync(queryOrderUrl);
            var pulledOrders = await _translator.Translate(nameof(QueryOrders), resp);
        }
    }

    public class OrderCenterHttpAdapterOptions
    {
        public string BaseUrl { get; set; }
    }
}
