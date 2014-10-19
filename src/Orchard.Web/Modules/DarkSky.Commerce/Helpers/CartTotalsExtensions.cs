using System.Collections.Generic;
using System.Linq;
using DarkSky.Commerce.Models;

namespace DarkSky.Commerce.Helpers {
    public static class CartTotalsExtensions {
         public static Totals Sum(this IEnumerable<Totals> cartTotals) {
             return cartTotals.Aggregate(new Totals(), (current, item) => current + item);
         }
    }
}