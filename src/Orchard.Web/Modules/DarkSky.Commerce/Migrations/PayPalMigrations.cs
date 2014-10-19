using System;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Migrations {
    [OrchardFeature("DarkSky.Commerce.PaymentProviders.PayPal")]
    public class PayPalMigrations : DataMigrationImpl {
         public int Create() {

             SchemaBuilder.CreateTable("PayPalSettingsPartRecord", table => table
                 .ContentPartRecord()
                 .Column<string>("UserName", c => c.WithLength(50))
                 .Column<string>("Password", c => c.WithLength(256))
                 .Column<string>("MerchantSignature", c => c.WithLength(4096))
                 .Column<bool>("Sandbox", c => c.NotNull()));

             SchemaBuilder.CreateTable("PayPalLogEntry", table => table
                 .Column<int>("Id", c => c.Identity().PrimaryKey())
                 .Column<DateTime>("TimeStamp")
                 .Column<string>("Ack", c => c.WithLength(50))
                 .Column<string>("CorrelationId", c => c.WithLength(50))
                 .Column<string>("Version", c => c.WithLength(15))
                 .Column<string>("Build", c => c.WithLength(15))
                 .Column<string>("Errors", c => c.Unlimited())
                 .Column<DateTime>("CreatedUtc"));

             return 1;
         }
    }
}