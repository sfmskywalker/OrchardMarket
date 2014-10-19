using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace DarkSky.OrchardMarket.Migrations {
    public class SiteWidgetsMigrations : DataMigrationImpl {
        public int Create() {
            ContentDefinitionManager.AlterTypeDefinition("HomeBeforeContent", type => type
                .WithPart("CommonPart")
                .WithPart("WidgetPart")
                .Creatable(false)
                .Draftable(false)
                .WithSetting("Stereotype", "Widget"));

            ContentDefinitionManager.AlterTypeDefinition("ExtensionsList", type => type
                .WithPart("CommonPart")
                .WithPart("WidgetPart")
                .WithPart("ProjectionPart")
                .Creatable(false)
                .Draftable(false)
                .WithSetting("Stereotype", "Widget"));

            return 1;
        }
    }
}