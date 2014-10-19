using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Models {

    [OrchardFeature("DarkSky.Commerce.Orders")]
    public class OrderDetail {
        public virtual int Id { get; set; }
        public virtual int OrderId { get; set; }
        public virtual int ProductId { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal UnitPrice { get; set; }
        public virtual float VatRate { get; set; }

        public virtual decimal SubTotal() {
            return UnitPrice*Quantity;
        }

        public virtual decimal Vat() {
            return SubTotal() * (decimal) VatRate;
        }

        public virtual decimal Total() {
            return SubTotal() + Vat();
        }

        public virtual Totals Totals() {
            var subTotal = UnitPrice*Quantity;
            var vat = subTotal*(decimal) VatRate;
            var total = subTotal + vat;
            return new Totals {
                SubTotal = subTotal,
                Vat = vat,
                Total = total
            };
        }
    }
}