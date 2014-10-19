using System.Linq;
using DarkSky.OrchardMarket.Services;
using DarkSky.OrchardMarket.ViewModels;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using DarkSky.OrchardMarket.Models;
using Orchard.Core.Common.Models;

namespace DarkSky.OrchardMarket.Drivers {
    [UsedImplicitly]
    public class PackagePartDriver : ContentPartDriver<PackagePart> {
        private readonly IPackageServices _packageServices;
        private const string TemplateName = "Parts/Package";

        public PackagePartDriver(IPackageServices packageServices) {
            _packageServices = packageServices;
        }

        protected override string Prefix {
            get { return "Package"; }
        }

        protected override DriverResult Display(PackagePart part, string displayType, dynamic shapeHelper) {
	        return Combined(
                ContentShape("Parts_Package_Summary", () => shapeHelper.Parts_Package_Summary()),
                ContentShape("Parts_Package", () => shapeHelper.Parts_Package(
                    Releases: part.Releases.Select(x => shapeHelper.Release(
                    Id: x.Id,
                    Version: x.Version,
                    Sales: x.Sales,
                    Downloads: x.Downloads,
                    SupportedOrchardVersionsRange: x.SupportedOrchardVersionsRange,
                    IsPublished: x.IsPublished(),
                    CreatedUtc: x.As<CommonPart>().CreatedUtc)),
                LogoUrl: _packageServices.GetLogoUrl(part)))
            );
        }

        protected override DriverResult Editor(PackagePart part, dynamic shapeHelper) {
            var viewModel = new PackageViewModelAdmin {
                ExtensionType = part.ExtensionType,
            };
	        return ContentShape("Parts_Package_Edit", () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: viewModel, Prefix: Prefix));
        }

        protected override DriverResult Editor(PackagePart part, IUpdateModel updater, dynamic shapeHelper) {
            var viewModel = new PackageViewModelAdmin();
	        if(updater.TryUpdateModel(viewModel, Prefix, null, null)) {
	            part.ExtensionType = viewModel.ExtensionType;
	        }
            return Editor(part, shapeHelper);
        }

    }
}