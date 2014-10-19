using System.Collections.Generic;
using System.Linq;
using DarkSky.OrchardMarket.Models;
using DarkSky.OrchardMarket.Services;
using Orchard.Data;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Environment;
using Orchard.Taxonomies.Services;

namespace DarkSky.OrchardMarket.Shapes {
    public class EditorShapes : IShapeTableProvider {
        private readonly Work<IRepository<Country>> _countryRepository;
        private readonly Work<ITaxonomyService> _taxonomyService;
        private readonly Work<IEnumerable<IPayoutMethod>> _payoutMethods;
        private readonly Work<IOrganizationService> _organizationService;

        public EditorShapes(
            Work<IRepository<Country>> countryRepository, 
            Work<ITaxonomyService> taxonomyService, 
            Work<IEnumerable<IPayoutMethod>> payoutMethods,
            Work<IOrganizationService> organizationService) {
            _countryRepository = countryRepository;
            _taxonomyService = taxonomyService;
            _payoutMethods = payoutMethods;
            _organizationService = organizationService;
        }

        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Editor_CountryDropDownList").OnDisplaying(displaying => {
                displaying.Shape.Countries = _countryRepository.Value.Table.OrderBy(x => x.Name).ToList();
            });

            builder.Describe("Editor_CategoriesCheckList").OnDisplaying(displaying => {
                var taxonomyService = _taxonomyService.Value;
                var taxonomy = taxonomyService.GetTaxonomyByName("Categories");
                displaying.Shape.Categories = taxonomyService.GetTerms(taxonomy.Id).ToList();
            });

            builder.Describe("Editor_PayoutMethodPicker").OnDisplaying(displaying => {
                displaying.Shape.PayoutMethods = _payoutMethods.Value.ToList();
            });

            builder.Describe("Editor_OrganizationDropDownList").OnDisplaying(displaying => {
                displaying.Shape.Organizations = _organizationService.Value.GetOrganizationsByCurrentUser().ToList();
            });
        }
    }
}