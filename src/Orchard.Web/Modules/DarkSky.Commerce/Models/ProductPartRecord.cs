using Orchard.ContentManagement.Records;

namespace DarkSky.Commerce.Models {
    public class ProductPartRecord : ContentPartRecord {
        public virtual decimal Price { get; set; }
    }
}