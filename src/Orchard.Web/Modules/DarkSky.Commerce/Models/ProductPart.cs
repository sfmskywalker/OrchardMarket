using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement.Utilities;
using Orchard.Core.Common.Models;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Models {
    [OrchardFeature("DarkSky.Commerce.Products")]
    public class ProductPart : ContentPart<ProductPartRecord>, IProductAspect {
        internal LazyField<VatPart> VatField = new LazyField<VatPart>();

        public IShopAspect Shop {
            get { return this.As<CommonPart>().Container.As<IShopAspect>(); }
            set { this.As<CommonPart>().Container = value; }
        }

        public decimal UnitPrice {
            get { return Record.UnitPrice; }
            set { Record.UnitPrice = value; }
        }

        [Required]
        public string Currency {
            get { return Record.Currency; }
            set { Record.Currency = value; }
        }

        public string ImageUrl {
            get { return ((dynamic) this).PrimaryImage.Url; }
            set { ((dynamic) this).PrimaryImage.Url = value; }}

        public VatPart Vat {
            get { return VatField.Value; }
            set { VatField.Value = value; }
        }

        public int Sales {
            get { return Record.Sales; }
            set { Record.Sales = value; }
        }
    }

    [OrchardFeature("DarkSky.Commerce.Products")]
    public class ProductPartRecord : ContentPartRecord {
        public virtual decimal UnitPrice { get; set; }
        public virtual string Currency { get; set; }
        public virtual int? VatId { get; set; }
        public virtual int Sales { get; set; }
    }
}