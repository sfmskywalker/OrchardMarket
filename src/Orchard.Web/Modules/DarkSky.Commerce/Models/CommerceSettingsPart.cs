using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace DarkSky.Commerce.Models {
	public class CommerceSettingsPart : ContentPart {

		[Required]
		public string Currency {
			get { return this.Retrieve(x => x.Currency, "USD"); }
			set { this.Store(x => x.Currency, value); }
		}

	}
}