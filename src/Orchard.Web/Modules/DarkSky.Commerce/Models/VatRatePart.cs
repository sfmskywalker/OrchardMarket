using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace DarkSky.Commerce.Models {
	public class VatRatePart : ContentPart<VatRatePartRecord> {
		public float Rate {
			get { return Record.Rate; }
			set { Record.Rate = value; }
		}
	}

	public class VatRatePartRecord : ContentPartRecord {
		public virtual float Rate { get; set; }
	}
}