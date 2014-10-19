using Orchard.ContentManagement.Records;

namespace DarkSky.OrchardMarket.Models {
    public class ExtensionPartRecord : ContentPartRecord {
        public virtual int PublisherId { get; set; }
        public virtual int Downloads { get; set; }
        public virtual int Sales { get; set; }
    }
}