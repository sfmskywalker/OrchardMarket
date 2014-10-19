using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace DarkSky.Helpdesk {
    public class Migrations : DataMigrationImpl {
        public int Create() {

            SchemaBuilder.CreateTable("SupportTicketPartRecord", table => table
                .ContentPartRecord()
                .Column<int>("ContentId")
                .Column<string>("Severity", c => c.WithLength(50))
                .Column<string>("Status", c => c.WithLength(50)));

            ContentDefinitionManager.AlterPartDefinition("HelpdeskPart", part => part.Attachable(false));

            ContentDefinitionManager.AlterPartDefinition("SupportTicketPart", part => part.Attachable(false));
            ContentDefinitionManager.AlterTypeDefinition("SupportTicket", type => type
                .WithPart("CommonPart")
                .WithPart("TitlePart")
                .WithPart("BodyPart", part => part.WithSetting("BodyTypePartSettings.Flavor", "markdown"))
                .WithPart("CommentsPart")
                .WithPart("SupportTicketPart"));

            return 1;
        }
    }
}