using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using DarkSky.OrchardMarket.Models;

namespace DarkSky.OrchardMarket.Drivers {
    [UsedImplicitly]
    public class UserProfilePartDriver : ContentPartDriver<UserProfilePart> {
	    private const string TemplateName = "Parts/UserProfile";

        protected override string Prefix {
            get { return "UserProfile"; }
        }

        protected override DriverResult Editor(UserProfilePart part, dynamic shapeHelper) {
	        return ContentShape("Parts_UserProfile_Edit", () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(UserProfilePart part, IUpdateModel updater, dynamic shapeHelper) {
	        updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

    }
}