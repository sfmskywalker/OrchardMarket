using System;
using System.Web.Mvc;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.PaymentProviders {
    [OrchardFeature("DarkSky.Commerce.PaymentProviders.GoogleCheckout")]
    public class GoogleCheckout : PaymentProvider {
        public override string DisplayName { get { return "Google Checkout"; } }
        public override dynamic BuildCheckoutButton(dynamic shapeHelper) {
            return shapeHelper.CheckoutButton_Google();
        }

        public override ActionResult InitiateCheckout(InitiateCheckoutArgs args) {
            throw new System.NotImplementedException();
        }
    }
}