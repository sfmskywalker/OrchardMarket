using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Models {
    [OrchardFeature("DarkSky.Commerce.PaymentProviders.PayPal")]
    public class PayPalSettingsPart : ContentPart<PayPalSettingsPartRecord> {
        
        public string UserName {
            get { return Record.UserName; }
            set { Record.UserName = value; }
        }

        public string Password {
            get { return Record.Password; }
            set { Record.Password = value; }
        }

        public string MerchantSignature {
            get { return Record.MerchantSignature; }
            set { Record.MerchantSignature = value; }
        }

        public bool Sandbox {
            get { return Record.Sandbox; }
            set { Record.Sandbox = value; }
        }
    }

    [OrchardFeature("DarkSky.Commerce.PaymentProviders.PayPal")]
    public class PayPalSettingsPartRecord : ContentPartRecord {
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }
        public virtual string MerchantSignature { get; set; }
        public virtual bool Sandbox { get; set; }
    }
}