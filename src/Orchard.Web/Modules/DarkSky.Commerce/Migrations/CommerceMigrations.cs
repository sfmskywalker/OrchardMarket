using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace DarkSky.Commerce.Migrations {
    public class CommerceMigrations : DataMigrationImpl {
         public int Create() {
             SchemaBuilder.CreateTable("ProductPartRecord", table => table
                 .ContentPartVersionRecord()
                 .Column<decimal>("Price"));

             ContentDefinitionManager.AlterPartDefinition("ProductPart", part => part.Attachable());

             SchemaBuilder.CreateTable("ExchangeRate", table => table
                 .Column<int>("Id", c => c.PrimaryKey().Identity())
                 .Column<string>("Currency", c => c.WithLength(3))
                 .Column<float>("Rate"));

             return 1;
         }
    }
}