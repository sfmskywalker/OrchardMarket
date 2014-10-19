using System.Web.Mvc;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.PaymentProviders {
    [OrchardFeature("DarkSky.Commerce.PaymentProviders.CreditCard")]
    public class CreditCardProvider : PaymentProvider {
        public override string DisplayName { get { return "Credit Card"; } }
        public override dynamic BuildCheckoutButton(dynamic shapeHelper) {
            return shapeHelper.CheckoutButton_CreditCard();
        }

        public override ActionResult InitiateCheckout(InitiateCheckoutArgs args) {
            throw new System.NotImplementedException();
        }
    }
}