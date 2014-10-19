using System;
using System.Web.Mvc;
using DarkSky.Commerce.Models;

namespace DarkSky.Commerce.Helpers {
    public static class UrlExtensions {
        public static string AddCart(this UrlHelper url, int productId, int quantity = 1) {
            return url.Action("Add", "ShoppingCart", new {area = "DarkSky.Commerce", productId, quantity});
        }

        public static string AddCart(this UrlHelper url, IProductAspect product, int quantity = 1) {
            return url.AddCart(product.Id, quantity);
        }
    }
}