using DarkSky.Commerce.Models;
using DarkSky.Commerce.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Handlers {
    [OrchardFeature("DarkSky.Commerce.Products")]
    public class VatPartHandler : ProductHandler {
        public VatPartHandler(IRepository<VatPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}