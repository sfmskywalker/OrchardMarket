using System.ComponentModel.DataAnnotations;
using DarkSky.OrchardMarket.Models;

namespace DarkSky.OrchardMarket.ViewModels {
    public class PackageViewModel {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [UIHint("Categories")]
        public int[] Categories { get; set; }

        public string Description { get; set; }
        public string Tags { get; set; }
        public decimal Price { get; set; }
        public ExtensionType ExtensionType { get; set; }
        public string LogoUrl { get; set; }
        public string Version { get; set; }
        public string SupportedOrchardVersions { get; set; }
    }
}