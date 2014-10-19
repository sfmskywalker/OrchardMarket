using DarkSky.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;

namespace DarkSky.Commerce.Handlers {
	public class CommerceSettingsPartHandler : ContentHandler {
		public CommerceSettingsPartHandler() {
			T = NullLocalizer.Instance;
			Filters.Add(new ActivatingFilter<CommerceSettingsPart>("Site"));
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