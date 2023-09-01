using DomainBase;
using Shippment.Domain.AggregateModels.EquipmentAggregate;
using Shippment.Domain.Events;
using System.Collections.ObjectModel;

namespace Shippment.Domain.AggregateModels.TransportOrderAggregate
{
    public class TransportOrder : Entity, IAggregateRoot
    {
        protected List<TransportCargo> _cargoList;

        protected TransportOrder()
        {
            _cargoList = new List<TransportCargo>();
        }

        public TransportOrder(long customerId, DeliverySpecification goal, IEnumerable<Cargo> cargoList)
            : this()
        {
            CustomerId = customerId;
            Goal = goal;
            PickupCargoInfo = PickupDescription.NoNeedPickup();
            Status = OrderStatus.Draft;
            foreach (var item in cargoList)
            {
                var cargo = new TransportCargo(0, "", item);
                _cargoList.Add(cargo);
            }
        }

        public TransportOrder(long customerId, DeliverySpecification goal, PickupDescription pickupCargo, IEnumerable<Cargo> cargoList)
            : this(customerId, goal, cargoList)
        {
            PickupCargoInfo = pickupCargo;
        }

        public long CustomerId { get; protected set; }
        public long ScheduleId { get; protected set; }
        public string TrackingNumber { get; protected set; }
        public DeliverySpecification Goal { get; protected set; }
        public OrderStatus Status { get; protected set; }
        public PickupDescription PickupCargoInfo { get; protected set; }
        public DateTime? OrderTime { get; protected set; }
        public ReadOnlyCollection<TransportCargo> CargoList
        {
            get => _cargoList.AsReadOnly();
            protected set => _cargoList = value.ToList();
        }

        public void Submit()
        {
            if (Status != OrderStatus.Draft)
                throw new InvalidOperationException();

            Status = OrderStatus.Ordered;
            OrderTime = DateTime.Now;
        }

        public void Accept(EquipmentDescription? pickupEquipment = null)
        {
            if (Status != OrderStatus.Ordered)
                throw new InvalidOperationException();

            Status = PickupCargoInfo.NeedPickupService ? OrderStatus.Pickup : OrderStatus.Accepted;
            AddDomainEvent(new AcceptTransportOrderDomainEvent
            {
                TransportOrderId = Id,
                AssignedPickupEquipment = pickupEquipment
            });
            // responding this event should do the following things
            //  1, if customer need pickup service, system should
            //     generate dispatching schedule for pickup cargo

        }

        public void FollowSchedule(long scheduleId)
        {
            if (Status != OrderStatus.Standby)
                throw new InvalidOperationException($"The transport order which Id: {Id} is not prepared");

            ScheduleId = scheduleId;
            var evt = new AssignedScheduleForOrderDomainEvent
            { 
                TrackingNumber = TrackingNumber,
                AssignedSchedule = scheduleId,
                OccuredTime = DateTime.Now
            };
            AddDomainEvent(evt);
        }

        public void SchedulePickupService(long scheduleId, string pickupCode, EquipmentDescription dispatched)
        {
            if (Status != OrderStatus.Accepted)
                throw new InvalidOperationException();

            var pickupService = PickupCargoInfo.Scheduled(scheduleId, pickupCode, dispatched);
            PickupCargoInfo = pickupService;
        }

        public void CheckCargo(Dictionary<string, Cargo> cargos)
        {
            if (!(Status == OrderStatus.Accepted || Status == OrderStatus.Pickup))
                throw new InvalidOperationException();

            if (cargos is null || !cargos.Any())
                throw new ArgumentNullException(nameof(cargos));

            _cargoList.Clear();

            foreach (var item in cargos)
            {
                string barcode = item.Key;
                if (!IsValidBarcode(barcode))
                    continue;

                Cargo cargo = item.Value;
                _cargoList.Add(new TransportCargo(Id, barcode, cargo));
            }
            TrackingNumber = $"TRS-{CustomerId}-00001";
            Status = OrderStatus.Standby;

            var evt = new CheckedTransportCargoDomainEvent(Id, TrackingNumber, Status);
            AddDomainEvent(evt);
        }

        private bool IsValidBarcode(string barcode)
        {
            return !string.IsNullOrEmpty(barcode);
        }

        public bool IsPickupDispatched()
        {
            if (!PickupCargoInfo.NeedPickupService)
                return false;

            return PickupCargoInfo.DispatchingId.HasValue;
        }

        public bool ChangePickupCargoLocation(string newAddr, string contact, string phone, string remark)
        {
            if ((int)Status > 4)
                return false;

            var newPickup = PickupCargoInfo.ChangePickupInformation(newAddr, contact, phone, remark);
            
            if (Status == OrderStatus.Accepted || Status == OrderStatus.Pickup)
            {
                PickupCargoInfo = newPickup;
                Status = OrderStatus.Ordered;
            }
            else
            {
                PickupCargoInfo = newPickup;
            }

            return true;
        }
    }
}
