using System.Web.Mvc;
using System.Web.Routing;
using DarkSky.Commerce.Services;
using Orchard.Environment;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Filters;
using Orchard.UI.Admin;

namespace DarkSky.Commerce.Filters {
    [OrchardFeature("DarkSky.Commerce.ShoppingCart")]
    public class ShoppingCartFilter : FilterProvider, IActionFilter {
        private readonly Work<IShoppingCartService> _shoppingCartService;
        private readonly RequestContext _requestContext;

        public ShoppingCartFilter(Work<IShoppingCartService> shoppingCartService, RequestContext requestContext) {
            _shoppingCartService = shoppingCartService;
            _requestContext = requestContext;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext) {
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) {
            if (AdminFilter.IsApplied(_requestContext))
                return;
            filterContext.Controller.ViewBag.ShoppingCart = _shoppingCartService.Value.GetCart();
        }
    }
}