using System.Linq;
using System.Web.Mvc;
using DarkSky.OrchardMarket.Models;
using DarkSky.OrchardMarket.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Themes;
using Orchard.UI.Navigation;

namespace DarkSky.OrchardMarket.Controllers {
    [Themed]
	public class BrowseController : ControllerBase {
        private readonly IPackageServices _packageServices;
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _services;
        protected dynamic New { get; set; }

        public BrowseController(IPackageServices packageServices, IOrchardServices services) {
            _packageServices = packageServices;
            New = services.New;
            _contentManager = services.ContentManager;
            _services = services;
        }

        public ActionResult Index(ExtensionType extensionType, PagerParameters pagerParameters, string category = null, string tag = null, string keyword = null) {
            var pager = new Pager(_services.WorkContext.CurrentSite, pagerParameters.Page, pagerParameters.PageSize);
            var args = new FindPackagesArgs {ExtensionType = extensionType, Category = category, Tag = tag, Keyword = keyword};
            var count = _packageServices.Count(args);
            var modules = _packageServices.Find(args, pager).ToList();
            var list = New.List();
            var pagerShape = New.Pager(pager);

            pagerShape.TotalItemCount(count);
            list.AddRange(modules.Select(x => _contentManager.BuildDisplay(x, "Summary")).ToList());

            return View("Browse", (object)New.ViewModel(List: list, Pager: pagerShape, ExtensionType: extensionType, Category: category, Tag: tag, Keyword: keyword));
        }

        public ActionResult Support() {
            return View("Browse");
        }
	}
}