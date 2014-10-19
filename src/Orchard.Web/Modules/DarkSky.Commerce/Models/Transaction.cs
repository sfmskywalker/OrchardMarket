using System;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Models {
    [OrchardFeature("DarkSky.Commerce.Checkout")]
    public class Transaction {
        public virtual int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual int CartId { get; set; }
        public virtual decimal OrderTotal { get; set; }
        public virtual TransactionStatus Status { get; set; }
        public virtual string ProviderToken { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
        public virtual DateTime? CompletedUtc { get; set; }
        public virtual string PaymentMethodName { get; set; }
    }
}