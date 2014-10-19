using System.Linq;
using System.Web.Mvc;
using DarkSky.Commerce.Models;
using DarkSky.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Themes;

namespace DarkSky.Commerce.Controllers {
    [OrchardFeature("DarkSky.Commerce.Products")]
    [Authorize]
    public class TransactionController : Controller {
        private readonly IInvoiceManager _invoiceManager;
        private readonly IContentManager _contentManager;
        private readonly IPaymentService _paymentService;
        protected dynamic New { get; set; }

        public TransactionController( IInvoiceManager invoiceManager, IContentManager contentManager, IShapeFactory shapeFactory, IPaymentService paymentService) {
            _invoiceManager = invoiceManager;
            _contentManager = contentManager;
            New = shapeFactory;
            _paymentService = paymentService;
        }

        [Themed]
        public ActionResult Complete(int id) {
            var invoices = _invoiceManager.GetInvoicesByTransaction(id).ToList();
            var viewModel = New.ViewModel(
                Invoices: invoices.Select(invoice => New.Invoices(
                    Id: invoice.Id,
                    Shop: invoice.Shop,
                    CreatedUtc: invoice.As<CommonPart>().CreatedUtc,
                    Status: invoice.Status,
                    Transaction: invoice.Transaction,
                    Details: invoice.Details.Select(detail => New.Detail(
                        Id: detail.Id,
                        Description: detail.Description,
                        UnitPrice: detail.UnitPrice,
                        ProductId: detail.ProductId,
                        Product: _contentManager.Get<IProductAspect>(detail.ProductId.Value),
                        Quantity: detail.Quantity,
                        SubTotal: detail.SubTotal(),
                        Vat: detail.Vat(),
                        Total: detail.Total())).ToList())
                ).ToList());
            return View(viewModel);
        }

        [Themed]
        public ActionResult Cancel(int id) {
            _paymentService.CancelTransaction(id);
            return View();
        }
    }
}