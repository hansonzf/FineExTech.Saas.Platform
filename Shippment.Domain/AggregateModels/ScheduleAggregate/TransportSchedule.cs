using DomainBase;
using Shippment.Domain.AggregateModels.EquipmentAggregate;
using Shippment.Domain.AggregateModels.LocationAggregate;
using Shippment.Domain.AggregateModels.RouterAggregate;
using Shippment.Domain.Events;

namespace Shippment.Domain.AggregateModels.ScheduleAggregate
{
    public class TransportSchedule : Entity, IAggregateRoot
    {
        private HashSet<CargoLoading> _cargoLoadingList;

        protected TransportSchedule() 
        {
            VolumeLoadRate = 0;
            WeightLoadRate = 0;
            _cargoLoadingList = new HashSet<CargoLoading>();
        }

        public TransportSchedule(Leg routeLeg, DateTime estimateSetoutTime, double transportInterval = 0)
            : this()
        {
            ScheduleNumber = DateTime.Now.ToString("yyyyMMddhhmmssffff");
            RouteLeg = routeLeg;
            From = routeLeg.From;
            To = routeLeg.To;
            Efficiency = new TimeManagement(estimateSetoutTime, transportInterval);
            Status = ScheduleStatus.Created;
            Type = ScheduleType.Transport;
        }

        public Leg RouteLeg { get; protected set; }
        public string ScheduleNumber { get; protected set; }
        public EquipmentDescription Equipment { get; protected set; }
        public LocationDescription From { get; protected set; }
        public LocationDescription To { get; protected set; }
        public TimeManagement Efficiency { get; protected set; }
        public ScheduleType Type { get; protected set; }
        public ScheduleStatus Status { get; protected set; }
        public double VolumeLoadRate { get; protected set; }
        public double WeightLoadRate { get; protected set; }


        public virtual bool DispatchingEquipment(EquipmentDescription equipment)
        {
            if (Status == ScheduleStatus.Executed || Status == ScheduleStatus.Completed)
                return false;

            Equipment = equipment;
            Status = ScheduleStatus.Standby;
            CalculateLoadingRate();

            var evt = new ScheduleDispatchedEquipmentDomainEvent
            { 
                Equipment = Equipment,
                ScheduleId = Id,
                EstimateSetoutTime = Efficiency.EstimateSetoutTime,
                EstimateTripInterval = Efficiency.EstimateTransportInterval,
                Destination = To
            };
            AddDomainEvent(evt);

            return true;
        }

        private void CalculateLoadingRate()
        {
            if (Equipment != null && _cargoLoadingList.Any())
            {
                double totalLoadedVolume = _cargoLoadingList.Sum(c => c.Cargo.Volume.Volume);
                double totalLoadedWeight = _cargoLoadingList.Sum(c => c.Cargo.Weight.Number);
                VolumeLoadRate = Math.Round(totalLoadedVolume / Equipment.MaxLoadVolume * 100, 2);
                WeightLoadRate = Math.Round(totalLoadedWeight / Equipment.MaxLoadWeight * 100, 2);
            }
        }

        public bool TakeOnCargo(string trackingNumber, string barcode, Cargo cargo)
        {
            if (Status != ScheduleStatus.Standby)
                return false;
            if (cargo == null)
                return false;
            if (string.IsNullOrEmpty(trackingNumber))
                return false;

            var loading = new CargoLoading(trackingNumber, barcode, cargo);
            _cargoLoadingList.Add(loading);
            CalculateLoadingRate();

            return true;
        }

        public bool TakeOffCargo(string trackingNumber, string barcode)
        {
            if (Status != ScheduleStatus.Standby || !_cargoLoadingList.Any())
                return false;

            var cargo = _cargoLoadingList.FirstOrDefault(c => c.TrackingNumber == trackingNumber && c.Barcode == barcode);
            if (cargo is null)
                return false;

            _cargoLoadingList.Remove(cargo);
            CalculateLoadingRate();

            return true;
        }

        public virtual bool Execute(LocationDescription departureLocation)
        {
            if (departureLocation is null)
                return false;

            if (Status != ScheduleStatus.Standby)
                throw new InvalidOperationException();

            if (departureLocation.LocationId == From.LocationId)
            {
                Efficiency = Efficiency.Leave();
                Status = ScheduleStatus.Executed;

                AddDomainEvent(new ScheduleExecutedDomainEvent { 
                    ScheduleId = Id,
                    OccuredTime = DateTime.Now,
                    Type = this.GetType().Name switch
                    {
                        "TransportSchedule" => ScheduleType.Transport,
                        "PickupSchedule" => ScheduleType.Pickup,
                        _ => throw new NotImplementedException()
                    },
                    Location = departureLocation
                });

                return true;
            }

            return false;
        }

        public virtual bool Cancel()
        {
            if (Status != ScheduleStatus.Standby)
                return false;

            Status = ScheduleStatus.Cancelled;
            Equipment = null;
            VolumeLoadRate = 0;
            WeightLoadRate = 0;
            AddDomainEvent(new CancelScheduleDomainEvent());

            return true;
        }

        public bool ChangeEstimateSetoutTime(DateTime newSetoutTime)
        {
            if (Status != ScheduleStatus.Standby)
                return false;

            DateTime beforeEstimate = Efficiency.EstimateSetoutTime;
            double tripInterval = Efficiency.EstimateTransportInterval;
            Efficiency = new TimeManagement(newSetoutTime, tripInterval);

            AddDomainEvent(new ScheduleSetoutTimeChangedDomainEvent());

            return true;
        }

        public bool DelayEstimateSetoutTimeByHours(double hours)
        {
            DateTime setoutTime = Efficiency.EstimateSetoutTime;
            DateTime newSetoutTime = setoutTime.AddHours(hours);

            return ChangeEstimateSetoutTime(newSetoutTime);
        }

        public bool AheadEstimateSetoutTimeByHours(double hours)
        {
            DateTime setoutTime = Efficiency.EstimateSetoutTime;
            DateTime newSetoutTime = setoutTime.AddHours(0 - hours);

            return ChangeEstimateSetoutTime(newSetoutTime);
        }

        public virtual bool CheckIn(LocationDescription destinationLocation)
        {
            if (Status != ScheduleStatus.Executed)
                throw new InvalidOperationException();

            if (To.LocationId == destinationLocation.LocationId)
            {
                Status = ScheduleStatus.Completed;
                Efficiency = Efficiency.Arrive();
                var evt = new ArrivedTransportDestinationDomainEvent
                { 
                    ScheduleId = Id,
                    ScheduleType = Type,
                    Setout = From,
                    Destination = To
                };
                AddDomainEvent(evt);
                return true;
            }

            return false;
        }

        public bool ChangeScheduleDestination(LocationDescription destination)
        {
            if (Status != ScheduleStatus.Standby)
                return false;

            if (destination == From)
                return false;

            To = destination;
            var evt = new ScheduleChangedDestinationDomainEvent();
            AddDomainEvent(evt);

            return true;
        }
    }
}
