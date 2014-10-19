using DarkSky.Commerce.Models;
using DarkSky.Commerce.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Drivers {
    [OrchardFeature("DarkSky.Commerce.Products")]
    public class ProductPartDriver : ContentPartDriver<ProductPart> {

        protected override string Prefix {
            get { return "Product"; }
        }

        protected override DriverResult Editor(ProductPart part, dynamic shapeHelper) {
            var viewModel = new ProductViewModel {
                Currency = part.Currency,
                UnitPrice = part.UnitPrice,
                VatId = part.Record.VatId
            };
            return ContentShape("Parts_Product_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Product", Model: viewModel, Prefix: Prefix));
        }

        protected override DriverResult Editor(ProductPart part, IUpdateModel updater, dynamic shapeHelper) {
            var viewModel = new ProductViewModel();
            if(updater.TryUpdateModel(viewModel, Prefix, null, null)) {
                part.Currency = viewModel.Currency;
                part.UnitPrice = viewModel.UnitPrice;
                part.Record.VatId = viewModel.VatId;
            }
            return Editor(part, shapeHelper);
        }
    }
}