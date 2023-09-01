using DomainBase;

namespace LocationApi.Domain.AggregateModels.RouteAggregate
{
    public interface IRouteRepository : IRepository<Route>
    {
        Task<Route> GetAsync(long routeId);
        Task<bool> AddRouteAsync(Route route);
    }
}
