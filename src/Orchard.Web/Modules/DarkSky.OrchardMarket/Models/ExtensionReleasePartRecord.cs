using Orchard.ContentManagement.Records;

namespace DarkSky.OrchardMarket.Models {
    public class ExtensionReleasePartRecord : ContentPartRecord {
        public virtual int ExtensionId { get; set; }
        public virtual int Downloads { get; set; }
        public virtual int Sales { get; set; }
        public virtual string Version { get; set; }
        public virtual string MinimumOrchardVersion { get; set; }
        public virtual string MaximumOrchardVersion { get; set; }
    }
}