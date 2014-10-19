using System.IO;
using DarkSky.OrchardMarket.Models;
using Orchard;

namespace DarkSky.OrchardMarket.Services {
    public interface IPackageStorageProvider : IDependency {
        string GetPath(PackageReleasePart release);
        void StorePackage(PackageReleasePart release, Stream stream);
        Stream LoadPackage(PackageReleasePart release);
    }
}