using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Migrations {
    [OrchardFeature("DarkSky.Commerce.Shops")]
    public class ShopMigrations : DataMigrationImpl {
         public int Create() {
             ContentDefinitionManager.AlterPartDefinition("ShopPart", part => part.Attachable(false));
             ContentDefinitionManager.AlterTypeDefinition("Shop", type => type
                 .WithPart("CommonPart")
                 .WithPart("TitlePart")
                 .WithPart("ShopPart")
                 .Creatable());

             return 1;
         }
    }
}