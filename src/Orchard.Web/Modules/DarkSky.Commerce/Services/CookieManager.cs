using System;
using System.ComponentModel;
using System.Web;
using System.Web.Routing;
using Orchard;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Services {
    public interface ICookieManager : IDependency {
        HttpCookie GetCookie(string key);
        T GetValue<T>(string key);
        string GetValue(string key);
        void SetValue<T>(string key, T value);
        void SetValue(string key, string value);
    }

    [OrchardFeature("DarkSky.Commerce")]
    public class CookieManager : ICookieManager {
        private readonly RequestContext _requestContext;

        public CookieManager(RequestContext requestContext) {
            _requestContext = requestContext;
        }

        public HttpCookie GetCookie(string key) {
            return _requestContext.HttpContext.Request.Cookies[key];
        }

        public T GetValue<T>(string key) {
            var text = GetValue(key);
            return !string.IsNullOrWhiteSpace(text) ? (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(text) : default(T);
        }

        public string GetValue(string key) {
            var cookie = GetCookie(key);
            return cookie == null ? null : cookie.Value;
        }

        public void SetValue<T>(string key, T value) {
            SetValue(key, TypeDescriptor.GetConverter(value).ConvertToString(value));
        }

        public void SetValue(string key, string value) {
            _requestContext.HttpContext.Response.SetCookie(new HttpCookie(key, value));
        }
    }
}