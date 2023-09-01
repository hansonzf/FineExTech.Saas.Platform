using DomainBase;
using Shippment.Domain.AggregateModels.EquipmentAggregate;

namespace Shippment.Domain.AggregateModels.TransportOrderAggregate
{
    public class PickupDescription : ValueObject
    {
        public bool NeedPickupService { get; private set; }
        public long? DispatchingId { get; private set; }
        public EquipmentType RequiredEquipmentType { get; private set; }
        public EquipmentDescription DispatchedEquipment { get; private set; }
        public string? PickupCode { get; private set; }
        public string? DetailAddress { get; private set; }
        public string? ContactName { get; private set; }
        public string? Phone { get; private set; }
        public DateTime? PickupTime { get; private set; }
        public string? Remark { get; private set; }

        private PickupDescription()
        { }

        public PickupDescription(bool needPickupService, EquipmentType requireEquipmentType, string detailAddress, string contactName, string phone, DateTime pickupTime, string remark)
        {
            NeedPickupService = needPickupService;
            RequiredEquipmentType = requireEquipmentType;
            DetailAddress = detailAddress;
            ContactName = contactName;
            Phone = phone;
            PickupTime = pickupTime;
            Remark = remark;
        }

        public PickupDescription Scheduled(long scheduleId, string pickupCode, EquipmentDescription dispatched)
        {
            var desc = new PickupDescription
            {
                NeedPickupService = NeedPickupService,
                RequiredEquipmentType = RequiredEquipmentType,
                ContactName = ContactName,
                Phone = Phone,
                PickupTime = PickupTime,
                DetailAddress = DetailAddress,
                DispatchingId = scheduleId,
                PickupCode = pickupCode,
                DispatchedEquipment = dispatched,
                Remark = Remark
            };

            return desc;
        }

        public bool IsContainValidPickupInformation()
        {
            bool result = true;
            result &= !string.IsNullOrEmpty(ContactName);
            result &= !string.IsNullOrEmpty(Phone);
            result &= !string.IsNullOrEmpty(DetailAddress);
            result &= !string.IsNullOrEmpty(PickupCode);
            result &= PickupTime.HasValue;

            return result;
        }

        public PickupDescription ChangePickupInformation(string newAddr, string contact, string phone, string remark)
        {
            var desc = new PickupDescription
            {
                NeedPickupService = NeedPickupService,
                RequiredEquipmentType = RequiredEquipmentType,
                ContactName = string.IsNullOrEmpty(contact) ? ContactName : contact,
                Phone = string.IsNullOrEmpty(phone) ? Phone : phone,
                PickupTime = PickupTime,
                DetailAddress = string.IsNullOrEmpty(newAddr) ? DetailAddress : newAddr,
                DispatchingId = DispatchingId,
                PickupCode = PickupCode,
                DispatchedEquipment = DispatchedEquipment,
                Remark = string.IsNullOrEmpty(remark) ? Remark : remark
            };

            return desc;
        }

        public static PickupDescription NoNeedPickup()
        {
            var desc = new PickupDescription 
            { 
                NeedPickupService = false 
            };

            return desc;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return NeedPickupService;
            yield return DispatchingId;
            yield return PickupCode;
            yield return DetailAddress;
            yield return ContactName;
            yield return Phone;
            yield return PickupTime;
            yield return Remark;
        }
    }
}
