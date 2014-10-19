using DarkSky.Commerce.Services.PayPal;

namespace DarkSky.Commerce.Tests.PayPal {
    public class SoapPayPalSettingsAccessor : IPayPalSettingsAccessor {
        public string UserName { get { return "sipkes_1352951579_biz_api1.hotmail.com"; } }
        public string Password { get { return "1352951599"; } }
        public string MerchantSignature { get { return "AqgUb26h66oETsqy9jRK8-5S8sM7AZwO.CWf.G3G0igjyfrrg1hX7z.r"; } }
        public bool Sandbox { get { return true; } }
    }
}