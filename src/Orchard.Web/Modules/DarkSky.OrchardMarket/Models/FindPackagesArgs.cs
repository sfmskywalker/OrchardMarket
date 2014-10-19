namespace DarkSky.OrchardMarket.Models {
    public class FindPackagesArgs {
        public ExtensionType ExtensionType { get; set; }
        public string Category { get; set; }
        public string Tag { get; set; }
        public string Keyword { get; set; }

        public static readonly FindPackagesArgs Modules = new FindPackagesArgs {ExtensionType = ExtensionType.Module};
        public static readonly FindPackagesArgs Themes = new FindPackagesArgs { ExtensionType = ExtensionType.Theme};
    }
}