using System.Linq;
using DarkSky.Commerce.Services;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Environment;

namespace DarkSky.Commerce.Shapes {
    public class VatEditorShapes : IShapeTableProvider {
        private readonly Work<IProductManager> _productManager;
        public VatEditorShapes(Work<IProductManager> productManager) {
            _productManager = productManager;
        }

        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Editor_VatPicker").OnDisplaying(displaying => {
                displaying.Shape.VatItems = _productManager.Value.GetVatItems().Where(x => x.IsActive).ToList();
            });
        }
    }
}