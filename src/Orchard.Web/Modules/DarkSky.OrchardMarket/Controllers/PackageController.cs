using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Contrib.Voting.Models;
using Contrib.Voting.Services;
using DarkSky.Commerce.Models;
using DarkSky.OrchardMarket.Helpers;
using DarkSky.OrchardMarket.Models;
using DarkSky.OrchardMarket.Services;
using DarkSky.OrchardMarket.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Contents.Controllers;
using Orchard.Core.Title.Models;
using Orchard.Themes;
using Orchard.UI.Notify;

namespace DarkSky.OrchardMarket.Controllers {
    [Authorize, Themed]
    public class PackageController : ControllerBase {
        private readonly IOrchardServices _services;
        private readonly IPackageServices _packageServices;
        private readonly IVotingService _votingService;
        private readonly IOrganizationService _organizationService;

        public PackageController(IOrchardServices services, IPackageServices packageServices, IVotingService votingService, IOrganizationService organizationService) {
            _services = services;
            _packageServices = packageServices;
            _votingService = votingService;
            _organizationService = organizationService;
        }

        public ActionResult Index() {
            var organizations = _organizationService.GetOrganizationsByCurrentUser().Select(x => x.Id).ToList();
            var packages = _packageServices.GetPackagesByOrganizations(organizations, VersionOptions.Latest).Select(x => _services.New.Package(
                    PackageId: x.Id,
                    Organization: x.Organization,
                    Name: x.As<TitlePart>().Title,
                    PackageType: x.ExtensionType,
                    UnitPrice: x.As<IProductAspect>().UnitPrice,
                    Downloads: x.Downloads,
                    Sales: x.As<IProductAspect>().Sales,
                    Version: x.LatestRelease.Version,
                    IsPublished: x.IsPublished()
                )).ToList();
            var viewModel = _services.New.ViewModel(Packages: packages);
            return View((object)viewModel);
        }

        public ActionResult Create() {
            if (!_organizationService.GetOrganizationsByCurrentUser().Any())
                return RedirectToAction("OrganizationRequired");
            return View(new CreatePackageViewModel());
        }

        [HttpPost]
        public ActionResult Create(CreatePackageViewModel model) {

            var packageFile = Request.Files["PackageFile"];
            PackageManifest manifest = null;

            if (packageFile.ContentLength == 0)
                ModelState.AddModelError("PackageFile", T("Please specify a nuget file to upload").ToString());
            else
                manifest = _packageServices.ReadPackage(packageFile);

            if (manifest == null)
                ModelState.AddModelError("PackageFile", T("Unknown format. Only Nuget packages are currently supported").ToString());

            if (!ModelState.IsValid)
                return View(model);

            var organizationId = model.OrganizationId;
            var package = organizationId != null ? _packageServices.GetPackageByName(manifest.Name, organizationId.Value, VersionOptions.Latest) : default(PackagePart);
            var packageExists = package != null;

            var organization = _organizationService.GetOrganization(organizationId.Value);
            package = _packageServices.CreatePackage(manifest, organization);
            package.LatestRelease.FileName = _packageServices.GenerateFileName(package.LatestRelease, Path.GetExtension(packageFile.FileName));
            _packageServices.SaveReleaseFile(package.LatestRelease, packageFile.InputStream);

            if(!packageExists)
                _services.Notifier.Information(T("Your Package has been created"));  
            else
                _services.Notifier.Warning(T("The uploaded file has been added as a release to {0}", package.Name));

            return RedirectToAction("Edit", new { id = package.Id });
        }

        public ActionResult AddRelease(int id) {
            //_services.Authorizer.Authorize()
            var package = _packageServices.GetPackage(id, VersionOptions.Latest);
            var viewModel = _services.New.ViewModel( Package: package);
            return View("AddRelease", (object)viewModel);
        }

        [HttpPost]
        public ActionResult AddRelease(HttpPostedFileBase packageFile, int packageId) {

            if (packageFile.ContentLength == 0)
                ModelState.AddModelError("packageFile", T("Please specify a nuget file to upload").Text);

            var package = _packageServices.GetPackage(packageId, VersionOptions.Latest);
            var manifest = _packageServices.ReadPackage(packageFile);

            if (package.Name != manifest.Name) {
                ModelState.AddModelError("packageFile", T("The uploaded package does not match the specified package's manifest").Text);
            }

            if (!ModelState.IsValid)
                return View(new { Package = package });

            var release = _packageServices.CreateRelease(package, manifest);

            release.FileName = _packageServices.GenerateFileName(package.LatestRelease, Path.GetExtension(packageFile.FileName));
            _packageServices.SaveReleaseFile(release, packageFile.InputStream);
            _services.Notifier.Information(T("Your Package has been updated with the new Release"));

            return RedirectToAction("Edit", new { id = package.Id });
        }

