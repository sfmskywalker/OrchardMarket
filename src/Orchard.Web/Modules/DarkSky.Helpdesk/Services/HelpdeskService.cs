using System.Collections.Generic;
using DarkSky.Helpdesk.Helpers;
using DarkSky.Helpdesk.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Security;

namespace DarkSky.Helpdesk.Services {
    public interface IHelpdeskService : IDependency {
        IEnumerable<SupportTicketPart> GetTicketsByContent(int contentId);
        SupportTicketPart CreateSupportTicket(IUser user, string subject, string message, SeverityLevel severity = SeverityLevel.Low, ContentItem content = null);
        SupportTicketPart GetTicket(int id);
    }

    public class HelpdeskService : IHelpdeskService {
        private readonly IContentManager _contentManager;
        public HelpdeskService(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public IEnumerable<SupportTicketPart> GetTicketsByContent(int contentId) {
            return _contentManager.Query<SupportTicketPart, SupportTicketPartRecord>().Where(x => x.ContentId == contentId).List();
        }

        public SupportTicketPart CreateSupportTicket(IUser user, string subject, string message, SeverityLevel severity = SeverityLevel.Low, ContentItem content = null) {
            var ticket = _contentManager.New<SupportTicketPart>("SupportTicket");

            ticket.Content = content;
            ticket.Message = message.TrimSafe();
            ticket.Subject = subject.TrimSafe();
            ticket.Severity = severity;
            ticket.User = user;

            _contentManager.Create(ticket);
            return ticket;
        }

        public SupportTicketPart GetTicket(int id) {
            return _contentManager.Get<SupportTicketPart>(id);
        }
    }
}