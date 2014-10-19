using DarkSky.Commerce.Models;
using Orchard.ContentManagement.Handlers;

namespace DarkSky.Commerce.Services {
    public interface IProductHandler : IContentHandler {
        void GetProductMetadata(GetProductMetadataContext context);
        void Purchase(PurchaseProductContext context);
    }

    public class ProductHandler : ContentHandler, IProductHandler {
        public virtual void GetProductMetadata(GetProductMetadataContext context) {}
        public virtual void Purchase(PurchaseProductContext context) {}
    }

    public class GetProductMetadataContext {
        public IProductAspect Product { get; set; }
        public ProductMetadata Metadata { get; set; } 
    }

    public class PurchaseProductContext {
        public OrderPart Order { get; set; }
        public InvoicePart Invoice { get; set; }
        public IProductAspect Product { get; set; }
        public int Quantity { get; set; }
    }
}