using System;
using System.Linq;
using DarkSky.OrchardMarket.Models;
using DarkSky.OrchardMarket.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace DarkSky.OrchardMarket.Handlers {
    public class PackageReleasePartHandler : ContentHandler {
        private readonly IPackageServices _packageServices;
        private readonly IContentManager _contentManager;

        public PackageReleasePartHandler(IRepository<PackageReleasePartRecord> repository, IPackageServices packageServices, IContentManager contentManager) {
            _packageServices = packageServices;
            _contentManager = contentManager;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<PackageReleasePart>(PropertySetHandlers);
            OnPublished<PackageReleasePart>(PublishedHandler);
            OnUnpublished<PackageReleasePart>(UnpublishedHandler);
        }

        private void PublishedHandler(PublishContentContext context, PackageReleasePart part) {
            if(!part.Package.IsPublished()) {
                _contentManager.Publish(part.Package.ContentItem);
            }
        }

        private void UnpublishedHandler(PublishContentContext context, PackageReleasePart part) {
            if(!part.Package.Releases.Any(x => x.IsPublished()))
                _contentManager.Unpublish(part.Package.ContentItem);
        }

        private void PropertySetHandlers(ActivatedContentContext context, PackageReleasePart part) {
            part.SupportedOrchardVersionRangeField.Loader(x => {
                VersionRange versionRange;
                VersionRange.TryParse(part.Record.SupportedOrchardVersionsRange, out versionRange);
                return versionRange ?? new VersionRange();
            });
            part.SupportedOrchardVersionRangeField.Setter(x => {
                part.Record.SupportedOrchardVersionsRange = x.ToString();
                return x;
            });

            part.VersionField.Loader(x => {
                Version version;
                Version.TryParse(part.Record.Version, out version);
                return version ?? new Version();
            });
            part.VersionField.Setter(x => {
                part.Record.Version = x.ToString();
                return x;
            });

            part.PackageField.Loader(x => _packageServices.GetPackage(part.Record.PackageId, VersionOptions.Latest));
            part.PackageField.Setter(x => {
                part.Record.PackageId = x != null ? x.Id : 0;
                return x;
            });
        }
    }
}