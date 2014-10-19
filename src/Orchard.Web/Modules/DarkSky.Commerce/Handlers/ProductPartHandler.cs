using DarkSky.Commerce.Models;
using DarkSky.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Handlers {
    [OrchardFeature("DarkSky.Commerce.Products")]
    public class ProductPartHandler : ProductHandler {
        private readonly IContentManager _contentManager;

        public ProductPartHandler(IRepository<ProductPartRecord> repository, IContentManager contentManager) {
            _contentManager = contentManager;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<ProductPart>(PropertyHandlers);
        }

        public override void GetProductMetadata(GetProductMetadataContext context) {
            context.Metadata.PrimaryImageUrl = context.Product.ImageUrl;
        }

        private void PropertyHandlers(ActivatedContentContext context, ProductPart part) {
            part.VatField.Loader(vatPart => part.Record.VatId != null ? _contentManager.Get<VatPart>(part.Record.VatId.Value) : null);
            part.VatField.Setter(vatPart => {
                part.Record.VatId = vatPart != null ? vatPart.Id : default(int?);
                return vatPart;
            });
        }
    }
}