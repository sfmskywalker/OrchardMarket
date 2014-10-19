using DarkSky.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Drivers {
    [OrchardFeature("DarkSky.Commerce.Products")]
    public class VatPartDriver : ContentPartDriver<VatPart> {

        protected override string Prefix {
            get { return "Vat"; }
        }

        protected override DriverResult Editor(VatPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Vat_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Vat", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(VatPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}