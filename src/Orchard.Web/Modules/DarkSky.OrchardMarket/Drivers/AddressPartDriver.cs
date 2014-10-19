using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using DarkSky.OrchardMarket.Models;

namespace DarkSky.OrchardMarket.Drivers {
    [UsedImplicitly]
    public class AddressPartDriver : ContentPartDriver<AddressPart> {
	    private const string TemplateName = "Parts/Address";

        protected override string Prefix {
            get { return "Address"; }
        }

        protected override DriverResult Editor(AddressPart part, dynamic shapeHelper) {
	        return ContentShape("Parts_Address_Edit", () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(AddressPart part, IUpdateModel updater, dynamic shapeHelper) {
	        updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}