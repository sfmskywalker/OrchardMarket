using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Migrations {
    [OrchardFeature("DarkSky.Commerce.Products")]
    public class ProductsMigrations : DataMigrationImpl {
         public int Create() {

             SchemaBuilder.CreateTable("VatPartRecord", table => table
                 .ContentPartRecord()
                 .Column<float>("Rate", c => c.NotNull())
                 .Column<bool>("IsActive", c => c.NotNull())
                 .Column<string>("Description", c => c.WithLength(1042)));

             SchemaBuilder.CreateTable("ProductPartRecord", table => table
                 .ContentPartRecord()
                 .Column<decimal>("UnitPrice", c => c.NotNull())
                 .Column<string>("Currency", c => c.WithLength(3).WithType(DbType.StringFixedLength))
                 .Column<int>("VatId")
                 .Column<int>("Sales"));


             ContentDefinitionManager.AlterPartDefinition("VatPart", part => part.Attachable(false));

             ContentDefinitionManager.AlterTypeDefinition("Vat", type => type
                 .WithPart("CommonPart")
                 .WithPart("TitlePart")
                 .WithPart("VatPart")
                 .Draftable(false)
                 .Creatable());

             ContentDefinitionManager.AlterPartDefinition("ProductPart", part =>  part
                 .WithField("PrimaryImage", f => f
                     .OfType("MediaPickerField")
                     .WithSetting("Hint", "The primary image of this product")
                     .WithSetting("AllowedExtensions", "png jpg gif")
                     .WithSetting("Required", "false")
                     .WithDisplayName("Primary Image"))
                 .Attachable());

             SchemaBuilder.CreateTable("PurchasePartRecord", table => table
                 .ContentPartRecord()
                 .Column<int>("InvoiceId")
                 .Column<int>("ProductId")
                 .Column<int>("Quantity", c => c.NotNull()));

             ContentDefinitionManager.AlterPartDefinition("PurchasePart", part => part.Attachable(false));
             ContentDefinitionManager.AlterTypeDefinition("Purchase", type => type
                 .WithPart("CommonPart")
                 .WithPart("PurchasePart"));

             return 1;
         }
    }
}