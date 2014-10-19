using System.Linq;
using DarkSky.OrchardMarket.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using DarkSky.OrchardMarket.Models;

namespace DarkSky.OrchardMarket.Handlers {
    public class UserProfilePartHandler : ContentHandler {
        private readonly IContentManager _contentManager;
        private readonly IOrganizationService _organizationService;

        public UserProfilePartHandler(IRepository<UserProfilePartRecord> repository, IContentManager contentManager, IOrganizationService organizationService) {
            _contentManager = contentManager;
            _organizationService = organizationService;
            Filters.Add(StorageFilter.For(repository));
            Filters.Add(new ActivatingFilter<UserProfilePart>("User"));
            OnActivated<UserProfilePart>(PropertySetHandlers);
        }

        private void PropertySetHandlers(ActivatedContentContext context, UserProfilePart part) {
            part.OrganizationsField.Loader(list => _organizationService.GetOrganizationsByUser(part.Id).ToList().AsReadOnly());

            part.AddressField.Setter(address => {
                part.Record.AddressId = address != null ? address.Id : default(int?);
                return address;
            });

            part.AddressField.Loader(address => {
                var addressItem = part.Record.AddressId != null ? _contentManager.Get<AddressPart>(part.Record.AddressId.Value) : null;

                if(addressItem == null) {
                    addressItem = _contentManager.Create<AddressPart>("Address");
                    part.Record.AddressId = addressItem.Id;
                }
                return addressItem;
            });
        }
    }
}