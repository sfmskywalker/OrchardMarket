using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DarkSky.Commerce.Models;
using DarkSky.Commerce.Services;
using DarkSky.OrchardMarket.Helpers;
using DarkSky.OrchardMarket.Models;
using DarkSky.OrchardMarket.Providers.Packaging;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.FileSystems.Media;
using Orchard.Projections.Services;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;
using Orchard.UI.Navigation;

namespace DarkSky.OrchardMarket.Services {
    public interface IPackageServices : IDependency {
        IEnumerable<PackagePart> GetPackagesByOrganizations(IEnumerable<int> organizationIds, VersionOptions versionOptions);
        IEnumerable<PackageReleasePart> GetReleases(PackagePart package, VersionOptions versionOptions);
        PackageReleasePart GetLatestRelease(PackagePart package);
        PackageReleasePart NewRelease(PackagePart package);
        void CreateRelease(PackageReleasePart release);
        PackagePart GetPackage(int id, VersionOptions versionOptions);
        PackagePart GetPackageByName(string name, int organizationId, VersionOptions versionOptions);
        PackageManifest ReadPackage(HttpPostedFileBase packageFile);
        PackagePart CreatePackage(PackageManifest manifest, OrganizationPart organization);
        string GenerateFileName(PackageReleasePart release, string extension);
        void SaveReleaseFile(PackageReleasePart release, Stream inputStream);
        PackageReleasePart CreateRelease(PackagePart package, PackageManifest manifest);
        void UpdateLogo(HttpPostedFileBase file, PackagePart package);
        string GetLogoUrl(PackagePart package);
        string GetLogoUrl(string path);
        IEnumerable<PackagePart> Find(FindPackagesArgs args, Pager pager);
        int Count(FindPackagesArgs args);
        void UpdateCategories(PackagePart package, IEnumerable<int> categories);
        void UpdateTags(PackagePart package, IEnumerable<string> tags);
        IEnumerable<TermPart> GetCategories(PackagePart package);
        IEnumerable<TermPart> GetTags(PackagePart package);
        Stream LoadPackage(PackageReleasePart release);
    }

    public class PackageServices : Component, IPackageServices {
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<IPackageParser> _packageProviders;
        private readonly IPackageStorageProvider _packageStorageProvider;
        private readonly IStorageProvider _storageProvider;
        private readonly IOrchardServices _services;
        private readonly IProjectionManager _projectionManager;
        private readonly IOrganizationService _organizationService;
        private readonly ITaxonomyService _taxonomyService;
        private readonly IProductManager _productManager;

        public PackageServices(
            IEnumerable<IPackageParser> packageProviders, 
            IPackageStorageProvider packageStorageProvider, 
            IStorageProvider storageProvider, 
            IOrchardServices services, 
            IProjectionManager projectionManager, 
            IOrganizationService organizationService, 
            ITaxonomyService taxonomyService, 
            IProductManager productManager) {
            _contentManager = services.ContentManager;
            _packageProviders = packageProviders;
            _packageStorageProvider = packageStorageProvider;
            _storageProvider = storageProvider;
            _services = services;
            _projectionManager = projectionManager;
            _organizationService = organizationService;
            _taxonomyService = taxonomyService;
            _productManager = productManager;
        }

        public IEnumerable<PackagePart> GetPackagesByOrganizations(IEnumerable<int> organizationIds, VersionOptions versionOptions) {
            return _contentManager.Query<PackagePart>(versionOptions, "Package").Where<CommonPartRecord>(x => organizationIds.Contains(x.Container.Id)).List();
        }

        public IEnumerable<PackageReleasePart> GetReleases(PackagePart package, VersionOptions versionOptions) {
            return package.ContentItem.ContentManager.Query<PackageReleasePart>(versionOptions, "PackageRelease").Where<PackageReleasePartRecord>(x => x.PackageId == package.Id).List();
        }

