using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace DarkSky.Commerce.Models {
	public class CommerceSettingsPart : ContentPart<CommerceSettingsPartRecord> {

		[Required]
		public string Currency {
			get { return Record.Currency; }
			set { Record.Currency = value; }
		}

	}

	public class CommerceSettingsPartRecord : ContentPartRecord {
		public virtual string Currency { get; set; }
	}
}