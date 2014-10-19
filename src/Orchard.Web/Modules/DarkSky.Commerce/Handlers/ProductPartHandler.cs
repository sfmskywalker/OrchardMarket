using DarkSky.Commerce.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace DarkSky.Commerce.Handlers {
	public class ProductPartHandler : ContentHandler {
		public ProductPartHandler(IRepository<ProductPartRecord> repository) {
			Filters.Add(StorageFilter.For(repository));
		}
	}
}