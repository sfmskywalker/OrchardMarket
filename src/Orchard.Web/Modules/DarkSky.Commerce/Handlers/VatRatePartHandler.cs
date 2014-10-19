using DarkSky.Commerce.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace DarkSky.Commerce.Handlers {
	public class VatRatePartHandler : ContentHandler {
		public VatRatePartHandler(IRepository<VatRatePartRecord> repository) {
			Filters.Add(StorageFilter.For(repository));
		}
	}
}