using DarkSky.OrchardMarket.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace DarkSky.OrchardMarket.Handlers {
    public class FundedOrderPartHandler : ContentHandler {
        public FundedOrderPartHandler(IRepository<FundedOrderPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}