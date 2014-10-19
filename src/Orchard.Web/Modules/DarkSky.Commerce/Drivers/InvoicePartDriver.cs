using System.Linq;
using DarkSky.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Drivers {
    [OrchardFeature("DarkSky.Commerce.Orders")]
    public class InvoicePartDriver : ContentPartDriver<InvoicePart> {
        private readonly IContentManager _contentManager;

        public InvoicePartDriver(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        protected override DriverResult Display(InvoicePart part, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_Invoice_Summary", () => shapeHelper.Parts_Invoice_Summary()),
                ContentShape("Parts_Invoice", () => shapeHelper.Parts_Invoice(
                    InvoiceDetails: part.Details.Select(detail => shapeHelper.Detail(
                        Id: detail.Id,
                        Description: detail.Description,
                        UnitPrice: detail.UnitPrice,
                        ProductId: detail.ProductId,
                        Product: detail.ProductId != null ? _contentManager.Get<IProductAspect>(detail.ProductId.Value) : default(IProductAspect),
                        Quantity: detail.Quantity,
                        SubTotal: detail.SubTotal(),
                        Vat: detail.Vat(),
                        Total: detail.Total())).ToList())));
        }
    }
}