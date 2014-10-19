using System;
using System.IO;
using Antlr4.StringTemplate;
using Orchard;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.FileSystems.VirtualPath;
using Orchard.Logging;

namespace DarkSky.OrchardMarket.Services {
    public interface IEmailTemplateService : IDependency {
        string GetTemplate(string relativePath);
        string ProcessTemplate(string template, dynamic model, IContent content = null);
    }

    public class EmailTemplateService : Component, IEmailTemplateService {
        private readonly ICacheManager _cacheManager;
        private readonly IVirtualPathMonitor _virtualPathMonitor;
        private readonly IOrchardServices _services;

        public EmailTemplateService(ICacheManager cacheManager, IVirtualPathMonitor virtualPathMonitor, IOrchardServices services) {
            _cacheManager = cacheManager;
            _virtualPathMonitor = virtualPathMonitor;
            _services = services;
        }

        public string GetTemplate(string relativePath) {
            var key = string.Format("EmailTemplate_{0}", relativePath);
            return _cacheManager.Get(key, context => {
                var path = _services.WorkContext.HttpContext.Server.MapPath(relativePath);
                context.Monitor.Invoke(_virtualPathMonitor.WhenPathChanges(relativePath));
                return File.ReadAllText(path);
            });
        }

        public string ProcessTemplate(string templateText, dynamic model, IContent content = null) {
            var text = string.Empty;
            if (!string.IsNullOrEmpty(templateText)) {
                try {
                    var template = new Template(templateText, '$', '$');

                    template.Add("CurrentSite", _services.WorkContext.CurrentSite);
                    template.Add("Model", model);
                    text = template.Render();
                }
                catch (Exception ex) {
                    text = ex.ToString();
                    Logger.Error(ex, "Error while sending email");
                }
            }
            return text;
        }
    }
}