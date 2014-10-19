using System;
using System.Web.Routing;
using Orchard;
using Orchard.Tokens;

namespace Contrib.Navigation.Services {
    public interface IRouteValuesProcessor : IDependency {
        RouteValueDictionary Parse(string routeValuesText);
    }
    public class RouteValuesProcessor : IRouteValuesProcessor {
        private readonly ITokenizer _tokenizer;
        public RouteValuesProcessor(ITokenizer tokenizer) {
            _tokenizer = tokenizer;
        }

        public RouteValueDictionary Parse(string routeValuesText) {
            var routeValues = new RouteValueDictionary();

            if (!string.IsNullOrWhiteSpace(routeValuesText)) {
                var items = routeValuesText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in items) {
                    var pair = item.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                    if (pair.Length == 2) {
                        var key = pair[0].Trim();
                        var value = pair[1].Trim();

                        value = _tokenizer.Replace(value, null, new ReplaceOptions { Encoding = ReplaceOptions.NoEncode });
                        routeValues[key] = value;
                    }
                }
            }

            return routeValues;
        }
    }

    
}