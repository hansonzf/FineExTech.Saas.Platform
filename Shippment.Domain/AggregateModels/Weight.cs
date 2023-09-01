using DomainBase;

namespace Shippment.Domain.AggregateModels
{
    public enum UnitOfWeight
    {
        Gram,
        KiloGram,
        Tonne
    }

    public class Weight : ValueObject
    {
        public Weight(double number, UnitOfWeight unit = UnitOfWeight.Tonne)
        {
            Number = number;
            Unit = unit;
        }

        public double Number { get; private set; }
        public UnitOfWeight Unit { get; private set; }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Number;
            yield return Unit;
        }
    }
}
