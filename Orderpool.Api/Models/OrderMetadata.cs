using DomainBase;

namespace Orderpool.Api.Models
{
    public class OrderMetadata : ValueObject
    {
        public OrderMetadata(string remoteOrder)
        {
            Id = Guid.NewGuid();
            RemoteOrderJson = remoteOrder;
        }

        public Guid Id { get; protected set; }
        public string RemoteOrderJson { get; protected set; }

        public OrderDigest Digest
        {
            get
            {
                OrderDigest digest = new OrderDigest();

                return digest;
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
            yield return RemoteOrderJson;
        }
    }
}
