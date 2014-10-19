using System.Linq;
using DarkSky.Commerce.Services;
using DarkSky.OrchardMarket.Models;
using DarkSky.OrchardMarket.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Taxonomies.Services;

namespace DarkSky.OrchardMarket.Handlers {
    public class PackagePartHandler : ProductHandler {
        private readonly IPackageServices _packageServices;
        private readonly IContentManager _contentManager;
        private readonly ITaxonomyService _taxonomyService;

        public PackagePartHandler(IRepository<PackagePartRecord> repository, IPackageServices packageServices, IContentManager contentManager, ITaxonomyService taxonomyService) {
            _packageServices = packageServices;
            _contentManager = contentManager;
            _taxonomyService = taxonomyService;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<PackagePart>(PropertyHandlers);
            OnPublished<PackagePart>(PublishedHandler);
            OnUnpublished<PackagePart>(UnpublishedHandler);
        }

        public override void GetProductMetadata(GetProductMetadataContext context) {
            if (!context.Product.Is<PackagePart>())
                return;

            var packagePart = context.Product.As<PackagePart>();
            context.Metadata.PrimaryImageUrl = packagePart.LogoUrl;
        }

        public override void Purchase(PurchaseProductContext context) {
            if (!context.Product.Is<PackagePart>())
                return;

            var package = context.Product.As<PackagePart>();
            package.Downloads++;
        }

        private void UnpublishedHandler(PublishContentContext context, PackagePart part) {
            foreach (var release in part.Releases.Where(x => x.IsPublished())) {
                _contentManager.Unpublish(release.ContentItem);
            }
            RecalculateCategoriesCount();
        }

        private void PublishedHandler(PublishContentContext context, PackagePart part) {
            RecalculateCategoriesCount();
            UpdateSearchIndex(part);
        }

        private void PropertyHandlers(ActivatedContentContext context, PackagePart part) {
            part.ReleasesField.Loader(releases => _packageServices.GetReleases(part, VersionOptions.Latest).ToList());
            part.LatestReleaseField.Loader(release => _packageServices.GetLatestRelease(part));
            part.LogoUrlField.Loader(url => _packageServices.GetLogoUrl(part));
            part.TagsField.Loader(x => _packageServices.GetTags(part).ToList().AsReadOnly());
            part.CategoriesField.Loader(x => _packageServices.GetCategories(part).ToList().AsReadOnly());
        }

        private void RecalculateCategoriesCount() {
            var taxonomy = _taxonomyService.GetTaxonomyByName("Categories");
            var terms = _taxonomyService.GetTerms(taxonomy.Id);

            foreach (var termPart in terms) {
                termPart.Record.Count = (int)_taxonomyService.GetContentItemsCount(termPart);
            }
        }

        private static void UpdateSearchIndex(PackagePart part) {
            part.SearchIndex = part.Name + part.DisplayName + part.Description;
        }
    }
}