        public PackageReleasePart GetLatestRelease(PackagePart package) {
            var query = from r in package.Releases select r;

            if(package.IsPublished())
                query = from r in query where r.IsPublished() select r;

            var release = query.OrderByDescending(x => x.Version).FirstOrDefault() ?? package.Releases.OrderByDescending(x => x.Version).FirstOrDefault();

            if(release == null) {;
                release = NewRelease(package);
                release.Package = package;
                release.Version = new Version(1, 0);
                release.ReleaseNotes = T("Initial Release").Text;
                CreateRelease(release);
            }

            return release;
        }

        public PackagePart CreatePackage(PackageManifest manifest, OrganizationPart organization) {
            var packagePart = GetPackageByName(manifest.Name, organization.Id, VersionOptions.Latest) ?? _contentManager.New<PackagePart>("Package");
            var productPart = packagePart.As<IProductAspect>();
            var shopPart = organization.As<IShopAspect>();

            packagePart.Name = manifest.Name;
            packagePart.Description = manifest.Description;
            packagePart.ExtensionType = manifest.ExtensionType;
            packagePart.User = organization.Manager;
            productPart.Vat = _productManager.GetDefaultVat();
            productPart.Shop = shopPart;

            if (packagePart.Id == 0)
                _contentManager.Create(packagePart, VersionOptions.Draft);

            var release = packagePart.LatestRelease;
            release.Version = manifest.Version;
            release.SupportedOrchardVersionsRange = manifest.SupportedOrchardVersionsRange;

            return packagePart;
        }

        public PackagePart GetPackageByName(string name, int organizationId, VersionOptions versionOptions) {
            return _contentManager
                .Query<PackagePart, PackagePartRecord>(versionOptions)
                .Join<CommonPartRecord>()
                .Where(x => x.Container.Id == organizationId)
                .Join<TitlePartRecord>().Where(x => x.Title == name)
                .ForType("Package")
                .List().FirstOrDefault();
        }

        public void SaveReleaseFile(PackageReleasePart release, Stream inputStream) {
            _packageStorageProvider.StorePackage(release, inputStream);
        }

        public string GenerateFileName(PackageReleasePart release, string extension) {
            var package = release.Package;
            return string.Format("{0}-{1}{2}", package.Name, release.Version, extension);
        }

        public PackageReleasePart NewRelease(PackagePart package) {
            var release = package.ContentItem.ContentManager.New<PackageReleasePart>("PackageRelease");
            release.Record.PackageId = package.Id;
            return release;
        }

        public void CreateRelease(PackageReleasePart release) {
            _contentManager.Create(release);
        }

        public PackageReleasePart CreateRelease(PackagePart package, PackageManifest manifest) {
            var release = NewRelease(package);
            release.Version = manifest.Version;
            release.SupportedOrchardVersionsRange = manifest.SupportedOrchardVersionsRange;
            CreateRelease(release);
            return release;
        }

        public PackagePart GetPackage(int id, VersionOptions versionOptions) {
            return _contentManager.Get<PackagePart>(id, versionOptions);
        }

        public PackageManifest ReadPackage(HttpPostedFileBase packageFile) {
            var provider = GetPackageProvider(packageFile);
            return provider != null ? provider.ReadFile(packageFile) : null;
        }

        public void UpdateLogo(HttpPostedFileBase file, PackagePart package) {
            var user = package.User;
            var folder = string.Format("Users\\{0}", user.Id);
            var fileName = string.Format("package-logo={0}-{1}", package.Id, Path.GetExtension(file.FileName));
            var path = _storageProvider.Combine(folder, fileName);
            _storageProvider.TryCreateFolder(folder);

            try {
                _storageProvider.DeleteFile(path);
            }
            catch { }

            _storageProvider.SaveStream(path, file.InputStream);
            package.As<IProductAspect>().ImageUrl = path;
        }

        public string GetLogoUrl(PackagePart package) {
            var url = package.As<IProductAspect>().ImageUrl;
            if(string.IsNullOrWhiteSpace(url)) {
                url = _organizationService.GetLogoUrl(package.Organization);
            }
            return GetLogoUrl(url);
        }

