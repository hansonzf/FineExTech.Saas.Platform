using DomainBase;

namespace Shippment.Domain.AggregateModels.EquipmentAggregate
{
    public class EquipmentDescription : ValueObject
    {
        public long EquipmentId { get; private set; }
        public string Identifier { get; private set; }
        public EquipmentType Type { get; private set; }
        public double MaxLoadVolume { get; private set; }
        public double MaxLoadWeight { get; private set; }

        public EquipmentDescription(long equipmentId, string trackIdentifier, EquipmentType type, double maxLoadVolume = 0, double maxLoadWeight = 0)
        {
            EquipmentId = equipmentId;
            Identifier = trackIdentifier;
            Type = type;
            MaxLoadVolume = maxLoadVolume;
            MaxLoadWeight = maxLoadWeight;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return EquipmentId;
            yield return Identifier;
            yield return Type;
            yield return MaxLoadVolume;
            yield return MaxLoadWeight;
        }
    }
}
