using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement.Utilities;
using Orchard.Core.Common.Models;
using Orchard.Security;

namespace DarkSky.Commerce.Models {

    public class PurchasePart : ContentPart<PurchasePartRecord> {
        internal readonly LazyField<IProductAspect> ProductField = new LazyField<IProductAspect>();
        internal readonly LazyField<InvoicePart> InvoiceField = new LazyField<InvoicePart>();

        public IUser User {
            get { return this.As<CommonPart>().Owner; }
            set { this.As<CommonPart>().Owner = value; }
        }

        public int Quantity {
            get { return Record.Quantity; }
            set { Record.Quantity = value; }
        }

        public IProductAspect Product {
            get { return ProductField.Value; }
            set { ProductField.Value = value; }
        }

        public InvoicePart Invoice {
            get { return InvoiceField.Value; }
            set { InvoiceField.Value = value; }
        }
    }

    public class PurchasePartRecord : ContentPartRecord {
        public virtual int InvoiceId { get; set; }
        public virtual int ProductId { get; set; }
        public virtual int Quantity { get; set; }
    }
}