using System;
using System.Collections.Generic;
using Autofac;
using DarkSky.Commerce.Models;
using DarkSky.Commerce.Services.PayPal;
using DarkSky.Commerce.Services.PayPal.Soap;
using NUnit.Framework;
using Orchard.Services;
using Orchard.Tests;
using Orchard.Tests.Stubs;

namespace DarkSky.Commerce.Tests.PayPal {
    [TestFixture]
    public class PayPalClientTests : DatabaseEnabledTestsBase {
        private IPayPalClient _payPalClient;

        public override void Register(ContainerBuilder builder) {
            builder.RegisterType<SoapPayPalSettingsAccessor>().As<IPayPalSettingsAccessor>();
            builder.RegisterType<PayPalClient>().As<IPayPalClient>();
            builder.RegisterInstance<IClock>(_clock = new StubClock());
        }

        public override void Init() {
            base.Init();
            _payPalClient = _container.Resolve<IPayPalClient>();
        }

        [Test]
        public void TestSetExpressCheckout() {
            var response = _payPalClient.SetExpressCheckout(new SetExpressCheckoutRequestDetailsType {
                Address = new AddressType {
                    Street1 = "Nachtwachtlaan 374",
                    CityName = "Amsterdam",
                    Country = CountryCodeType.NL,
                    CountryName = "Netherlands",
                    PostalCode = "1058 EN"
                },
                OrderTotal = new BasicAmountType { currencyID = CurrencyCodeType.USD, Value = "25.95" },
                CancelURL = "http://acme.com/cancel",
                ReturnURL = "http://acme.com/return"
            });

            Assert.NotNull(response.Token);
        }

        protected override IEnumerable<Type> DatabaseTypes {
            get {
                yield return typeof(PayPalLogEntry);
            }
        }
    }
}
