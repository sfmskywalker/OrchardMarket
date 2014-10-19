using System;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace DarkSky.OrchardMarket.Migrations {
    public class MarketMigrations : DataMigrationImpl {
        
        public int Create() {

            SchemaBuilder.CreateTable("MarketSettingsPartRecord", table => table
                .ContentPartRecord()
                .Column<float>("PayoutPercentage"));

            SchemaBuilder.CreateTable("FundedOrderPartRecord", table => table
                .ContentPartRecord()
                .Column<bool>("Funded", c => c.NotNull())
                .Column<DateTime>("FundedUtc", c => c.Nullable()));

            SchemaBuilder.CreateTable("PayoutOption", table => table
                .Column<int>("Id", c => c.Identity().PrimaryKey())
                .Column<int>("OrganizationId")
                .Column<string>("PayoutMethodName", c => c.WithLength(256))
                .Column<string>("Data", c => c.Unlimited())
                .Column<bool>("IsActive", c => c.NotNull()));

            SchemaBuilder.CreateTable("Revenue", table => table
                .Column<int>("Id", c => c.Identity().PrimaryKey())
                .Column<int>("OrganizationId")
                .Column<int>("InvoiceDetailId")
                .Column<float>("RevenuePercentage")
                .Column<decimal>("SalesTotal")
                .Column<decimal>("RevenueTotal")
                .Column<bool>("Paid")
                .Column<int>("PayoutOptionId", c => c.Nullable())
                .Column<DateTime>("CreatedUtc")
                .Column<DateTime>("PaidUtc", c => c.Nullable()));

            ContentDefinitionManager.AlterPartDefinition("FundedOrderPart", part => part.Attachable(false));
            ContentDefinitionManager.AlterTypeDefinition("Order", type => type
                .WithPart("FundedOrderPart"));

            ContentDefinitionManager.AlterPartDefinition("MarketSettingsPart", part => part.Attachable(false));

			return 1;
		}
    }
}