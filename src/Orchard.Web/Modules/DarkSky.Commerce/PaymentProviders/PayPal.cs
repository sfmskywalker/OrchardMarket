using System;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;
using DarkSky.Commerce.Helpers;
using DarkSky.Commerce.Services.PayPal;
using DarkSky.Commerce.Services.PayPal.Soap;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.PaymentProviders {
    [OrchardFeature("DarkSky.Commerce.PaymentProviders.PayPal")]
    public class PayPal : PaymentProvider {
        private readonly IPayPalClient _payPalClient;
        public PayPal(IPayPalClient payPalClient) {
            _payPalClient = payPalClient;
        }

        public override string DisplayName { get { return "PayPal"; } }
        public override dynamic BuildCheckoutButton(dynamic shapeHelper) {
            return shapeHelper.CheckoutButton_PayPal();
        }

        public override ActionResult InitiateCheckout(InitiateCheckoutArgs args) {
            var customer = args.CustomerInfo;
            var address = customer.Address;
            var uri = args.ControllerContext.HttpContext.Request.Url;
            var response = _payPalClient.SetExpressCheckout(new SetExpressCheckoutRequestDetailsType {
                OrderTotal = new BasicAmountType {
                    currencyID = CurrencyCodeType.USD,
                    Value = args.Cart.Totals().Total.ToString("n2", CultureInfo.InvariantCulture)
                },
                Address = new AddressType {
                    Street1 = address.AddressLine1,
                    Street2 = address.AddressLine2,
                    CityName = address.City,
                    PostalCode = address.PostalCode,
                    StateOrProvince = address.StateOrProvince,
                    Country = address.Country.Code.ParseEnum<CountryCodeType>(),
                    CountryName = address.Country.Name,
                    CountrySpecified = !string.IsNullOrWhiteSpace(address.Country.Code),
                    Name = customer.FullName
                },
                ReturnURL = args.UrlHelper.Action("Complete", "PayPal", new RouteValueDictionary(new { area = "DarkSky.Commerce" }), uri.Scheme, uri.Host),
                CancelURL = args.UrlHelper.Action("Cancel", "PayPal", new RouteValueDictionary(new { area = "DarkSky.Commerce" }), uri.Scheme, uri.Host)
            });

            args.Transaction.ProviderToken = response.Token;

            return new RedirectResult(_payPalClient.GetExpressCheckoutUrl(response.Token, PayPalUserAction.Commit), true);
        }
    }
}