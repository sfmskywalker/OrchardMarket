using System.Xml;
using DarkSky.Commerce.Models;
using DarkSky.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace DarkSky.Commerce.Drivers {
	public class ExchangeRatePartDriver : ContentPartDriver<ExchangeRatePart> {
		private readonly IOrchardServices _services;

		public ExchangeRatePartDriver(IOrchardServices services) {
			_services = services;
		}

		protected override string Prefix {
			get { return "ExchangeRate"; }
		}

		protected override DriverResult Editor(ExchangeRatePart part, dynamic shapeHelper) {
			return Editor(part, null, shapeHelper);
		}

		protected override DriverResult Editor(ExchangeRatePart part, IUpdateModel updater, dynamic shapeHelper) {
			var viewModel = new ExchangeRateViewModel {
				Currency = part.Currency,
				Rate = part.Rate
			};

			if (updater != null)
				if (updater.TryUpdateModel(viewModel, Prefix, null, null)) {
					part.Rate = viewModel.Rate ?? 0f;
					part.Currency = viewModel.Currency;
				}

			return ContentShape("Parts_ExchangeRate_Edit", () => {
				var settings = _services.WorkContext.CurrentSite.As<CommerceSettingsPart>();
				viewModel.BaseCurrency = settings.Currency;
				return shapeHelper.EditorTemplate(TemplateName: "Parts/ExchangeRate", Model: viewModel, Prefix: Prefix);
			});
		}

		protected override void Importing(ExchangeRatePart part, ImportContentContext context) {
			context.ImportAttribute(part.PartDefinition.Name, "Rate", x => part.Rate = XmlConvert.ToSingle(x));
		}

		protected override void Exporting(ExchangeRatePart part, ExportContentContext context) {
			context.Element(part.PartDefinition.Name).SetAttributeValue("Rate", part.Rate);
		}
	}
}