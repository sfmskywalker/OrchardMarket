using DarkSky.Helpdesk.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace DarkSky.Helpdesk.Handlers {
    public class SupportTicketPartHandler : ContentHandler {
        private readonly IContentManager _contentManager;

        public SupportTicketPartHandler(IRepository<SupportTicketPartRecord> repository, IContentManager contentManager) {
            _contentManager = contentManager;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<SupportTicketPart>(PropertyHandlers);
        }

        private void PropertyHandlers(ActivatedContentContext context, SupportTicketPart part) {
            part.ContentField.Loader(x => part.Record.ContentId != null ? _contentManager.Get(part.Record.ContentId.Value) : null);
            part.ContentField.Setter(x => { part.Record.ContentId = x != null ? x.Id : default(int?); return x; });
        }
    }
}