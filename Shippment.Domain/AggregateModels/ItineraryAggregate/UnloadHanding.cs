using Shippment.Domain.AggregateModels.LocationAggregate;

namespace Shippment.Domain.AggregateModels.ItineraryAggregate
{
    public class UnloadHanding : Handing
    {
        public UnloadHanding(LocationDescription location)
            : base(location)
        {
            HandingType = Handing.Unloading;
        }

        public override void Process(string trackingNumber)
        {
            TrackingNumber = trackingNumber;
            OperationTime = DateTime.Now;
            HandingDescription = 
@$"{OperationTime}
 【{Location.City}】您的货物在 【{Location.LocationName}】卸货";
        }
    }
}
