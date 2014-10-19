using System;
using System.Web.Mvc;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Taxonomies.Projections;
using Orchard.Taxonomies.Services;

namespace DarkSky.OrchardMarket.Providers.Projections.Filters {
    
    public class RoutedTermFilterForms : IFormProvider {
        private readonly ITaxonomyService _taxonomyService;
        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public RoutedTermFilterForms(
            IShapeFactory shapeFactory,
            ITaxonomyService taxonomyService) {
            _taxonomyService = taxonomyService;
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(dynamic context) {
            Func<IShapeFactory, object> form =
                shape => {

                    var f = Shape.Form(
                        Id: "SelectRouteValueName",
                        _RouteValueName: Shape.TextBox(
                            Id: "RouteValueName",
                            Name: "RouteValueName",
                            Title: T("Route Value Name"),
                            Description: T("Enter the name of the route value.")
                            ),
                        _Taxonomies: Shape.SelectList(
                            Id: "TaxonomyId",
                            Name: "TaxonomyId",
                            Title: "Taxonomy",
                            Description: "Select a taxonomy",
                            Multiple: false
                            )
                        );

                    foreach (var taxonomy in _taxonomyService.GetTaxonomies()) {
                        f._Taxonomies.Add(new SelectListItem { Value = taxonomy.Id.ToString(), Text = taxonomy.Name });
                    }

                    return f;
                };

            context.Form("SelectRouteValueName", form);

        }
    }
}