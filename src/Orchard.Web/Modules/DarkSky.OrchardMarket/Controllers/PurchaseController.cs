using System.Linq;
using System.Web.Mvc;
using DarkSky.Commerce.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Themes;

namespace DarkSky.OrchardMarket.Controllers {
    [Authorize]
	public class PurchaseController : ControllerBase {
        private readonly IOrchardServices _services;
        private readonly IPurchaseManager _purchaseManager;

        public PurchaseController(IOrchardServices services, IPurchaseManager purchaseManager) {
            _services = services;
            _purchaseManager = purchaseManager;
        }

        [Themed]
		public ActionResult Index() {
            var user = _services.WorkContext.CurrentUser;
            var purchases = _purchaseManager.GetPurchasesByUser(user.Id).ToList();
            
            var viewModel = _services.New.MyPurchases(
                Purchases: purchases.Select(purchase => _services.New.Purchase(
                    Id: purchase.Id,
                    Product: purchase.Product,
                    Shop: purchase.Product.Shop,
                    CreatedUtc: purchase.As<CommonPart>().CreatedUtc,
                    Content: purchase)
                ).ToList()
            );

			return View((object)viewModel);
		}
	}
}