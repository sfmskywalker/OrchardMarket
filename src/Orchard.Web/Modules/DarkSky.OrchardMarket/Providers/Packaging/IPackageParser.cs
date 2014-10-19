using System.Web;
using DarkSky.OrchardMarket.Models;
using DarkSky.OrchardMarket.Services;
using Orchard;

namespace DarkSky.OrchardMarket.Providers.Packaging {
    public interface IPackageParser : IDependency {
        bool SupportsFile(HttpPostedFileBase file);
        PackageManifest ReadFile(HttpPostedFileBase file);
    }
}