using System.Linq;
using DarkSky.Commerce.Models;
using DarkSky.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Handlers {
    [OrchardFeature("DarkSky.Commerce.Orders")]
    public class InvoicePartHandler : ContentHandler {
        private readonly IContentManager _contentManager;
        private readonly IInvoiceManager _invoiceManager;

        public InvoicePartHandler(IRepository<InvoicePartRecord> repository, IContentManager contentManager, IInvoiceManager invoiceManager) {
            _contentManager = contentManager;
            _invoiceManager = invoiceManager;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<InvoicePart>(PropertyHandlers);
        }

        private void PropertyHandlers(ActivatedContentContext context, InvoicePart part) {
            part.OrderField.Loader(orderPart => part.OrderId != null ? _contentManager.Get<OrderPart>(part.OrderId.Value) : null);
            part.OrderField.Setter(x => { part.OrderId = x.Id; return x; });
            part.DetailsField.Loader(list => _invoiceManager.GetDetails(part.Id).ToList());
        }
    }
}