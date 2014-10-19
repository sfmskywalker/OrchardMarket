using System;
using System.Web.Mvc;
using DarkSky.OrchardMarket.Models;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;

namespace DarkSky.OrchardMarket.Providers.Projections.FilterEditors.Forms {

    public class ExtensionTypeFilterForm : IFormProvider {
        public const string FormName = "ExtensionTypeFilter";
        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public ExtensionTypeFilterForm(IShapeFactory shapeFactory) {
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context) {
            Func<IShapeFactory, object> form =
                shape => {

                    var f = Shape.Form(
                        Id: "ExtensionTypeFilter",
                        _Value: Shape.SelectList(
                            Id: "ExtensionType", Name: "ExtensionType",
                            Title: T("Extension Type"),
                            Size: 1,
                            Multiple: false
                        ));

                    f._Value.Add(new SelectListItem { Value = ExtensionType.Module.ToString(), Text = T("Module").Text });
                    f._Value.Add(new SelectListItem { Value = ExtensionType.Theme.ToString(), Text = T("Theme").Text });

                    return f;
                };

            context.Form(FormName, form);
        }

        public static Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState, string property) {
            object value = Convert.ToString(formState.ExtensionType) ?? "";
            return x => x.Eq(property, value);
        }

        public static LocalizedString DisplayFilter(string fieldName, dynamic formState, Localizer T) {
            string value = Convert.ToString(formState.ExtensionType) ?? "";
            return T("{0} is equal to '{1}'", fieldName, value);
        }
    }
}