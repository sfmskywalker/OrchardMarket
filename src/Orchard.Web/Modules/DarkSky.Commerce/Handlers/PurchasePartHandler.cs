using DarkSky.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Handlers {
    [OrchardFeature("DarkSky.Commerce.Products")]
    public class PurchasePartHandler : ContentHandler {
        private readonly IContentManager _contentManager;

        public PurchasePartHandler(IRepository<PurchasePartRecord> repository, IContentManager contentManager) {
            _contentManager = contentManager;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<PurchasePart>(PropertyHandlers);
        }

        private void PropertyHandlers(ActivatedContentContext context, PurchasePart part) {
            part.ProductField.Loader(purchasePart => _contentManager.Get<IProductAspect>(part.Record.ProductId));
            part.ProductField.Setter(product => { part.Record.ProductId = product.Id; return product; });
            part.InvoiceField.Loader(purchasePart => _contentManager.Get<InvoicePart>(part.Record.InvoiceId));
            part.InvoiceField.Setter(invoice => { part.Record.InvoiceId = invoice.Id; return invoice; });
        }
    }
}