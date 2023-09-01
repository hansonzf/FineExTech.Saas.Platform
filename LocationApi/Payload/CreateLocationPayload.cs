using FluentValidation;

namespace LocationApi.Payload
{
    public class CreateLocationPayload : PayloadBase
    {
        public long OwnerId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public AddressValue Address { get; set; }
    }

    public class AddressValue
    {
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string? PostalCode { get; set; }
        public string DetailAddress { get; set; }
    }

    public class CreateLocationPayloadValidator : AbstractValidator<CreateLocationPayload>
    {
        public CreateLocationPayloadValidator()
        {
            RuleFor(x => x.OwnerId).GreaterThan(0);
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            //RuleFor(addr => addr.Address.DetailAddress).NotEmpty();
        }
    }
}
