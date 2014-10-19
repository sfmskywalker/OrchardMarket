using DarkSky.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace DarkSky.Commerce.Handlers {
    [OrchardFeature("DarkSky.Commerce.PaymentProviders.PayPal")]
    public class PayPalSettingsPartHandler : ContentHandler {
        public PayPalSettingsPartHandler(IRepository<PayPalSettingsPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
            Filters.Add(new ActivatingFilter<PayPalSettingsPart>("Site"));
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("PayPal")));
        }
    }
}