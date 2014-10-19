using Orchard.UI.Resources;

namespace DarkSky.OrchardMarket {
	public class Resources : IResourceManifestProvider {
		public void BuildManifests(ResourceManifestBuilder builder) {
			var manifest = builder.Add();

			manifest.DefineScript("OrchardMarket.Common").SetUrl("Common.min.js", "Common.js").SetDependencies("jQuery");
		}
	}
}