using System;
using DarkSky.Commerce.Services.PayPal.Soap;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Models {
    [OrchardFeature("DarkSky.Commerce.PaymentProviders.PayPal")]
    public class PayPalLogEntry {
        public virtual int Id { get; set; }
        public virtual DateTime TimeStamp { get; set; }
        public virtual AckCodeType Ack { get; set; }
        public virtual string CorrelationId { get; set; }
        public virtual string Version { get; set; }
        public virtual string Build { get; set; }
        public virtual string Errors { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
    }

}