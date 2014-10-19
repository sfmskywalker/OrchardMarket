using System.Linq;
using DarkSky.Commerce.EventHandlers;
using DarkSky.Commerce.Models;
using DarkSky.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Handlers {
    [OrchardFeature("DarkSky.Commerce.Orders")]
    public class DefaultInvoiceEventHandler : IInvoiceEventHandler {
        private readonly IContentManager _contentManager;
        private readonly IPurchaseManager _purchaseManager;
        private readonly IProductManager _productManager;

        public DefaultInvoiceEventHandler(IContentManager contentManager, IPurchaseManager purchaseManager, IProductManager productManager) {
            _contentManager = contentManager;
            _purchaseManager = purchaseManager;
            _productManager = productManager;
        }

        public void StatusChanged(InvoiceStatusChangedContext context) {
            if (context.NewStatus != InvoiceStatus.Paid) return;

            UpdateSalesCount(context);
            CreatePurchases(context);
        }

        private void CreatePurchases(InvoiceStatusChangedContext context) {
            foreach (var detail in context.Invoice.Details.Where(x => x.ProductId != null)) {
                var product = _productManager.GetProduct(detail.ProductId.Value);
                var purchase = _purchaseManager.CreatePurchase(context.Invoice, product, detail.Quantity);
            }
        }

        private void UpdateSalesCount(InvoiceStatusChangedContext context) {
            foreach (var item in context.Invoice.Details.Where(x => x.ProductId != null)) {
                var product = _contentManager.Get<IProductAspect>(item.ProductId.Value);

                if (product != null) {
                    product.Sales += item.Quantity;
                }
            }
        }
    }
}