using System;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Migrations {
    [OrchardFeature("DarkSky.Commerce.Checkout")]
    public class CheckoutMigrations : DataMigrationImpl {
         public int Create() {

             SchemaBuilder.CreateTable("Transaction", table => table
                 .Column<int>("Id", c => c.Identity().PrimaryKey())
                 .Column<int>("UserId", c => c.NotNull())
                 .Column<int>("CartId", c => c.NotNull())
                 .Column<decimal>("OrderTotal", c => c.NotNull())
                 .Column<string>("Status", c => c.WithLength(50))
                 .Column<string>("ProviderToken", c => c.WithLength(1024))
                 .Column<string>("PaymentMethodName", c => c.WithLength(50))
                 .Column<DateTime>("CreatedUtc")
                 .Column<DateTime>("CompletedUtc"));

             SchemaBuilder.CreateTable("Payment", table => table
                 .Column<int>("Id", c => c.Identity().PrimaryKey())
                 .Column<string>("Token", c => c.WithLength(50))
                 .Column<int>("Transaction_Id")
                 .Column<DateTime>("CreatedUtc"));

             SchemaBuilder.CreateForeignKey("FK_Payment_Transaction", "Payment", new[] { "Transaction_Id" }, "Transaction", new[] { "Id" });

             return 1;
         }
    }
}