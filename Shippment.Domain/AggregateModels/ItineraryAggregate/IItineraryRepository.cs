using DomainBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shippment.Domain.AggregateModels.ItineraryAggregate
{
    public interface IItineraryRepository : IRepository<Itinerary>
    {
        Task<bool> CreateNewItineraryAsync(Itinerary itinerary);
        Task<Itinerary> GetAsync(string trackingNumber);
        Task<bool> SaveItineraryAsync(Itinerary itinerary);
    }
}
