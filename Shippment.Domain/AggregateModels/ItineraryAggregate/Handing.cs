using DomainBase;
using Shippment.Domain.AggregateModels.LocationAggregate;

namespace Shippment.Domain.AggregateModels.ItineraryAggregate
{
    public abstract class Handing : Entity
    {
        internal const int Loading = 1;
        internal const int Unloading = 2;
        internal const int Departing = 31;
        internal const int Arraiving = 32;
        internal const int Receiving = 21;
        internal const int Sending = 22;
            
        protected Handing()
        { }

        protected Handing(LocationDescription location)
        {
            Location = location;
        }

        public string TrackingNumber { get; protected set; }
        public LocationDescription Location { get; protected set; }
        public DateTime OperationTime { get; protected set; }
        public string HandingDescription { get; protected set; }
        public int HandingType { get; protected set; }

        public abstract void Process(string trackingNumber);
    }
}
