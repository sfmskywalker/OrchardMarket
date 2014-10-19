using System;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Email.Models;
using Orchard.Logging;

namespace DarkSky.OrchardMarket.Services {
    public interface IEmailService : IDependency {
        void Send(MailMessage message, string templatePath = null, dynamic model = null, IContent content = null);

        /// <summary>
        /// Generates a fully qualified url based on the specified action, controller and routevalues.
        /// </summary>
        string Action(string actionName, string controllerName, object routeValues);
    }

    public class EmailService : Component, IEmailService {
        private readonly IOrchardServices _orchardServices;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly RequestContext _requestContext;

        public EmailService(IOrchardServices orchardServices, IEmailTemplateService emailTemplateService, RequestContext requestContext) {
            _orchardServices = orchardServices;
            _emailTemplateService = emailTemplateService;
            _requestContext = requestContext;
        }

        public void Send(MailMessage message, string templatePath = null, dynamic model = null, IContent content = null) {
            var smtpSettings = _orchardServices.WorkContext.CurrentSite.As<SmtpSettingsPart>();

            // Can't process emails if the Smtp settings have not yet been set
            if (smtpSettings == null || !smtpSettings.IsValid()) {
                return;
            }

            if (string.IsNullOrWhiteSpace(message.Body) && ! string.IsNullOrWhiteSpace(templatePath)) {
                var template = _emailTemplateService.GetTemplate(templatePath);
                var body = _emailTemplateService.ProcessTemplate(template, model, content);

                message.Body = body;
                message.IsBodyHtml = true;
            }

            if (message.From == null) {
                message.From = new MailAddress(smtpSettings.Address);
            }

            using (var smtpClient = new SmtpClient()) {
                smtpClient.UseDefaultCredentials = !smtpSettings.RequireCredentials;
                if (!smtpClient.UseDefaultCredentials && !String.IsNullOrWhiteSpace(smtpSettings.UserName)) {
                    smtpClient.Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password);
                }

                if (smtpSettings.Host != null)
                    smtpClient.Host = smtpSettings.Host;

                smtpClient.Port = smtpSettings.Port;
                smtpClient.EnableSsl = smtpSettings.EnableSsl;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                try {
                    smtpClient.Send(message);
                    Logger.Debug("Message sent to {0}", message.To[0].Address);
                }
                catch (Exception e) {
                    Logger.Error(e, "An unexpected error while sending a message to {0}", message.To[0].Address);
                }
            }
        }

        public string Action(string actionName, string controllerName, object routeValues) {
            var urlHelper = new UrlHelper(_requestContext);
            var url = _requestContext.HttpContext.Request.Url;
            var scheme = url.Scheme;
            var host = url.Host;
            return urlHelper.Action(actionName, controllerName, new RouteValueDictionary(routeValues), scheme, host);
        }
    }
}