using System.Web.Mvc;
using DarkSky.OrchardMarket.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Themes;
using Orchard.UI.Notify;

namespace DarkSky.OrchardMarket.Controllers {
    [Authorize, Themed]
	public class PackageReleaseController : ControllerBase {
        private readonly IOrchardServices _services;

        public PackageReleaseController(IOrchardServices services) {
            _services = services;
        }

        public ActionResult Publish(int id) {
            var release = _services.ContentManager.Get<PackageReleasePart>(id, VersionOptions.Latest);

            if(!release.IsPublished()) {
                _services.ContentManager.Publish(release.ContentItem);
            }

            _services.Notifier.Information(T("{0} has been succesfully published", release.Name));
            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        public ActionResult Unpublish(int id) {
            var release = _services.ContentManager.Get<PackageReleasePart>(id, VersionOptions.Latest);

            if (release.IsPublished()) {
                _services.ContentManager.Unpublish(release.ContentItem);
            }

            _services.Notifier.Information(T("{0} has been succesfully unpublished", release.Name));
            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        public ActionResult Delete(int id) {
            var release = _services.ContentManager.Get<PackageReleasePart>(id, VersionOptions.Latest);

            _services.ContentManager.Remove(release.ContentItem);
            _services.Notifier.Information(T("{0} has been succesfully removed", release.Name));
            return Redirect(Request.UrlReferrer.PathAndQuery);
        }
	}
}