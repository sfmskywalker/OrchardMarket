using System.Linq;
using System.Web.Mvc;
using DarkSky.Commerce.Services;
using DarkSky.OrchardMarket.Models;
using DarkSky.OrchardMarket.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Themes;

namespace DarkSky.OrchardMarket.Controllers {
    [Authorize]
	public class SalesController : ControllerBase {
        private readonly IOrchardServices _services;
        private readonly IOrderManager _orderManager;
        private readonly IOrganizationService _organizationService;

        public SalesController(IOrchardServices services, IOrderManager orderManager, IOrganizationService organizationService) {
            _services = services;
            _orderManager = orderManager;
            _organizationService = organizationService;
        }

        [Themed]
		public ActionResult Index() {
            var organizations = _organizationService.GetOrganizationsByCurrentUser().Select(x => x.Id).ToList();
            var orders = organizations.SelectMany(x => _orderManager.GetOrdersByShop(x)).ToList();
            var revenueRate = _services.WorkContext.CurrentSite.As<MarketSettingsPart>().PayoutPercentage;

            var viewModel = _services.New.MySales(
                RevenueRate: revenueRate,
                Orders: orders.Select(order => _services.New.Order(
                    Id: order.Id,
                    Totals: order.Totals(),
                    Customer: order.User,
                    CreatedUtc: order.As<CommonPart>().CreatedUtc,
                    Status: order.Status,
                    Content: order)
                ).ToList()
            );

			return View((object)viewModel);
		}
	}
}