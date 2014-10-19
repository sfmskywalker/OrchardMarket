using System.Data;
using DarkSky.OrchardMarket.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data;
using Orchard.Data.Migration;
using Orchard.Localization;
using Orchard.Projections.Models;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;

namespace DarkSky.OrchardMarket.Migrations {
    public class GalleryMigrations : DataMigrationImpl {
        private readonly IContentManager _contentManager;
        private readonly ITaxonomyService _taxonomyService;
        private readonly IRepository<MemberBindingRecord> _memberBindingRepository;

        public GalleryMigrations(IContentManager contentManager, ITaxonomyService taxonomyService, IRepository<MemberBindingRecord> memberBindingRepository) {
            _contentManager = contentManager;
            _taxonomyService = taxonomyService;
            _memberBindingRepository = memberBindingRepository;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public int Create() {

            CreateTaxonomy("Tags");
            CreateTaxonomy("Categories");

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

            SchemaBuilder.CreateTable("PackagePartRecord", table => table
                .ContentPartRecord()
                .Column("ExtensionType", DbType.String, column => column.WithLength(20))
                .Column("Downloads", DbType.Int32, column => column.NotNull().WithDefault(0))
                .Column("SearchIndex", DbType.String, column => column.Unlimited()));

            ContentDefinitionManager.AlterPartDefinition("PackagePart", part => part
                .WithField("Licenses", field => field
                    .OfType("ContentPickerField")
                    .WithSetting("ContentPickerFieldSettings.Required", "false")
                    .WithSetting("ContentPickerFieldSettings.Multiple", "true"))
                .WithField("Tags", field => field
                    .OfType("TaxonomyField")
                    .WithSetting("TaxonomyFieldSettings.AllowCustomTerms", "true")
                    .WithSetting("TaxonomyFieldSettings.Taxonomy", "Tags"))
                .WithField("Categories", field => field
                    .OfType("TaxonomyField")
                    .WithSetting("TaxonomyFieldSettings.AllowCustomTerms", "true")
                    .WithSetting("TaxonomyFieldSettings.Taxonomy", "Categories"))
                .Attachable(false));

            ContentDefinitionManager.AlterTypeDefinition("Package", type => type
                .WithPart("CommonPart")
                .WithPart("TitlePart")
                .WithPart("AutoroutePart", p => p
                    .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                    .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                    .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Title', Pattern: 'Catalog/{Package.ExtensionType}s/{Content.Slug}', Description: 'Modules/my-module'}]")
                    .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                .WithPart("BodyPart")
                .WithPart("PackagePart")
                .WithPart("ProductPart")
                .WithPart("ReviewsPart")
                .Draftable()
                .Creatable());

            SchemaBuilder.CreateTable("PackageReleasePartRecord", table => table
                .ContentPartRecord()
                .Column("PackageId", DbType.Int32)
                .Column("Downloads", DbType.Int32, column => column.NotNull().WithDefault(0))
                .Column("Sales", DbType.Int32, column => column.NotNull().WithDefault(0))
                .Column("Version", DbType.String, column => column.WithLength(20))
                .Column("SupportedOrchardVersionsRange", DbType.String, column => column.WithLength(50))
                .Column("ReleaseNotes", DbType.String, column => column.Unlimited())
                .Column("FileName", DbType.String, column => column.WithLength(256)));

            ContentDefinitionManager.AlterPartDefinition("PackageReleasePart", part => part
                .Attachable(false));

            ContentDefinitionManager.AlterTypeDefinition("PackageRelease", type => type
                .WithPart("CommonPart")
                .WithPart("PackageReleasePart")
                .Draftable()
                .Creatable(false));

            ContentDefinitionManager.AlterPartDefinition("PackageCatalogPart", part => part.Attachable());
            ContentDefinitionManager.AlterTypeDefinition("PackageCatalog", type => type
                .WithPart("CommonPart")
                .WithPart("TitlePart")
                .WithPart("AutoroutePart")
                .WithPart("NavigationPart")
                .WithPart("PackageCatalogPart")
                .Draftable(false)
                .Creatable()
                .DisplayedAs("Packages Catalog"));

            ContentDefinitionManager.AlterTypeDefinition("PackagesList", type => type
                .WithPart("CommonPart")
                .WithPart("WidgetPart")
                .WithPart("ProjectionPart")
                .Creatable(false)
                .Draftable(false)
                .WithSetting("Stereotype", "Widget"));

            SchemaBuilder.CreateTable("GallerySettingsPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("PackagesPath", c => c.WithLength(256))
                .Column<int>("ModulesQueryId", c => c.Nullable())
                .Column<int>("ThemesQueryId", c => c.Nullable()));

            ContentDefinitionManager.AlterPartDefinition("GallerySettingsPart", part => part.Attachable(false));

            // Default Model Bindings - PackagePartRecord
            _memberBindingRepository.Create(new MemberBindingRecord {
                Type = typeof(PackagePartRecord).FullName,
                Member = "ExtensionType",
                DisplayName = T("Extension type").Text,
                Description = T("The extension type of the package. Can be Module or Theme").Text
            });

            _memberBindingRepository.Create(new MemberBindingRecord {
                Type = typeof(PackagePartRecord).FullName,
                Member = "Downloads",
                DisplayName = T("Downloads").Text,
                Description = T("The number of downloads of the package.").Text
            });

            return 1;
        }

        private void CreateTaxonomy(string name) {
            var taxonomyPart = _contentManager.Create<TaxonomyPart>("Taxonomy", part => part.Name = name);
            _taxonomyService.CreateTermContentType(taxonomyPart);
        }
    }
}