using DomainBase;

namespace Partner.Api.Domain.AggregateModels
{
    public class Contact : ValueObject
    {
        public string Name { get; private set; }
        public bool Gender { get; private set; }
        public string Title { get; private set; }
        public string ContactWay { get; private set; }
        public string Number { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Gender;
            yield return Title;
            yield return ContactWay;
            yield return Number;
        }
    }
}
