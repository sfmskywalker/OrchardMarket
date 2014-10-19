using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DarkSky.Commerce.PaymentProviders;
using DarkSky.Commerce.Services;
using DarkSky.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Themes;

namespace DarkSky.Commerce.Controllers {
    [OrchardFeature("DarkSky.Commerce.Products")]
    [Authorize]
    public class InvoiceController : Controller {
        private readonly IOrchardServices _services;
        private readonly IInvoiceManager _invoiceManager;
        public Localizer T { get; set; }
        private dynamic New { get; set; }

        public InvoiceController(
            IOrchardServices services, 
            IInvoiceManager invoiceManager) {
            _services = services;
            _invoiceManager = invoiceManager;
            New = services.New;
            T = NullLocalizer.Instance;
        }

        [Themed]
        public ActionResult Index() {
            var user = _services.WorkContext.CurrentUser;
            var invoices = _invoiceManager.GetInvoices(user).ToList();
            return View(invoices);
        }
    }
}