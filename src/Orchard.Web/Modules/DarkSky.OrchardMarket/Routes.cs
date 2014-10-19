using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace DarkSky.OrchardMarket {
    public class Routes : IRouteProvider {

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in routes) {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            yield return CreateRoute("My/Account", "Account", "Index");
            yield return CreateRoute("My/Account/Edit", "Account", "Edit");
            yield return CreateRoute("My/Profile", "Profile", "Index");
            yield return CreateRoute("My/Profile/Edit", "Profile", "Edit");
            yield return CreateRoute("My/Organization", "Organization", "Index");
            yield return CreateRoute("My/Organization/Edit/{id}", "Organization", "Edit");
            yield return CreateRoute("My/Organization/Create", "Organization", "Create");
            yield return CreateRoute("My/Organization/Users/Add/{id}", "Organization", "AddUser");
            yield return CreateRoute("My/Organization/Users/{id}", "Organization", "Users");
            yield return CreateRoute("My/Organization/Join", "Organization", "Join");
            yield return CreateRoute("My/Organization/{id}", "Organization", "Details");
            yield return CreateRoute("My/Packages", "Package", "Index");
            yield return CreateRoute("My/Packages/Create", "Package", "Create");
            yield return CreateRoute("My/Packages/AddRelease/{id}", "Package", "AddRelease");
            yield return CreateRoute("My/Packages/Edit/{id}", "Package", "Edit", new { id = UrlParameter.Optional });
            yield return CreateRoute("My/Packages/Details/{id}", "Package", "Details");
            yield return CreateRoute("My/Packages/StartOrJoinOrganization", "Package", "OrganizationRequired");
            yield return CreateRoute("My/Invoices", "Invoice", "Index", area: "DarkSky.Commerce");
            yield return CreateRoute("My/Sales", "Sales", "Index");
            yield return CreateRoute("My/Purchases", "Purchase", "Index");
            yield return CreateRoute("My/PayoutOptions", "PayoutOption", "Index");
            yield return CreateRoute("My/PayoutOptions/Add", "PayoutOption", "Add");
            yield return CreateRoute("My/PayoutOptions/Edit/{id}", "PayoutOption", "Edit", new { id = UrlParameter.Optional });
            yield return CreateRoute("Browse/{extensionType}/{category}", "Browse", "Index", new { category = UrlParameter.Optional });
            yield return CreateRoute("My/Support/{contentId}", "SupportTicket", "ListByContent", new { contentID = UrlParameter.Optional }, "DarkSky.Helpdesk");
            yield return CreateRoute("My/Support/Create/{contentId}", "SupportTicket", "Create", new { contentID = UrlParameter.Optional }, "DarkSky.Helpdesk");
            yield return CreateRoute("My/Support/Details/{id}", "SupportTicket", "Details", new { id = UrlParameter.Optional }, "DarkSky.Helpdesk");
        }

        private RouteDescriptor CreateRoute(string url, string controller, string action, object extraRouteValues = null, string area = null) {
            var routeValueDictionary = new RouteValueDictionary(extraRouteValues);

            area = area ?? ModuleInfo.ModuleName;
            routeValueDictionary["area"] = area;
            routeValueDictionary["controller"] = controller;
            routeValueDictionary["action"] = action;

            var descriptor = new RouteDescriptor {
                Route = new Route(url,
                    routeValueDictionary,
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                        { "area", area}
                    },
                    new MvcRouteHandler())
            };

            

            return descriptor;
        }
    }
}