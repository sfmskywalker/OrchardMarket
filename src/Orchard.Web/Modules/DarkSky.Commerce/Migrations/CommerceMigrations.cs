using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace DarkSky.Commerce.Migrations {
	public class CommerceMigrations : DataMigrationImpl {
		 public int Create() {
			 SchemaBuilder.CreateTable("ProductPartRecord", table => table
				 .ContentPartVersionRecord()
				 .Column("Price", DbType.Decimal, column => column.WithDefault(0))
				.Column("VatRateId", DbType.Int32));

			 ContentDefinitionManager.AlterPartDefinition("ProductPart", part => part.Attachable());

			 SchemaBuilder.CreateTable("VatRatePartRecord", table => table
				 .ContentPartRecord()
				 .Column("Rate", DbType.Single));

			 ContentDefinitionManager.AlterPartDefinition("VatRatePart", part => part.Attachable(false));
			 ContentDefinitionManager.AlterTypeDefinition("VatRate", type => type
				 .WithPart("CommonPart")
				 .WithPart("VatRatePart")
				 .WithPart("TitlePart")
				 .Draftable(false)
				 .Creatable());

			 SchemaBuilder.CreateTable("ExchangeRatePartRecord", table => table
				 .ContentPartRecord()
				 .Column("Currency", DbType.String, column => column.WithDefault("USD").WithLength(3))
				 .Column("Rate", DbType.Single));

			 ContentDefinitionManager.AlterPartDefinition("ExchangeRatePart", part => part.Attachable(false));
			 ContentDefinitionManager.AlterTypeDefinition("ExchangeRate", type => type
				 .WithPart("CommonPart")
				 .WithPart("ExchangeRatePart")
				 .Draftable(false)
				 .Creatable());

			 SchemaBuilder.CreateTable("CommerceSettingsPartRecord", table => table
				 .ContentPartRecord()
				 .Column("Currency", DbType.String, column => column
					 .WithDefault("USD")
					 .WithLength(3)));

			 return 1;
		 }
	}
}