using DomainBase;

namespace LocationApi.Domain
{
    public class Address : ValueObject
    {
        public string Country { get; private set; }
        public string Province { get; private set; }
        public string City { get; private set; }
        public string District { get; private set;  }
        public string PostalCode { get; private set; }
        public string DetailAddress { get; private set;  }

        private Address()
        { }

        public Address(string country, string province, string city, string district, string postalCode, string detail)
        {
            Country = country;
            Province = province;
            City = city;
            District = district;
            PostalCode = postalCode;
            DetailAddress = detail;
        }

        public bool IsVaild()
        {
            bool result = true;
            result &= Country != null && Province != null && City != null && DetailAddress != null;

            return result;
        }

        public static Address Null()
        {
            var addr = new Address
            {
                Country = default,
                Province = default,
                City = default,
                District = default,
                PostalCode = default,
                DetailAddress = default
            };

            return addr;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Country;
            yield return Province;
            yield return City;
            yield return District;
            yield return PostalCode;
            yield return DetailAddress;
        }
    }
}
