using System.Linq;
using DarkSky.Commerce.Models;
using DarkSky.Commerce.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Handlers {
    [OrchardFeature("DarkSky.Commerce.Orders")]
    public class OrderPartHandler : ContentHandler {
        private readonly IOrderManager _orderManager;

        public OrderPartHandler(IRepository<OrderPartRecord> repository, IOrderManager orderManager) {
            _orderManager = orderManager;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<OrderPart>(PropertyHandlers);
        }

        private void PropertyHandlers(ActivatedContentContext context, OrderPart part) {
            part.DetailsField.Loader(list => _orderManager.GetDetails(part.Id).ToList());
        }
    }
}