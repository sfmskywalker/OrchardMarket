using System;

namespace DarkSky.OrchardMarket.Models {
    public class PackageManifest {
        public string Name { get; set; }
        public string Description { get; set; }
        public ExtensionType ExtensionType { get; set; }
        public Version Version { get; set; }
        public VersionRange SupportedOrchardVersionsRange { get; set; }
        public string[] Tags { get; set; }
    }
}