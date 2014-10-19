using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace DarkSky.Commerce.Models {
	public class ProductPart : ContentPart<ProductPartRecord> {
		public decimal Price {
			get { return Record.Price; }
			set { Record.Price = value; }
		}

		public int VatRateId {
			get { return Record.VatRateId; }
			set { Record.VatRateId = value; }
		}
	}

	public class ProductPartRecord : ContentPartRecord {
		public virtual decimal Price { get; set; }
		public virtual int VatRateId { get; set; } 
	}
}