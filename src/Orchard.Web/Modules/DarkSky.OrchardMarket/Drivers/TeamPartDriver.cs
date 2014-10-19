using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using DarkSky.OrchardMarket.Models;

namespace DarkSky.OrchardMarket.Drivers {
    [UsedImplicitly]
    public class TeamPartDriver : ContentPartDriver<OrganizationPart> {
	    private const string TemplateName = "Parts/company";

        protected override string Prefix {
            get { return "company"; }
        }

        protected override DriverResult Editor(OrganizationPart part, dynamic shapeHelper) {
	        return ContentShape("Parts_Team_Edit", () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(OrganizationPart part, IUpdateModel updater, dynamic shapeHelper) {
	        updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

    }
}