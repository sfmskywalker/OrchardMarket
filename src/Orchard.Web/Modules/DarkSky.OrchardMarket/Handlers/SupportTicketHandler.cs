using DarkSky.Helpdesk.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using DarkSky.OrchardMarket.Models;

namespace DarkSky.OrchardMarket.Handlers {
    public class SupportTicketHandler : ContentHandler {
        
        public SupportTicketHandler() {
            OnCreated<SupportTicketPart>(OnSupportTicketCreated);
        }

        private static void OnSupportTicketCreated(CreateContentContext context, SupportTicketPart part) {
            if (part.Content == null || !part.Content.Is<PackagePart>())
                return;

            AssignHelpdesk(part);
        }

        private static void AssignHelpdesk(SupportTicketPart part) {
            var package = part.Content.As<PackagePart>();
            var organization = package.Organization;
            var helpdesk = organization.As<HelpdeskPart>();

            part.Helpdesk = helpdesk;
        }
    }
}