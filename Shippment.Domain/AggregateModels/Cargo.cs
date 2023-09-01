using DomainBase;

namespace Shippment.Domain.AggregateModels
{
    public class Cargo : ValueObject
    {
        public Cargo(string name, Cube volume, Weight weight, int hardIndex)
        {
            Volume = volume;
            Weight = weight;
            HardIndex = hardIndex;
        }

        public string Name { get; private set; }
        public Cube Volume { get; private set; }
        public Weight Weight { get; private set; }
        /// <summary>
        /// 由数字 1 - 9 表示，1表示货物脆弱，不可堆叠在下；9表示该货物硬度高，可堆叠在最下方
        /// </summary>
        public int HardIndex { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Volume;
            yield return Weight;
            yield return HardIndex;
        }
    }
}