        public ActionResult Edit(int id) {
            var package = _packageServices.GetPackage(id, VersionOptions.Latest);
            var product = package.As<IProductAspect>();
            var model = new PackageViewModel {
                Description = package.Description,
                Name = package.Name,
                ExtensionType = package.ExtensionType,
                Price = product.UnitPrice,
                Id = package.Id,
                LogoUrl = _packageServices.GetLogoUrl(package),
                Version = package.LatestRelease.Version.ToString(),
                SupportedOrchardVersions = package.LatestRelease.SupportedOrchardVersionsRange.ToString(),
                Categories = _packageServices.GetCategories(package).Select(x => x.Id).ToArray(),
                Tags = string.Join(", ", _packageServices.GetTags(package).Select(x => x.Name))
            };
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public ActionResult Save(PackageViewModel model, HttpPostedFileBase logoFile) {
            return UpdatePackage(model, logoFile, package => _services.Notifier.Information(T("Your Package has been updated")));
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Publish")]
        public ActionResult Edit(PackageViewModel model, HttpPostedFileBase logoFile) {
            return UpdatePackage(model, logoFile, package => {
                _services.ContentManager.Publish(package.ContentItem);
                _services.Notifier.Information(T("Your Package has been updated"));
            });
        }

        public ActionResult Details(int id) {
            var package = _packageServices.GetPackage(id, VersionOptions.Latest);
            var viewModel = _services.New.ViewModel(
                Package: package,
                Rating: _votingService.GetResult(package.Id, "average") ?? new ResultRecord(),
                Tags: _packageServices.GetTags(package).ToList(),
                Releases: package.Releases.Select(x => _services.New.Release(
                    Id: x.Id,
                    Version: x.Version,
                    Sales: x.Sales,
                    Downloads: x.Downloads,
                    SupportedOrchardVersionsRange: x.SupportedOrchardVersionsRange,
                    IsPublished: x.IsPublished(),
                    CreatedUtc: x.As<CommonPart>().CreatedUtc)),
                LogoUrl: _packageServices.GetLogoUrl(package)
            );
            return View((object)viewModel);
        }

        public ActionResult Publish(int id) {
            var package = _services.ContentManager.Get<PackagePart>(id, VersionOptions.Draft);

            _services.ContentManager.Publish(package.ContentItem);
            _services.Notifier.Information(T("{0} has been succesfully published", package.Name));
            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        public ActionResult Unpublish(int id) {
            var package = _services.ContentManager.Get<PackagePart>(id, VersionOptions.Published);

            _services.ContentManager.Unpublish(package.ContentItem);
            _services.Notifier.Information(T("{0} has been succesfully unpublished", package.Name));
            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        public ActionResult Download(int id) {
            var package = _packageServices.GetPackage(id, VersionOptions.Published);
            var release = package.LatestRelease;
            var stream = _packageServices.LoadPackage(release);
            return File(stream, "application/x-zip-compressed", string.Format("{0}.nupkg", package.DisplayName));
        }

        private ActionResult UpdatePackage(PackageViewModel model, HttpPostedFileBase logoFile, Action<PackagePart> publishAction) {

            Version version;
            VersionRange supportedOrchardVersions;

            if (!Version.TryParse(model.Version, out version)) {
                ModelState.AddModelError("Version", T("The specified version is invalid").Text);
            }

            if (!VersionRange.TryParse(model.SupportedOrchardVersions, out supportedOrchardVersions)) {
                ModelState.AddModelError("SupportedOrchardVersions", T("The specified supported Orchard version range is invalid").Text);
            }

            if (!ModelState.IsValid) {
                return View(model);
            }

            var package = _packageServices.GetPackage(model.Id, VersionOptions.DraftRequired);
            var product = package.As<IProductAspect>();
            var tags = model.Tags ?? "";

            package.Name = model.Name.TrimSafe();
            package.Description = model.Description.TrimSafe();
            package.ExtensionType = model.ExtensionType;
            package.User = _services.WorkContext.CurrentUser;
            package.LatestRelease.Version = version;
            package.LatestRelease.SupportedOrchardVersionsRange = supportedOrchardVersions;
            product.UnitPrice = model.Price;
            _packageServices.UpdateCategories(package, model.Categories);
            _packageServices.UpdateTags(package, tags.Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));

            if (logoFile != null && logoFile.ContentLength > 0)
                _packageServices.UpdateLogo(logoFile, package);

            publishAction(package);
            return RedirectToAction("Details", new { id = package.Id });
        }

        public ActionResult OrganizationRequired() {
            return View();
        }
    }
}