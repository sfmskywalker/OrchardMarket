using System;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Migrations {
    [OrchardFeature("DarkSky.Commerce.ShoppingCart")]
    public class ShoppingCartMigrations : DataMigrationImpl {
         public int Create() {

             SchemaBuilder.CreateTable("ShoppingCartRecord", table => table
                 .Column<int>("Id", c => c.Identity().PrimaryKey())
                 .Column<string>("Guid")
                 .Column<bool>("IsClosed")
                 .Column<DateTime>("ClosedUtc")
                 .Column<DateTime>("CreatedUtc"));

             SchemaBuilder.CreateTable("ShoppingCartItem", table => table
                 .Column<int>("Id", c => c.Identity().PrimaryKey())
                 .Column<int>("ShoppingCartRecord_Id", c => c.NotNull())
                 .Column<int>("ProductId")
                 .Column<int>("Quantity"));

             SchemaBuilder.CreateForeignKey("FK_ShoppingCartItem_ShoppingCart", "ShoppingCartItem", new[] { "ShoppingCartRecord_Id" }, "ShoppingCartRecord", new[] { "Id" });

             return 1;
         }
    }
}