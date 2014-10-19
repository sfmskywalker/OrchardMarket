using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

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

	public class ExtensionReleasePartRecord : ContentPartRecord {
		public virtual int ExtensionId { get; set; }
		public virtual int Downloads { get; set; }
		public virtual int Sales { get; set; }
		public virtual string Version { get; set; }
		public virtual string MinimumOrchardVersion { get; set; }
		public virtual string MaximumOrchardVersion { get; set; }
	}
}