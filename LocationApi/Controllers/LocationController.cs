using LocationApi.Domain;
using LocationApi.Domain.AggregateModels.LocationAggregate;
using LocationApi.Payload;
using Microsoft.AspNetCore.Mvc;

namespace LocationApi.Controllers
{
    [ApiController]
    [Route("api/locations")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _repository;
        private readonly ILogger _logger;

        public LocationController(ILocationRepository repository, ILogger<LocationController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET api/locations/1
        [HttpGet("{locationId}")]
        public async Task<ActionResult<Location>> GetLocation(long locationId)
        {
            if (locationId <= 0)
                return BadRequest("Parameter locationId must large than 0!");

            var location = await _repository.GetAsync(locationId);
            if (location is null)
                return NoContent();
            else
                return Ok(location);
        }

        // GET api/locations/ownedby/1
        [HttpGet("ownedby/{ownerId}")]
        public async Task<ActionResult<PaginatedItemsPayload<Location>>> GetLocations(long ownerId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
        {
            if (ownerId <= 0)
                return BadRequest("Parameter ownerId must large than 0!");

            var result = await _repository.GetByOwnerAsync(ownerId, pageIndex, pageSize);
            var paylaod = new PaginatedItemsPayload<Location>
            { 
                Data = result.Data,
                TotalCount = result.TotalCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Result = true,
                Message = string.Empty
            };
            if (result.TotalCount > 0)
                return Ok(paylaod);
            else
                return NoContent();
        }

        // POST api/locations
        [HttpPost]
        public async Task<ActionResult<Location>> CreateNewLocation([FromBody] CreateLocationPayload payload)
        {
            var ad = payload.Address;
            var address = new Address(ad.Country, ad.Province, ad.City, ad.District, ad.PostalCode, ad.DetailAddress);
            var location = new Location(payload.OwnerId, payload.Code, payload.Name, address);

            try
            {
                await _repository.AddAsync(location);
                await _repository.UnitOfWork.SaveChangesAsync();

                return CreatedAtAction(nameof(GetLocation), new { locationId = location.Id }, location);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/locations/1/address
        [HttpPost("{locationId}/address")]
        public async Task<IActionResult> UpdateLocationAddress(long locationId, [FromBody] AddressValue address)
        {
            var addr = new Address(
                address.Country, 
                address.Province, 
                address.City, 
                address.District, 
                address.PostalCode, 
                address.DetailAddress);
            var location = await _repository.GetAsync(locationId);

            if (!addr.IsVaild())
                return BadRequest();

            location.UpdateAddress(addr);
            try
            {
                bool result = await _repository.UnitOfWork.SaveEntitiesAsync();
                if (result)
                    return Ok();
                else
                    return StatusCode(500);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{locationId}")]
        public async Task<IActionResult> DeleteLocation(long locationId)
        {
            if (locationId <= 0)
                return BadRequest("Parameter locationId must large than 0!");

            bool result = await _repository.DeleteAsync(locationId);
            await _repository.UnitOfWork.SaveEntitiesAsync();
            if (result)
                return Ok();
            else
                return NoContent();
        }
    }
}