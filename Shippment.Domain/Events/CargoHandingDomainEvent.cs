using MediatR;
using Shippment.Domain.AggregateModels.ItineraryAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shippment.Domain.Events
{
    public record CargoHandingDomainEvent : INotification
    {
        public CargoHandingDomainEvent()
        { }

        public long LocationId { get; init; }
        public string TrackingNumber { get; init; }
    }
}
