using DomainBase;

namespace Shippment.Domain.AggregateModels
{
    public class Line : ValueObject
    {
        public Line(double number, UnitOfLength unit = UnitOfLength.Meter)
        {
            Number = number;
            Unit = unit;
        }

        public double Number { get; private set; }
        public UnitOfLength Unit { get; private set; }

        public Line ChangeUnit(UnitOfLength newUnit)
        {
            Number *= Math.Pow(10, (int)Unit - (int)newUnit);
            return new Line(Number, newUnit);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Number;
            yield return Unit;
        }
    }
}
