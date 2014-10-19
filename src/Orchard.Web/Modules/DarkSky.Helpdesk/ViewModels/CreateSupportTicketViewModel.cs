using DarkSky.Helpdesk.Models;

namespace DarkSky.Helpdesk.ViewModels {
    public class CreateSupportTicketViewModel {
        public int? ContentId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public SeverityLevel Severity { get; set; }
    }
}