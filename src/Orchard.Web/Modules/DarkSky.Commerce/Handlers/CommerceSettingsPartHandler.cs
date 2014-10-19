using DarkSky.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;

namespace DarkSky.Commerce.Handlers {
	public class CommerceSettingsPartHandler : ContentHandler {
		public CommerceSettingsPartHandler(IRepository<CommerceSettingsPartRecord> repository) {
			T = NullLocalizer.Instance;
			Filters.Add(new ActivatingFilter<CommerceSettingsPart>("Site"));
			Filters.Add(StorageFilter.For(repository));
			Filters.Add(new TemplateFilterForRecord<CommerceSettingsPartRecord>("CommerceSettings", "Parts/CommerceSettings", "commerce"));
		}

		public Localizer T { get; set; }

		protected override void GetItemMetadata(GetContentItemMetadataContext context) {
			if (context.ContentItem.ContentType != "Site")
				return;

			base.GetItemMetadata(context);
			context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Commerce")));
		}
	}
}