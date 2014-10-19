using System.Linq;
using DarkSky.OrchardMarket.Models;
using DarkSky.OrchardMarket.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace DarkSky.OrchardMarket.Handlers {
    public class OrganizationPartHandler : ContentHandler {
        private readonly IOrganizationService _organizationService;

        public OrganizationPartHandler(IRepository<OrganizationPartRecord> repository, IOrganizationService organizationService) {
            _organizationService = organizationService;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<OrganizationPart>(PropertyHandlers);
        }

        private void PropertyHandlers(ActivatedContentContext context, OrganizationPart part) {
            
            part.AddressField.Setter(address => {
                part.Record.AddressId = address != null ? address.Id : default(int?);
                return address;
            });

            part.AddressField.Loader(address => {
                var addressItem = part.Record.AddressId != null ? part.ContentItem.ContentManager.Get<AddressPart>(part.Record.AddressId.Value) : null;

                if (addressItem == null) {
                    addressItem = part.ContentItem.ContentManager.Create<AddressPart>("Address");
                    part.Record.AddressId = addressItem.Id;
                }
                return addressItem;
            });

            part.UsersField.Loader(list => _organizationService.GetUsersInOrganization(part.Id).ToList().AsReadOnly());
        }
    }
}