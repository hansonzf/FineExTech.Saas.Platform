using DomainBase;
using Partner.Api.Domain.Enums;
using System.Collections.ObjectModel;

namespace Partner.Api.Domain.AggregateModels
{
    public class Collaboration : Entity
    {
        private List<Contact> _contacts;

        private Collaboration()
        {
            _contacts = new List<Contact>();
        }

        internal Collaboration(CollaborateType type, long partnerId)
            : this()
        {
            Type = type;
            CompanyId = partnerId;
            CreatedTime = DateTime.Now;
        }

        public CollaborateType Type { get; private set; }
        public long CompanyId { get; private set; }
        public DateTime CreatedTime { get; private set; }
        public ReadOnlyCollection<Contact> Contacts
        {
            get => _contacts.AsReadOnly();
            private set => _contacts = new List<Contact>(value);
        }
    }
}
