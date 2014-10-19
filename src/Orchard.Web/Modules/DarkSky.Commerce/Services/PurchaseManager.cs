using System.Collections.Generic;
using DarkSky.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Services {
    public interface IPurchaseManager : IDependency {
        PurchasePart CreatePurchase(InvoicePart invoice, IProductAspect product, int quantity);
        IEnumerable<PurchasePart> GetPurchasesByUser(int userId);
    }

    [OrchardFeature("DarkSky.Commerce.Products")]
    public class PurchaseManager : IPurchaseManager {
        private readonly IContentManager _contentManager;
        private readonly IProductHandler _productHandler;

        public PurchaseManager(IContentManager contentManager, IProductHandler productHandler) {
            _contentManager = contentManager;
            _productHandler = productHandler;
        }

        public PurchasePart CreatePurchase(InvoicePart invoice, IProductAspect product, int quantity) {
            var purchase = _contentManager.Create<PurchasePart>("Purchase", p => {
                p.Invoice = invoice;
                p.User = invoice.Order.User;
                p.Product = product;
                p.Quantity = quantity;
            });

            var context = new PurchaseProductContext {
                Invoice = invoice,
                Quantity = quantity,
                Product = product,
                Order = invoice.Order
            };

            _productHandler.Purchase(context);
            return purchase;
        }

        public IEnumerable<PurchasePart> GetPurchasesByUser(int userId) {
            return _contentManager.Query<PurchasePart>().Join<CommonPartRecord>().Where(x => x.OwnerId == userId).List();
        }
    }
}