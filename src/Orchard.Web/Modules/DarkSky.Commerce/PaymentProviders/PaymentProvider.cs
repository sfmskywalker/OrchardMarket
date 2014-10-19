using System;
using System.Web.Mvc;
using DarkSky.Commerce.Models;
using DarkSky.Commerce.Services;
using Orchard;
using Orchard.Security;

namespace DarkSky.Commerce.PaymentProviders {
    public interface IPaymentProvider : IDependency {
        string Name { get; }
        string DisplayName { get; }
        dynamic BuildCheckoutButton(dynamic shapeHelper);
        ActionResult InitiateCheckout(InitiateCheckoutArgs args);
    }

    public abstract class PaymentProvider : IPaymentProvider {
        public virtual string Name {
            get { return GetType().FullName; }
        }

        public abstract string DisplayName { get; }
        public abstract dynamic BuildCheckoutButton(dynamic shapeHelper);
        public abstract ActionResult InitiateCheckout(InitiateCheckoutArgs args);

        public override string ToString() {
            return DisplayName;
        }
    }

    public class InitiateCheckoutArgs {
        public ControllerContext ControllerContext { get; set; }
        public UrlHelper UrlHelper { get; set; }
        public Transaction Transaction { get; set; }
        public IUser User { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
        public IShoppingCart Cart { get; set; }
    }
}