using System.Xml;
using DarkSky.Commerce.Models;
using DarkSky.Commerce.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace DarkSky.Commerce.Drivers {
	public class VatRatePartDriver : ContentPartDriver<VatRatePart> {
		protected override string Prefix {
			get { return "VatRate"; }
		}

		protected override DriverResult Editor(VatRatePart part, dynamic shapeHelper) {
			return Editor(part, null, shapeHelper);
		}

		protected override DriverResult Editor(VatRatePart part, IUpdateModel updater, dynamic shapeHelper) {
			if (updater != null) updater.TryUpdateModel(part, Prefix, null, null);
			return ContentShape("Parts_VatRate_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/VatRate", Model: part, Prefix: Prefix));
		}

		protected override void Importing(VatRatePart part, ImportContentContext context) {
			context.ImportAttribute(part.PartDefinition.Name, "Rate", x => part.Rate = XmlConvert.ToSingle(x));
		}

		protected override void Exporting(VatRatePart part, ExportContentContext context) {
			context.Element(part.PartDefinition.Name).SetAttributeValue("Rate", part.Rate);
		}
	}
}