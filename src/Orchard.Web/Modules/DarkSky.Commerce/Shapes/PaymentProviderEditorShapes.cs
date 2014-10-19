using System.Collections.Generic;
using System.Linq;
using DarkSky.Commerce.PaymentProviders;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Shapes {
    [OrchardFeature("DarkSky.Commerce.PaymentProviders")]
    public class PaymentProviderEditorShapes : IShapeTableProvider {
        private readonly IEnumerable<IPaymentProvider> _paymentProviders;

        public PaymentProviderEditorShapes(IEnumerable<IPaymentProvider> paymentProviders) {
            _paymentProviders = paymentProviders;
        }

        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Editor_PaymentProviderPicker").OnDisplaying(displaying => {
                displaying.Shape.PaymentProviders = _paymentProviders.ToList();
            });
        }
    }
}