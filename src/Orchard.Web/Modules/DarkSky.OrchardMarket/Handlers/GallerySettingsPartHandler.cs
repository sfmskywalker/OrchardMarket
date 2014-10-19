using DarkSky.OrchardMarket.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;

namespace DarkSky.OrchardMarket.Handlers {
    public class GallerySettingsPartHandler : ContentHandler {
        
        public GallerySettingsPartHandler(IRepository<GallerySettingsPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
            Filters.Add(new ActivatingFilter<GallerySettingsPart>("Site"));
            Filters.Add(new TemplateFilterForRecord<GallerySettingsPartRecord>("GallerySettings", "Parts/GallerySettings", "Market Place"));
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