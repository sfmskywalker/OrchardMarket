using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace DarkSky.OrchardMarket.Migrations {
	public class MarketMigrations : DataMigrationImpl {

		public int Create() {

			ContentDefinitionManager.AlterPartDefinition("PublisherPart", part => part
				.WithField("Logo", field => field
					.OfType("MediaPickerField")
					.WithSetting("MediaPickerFieldSettings.AllowedExtensions", "png jpg gif"))
				.Attachable(false));

			ContentDefinitionManager.AlterTypeDefinition("Publisher", type => type
				.WithPart("CommonPart")
				.WithPart("TitlePart")
				.WithPart("AutoroutePart")
				.WithPart("PublisherPart")
				.Draftable(false)
				.Creatable());

			ContentDefinitionManager.AlterPartDefinition("LicensePart", part => part
				.WithField("StartDate", field => field
					.OfType("DateTimeField")
					.WithSetting("DateTimeFieldSettings.Display", "DateOnly"))
				.Attachable(false));

			ContentDefinitionManager.AlterTypeDefinition("License", type => type
				.WithPart("CommonPart")
				.WithPart("TitlePart")
				.WithPart("BodyPart")
				.WithPart("LicensePart")
				.Draftable()
				.Creatable());

			SchemaBuilder.CreateTable("ExtensionPartRecord", table => table
				.ContentPartRecord()
				.Column("PublisherId", DbType.Int32, column => column.NotNull())
				.Column("Downloads", DbType.Int32, column => column
					.NotNull()
					.WithDefault(0))
				.Column("Sales", DbType.Int32, column => column
					.NotNull()
					.WithDefault(0)));

			ContentDefinitionManager.AlterPartDefinition("ExtensionPart", part => part
				.WithField("Licenses", field => field
					.OfType("ContentPickerField")
					.WithSetting("ContentPickerFieldSettings.Required", "true")
					.WithSetting("ContentPickerFieldSettings.Multiple", "true"))
				.Attachable(false));

			ContentDefinitionManager.AlterTypeDefinition("Module", type => type
				.WithPart("CommonPart")
				.WithPart("TitlePart")
				.WithPart("AutoroutePart")
				.WithPart("BodyPart")
				.WithPart("ExtensionPart")
				.WithPart("ProductPart")
				.Draftable()
				.Creatable(false));

			ContentDefinitionManager.AlterTypeDefinition("Theme", type => type
				.WithPart("CommonPart")
				.WithPart("TitlePart")
				.WithPart("AutoroutePart")
				.WithPart("BodyPart")
				.WithPart("ExtensionPart")
				.WithPart("ProductPart")
				.Draftable()
				.Creatable(false));

			SchemaBuilder.CreateTable("ExtensionReleasePartRecord", table => table
				.ContentPartRecord()
				.Column("ExtensionId", DbType.Int32)
				.Column("Downloads", DbType.Int32, column => column
					.NotNull()
					.WithDefault(0))
				.Column("Sales", DbType.Int32, column => column
					.NotNull()
					.WithDefault(0)));

			ContentDefinitionManager.AlterPartDefinition("ExtensionReleasePart", part => part
				.WithField("ReleaseNotes", field => field
					.OfType("TextField")
					.WithSetting("Flavor", "html")
					.WithSetting("Required", "true"))
				.Attachable(false));

			ContentDefinitionManager.AlterTypeDefinition("ExtensionRelease", type => type
				.WithPart("CommonPart")
				.WithPart("ExtensionReleasePart")
				.Draftable()
				.Creatable(false));

			ContentDefinitionManager.AlterPartDefinition("ExtensionsCatalogPart", part => part.Attachable());
			ContentDefinitionManager.AlterTypeDefinition("ModulesCatalog", type => type
				.WithPart("CommonPart")
				.WithPart("TitlePart")
				.WithPart("AutoroutePart")
				.WithPart("ExtensionsCatalogPart")
				.Draftable(false)
				.Creatable()
				.DisplayedAs("Modules Catalog"));

			ContentDefinitionManager.AlterTypeDefinition("ThemesCatalog", type => type
				.WithPart("CommonPart")
				.WithPart("TitlePart")
				.WithPart("AutoroutePart")
				.WithPart("ExtensionsCatalogPart")
				.Draftable(false)
				.Creatable()
				.DisplayedAs("Themes Catalog"));

			return 1;
		} 
	}
}