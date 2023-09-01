using DomainBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shippment.Domain.AggregateModels.ScheduleAggregate
{
    public class CargoLoading : Entity
    {
        public string TrackingNumber { get; protected set; }
        public string Barcode { get; protected set; }
        public Cargo Cargo { get; protected set; }

        internal CargoLoading(string trackingNumber, string barcode, Cargo cargo)
        {
            TrackingNumber = trackingNumber;
            Barcode = barcode;
            Cargo = cargo;
        }
    }
}
