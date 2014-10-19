using DarkSky.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Drivers {
    [OrchardFeature("DarkSky.Commerce.PaymentProviders.PayPal")]
    public class PayPalSettingPartDriver : ContentPartDriver<PayPalSettingsPart> {

        protected override string Prefix {
            get { return "PayPalSettings"; }
        }

        protected override DriverResult Editor(PayPalSettingsPart part, dynamic shapeHelper) {
            return ContentShape("Parts_PayPalSettings_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/PayPalSettings", Model: part, Prefix: Prefix)).OnGroup("PayPal");
        }

        protected override DriverResult Editor(PayPalSettingsPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}