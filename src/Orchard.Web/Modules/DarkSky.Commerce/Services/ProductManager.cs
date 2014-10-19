using System;
using System.Collections.Generic;
using System.Linq;
using DarkSky.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;

namespace DarkSky.Commerce.Services {
    public interface IProductManager : IDependency {
        ProductMetadata GetProductMetadata(IProductAspect product);
        IEnumerable<IProductAspect> GetProducts(IEnumerable<int> ids, VersionOptions versionOptions);
        Totals CalculateItemTotals(ProductQuantity args);
        VatPart CreateVat(float percentage, string title, string description = null, bool active = true);
        IEnumerable<VatPart> GetVatItems();
        VatPart GetDefaultVat();
        IProductAspect GetProduct(int id);
    }

    public class ProductManager : IProductManager {
        private readonly Lazy<IProductHandler> _productHandler;
        private readonly IContentManager _contentManager;
        private VatPart _defaultVat;

        public ProductManager(Lazy<IProductHandler> productHandler, IContentManager contentManager) {
            _productHandler = productHandler;
            _contentManager = contentManager;
        }

        public ProductMetadata GetProductMetadata(IProductAspect product) {
            var context = new GetProductMetadataContext {
                Product = product,
                Metadata = new ProductMetadata()
            };
            _productHandler.Value.GetProductMetadata(context);
            return context.Metadata;
        }

        public IEnumerable<IProductAspect> GetProducts(IEnumerable<int> ids, VersionOptions versionOptions) {
            return _contentManager.GetMany<IProductAspect>(ids, versionOptions, QueryHints.Empty);
        }

        public VatPart CreateVat(float percentage, string title, string description = null, bool active = true) {
            return _contentManager.Create<VatPart>("Vat", item => {
                item.Rate = percentage;
                item.Description = description;
                item.IsActive = active;
                item.Name = title;
            });
        }

        public IEnumerable<VatPart> GetVatItems() {
            return _contentManager.Query<VatPart>(VersionOptions.Published).List();
        }

        public VatPart GetDefaultVat() {
            return _defaultVat ?? (_defaultVat = _contentManager.Query<VatPart, VatPartRecord>(VersionOptions.Published).OrderByDescending(x => x.Rate).List().FirstOrDefault());
        }

        public IProductAspect GetProduct(int id) {
            return _contentManager.Get<IProductAspect>(id);
        }

        public Totals CalculateItemTotals(ProductQuantity args) {
            var vatPart = GetDefaultVat() ?? new VatPart { Rate = 0f };
            var subTotal = args.SubTotal;
            var vat = (decimal)vatPart.Rate*subTotal;
            var total = subTotal + vat;

            return new Totals {
                SubTotal = subTotal,
                Vat = vat,
                Total = total
            };
        }
    }
}