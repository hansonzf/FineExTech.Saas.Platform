using LocationApi.Domain.AggregateModels.LocationAggregate;
using LocationApi.Domain.AggregateModels.RouteAggregate;
using LocationApi.Payload;
using Microsoft.AspNetCore.Mvc;
using Route = LocationApi.Domain.AggregateModels.RouteAggregate.Route;

namespace LocationApi.Controllers
{
    [Route("api/routes")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        private readonly IRouteRepository _repository;

        public RoutesController(IRouteRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetRouteById(long routeId)
        {
            var route = await _repository.GetAsync(routeId);
            return Ok(route);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoute(RoutePayload payload)
        {
            LocationDescription origin = new(payload.Origin.LocationId, payload.Origin.LocationName, payload.Origin.City);
            LocationDescription destination = new(payload.Destination.LocationId, payload.Destination.LocationName, payload.Destination.City);
            var segments = payload.Segments.Select(s => new Segment(
                    new LocationDescription(s.From.LocationId, s.From.LocationName, s.From.City),
                    new LocationDescription(s.To.LocationId, s.To.LocationName, s.To.City),
                    s.Distance
                )).ToArray();
            var route = new Route(payload.Owner, payload.RouteName, origin, destination, segments);
            await _repository.AddRouteAsync(route);
            await _repository.UnitOfWork.SaveEntitiesAsync();

            return CreatedAtAction(nameof(GetRouteById), new { routeId = route.Id }, route);
        }
    }
}
