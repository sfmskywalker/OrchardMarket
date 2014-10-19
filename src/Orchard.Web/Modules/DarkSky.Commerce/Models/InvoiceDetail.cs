using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Models {
    [OrchardFeature("DarkSky.Commerce.Orders")]
    public class InvoiceDetail {
        public virtual int Id { get; set; }
        public virtual int? InvoiceId { get; set; }
        public virtual int? ProductId { get; set; }
        public virtual string Description { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal UnitPrice { get; set; }
        public virtual float VatRate { get; set; }

        public virtual decimal SubTotal() {
            return UnitPrice*Quantity;
        }

        public virtual decimal Vat() {
            return SubTotal()*(decimal)VatRate;
        }

        public virtual decimal Total() {
            return SubTotal() + Vat();
        }
    }
}