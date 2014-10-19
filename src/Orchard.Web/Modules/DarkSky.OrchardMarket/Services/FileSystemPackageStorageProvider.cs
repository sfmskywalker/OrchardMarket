using System.IO;
using System.Web;
using System.Web.Hosting;
using DarkSky.OrchardMarket.Models;
using Orchard;
using Orchard.ContentManagement;

namespace DarkSky.OrchardMarket.Services {
    public class FileSystemPackageStorageProvider : IPackageStorageProvider {
        private readonly IOrchardServices _services;

        public FileSystemPackageStorageProvider(IOrchardServices services) {
            _services = services;
        }

        public string GetPath(PackageReleasePart release) {
            var settings = _services.WorkContext.CurrentSite.As<GallerySettingsPart>();
            var package = release.Package;
            var packagePath = string.Format("{0}/{1}/{2}", package.User.UserName, package.ExtensionType, release.FileName);
            var path = VirtualPathUtility.AppendTrailingSlash(settings.PackagesPath) + packagePath;
            return HostingEnvironment.MapPath(path);
        }

        public void StorePackage(PackageReleasePart release, Stream stream) {
            SaveStream(GetPath(release), stream);
        }

        public Stream LoadPackage(PackageReleasePart release) {
            return File.OpenRead(GetPath(release));
        }

        public void SaveStream(string path, Stream inputStream) {

            EnsureDirectory(path);
            using (var outputStream = File.OpenWrite(path)) {
                var buffer = new byte[8192];
                inputStream.Position = 0;
                while (true) {
                    var length = inputStream.Read(buffer, 0, buffer.Length);
                    if (length <= 0)
                        break;
                    outputStream.Write(buffer, 0, length);
                }
            }
        }

        private static void EnsureDirectory(string path) {
            var directory = Path.GetDirectoryName(path);

            if(!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
        }
    }
}