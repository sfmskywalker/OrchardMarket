using Orchard.ContentManagement;

namespace DarkSky.Commerce.Models {
	public class ProductPart : ContentPart<ProductPartRecord> {
		public decimal Price {
			get { return Record.Price; }
			set { Record.Price = value; }
		}
	}
}