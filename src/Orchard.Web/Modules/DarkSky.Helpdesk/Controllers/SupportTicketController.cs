using System.Linq;
using System.Web.Mvc;
using DarkSky.Helpdesk.Services;
using DarkSky.Helpdesk.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Themes;
using Orchard.UI.Notify;

namespace DarkSky.Helpdesk.Controllers {
    [Authorize]
	public class SupportTicketController : Controller {
        private readonly IHelpdeskService _helpdeskService;
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _services;
        private readonly INotifier _notifier;
        protected dynamic New { get; set; }
        protected Localizer T { get; set; }

        public SupportTicketController(IHelpdeskService helpdeskService, IOrchardServices services) {
            _helpdeskService = helpdeskService;
            _services = services;
            New = services.New;
            T = NullLocalizer.Instance;
            _contentManager = _services.ContentManager;
            _notifier = services.Notifier;
        }

        [Themed]
		public ActionResult ListByContent(int contentId) {
            var content = _contentManager.Get(contentId);
            var tickets = _helpdeskService.GetTicketsByContent(contentId).ToList();
            var model = New.ViewModel(
                Tickets: tickets,
                Content: content,
                ContentDisplayText: _contentManager.GetItemMetadata(content).DisplayText
            );
			return View(model);
		}

        [Themed]
        public ActionResult Create(int? contentId) {
            var content = contentId != null ? _contentManager.Get(contentId.Value) : default(ContentItem);
            var model = new CreateSupportTicketViewModel {ContentId = contentId};
            var shape = New.ViewModel(
                Content: content,
                ContentDisplayText: content != null ? _contentManager.GetItemMetadata(content).DisplayText : default(string),
                Model: model);
            return View(shape);
        }

        [Themed, HttpPost]
        public ActionResult Create(CreateSupportTicketViewModel viewModel) {
            if(!ModelState.IsValid)
                return View(viewModel);

            var user = _services.WorkContext.CurrentUser;
            var content = viewModel.ContentId != null ? _contentManager.Get(viewModel.ContentId.Value) : default(ContentItem);
            var ticket = _helpdeskService.CreateSupportTicket(user, viewModel.Subject, viewModel.Message, viewModel.Severity, content);

            _notifier.Information(T("Your support ticket has been created with ID {0}.", ticket.Id));
            return content != null ? RedirectToAction("ListByContent", new {contentId = viewModel.ContentId}) : RedirectToAction("Index");
        }

        [Themed]
        public ActionResult Details(int id) {
            var ticket = _helpdeskService.GetTicket(id);
            var ticketShape = _contentManager.BuildDisplay(ticket, "Detail");
            var shape = New.ViewModel(
                TicketShape: ticketShape,
                Ticket: ticket);
            return View(shape);
        }
	}
}