using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using DarkSky.OrchardMarket.Models;

namespace DarkSky.OrchardMarket.Drivers {
    [UsedImplicitly]
    public class PackageReleasePartDriver : ContentPartDriver<PackageReleasePart> {
	    private const string TemplateName = "Parts/PackageRelease";

        protected override string Prefix {
            get { return "PackageRelease"; }
        }

        protected override DriverResult Display(PackageReleasePart part, string displayType, dynamic shapeHelper) {
	        return ContentShape("Parts_PackageRelease", () => shapeHelper.Parts_PackageRelease());
        }

        protected override DriverResult Editor(PackageReleasePart part, dynamic shapeHelper) {
	        return ContentShape("Parts_PackageRelease_Edit", () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(PackageReleasePart part, IUpdateModel updater, dynamic shapeHelper) {
	        updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

    }
}