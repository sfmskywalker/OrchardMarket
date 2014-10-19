using Orchard.ContentManagement;

namespace DarkSky.OrchardMarket.Models {
	public class ExtensionReleasePart : ContentPart<ExtensionReleasePartRecord> {
		public int ExtensionId {
			get { return Record.ExtensionId; }
			set { Record.ExtensionId = value; }
		}

		public int Downloads {
			get { return Record.Downloads; }
			set { Record.Downloads = value; }
		}

		public int Sales {
			get { return Record.Sales; }
			set { Record.Sales = value; }
		}

		public string MinimumOrchardVersion {
			get { return Record.MinimumOrchardVersion; }
			set { Record.MinimumOrchardVersion = value; }
		}

		public string MaximumOrchardVersion {
			get { return Record.MaximumOrchardVersion; }
			set { Record.MaximumOrchardVersion = value; }
		}
	}
}