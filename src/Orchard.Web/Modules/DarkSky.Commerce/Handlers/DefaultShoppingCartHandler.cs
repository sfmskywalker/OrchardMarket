using System.Web.Mvc;
using System.Web.Routing;
using DarkSky.Commerce.EventHandlers;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.UI.Notify;

namespace DarkSky.Commerce.Handlers {
    [OrchardFeature("DarkSky.Commerce.ShoppingCart.DefaultNotifications")]
    public class DefaultShoppingCartHandler : Component, IShoppingCartHandler {
        private readonly INotifier _notifier;
        private readonly IContentManager _contentManager;
        private readonly RequestContext _requestContext;

        public DefaultShoppingCartHandler(INotifier notifier, IContentManager contentManager, RequestContext requestContext) {
            _notifier = notifier;
            _contentManager = contentManager;
            _requestContext = requestContext;
        }

        public void Display(CartItemDisplayContext context) {
        }

        public void Adding(AddingShoppingCartItemContext context) {
            if(!context.IsQuantityValid)
                _notifier.Error(T("Quantity must be 1 or higher and {0} or less", context.Settings.MaxQuantityPerItem));

            if(context.Product == null)
                _notifier.Error(T("No product with ID {0} could be found", context.ProductId));
        }

        public void Added(AddedShoppingCartItemContext context) {
            var urlHelper = new UrlHelper(_requestContext);
            _notifier.Information(T("{0} has been added to the shopping cart. <a class=\"proceed-to-checkout\" href=\"{1}\">Proceed to Checkout</a>", _contentManager.GetItemMetadata(context.Product).DisplayText, urlHelper.Action("Index", "ShoppingCart", new { area = "DarkSky.Commerce" })));
        }

        public void Removing(RemovingShoppingCartItemContext context) {
        }

        public void Removed(RemovedShoppingCartItemContext context) {
            _notifier.Information(T("{0} has been removed from the shopping cart", _contentManager.GetItemMetadata(context.Product).DisplayText));
        }
    }
}