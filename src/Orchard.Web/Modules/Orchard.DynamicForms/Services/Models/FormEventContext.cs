using System.Collections.Specialized;
using System.Web.Mvc;
using Orchard.DynamicForms.Elements;

namespace Orchard.DynamicForms.Services.Models {
    public class FormEventContext {
        public Form Form { get; set; }
        public NameValueCollection Values { get; set; }
        public IFormService FormService { get; set; }
        public IValueProvider ValueProvider { get; set; }
    }
}