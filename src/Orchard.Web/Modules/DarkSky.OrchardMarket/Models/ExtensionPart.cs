using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Records;

namespace DarkSky.OrchardMarket.Models {
	public class ExtensionPart : ContentPart<ExtensionPartRecord> {
		public int PublisherId {
			get { return Record.PublisherId; }
			set { Record.PublisherId = value; }
		}

		public int Downloads {
			get { return Record.Downloads; }
			set { Record.Downloads = value; }
		}

		public int Sales {
			get { return Record.Sales; }
			set { Record.Sales = value; }
		}

		public string Name {
			get { return this.As<ITitleAspect>().Title; }
		}
	}

	public class ExtensionPartRecord : ContentPartRecord {
		public virtual int PublisherId { get; set; }
		public virtual int Downloads { get; set; }
		public virtual int Sales { get; set; }
	}
}