using System.Web;
using System.Web.Mvc;

namespace DarkSky.OrchardMarket.Helpers {
    public static class PercentageHtmlHelpers {
         public static IHtmlString Percentage(this HtmlHelper html, float percentage) {
             return MvcHtmlString.Create(string.Format("{0}%", percentage * 100));
         }
    }
}