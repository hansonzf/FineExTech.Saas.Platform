using Shippment.Domain.AggregateModels.LocationAggregate;

namespace Shippment.Domain.AggregateModels.ItineraryAggregate
{
    public class LoadHanding : Handing
    {
        public LoadHanding(LocationDescription location)
            : base(location)
        {
            HandingType = Handing.Loading;
        }

        public override void Process(string trackingNumber)
        {
            TrackingNumber = trackingNumber;
            OperationTime = DateTime.Now;
            HandingDescription = 
@$"{OperationTime}
 【{Location.City}】您的货物在 【{Location.LocationName}】已装车";
        }
    }
}
