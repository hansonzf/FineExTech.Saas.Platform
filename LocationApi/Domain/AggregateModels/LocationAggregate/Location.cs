using DomainBase;

namespace LocationApi.Domain.AggregateModels.LocationAggregate
{
    public class Location : Entity, IAggregateRoot
    {
        public long Owner { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public Address Addr { get; private set; }

        protected Location()
        { }

        public Location(long owner, string code, string name, Address addr)
        {
            Owner = owner;
            Code = code;
            Name = name;
            Addr = addr.IsVaild() ? addr : Address.Null();
        }

        public void UpdateAddress(Address address)
        {
            if (address.IsVaild())
                Addr = address;
        }

        public override string ToString()
        {
            return $"【{Addr.City}】 {Name}";
        }
    }
}
