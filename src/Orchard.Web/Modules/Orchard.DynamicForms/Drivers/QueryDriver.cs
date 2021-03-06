﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.DynamicForms.Elements;
using Orchard.Environment.Extensions;
using Orchard.Forms.Services;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Framework.Drivers;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using Orchard.Tokens;
using Orchard.Utility.Extensions;
using DescribeContext = Orchard.Forms.Services.DescribeContext;

namespace Orchard.DynamicForms.Drivers {
    [OrchardFeature("Orchard.DynamicForms.Projections")]
    public class QueryDriver : FormsElementDriver<Query> {
        private readonly IProjectionManager _projectionManager;
        private readonly IContentManager _contentManager;
        private readonly ITokenizer _tokenizer;

        public QueryDriver(IFormManager formManager, IProjectionManager projectionManager, IContentManager contentManager, ITokenizer tokenizer)
            : base(formManager) {
            _projectionManager = projectionManager;
            _contentManager = contentManager;
            _tokenizer = tokenizer;
        }

        protected override IEnumerable<string> FormNames {
            get {
                yield return "AutoLabel";
                yield return "QueryForm";
            }
        }

        protected override void DescribeForm(DescribeContext context) {
            context.Form("QueryForm", factory => {
                var shape = (dynamic)factory;
                var form = shape.Fieldset(
                    Id: "QueryForm",
                    _OptionLabel: shape.Textbox(
                        Id: "OptionLabel",
                        Name: "OptionLabel",
                        Title: "Option Label",
                        Description: T("Optionally specify a label for the first option. If no label is specified, no empty option will be rendered."),
                        Classes: new[]{"text", "large", "tokenized"}),
                    _Query: shape.SelectList(
                        Id: "QueryId",
                        Name: "QueryId",
                        Title: "Query",
                        Description: T("Select the query to use as a source for the list.")),
                    _TextExpression: shape.Textbox(
                        Id: "TextExpression",
                        Name: "TextExpression",
                        Title: "Text Expression",
                        Value: "{Content.DisplayText}",
                        Description: T("Specify the expression to get the display text of each option."),
                        Classes: new[]{"text", "large", "tokenized"}),
                    _ValueExpression: shape.Textbox(
                        Id: "ValueExpression",
                        Name: "ValueExpression",
                        Title: "Value Expression",
                        Value: "{Content.Id}",
                        Description: T("Specify the expression to get the value of each option."),
                        Classes: new[]{"text", "large", "tokenized"}),
                    _InputType: shape.SelectList(
                        Id: "InputType",
                        Name: "InputType",
                        Title: "Input Type",
                        Description: T("The control to render when presenting the list of options.")));

                // Query
                var queries = _contentManager.Query<QueryPart, QueryPartRecord>().Join<TitlePartRecord>().OrderBy(x => x.Title).List().ToArray();
                foreach (var query in queries) {
                    form._Query.Items.Add(new SelectListItem {Text = query.Name, Value = query.Id.ToString(CultureInfo.InvariantCulture)});
                }

                // Input Type
                form._InputType.Items.Add(new SelectListItem { Text = T("Select List").Text, Value = "SelectList" });
                form._InputType.Items.Add(new SelectListItem { Text = T("Multi Select List").Text, Value = "MultiSelectList" });
                form._InputType.Items.Add(new SelectListItem { Text = T("Radio List").Text, Value = "RadioList" });
                form._InputType.Items.Add(new SelectListItem { Text = T("Check List").Text, Value = "CheckList" });

                return form;
            });
        }

        protected override void OnDisplaying(Query element, ElementDisplayContext context) {
            var queryId = element.QueryId;
            var typeName = element.GetType().Name;
            var category = element.Category.ToSafeName();
            var displayType = context.DisplayType;

            context.ElementShape.Options = GetOptions(element, queryId).ToArray();
            context.ElementShape.Metadata.Alternates.Add(String.Format("Element__{0}__{1}__{2}", category, typeName, element.InputType));
            context.ElementShape.Metadata.Alternates.Add(String.Format("Element_{0}__{1}__{2}__{3}", displayType, category, typeName, element.InputType));
        }

        private IEnumerable<SelectListItem> GetOptions(Query element, int? queryId) {
            var optionLabel = element.OptionLabel;

            if (!String.IsNullOrWhiteSpace(optionLabel)) {
                yield return new SelectListItem { Text = optionLabel };
            }

            if (queryId == null)
                yield break;


            var contentItems = _projectionManager.GetContentItems(queryId.Value).ToArray();
            var valueExpression = !String.IsNullOrWhiteSpace(element.ValueExpression) ? element.ValueExpression : "{Content.Id}";
            var textExpression = !String.IsNullOrWhiteSpace(element.TextExpression) ? element.TextExpression : "{Content.DisplayText}";
            
            foreach (var contentItem in contentItems) {
                var data = new {Content = contentItem};
                var value = _tokenizer.Replace(valueExpression, data);
                var text = _tokenizer.Replace(textExpression, data);

                yield return new SelectListItem {
                    Text = text,
                    Value = value
                };
            }
        }
    }
}