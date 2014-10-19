using System;
using System.Linq;
using Orchard.Commands;
using Orchard.ContentManagement;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;

namespace DarkSky.OrchardMarket.Commands {
    public class TaxonomyCommands : DefaultOrchardCommandHandler {
        private readonly ITaxonomyService _taxonomyService;
        private readonly IContentManager _contentManager;

        [OrchardSwitch]
        public string ParentTerm { get; set; }

        public TaxonomyCommands(ITaxonomyService taxonomyService, IContentManager contentManager) {
            _taxonomyService = taxonomyService;
            _contentManager = contentManager;
        }

        [CommandName("taxonomy create")]
        [CommandHelp("taxonomy create <name>")]
        public void CreateTaxonomy(string name) {
            var taxonomy = _contentManager.Create<TaxonomyPart>("Taxonomy", item => item.Name = name);
            _taxonomyService.CreateTermContentType(taxonomy);
            Context.Output.WriteLine(T("{0} taxonomy created successfully.", name).Text);
        }

        [CommandName("term create")]
        [OrchardSwitches("ParentTerm")]
        [CommandHelp("term create <taxonomyname> <term1, term2, term3> /ParentTerm: term0")]
        public void CreateTerm(string taxonomyName, string termNames) {
            var terms = termNames.Split(new[] {',', ';'}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
            var taxonomy = _taxonomyService.GetTaxonomyByName(taxonomyName);
            var parentTerm = ParentTerm != null ? _taxonomyService.GetTermByName(taxonomy.Id, ParentTerm) : default(TermPart);

            foreach (var termName in terms) {
                var termPart = _taxonomyService.NewTerm(taxonomy);
                termPart.Name = termName;

                if (parentTerm != null) {
                    termPart.Container = parentTerm;
                    _taxonomyService.ProcessPath(termPart);
                }

                _contentManager.Create(termPart);
            }

            Context.Output.WriteLine(T("Terms for {0} taxonomy created successfully.", taxonomyName).Text);
        }
    }
}