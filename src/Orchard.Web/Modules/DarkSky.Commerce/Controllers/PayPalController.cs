using System.Web.Mvc;
using DarkSky.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Security;

namespace DarkSky.Commerce.Controllers {
    [OrchardFeature("DarkSky.Commerce.PaymentProviders.PayPal")]
    public class PayPalController : Controller {
        private readonly IPaymentService _paymentService;
        private readonly IOrderManager _orderManager;
        private readonly IContentManager _contentManager;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IInvoiceManager _invoiceManager;

        public PayPalController(IPaymentService paymentService, IOrderManager orderManager, IContentManager contentManager, IShoppingCartService shoppingCartService, IInvoiceManager invoiceManager) {
            _paymentService = paymentService;
            _orderManager = orderManager;
            _contentManager = contentManager;
            _shoppingCartService = shoppingCartService;
            _invoiceManager = invoiceManager;
        }

        public ActionResult Complete(string token, string payerId) {
            var transaction = _paymentService.GetTransactionByProviderToken(token);

            if (transaction == null)
                return HttpNotFound("No transaction found with the specified token");

            if (_paymentService.PaymentExists(transaction.Id, payerId))
                return RedirectToAction("AlreadyPaid");

            var user = _contentManager.Get<IUser>(transaction.UserId);
            var payment = _paymentService.CreatePayment(new CreatePaymentArgs { Transaction = transaction, PaymentToken = payerId});
            var shoppingCart = _shoppingCartService.GetCart(transaction.CartId);
            var orders = _orderManager.CreateOrders(user, shoppingCart, transaction);
            var invoices = _invoiceManager.CreateInvoices(user, orders);
            
            _invoiceManager.PayInvoices(invoices, payment);
            return RedirectToAction("Complete", "Transaction", new {id = transaction.Id, area = "DarkSky.Commerce"});
        }

        public ActionResult AlreadyPaid() {
            return View();
        }

        public ActionResult Cancel(string token) {
            var transaction = _paymentService.GetTransactionByProviderToken(token);

            if (transaction == null)
                return HttpNotFound("No transaction found with the specified token");

            return RedirectToAction("Cancel", "Transaction", new { id = transaction.Id, area = "DarkSky.Commerce" });
        }
    }
}