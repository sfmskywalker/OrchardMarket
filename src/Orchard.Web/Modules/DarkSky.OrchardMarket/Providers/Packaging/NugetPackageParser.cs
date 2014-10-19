using System;
using System.IO;
using System.Linq;
using System.Web;
using DarkSky.OrchardMarket.Models;
using NuGet;
using Orchard.Environment.Extensions.Folders;

namespace DarkSky.OrchardMarket.Providers.Packaging {
    public class NugetPackageParser : IPackageParser {
        public bool SupportsFile(HttpPostedFileBase file) {
            return Path.GetExtension(file.FileName) == ".nupkg";
        }

        public PackageManifest ReadFile(HttpPostedFileBase file) {
            var factory = new ZipPackageFactory();
            var data = file.InputStream.ReadAllBytes();
            var package = factory.CreatePackage(() => new MemoryStream(data));
            var manifestFile = package.GetFiles().FirstOrDefault(f => f.Path.EndsWith("Module.txt") || f.Path.EndsWith("Theme.txt"));
            var extensionType = manifestFile.Path.EndsWith("Module.txt", StringComparison.InvariantCultureIgnoreCase) ? ExtensionType.Module : ExtensionType.Theme;
            var manifestText = manifestFile.GetStream().ReadToEnd();
            var extensionDescriptor = ExtensionHarvester.GetDescriptorForExtension(manifestFile.Path, package.Id, extensionType.ToString(), manifestText);
            var tags = extensionDescriptor.Tags ?? "";

            return new PackageManifest {
                Name = package.Title,
                Description = package.Description,
                Version = package.Version,
                SupportedOrchardVersionsRange = new VersionRange(),
                ExtensionType = extensionType,
                Tags = tags.Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries)
            };
        }
    }
}