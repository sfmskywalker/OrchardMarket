using System;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Migrations {
    [OrchardFeature("DarkSky.Commerce.Orders")]
    public class OrdersMigrations : DataMigrationImpl {
         public int Create() {

             SchemaBuilder.CreateTable("OrderPartRecord", table => table
                 .ContentPartRecord()
                 .Column<string>("Status", c => c.WithLength(25))
                 .Column<DateTime>("CompletedUtc")
                 .Column<int>("Transaction_Id"));

             SchemaBuilder.CreateTable("OrderDetail", table => table
                 .Column<int>("Id", c => c.PrimaryKey().Identity())
                 .Column<int>("OrderId")
                 .Column<int>("ProductId")
                 .Column<int>("Quantity")
                 .Column<decimal>("UnitPrice")
                 .Column<float>("VatRate"));

             SchemaBuilder.CreateTable("InvoicePartRecord", table => table
                 .ContentPartRecord()
                 .Column<int>("OrderId")
                 .Column<int>("Payment_Id")
                 .Column<string>("Status", c => c.WithLength(25))
                 .Column<DateTime>("CompletedUtc")
                 .Column<int>("Transaction_Id"));

             SchemaBuilder.CreateTable("InvoiceDetail", table => table
                 .Column<int>("Id", c => c.PrimaryKey().Identity())
                 .Column<int>("InvoiceId")
                 .Column<int>("ProductId")
                 .Column<string>("Description", c => c.Unlimited())
                 .Column<int>("Quantity")
                 .Column<decimal>("UnitPrice")
                 .Column<float>("VatRate"));

             ContentDefinitionManager.AlterPartDefinition("OrderPart", part => part.Attachable(false));
             ContentDefinitionManager.AlterPartDefinition("InvoicePart", part => part.Attachable(false));

             ContentDefinitionManager.AlterTypeDefinition("Order", type => type
                 .WithPart("CommonPart")
                 .WithPart("AutoroutePart", part => part
                    .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                    .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                    .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Order ID', Pattern: 'orders/{Content.Id}', Description: 'orders/1024'}]")
                    .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                .WithPart("OrderPart"));

             ContentDefinitionManager.AlterTypeDefinition("Invoice", type => type
                 .WithPart("CommonPart")
                 .WithPart("AutoroutePart", part => part
                    .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                    .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                    .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Invoice ID', Pattern: 'invoices/{Content.Id}', Description: 'invoices/1024'}]")
                    .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                .WithPart("InvoicePart"));
             return 1;
         }
    }
}