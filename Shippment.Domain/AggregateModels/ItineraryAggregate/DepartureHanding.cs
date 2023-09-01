using Shippment.Domain.AggregateModels.LocationAggregate;

namespace Shippment.Domain.AggregateModels.ItineraryAggregate
{
    public class DepartureHanding : Handing
    {
        public DepartureHanding(LocationDescription location)
            : base(location)
        {
            HandingType = Handing.Departing;
        }

        public override void Process(string trackingNumber)
        {
            TrackingNumber = trackingNumber;
            OperationTime = DateTime.Now;
            HandingDescription = 
@$"{OperationTime}
 【{Location.City}】您的货物从 【{Location.LocationName}】发出";
        }
    }
}
