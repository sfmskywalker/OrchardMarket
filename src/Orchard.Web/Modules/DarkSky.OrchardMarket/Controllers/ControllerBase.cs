using System.Web.Mvc;
using Orchard.Localization;
using Orchard.Mvc;

namespace DarkSky.OrchardMarket.Controllers {
	public abstract class ControllerBase : Controller {
	    public Localizer T { get; set; }
        
        protected ControllerBase() {
	        T = NullLocalizer.Instance;
	    }

        protected ShapeResult Shape(dynamic shape) {
            return new ShapeResult(this, shape);
        }
	}
}