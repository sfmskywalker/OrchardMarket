using System.ServiceModel;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using DarkSky.Commerce.Models;
using DarkSky.Commerce.Services.PayPal.Soap;
using Orchard;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Services;

namespace DarkSky.Commerce.Services.PayPal {
    public interface IPayPalClient : IDependency {
        string ApiUrl { get; }
        string SiteUrl { get; }
        SetExpressCheckoutResponseType SetExpressCheckout(SetExpressCheckoutRequestDetailsType args);
        string GetExpressCheckoutUrl(string token, PayPalUserAction userAction);
    }

    public enum PayPalUserAction {
        Continue,
        Commit
    }

    [OrchardFeature("DarkSky.Commerce.PaymentProviders.PayPal")]
    public class PayPalClient : IPayPalClient {

        private const string ApiVersion = "95";
        private readonly IPayPalSettingsAccessor _settings;
        private PayPalAPIAAInterface _api;
        private readonly IRepository<PayPalLogEntry> _logRepository;
        private readonly IClock _clock;

        private PayPalAPIAAInterface Api {
            get {
                return _api ?? (_api = new PayPalAPIAAInterfaceClient(new BasicHttpBinding {
                    Name = "PayPalAPISoapBinding",
                    Security = new BasicHttpSecurity { Mode = BasicHttpSecurityMode.Transport},
                }, new EndpointAddress(ApiUrl)));
            }
        }

        public string ApiUrl {
            get { return string.Format("https://api-3t.{0}paypal.com/2.0/", _settings.Sandbox ? "sandbox." : ""); }
        }

        public string SiteUrl {
            get { return string.Format("https://www.{0}paypal.com/cgi-bin/webscr", _settings.Sandbox ? "sandbox." : ""); }
        }

        private CustomSecurityHeaderType Credentials {
            get {
                return new CustomSecurityHeaderType{
                    Credentials = new UserIdPasswordType {
                        Username = _settings.UserName,
                        Password = _settings.Password,
                        Signature = _settings.MerchantSignature
                    }
                };
            }
        }

        public PayPalClient(IPayPalSettingsAccessor settings, IRepository<PayPalLogEntry> logRepository, IClock clock) {
            _settings = settings;
            _logRepository = logRepository;
            _clock = clock;
        }

        public SetExpressCheckoutResponseType SetExpressCheckout(SetExpressCheckoutRequestDetailsType args) {
            var response = new TaskFactory<SetExpressCheckoutResponse>()
                .FromAsync(
                    Api.BeginSetExpressCheckout, 
                    Api.EndSetExpressCheckout, new SetExpressCheckoutRequest {
                        RequesterCredentials = Credentials,
                        SetExpressCheckoutReq = new SetExpressCheckoutReq { 
                            SetExpressCheckoutRequest = new SetExpressCheckoutRequestType {
                                Version = ApiVersion,
                                DetailLevel = new[]{ DetailLevelCodeType.ReturnAll },
                                SetExpressCheckoutRequestDetails = args
                            }
                        }
                    }, null).Result;

            LogResponse(response.SetExpressCheckoutResponse1);
            return response.SetExpressCheckoutResponse1;
        }

        private void LogResponse(AbstractResponseType response) {
            _logRepository.Create(new PayPalLogEntry {
                TimeStamp = response.Timestamp,
                CorrelationId = response.CorrelationID,
                Ack = response.Ack,
                Build = response.Build,
                Version = response.Version,
                CreatedUtc = _clock.UtcNow,
                Errors = new JavaScriptSerializer().Serialize(response.Errors)
            });
        }

        public string GetExpressCheckoutUrl(string token, PayPalUserAction userAction = PayPalUserAction.Continue) {
            return string.Format("{0}?cmd=_express-checkout&token={1}&useraction={2}", SiteUrl, token, userAction.ToString().ToLower());
        }
    }
}