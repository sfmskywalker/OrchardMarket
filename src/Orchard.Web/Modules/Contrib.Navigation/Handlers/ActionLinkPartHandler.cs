using Contrib.Navigation.Models;
using Contrib.Navigation.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Contrib.Navigation.Handlers {
    public class ActionLinkPartHandler : ContentHandler {
        private readonly IRouteValuesProcessor _routeValuesProcessor;

        public ActionLinkPartHandler(IRepository<ActionLinkPartRecord> repository, IRouteValuesProcessor routeValuesProcessor) {
            _routeValuesProcessor = routeValuesProcessor;
            Filters.Add(StorageFilter.For(repository));
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {

            if (context.ContentItem.ContentType != "ActionLink")
                return;

            var actionLinkPart = context.ContentItem.As<ActionLinkPart>();
            var displayRouteValues = _routeValuesProcessor.Parse(actionLinkPart.RouteValues);

            displayRouteValues["area"] = actionLinkPart.AreaName;
            displayRouteValues["controller"] = actionLinkPart.ControllerName;
            displayRouteValues["action"] = actionLinkPart.ActionName;

            context.Metadata.DisplayRouteValues = displayRouteValues;
        }
    }
}