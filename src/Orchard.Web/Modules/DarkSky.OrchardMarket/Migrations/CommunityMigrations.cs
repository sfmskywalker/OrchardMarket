using System;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace DarkSky.OrchardMarket.Migrations {
    public class CommunityMigrations : DataMigrationImpl {

        public int Create() {

            SchemaBuilder.CreateTable("UserProfilePartRecord", table => table
                .ContentPartRecord()
                .Column<string>("FirstName", column => column.WithLength(50))
                .Column<string>("LastName", column => column.WithLength(50))
                .Column<string>("AvatarUrl", column => column.WithLength(1024))
                .Column<int>("AddressId", column => column.Nullable())
                .Column<DateTime>("LastLoginUtc", column => column.Nullable()));

            ContentDefinitionManager.AlterPartDefinition("UserProfilePart", part => part
                .Attachable(false));

            SchemaBuilder.CreateTable("OrganizationPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("Name", c => c.WithLength(256))
                .Column<string>("Description", c => c.Unlimited())
                .Column<string>("IndustryBranch", c => c.WithLength(50))
                .Column<string>("LogoUrl", c => c.WithLength(1024))
                .Column<int>("AddressId", column => column.Nullable())
                .Column<decimal>("Balance", column => column.NotNull()));

            SchemaBuilder.CreateTable("UsersInOrganization", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("OrganizationId")
                .Column<int>("UserId")
                .Column<DateTime>("CreatedUtc"));

            SchemaBuilder.CreateTable("AddressPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("AddressLine1", c => c.WithLength(256))
                .Column<string>("AddressLine2", c => c.WithLength(256))
                .Column<string>("Zipcode", c => c.WithLength(20))
                .Column<string>("City", c => c.WithLength(50))
                .Column<int>("CountryId", c => c.Nullable()));

            SchemaBuilder.CreateTable("Country", table => table
                .Column<int>("Id", c => c.Identity().PrimaryKey())
                .Column<string>("Name", c => c.WithLength(50)));

            SchemaBuilder.CreateTable("Invitation", table => table
                .Column<int>("Id", c => c.Identity().PrimaryKey())
                .Column<int>("OrganizationId")
                .Column<int>("UserId")
                .Column<string>("Token", c => c.WithLength(50))
                .Column<string>("Status", c => c.WithLength(15))
                .Column<DateTime>("CreatedUtc")
                .Column<DateTime>("AcceptedUtc", c => c.Nullable()));

            SchemaBuilder.CreateTable("JoinRequest", table => table
                .Column<int>("Id", c => c.Identity().PrimaryKey())
                .Column<int>("OrganizationId")
                .Column<int>("UserId")
                .Column<string>("Token", c => c.WithLength(50))
                .Column<string>("Status", c => c.WithLength(15))
                .Column<DateTime>("CreatedUtc")
                .Column<DateTime>("AcceptedUtc", c => c.Nullable()));

            ContentDefinitionManager.AlterTypeDefinition("Organization", type => type
                .WithPart("CommonPart")
                .WithPart("AutoroutePart")
                .WithPart("OrganizationPart")
                .WithPart("ShopPart")
                .WithPart("HelpdeskPart")
                .Creatable()
                .Draftable(false));

            ContentDefinitionManager.AlterTypeDefinition("Address", type => type
                .WithPart("CommonPart")
                .WithPart("AddressPart")
                .Creatable()
                .Draftable(false));

            return 1;
        }
    }
}