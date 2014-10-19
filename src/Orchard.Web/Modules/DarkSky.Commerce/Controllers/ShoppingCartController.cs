using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DarkSky.Commerce.EventHandlers;
using DarkSky.Commerce.Models;
using DarkSky.Commerce.Services;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Themes;

namespace DarkSky.Commerce.Controllers {
    [OrchardFeature("DarkSky.Commerce.ShoppingCart")]
    public class ShoppingCartController : Controller {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShoppingCartSettingsAccessor _settings;
        private readonly IContentManager _contentManager;
        private readonly IProductManager _productManager;
        private readonly IShoppingCartHandler _shoppingCartHandler;
        public Localizer T { get; set; }
        private dynamic New { get; set; }

        public ShoppingCartController(IShoppingCartService shoppingCartService, IShoppingCartSettingsAccessor settings, IContentManager contentManager, IShapeFactory shapeFactory, IProductManager productManager, IShoppingCartHandler shoppingCartHandler) {
            _shoppingCartService = shoppingCartService;
            _settings = settings;
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
            New = shapeFactory;
            _productManager = productManager;
            _shoppingCartHandler = shoppingCartHandler;
        }


        [Themed]
        public ActionResult Index() {
            var shoppingCart = _shoppingCartService.GetCart();
            var products = shoppingCart.GetProducts().ToList();
            var items = new List<dynamic>(products.Count);

            foreach (var item in shoppingCart.Items) {
                var product = products.FirstOrDefault(x => x.Product.Id == item.ProductId);
                if (product == null) continue;

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

                _shoppingCartHandler.Display(new CartItemDisplayContext { Product = product, Shape = cartItemShape });
                items.Add(cartItemShape);
            }

            var shape = New.ViewModel(
                ShoppingCart: shoppingCart,
                Totals: shoppingCart.Totals(),
                Items: items.ToList());
            return View(shape);
        }

        public ActionResult Add(int productId, int quantity = 1, string returnUrl = null) {
            var urlReferrer = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : Url.Action("Index");
            var actionResult = Redirect(string.IsNullOrWhiteSpace(returnUrl) ? urlReferrer : returnUrl);
            var product = _contentManager.Get<IProductAspect>(productId, VersionOptions.Published);
            var isQuantityValid = quantity > 0 && quantity <= _settings.MaxQuantityPerItem;

            _shoppingCartHandler.Adding(new AddingShoppingCartItemContext {
                ProductId = productId,
                Product = product,
                Quantity = quantity,
                IsQuantityValid = isQuantityValid,
                Settings = _settings
            });

            if (!isQuantityValid || product == null) {
                return actionResult;
            }
            
            var cart = _shoppingCartService.GetCart();
            var cartItem = cart.Add(productId, quantity);
            _shoppingCartHandler.Added(new AddedShoppingCartItemContext { Product = product, Item = cartItem });
            return actionResult;
        }

        public ActionResult Remove(int id) {
            var cart = _shoppingCartService.GetCart();
            var item = cart.GetItem(id);
            var product = _contentManager.Get<IProductAspect>(item.ProductId, VersionOptions.Published);

            _shoppingCartHandler.Removing(new RemovingShoppingCartItemContext { Item = item, Product = product });
            cart.Remove(id);
            _shoppingCartHandler.Removed(new RemovedShoppingCartItemContext { Item = item, Product = product });

            return RedirectToAction("Index");
        }

        public ActionResult ContinueShopping() {
            // TODO: Return to last visited or default catalog
            return Redirect("~/");
        }

        public ActionResult Checkout() {
            return RedirectToAction("Index", "Checkout");
        }
    }

}