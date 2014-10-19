using DarkSky.Commerce.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace DarkSky.Commerce.Handlers {
	public class ExchangeRatePartHandler : ContentHandler {
		public ExchangeRatePartHandler(IRepository<ExchangeRatePartRecord> repository) {
			Filters.Add(StorageFilter.For(repository));
		}
	}
}