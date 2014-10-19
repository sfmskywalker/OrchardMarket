using System;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Models {
    [OrchardFeature("DarkSky.Commerce.PaymentProviders")]
    public class Payment {
        public virtual int Id { get; set; }
        public virtual string Token { get; set; }
        public virtual Transaction Transaction { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
    }
}