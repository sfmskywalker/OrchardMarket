using System;
using System.Web.Routing;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.Services;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;

namespace DarkSky.OrchardMarket.Providers.Projections.Filters {

    public class RoutedTermFilter : IFilterProvider {
        private readonly ITaxonomyService _taxonomyService;
        private readonly IOrchardServices _services;
        private readonly RequestContext _requestContext;

        public RoutedTermFilter(ITaxonomyService taxonomyService, IOrchardServices services, RequestContext requestContext) {
            _taxonomyService = taxonomyService;
            _services = services;
            _requestContext = requestContext;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeFilterContext describe) {
            describe.For("Taxonomy", T("Taxonomy"), T("Taxonomy"))
                .Element("RoutedTerm", T("Has Routed Term"), T("Categorized content items"),
                    ApplyFilter,
                    DisplayFilter,
                    "SelectRouteValueName"
                );
        }

        public void ApplyFilter(dynamic context) {
            var routeValueName = Convert.ToString(context.State.RouteValueName);
            var termPath = _requestContext.RouteData.Values[routeValueName] ?? _requestContext.HttpContext.Request.QueryString[routeValueName];
            var taxonomyId = Convert.ToInt32(context.State.TaxonomyId);
            var term = termPath != null ? _taxonomyService.GetTermByName(taxonomyId, termPath) : default(TermPart);
            Action<IHqlExpressionFactory> predicate = a => a.Eq("Id", term != null ? term.Id : 0);
            Action<IAliasFactory> selector = alias => alias.ContentPartRecord<TermsPartRecord>().Property("Terms", "terms").Property("TermRecord", "termRecord");
            context.Query.Where(selector, predicate);
        }

        public LocalizedString DisplayFilter(dynamic context) {
            string routeValueName = Convert.ToString(context.State.RouteValueName);
            return String.IsNullOrEmpty(routeValueName) ? T("Routed term") : T("Categorized with the term provided by the {0} route value", routeValueName);
        }
    }

}