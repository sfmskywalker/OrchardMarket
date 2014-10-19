using System;
using DarkSky.OrchardMarket.Models;
using DarkSky.OrchardMarket.Providers.Projections.FilterEditors.Forms;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Projections.FilterEditors;

namespace DarkSky.OrchardMarket.Providers.Projections.FilterEditors {
    public class ExtensionTypeFilterEditor : IFilterEditor {
        public ExtensionTypeFilterEditor() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public bool CanHandle(Type type) {
            return typeof (ExtensionType) == type;
        }

        public string FormName {
            get { return ExtensionTypeFilterForm.FormName; }
        }

        public Action<IHqlExpressionFactory> Filter(string property, dynamic formState) {
            return ExtensionTypeFilterForm.GetFilterPredicate(formState, property);
        }

        public LocalizedString Display(string property, dynamic formState) {
            return ExtensionTypeFilterForm.DisplayFilter(property, formState, T);
        }
    }
}