        public string GetLogoUrl(string path) {
            return !string.IsNullOrWhiteSpace(path) ? string.Format(_storageProvider.GetPublicUrl(path)) : null;
        }

        public IEnumerable<PackagePart> Find(FindPackagesArgs args, Pager pager) {
            if (string.IsNullOrWhiteSpace(args.Category) && string.IsNullOrWhiteSpace(args.Tag)) {
                return _contentManager.Query<PackagePart, PackagePartRecord>().Where(x => x.SearchIndex.Contains(args.Keyword)).List();
            }

            var queryId = GetQueryId(args.ExtensionType);
            var packages = queryId != null ? _projectionManager.GetContentItems(queryId.Value, pager.GetStartIndex(), pager.PageSize) : QueryPackages(args).Slice(pager.GetStartIndex(), pager.PageSize).Select(x => x.ContentItem);
            return packages.Select(x => x.As<PackagePart>());
        }

        public int Count(FindPackagesArgs args) {

            if (string.IsNullOrWhiteSpace(args.Category) && string.IsNullOrWhiteSpace(args.Tag)) {
                return Find(args).Count();
            }

            var queryId = GetQueryId(args.ExtensionType);
            return queryId != null ? _projectionManager.GetContentItems(queryId.Value).Count() : QueryPackages(args).Count();
        }

        public void UpdateCategories(PackagePart package, IEnumerable<int> categories) {
            var terms = categories.Select(_taxonomyService.GetTerm).ToList();
            _taxonomyService.UpdateTerms(package.ContentItem, terms, "Category");
        }

        public void UpdateTags(PackagePart package, IEnumerable<string> tags) {
            var taxonomy = _taxonomyService.GetTaxonomyByName("Tags");
            var terms = new List<TermPart>();

            foreach (var tag in tags.Select(x => x.TrimSafe())) {
                var term = _taxonomyService.GetTermByName(taxonomy.Id, tag);

                if (term == null) {
                    term = _taxonomyService.NewTerm(taxonomy);
                    term.Name = tag;
                    term.Selectable = true;
                    _contentManager.Create(term, VersionOptions.Published);
                }

                terms.Add(term);
            }

            _taxonomyService.UpdateTerms(package.ContentItem, terms, "Tags");
        }

        public IEnumerable<TermPart> GetCategories(PackagePart package) {
            return _taxonomyService.GetTermsForContentItem(package.Id, "Category");
        }

        public IEnumerable<TermPart> GetTags(PackagePart package) {
            return _taxonomyService.GetTermsForContentItem(package.Id, "Tags");
        }

        public IEnumerable<PackagePart> Find(FindPackagesArgs args) {
            var queryId = GetQueryId(args.ExtensionType);
            var packages = queryId != null ? _projectionManager.GetContentItems(queryId.Value) : QueryPackages(args).List().Select(x => x.ContentItem);
            return packages.Select(x => x.As<PackagePart>());
        }

        public Stream LoadPackage(PackageReleasePart release) {
            return _packageStorageProvider.LoadPackage(release);
        }

        private IContentQuery<PackagePart> QueryPackages(FindPackagesArgs args) {
            return _contentManager
                .Query<PackagePart, PackagePartRecord>(VersionOptions.Published)
                .Where(x => x.ExtensionType == args.ExtensionType)
                .ForType("Package");
        } 

        private int? GetQueryId(ExtensionType extensionType) {
            var settings = _services.WorkContext.CurrentSite.As<GallerySettingsPart>();
            var queryDictionary = new Dictionary<ExtensionType, int?> {
                {ExtensionType.Module, settings.ModulesQueryId},
                {ExtensionType.Theme, settings.ThemesQueryId}
            };
            return queryDictionary[extensionType];
        }

        private IPackageParser GetPackageProvider(HttpPostedFileBase packageFile) {
            return _packageProviders.FirstOrDefault(x => x.SupportsFile(packageFile));
        }
    }
}