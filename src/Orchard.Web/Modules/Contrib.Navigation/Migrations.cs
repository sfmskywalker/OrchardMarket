using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Contrib.Navigation {
    public class MenuMigrations : DataMigrationImpl {

        public int Create() {

            // Create the ActionLinkPartRecord table
            SchemaBuilder.CreateTable("ActionLinkPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("ActionName", c => c.WithLength(256))
                .Column<string>("ControllerName", c => c.WithLength(256))
                .Column<string>("AreaName", c => c.WithLength(256))
                .Column<string>("RouteValues", c => c.WithLength(256)));

            // Define the ActionLinkPart
            ContentDefinitionManager.AlterPartDefinition("ActionLinkPart", part => part
                .Attachable(false));

            // Define the ActionLink content type and set it up to turn it into a menu item type
            ContentDefinitionManager.AlterTypeDefinition("ActionLink", type => type
                .WithPart("ActionLinkPart")     // Our custom part that will hold the Action, Controller, Area and RouteValues information
                .WithPart("MenuPart")           // Required so that the Navigation system can attach our custom menu items to a menu
                .WithPart("CommonPart")         // Required, contains common informatin such as the owner and creation date of our type. Many modules depend on this part being present
                .WithPart("IdentityPart")       // To support import / export, our type needs an identity since we won;t be providing one ourselves
                .DisplayedAs("Action Link")     // Specify the name to be displayed to the admin user

                // The value of the Description setting will be shown in the Navigation section where our custom menu item type will appear
                .WithSetting("Description", "Represents a custom link with a text and an action, controller and routevalues.")

                // Required by the Navigation module
                .WithSetting("Stereotype", "MenuItem")

                // We don't want our menu items to be draftable
                .Draftable(false)

                // We don't want the user to be able to create new ActionLink items outside of the context of a menu
                .Creatable(false)
                );

            return 1;
        }
    }
}