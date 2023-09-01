using DomainBase;

namespace Orderpool.Api.Models
{
    public class OrderDigest : ValueObject
    {
        public long OrderId { get; private set; }
        public int PartnerId { get; private set; }
        public int TransportMode { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            throw new NotImplementedException();
        }
    }

    public class OrderItem : ValueObject
    {
        public int ProductId { get; private set; }
        public int Qty { get; private set; }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ProductId;
            yield return Qty;
        }
    }
}
