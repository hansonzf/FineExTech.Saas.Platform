using DomainBase;

namespace Partner.Api.Domain.AggregateModels
{
    public class Company : Entity, IAggregateRoot
    {
        private List<Contact> _contacts;
        private List<Collaboration> _collaborations;

        public Company()
        {
            _contacts = new List<Contact>();
            _collaborations = new List<Collaboration>();
        }

        public string Name { get; private set; }
        
    }
}
