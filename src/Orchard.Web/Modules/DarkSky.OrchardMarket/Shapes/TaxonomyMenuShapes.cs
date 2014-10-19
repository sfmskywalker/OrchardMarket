using Orchard.DisplayManagement.Descriptors;

namespace DarkSky.OrchardMarket.Shapes {
    public class TaxonomyMenuShapes : IShapeTableProvider {
        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Parts_Taxonomies_Menu").OnDisplaying(displaying => {
                var taxonomyName = (string)displaying.Shape.Taxonomy.Name;
                displaying.ShapeMetadata.Alternates.Add(string.Format("Parts_Taxonomy_Menu__{0}", taxonomyName));
            });
        }
    }
}