using DomainBase;
using Shippment.Domain.AggregateModels.LocationAggregate;

namespace Shippment.Domain.AggregateModels.EquipmentAggregate
{
    public class Equipment : Entity, IAggregateRoot
    {
        protected Equipment()
        { }

        public Equipment(string identifier, bool isSelfSupport, LocationDescription parkingLocation)
        {
            Identifier = identifier;
            IsSelfSupport = isSelfSupport;
            CurrentLocation = parkingLocation;
            IsInuse = false;
            EstimateReleaseTime = DateTimeConstant.MaxDateTime;
        }

        public string Identifier { get; protected set; }
        public EquipmentType Type { get; protected set; }
        public double MaxLoadVolume { get; protected set; }
        public double MaxLoadWeight { get; protected set; }
        public bool IsSelfSupport { get; protected set; }
        public LocationDescription CurrentLocation { get; protected set; }
        public bool IsInuse { get; protected set; }
        public DateTime EstimateReleaseTime { get; protected set; }
        public LocationDescription? Destination { get; protected set; }

        public EquipmentDescription Description
        {
            get
            {
                if (!IsTransient())
                {
                    var desc = new EquipmentDescription(Id, Identifier, Type, MaxLoadVolume, MaxLoadWeight);
                    return desc;
                }

                return null;
            }
        }

        public void AddEquipmentDescription(EquipmentType type, double maxLoadVolume, double maxLoadWeight)
        {
            Type = type;
            MaxLoadVolume = maxLoadVolume;
            MaxLoadWeight = maxLoadWeight;
        }

        public bool AssignTask(DateTime estimateSetoutTime, double estimateInvterval, LocationDescription destination)
        {
            if (IsInuse)
                return false;

            IsInuse = true;
            Destination = destination;
            EstimateReleaseTime = estimateSetoutTime.AddHours(estimateInvterval);

            return true;
        }

        public bool CancelTask()
        {
            if (!IsInuse)
                return false;

            IsInuse = false;
            Destination = null;
            EstimateReleaseTime = DateTimeConstant.MinDateTime;

            return true;
        }

        public void Arrived(LocationDescription location)
        {
            if (location is null)
                return;

            CurrentLocation = location;
            IsInuse = false;
            Destination = null;
        }
    }
}
