using System.Web.Mvc;
using Orchard.Mvc.Filters;

namespace DarkSky.OrchardMarket.Filters {
    public class DefaultLogoFilter : FilterProvider, IActionFilter {
        public void OnActionExecuting(ActionExecutingContext filterContext) {
            filterContext.Controller.ViewBag.DefaultModuleLogoUrl = "~/Modules/DarkSky.OrchardMarket/Content/default-package-logo.png";
            filterContext.Controller.ViewBag.DefaultModuleLogoUrlSmall = "~/Modules/DarkSky.OrchardMarket/Content/module-icon.png";
            filterContext.Controller.ViewBag.DefaultThemeLogoUrl = "~/Modules/DarkSky.OrchardMarket/Content/default-package-logo.png";
            filterContext.Controller.ViewBag.DefaultThemeLogoUrlSmall = "~/Modules/DarkSky.OrchardMarket/Content/module-icon.png";
            filterContext.Controller.ViewBag.DefaultTeamLogoUrl = "~/Modules/DarkSky.OrchardMarket/Content/module-icon.png";
            filterContext.Controller.ViewBag.DefaultAvatarUrl = "~/Modules/DarkSky.OrchardMarket/Content/default-package-logo.png";
            filterContext.Controller.ViewBag.DefaultOrganizationLogoUrl = "~/Modules/DarkSky.OrchardMarket/Content/organization-logo-placeholder.png";
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) {
        }
    }
}