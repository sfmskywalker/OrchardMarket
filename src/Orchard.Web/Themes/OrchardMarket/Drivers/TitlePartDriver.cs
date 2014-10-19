using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Title.Models;

namespace OrchardMarket.Drivers {
    public class TitlePartDriver : ContentPartDriver<TitlePart> {
	    private readonly IOrchardServices _services;
	    public TitlePartDriver(IOrchardServices services) {
		    _services = services;
	    }

	    protected override DriverResult Display(TitlePart part, string displayType, dynamic shapeHelper) {
            if(displayType == "Detail")
		        _services.WorkContext.Layout.TitleZone.Add(shapeHelper.Content_Title());
            return new DriverResult();
        }
    }
}