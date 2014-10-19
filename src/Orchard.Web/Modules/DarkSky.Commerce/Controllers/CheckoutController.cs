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
    [OrchardFeature("DarkSky.Commerce.Checkout")]
    [Authorize]
    public class CheckoutController : Controller {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductManager _productManager;
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _services;
        private readonly IEnumerable<IPaymentProvider> _paymentProviders;
        private readonly IPaymentService _paymentService;
        private readonly ICustomerService _customerService;
        public Localizer T { get; set; }
        private dynamic New { get; set; }

        public CheckoutController(
            IShoppingCartService shoppingCartService, 
            IProductManager productManager, 
            IOrchardServices services, 
            IEnumerable<IPaymentProvider> paymentProviders, 
            IPaymentService paymentService, 
            ICustomerService customerService) {
            _shoppingCartService = shoppingCartService;
            _productManager = productManager;
            _contentManager = services.ContentManager;
            New = services.New;
            _services = services;
            _paymentProviders = paymentProviders;
            _paymentService = paymentService;
            _customerService = customerService;
            T = NullLocalizer.Instance;
        }

        [Themed]
        public ActionResult Index() {
            var shape = CreateViewModelShape();
            return View(shape);
        }

        [Themed]
        [HttpPost]
        [ActionName("Index")]
        public ActionResult Proceed(CheckoutViewModel viewModel) {
            if(!ModelState.IsValid) {
                return View(CreateViewModelShape(viewModel));
            }
            var cart = _shoppingCartService.GetCart();
            var user = _services.WorkContext.CurrentUser;
            var customerInfo = _customerService.GetCustomerInfo(user);
            var checkoutProvider = _paymentService.GetPaymentProviderByName(viewModel.CheckoutProvider);
            var transaction = _paymentService.CreateTransaction(user, cart, checkoutProvider);
            var actionResult = checkoutProvider.InitiateCheckout(new InitiateCheckoutArgs {
                ControllerContext = ControllerContext,
                UrlHelper = Url,
                Cart = cart,
                User = user,
                CustomerInfo = customerInfo,
                Transaction = transaction
            });
            return actionResult;
        }

        private dynamic CreateViewModelShape(CheckoutViewModel model = null) {
            var shoppingCart = _shoppingCartService.GetCart();
            var products = shoppingCart.GetProducts().ToList();
            var items = new List<dynamic>(products.Count);
            foreach (var item in shoppingCart.Items) {
                var product = products.FirstOrDefault(x => x.Product.Id == item.ProductId);
                var productMetadata = _productManager.GetProductMetadata(product.Product);
                var vat = product.Product.Vat;
                var cartItemShape = New.Item(
                    Id: item.Id,
                    ProductId: item.ProductId,
                    Product: product.Product,
                    ImageUrl: productMetadata.PrimaryImageUrl,
                    DisplayText: _contentManager.GetItemMetadata(product.Product).DisplayText,
                    Quantity: item.Quantity,
                    UnitPrice: product.Product.UnitPrice,
                    Vat: vat,
                    Totals: _productManager.CalculateItemTotals(product)
                    );

                items.Add(cartItemShape);
            }

            var shape = New.Checkout(
                ShoppingCart: shoppingCart,
                Totals: shoppingCart.Totals(),
                User: _services.WorkContext.CurrentUser,
                Items: items.ToList(),
                PaymentProviders: _paymentProviders.Select(x => New.PaymentProvider(
                    DisplayName: x.DisplayName,
                    Identity: x.Name,
                    CheckoutButton: x.BuildCheckoutButton(New))).ToList(),
                ViewModel: model ?? new CheckoutViewModel());
            return shape;
        }

        
    }
}