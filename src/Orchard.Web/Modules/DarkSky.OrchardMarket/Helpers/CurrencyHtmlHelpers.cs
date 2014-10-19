using System.Web;
using System.Web.Mvc;

namespace DarkSky.OrchardMarket.Helpers {
    public static class CurrencyHtmlHelpers {
         public static IHtmlString Price(this HtmlHelper html, decimal amount) {
             return MvcHtmlString.Create(amount.ToString("c"));
         }
    }
}