using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;

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
}