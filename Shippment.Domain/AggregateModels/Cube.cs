using DomainBase;

namespace Shippment.Domain.AggregateModels
{
    public class Cube : ValueObject
    {
        public Cube(double widthNumber, double heightNumber, double lengthNumber)
        {
            Width = new Line(widthNumber);
            Height = new Line(heightNumber);
            Length = new Line(lengthNumber);
        }

        public Cube(Line width, Line height, Line length)
        {
            Width = width;
            Height = height;
            Length = length;
        }

        public Line Width { get; private set; }
        public Line Height { get; private set; }
        public Line Length { get; private set; }
        public UnitOfVolume Unit { get; private set; }
        public double Volume
        {
            get
            {
                if (!IsValid())
                    return 0f;

                Length = Length.ChangeUnit(UnitOfLength.Meter);
                Width = Width.ChangeUnit(UnitOfLength.Meter);
                Height = Height.ChangeUnit(UnitOfLength.Meter);

                return Length.Number * Width.Number * Height.Number;
            }
        }

        public double ConvertUnit(UnitOfVolume unit)
        {
            if (!IsValid())
                return 0f;

            switch (unit)
            {
                case UnitOfVolume.CubicCentimeter:
                    Length = Length.ChangeUnit(UnitOfLength.Centimeter);
                    Width = Width.ChangeUnit(UnitOfLength.Centimeter);
                    Height = Height.ChangeUnit(UnitOfLength.Centimeter);
                    break;
                case UnitOfVolume.CubicDecimeter:
                    Length = Length.ChangeUnit(UnitOfLength.Decimeter);
                    Width = Width.ChangeUnit(UnitOfLength.Decimeter);
                    Height = Height.ChangeUnit(UnitOfLength.Decimeter);
                    break;
                case UnitOfVolume.CubicMeter:
                    Length = Length.ChangeUnit(UnitOfLength.Meter);
                    Width = Width.ChangeUnit(UnitOfLength.Meter);
                    Height = Height.ChangeUnit(UnitOfLength.Meter);
                    break;
                default:
                    throw new InvalidOperationException("The unit does not supported currently!");
            }

            return Length.Number * Width.Number * Height.Number;
        }

        private bool IsValid()
        {
            return Width is not null &&
                Height is not null &&
                Length is not null;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Length;
            yield return Width;
            yield return Height;
            yield return Unit;
        }
    }
}
