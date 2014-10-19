using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using DarkSky.OrchardMarket.Models;

namespace DarkSky.OrchardMarket.Drivers {
    [UsedImplicitly]
    public class ExtensionsCatalogPartDriver : ContentPartDriver<PackageCatalogPart> {
	    private const string TemplateName = "Parts/ExtensionsCatalog";

        protected override string Prefix {
            get { return "ExtensionsCatalog"; }
        }

        protected override DriverResult Display(PackageCatalogPart part, string displayType, dynamic shapeHelper) {
	        return ContentShape("Parts_ExtensionsCatalog", () => shapeHelper.Parts_ExtensionsCatalog());
        }

        protected override DriverResult Editor(PackageCatalogPart part, dynamic shapeHelper) {
	        return ContentShape("Parts_ExtensionsCatalog_Edit", () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(PackageCatalogPart part, IUpdateModel updater, dynamic shapeHelper) {
	        updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

    }
}