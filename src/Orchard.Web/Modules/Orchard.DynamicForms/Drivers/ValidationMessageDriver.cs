﻿using System.Collections.Generic;
using Orchard.DynamicForms.Elements;
using Orchard.Forms.Services;
using Orchard.Layouts.Framework.Drivers;

namespace Orchard.DynamicForms.Drivers {
    public class ValidationMessageDriver : FormsElementDriver<ValidationMessage> {
        public ValidationMessageDriver(IFormManager formManager) : base(formManager) {}

        protected override IEnumerable<string> FormNames {
            get { yield return "ValidationMessage"; }
        }

        protected override void DescribeForm(DescribeContext context) {
            context.Form("ValidationMessage", factory => {
                var shape = (dynamic)factory;
                var form = shape.Fieldset(
                    Id: "ValidationMessage",
                    _ValidationMessageFor: shape.Textbox(
                        Id: "ValidationMessageFor",
                        Name: "ValidationMessageFor",
                        Title: "For",
                        Classes: new[] { "text", "large" },
                        Description: T("The name of the field this validation message is for.")));

                return form;
            });
        }
    }
}