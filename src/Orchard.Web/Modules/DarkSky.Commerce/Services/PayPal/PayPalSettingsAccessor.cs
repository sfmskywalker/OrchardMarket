using System;
using DarkSky.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Settings;

namespace DarkSky.Commerce.Services.PayPal {
    public interface IPayPalSettingsAccessor : IDependency {
        string UserName { get; }
        string Password { get; }
        string MerchantSignature { get; }
        bool Sandbox { get; }
    }

    [OrchardFeature("DarkSky.Commerce.PaymentProviders.PayPal")]
    public class PayPalSettingsAccessor : IPayPalSettingsAccessor {
        private readonly IWorkContextAccessor _wca;
        private readonly Lazy<ISite> _currentSite;
        private readonly Lazy<PayPalSettingsPart> _payPalSettingsPart;

        public PayPalSettingsAccessor(IWorkContextAccessor wca) {
            _wca = wca;
            _currentSite = new Lazy<ISite>(() => _wca.GetContext().CurrentSite);
            _payPalSettingsPart = new Lazy<PayPalSettingsPart>(() => _currentSite.Value.As<PayPalSettingsPart>());
        }

        public PayPalSettingsPart Settings {
            get { return _payPalSettingsPart.Value; }
        }

        public string UserName {
            get { return Settings.UserName; }
        }
        
        public string Password {
            get { return Settings.Password; }
        }

        public string MerchantSignature {
            get { return Settings.MerchantSignature; }
        }

        public bool Sandbox {
            get { return Settings.Sandbox; }
        }
    }
}