using DarkSky.OrchardMarket.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;

namespace DarkSky.OrchardMarket.Handlers {
    public class MarketSettingsPartHandler : ContentHandler {

        public MarketSettingsPartHandler(IRepository<MarketSettingsPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
            Filters.Add(new ActivatingFilter<MarketSettingsPart>("Site"));
            Filters.Add(new TemplateFilterForRecord<MarketSettingsPartRecord>("MarketSettings", "Parts/MarketSettings", "Market Place"));
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            if (context.ContentItem.ContentType != "Site")
                return;
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Market Place")));
        }
    }
}