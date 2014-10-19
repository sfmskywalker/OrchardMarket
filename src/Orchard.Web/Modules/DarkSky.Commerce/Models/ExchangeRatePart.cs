using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Records;

namespace DarkSky.Commerce.Models {
	public class ExchangeRatePart : ContentPart<ExchangeRatePartRecord>, ITitleAspect {
		public string Currency {
			get { return Record.Currency; }
			set { Record.Currency = value; }
		}

		public float Rate {
			get { return Record.Rate; }
			set { Record.Rate = value; }
		}

		public string Title {
			get { return string.Format("{0} / {1}", Currency, Rate); }
		}
	}

	public class ExchangeRatePartRecord : ContentPartRecord {
		public virtual string Currency { get; set; }
		public virtual float Rate { get; set; }
	}
}