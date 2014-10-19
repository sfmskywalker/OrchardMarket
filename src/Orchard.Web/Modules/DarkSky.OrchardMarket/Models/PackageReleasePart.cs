using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement.Utilities;

namespace DarkSky.OrchardMarket.Models {
	public class PackageReleasePart : ContentPart<PackageReleasePartRecord> {
		
        internal LazyField<PackagePart> PackageField = new LazyField<PackagePart>();
        internal LazyField<Version> VersionField = new LazyField<Version>();
        internal LazyField<VersionRange> SupportedOrchardVersionRangeField = new LazyField<VersionRange>();

	    public PackagePart Package {
            get { return PackageField.Value; }
            set { PackageField.Value = value; }
	    }

		public int Downloads {
			get { return Record.Downloads; }
			set { Record.Downloads = value; }
		}

		public int Sales {
			get { return Record.Sales; }
			set { Record.Sales = value; }
		}

        public Version Version {
            get { return VersionField.Value; }
            set { VersionField.Value = value; }
        }

        public VersionRange SupportedOrchardVersionsRange {
            get { return SupportedOrchardVersionRangeField.Value; }
            set { SupportedOrchardVersionRangeField.Value = value; }
        }

        public string ReleaseNotes {
            get { return Record.ReleaseNotes; }
            set { Record.ReleaseNotes = value; }
        }

        public string FileName {
            get { return Record.FileName; }
            set { Record.FileName = value; }
        }

	    public string Name {
            get { return string.Format("{0} - {1}", Package.Name, Version); }
	    }
	}

    public class PackageReleasePartRecord : ContentPartRecord {
		public virtual int PackageId { get; set; }
		public virtual int Downloads { get; set; }
		public virtual int Sales { get; set; }
		public virtual string Version { get; set; }
		public virtual string SupportedOrchardVersionsRange { get; set; }
        public virtual string ReleaseNotes { get; set; }
        public virtual string FileName { get; set; }
	}
}