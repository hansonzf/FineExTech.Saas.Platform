using Moq;
using Shipment.Domain.Test.MockAggregate;
using Shippment.Domain.AggregateModels.RouterAggregate;

namespace Shipment.Domain.Test.TestFixture
{
    public class RouteTestFixture
    {
        private readonly List<Route> _routes;
        private Mock<IRouteRepository> _mockRepository;

        public RouteTestFixture()
        {
            _routes = RouteProxy.SeedTestData();
            _mockRepository = new Mock<IRouteRepository>();

            _mockRepository.Setup(rp => rp.GetAsync(It.IsAny<long>()))
                .ReturnsAsync((long id) => _routes.FirstOrDefault(r => r.Id == id));
        }

        public IRouteRepository RouteRepository => _mockRepository.Object;
    }
}
