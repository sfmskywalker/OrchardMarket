using DarkSky.OrchardMarket.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace DarkSky.OrchardMarket.Handlers {
    public class AddressPartHandler : ContentHandler {
        private readonly IRepository<Country> _countryRepository;

        public AddressPartHandler(IRepository<AddressPartRecord> repository, IRepository<Country> countryRepository) {
            _countryRepository = countryRepository;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<AddressPart>(PropertySetHandlers);
        }

        private void PropertySetHandlers(ActivatedContentContext context, AddressPart part) {
            part.CountryField.Setter(country => {
                part.Record.CountryId = country != null ? country.Id : default(int?);
                return country;
            });

            part.CountryField.Loader(country => part.Record.CountryId != null ? _countryRepository.Get(part.Record.CountryId.Value) : new Country{ Name = ""});
        }
    }
}