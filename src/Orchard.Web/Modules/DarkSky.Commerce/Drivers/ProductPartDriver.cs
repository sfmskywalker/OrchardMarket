﻿using System.Xml;
using DarkSky.Commerce.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace DarkSky.Commerce.Drivers {
	public class ProductPartDriver : ContentPartDriver<ProductPart> {

		protected override DriverResult Editor(ProductPart part, dynamic shapeHelper) {
			return ContentShape("Parts_Product_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Product", Model: part, Prefix: Prefix));
		}

		protected override DriverResult Editor(ProductPart part, IUpdateModel updater, dynamic shapeHelper) {
			updater.TryUpdateModel(part, Prefix, null, null);
			return Editor(part, shapeHelper);
		}

		protected override void Importing(ProductPart part, ImportContentContext context) {
			context.ImportAttribute(part.PartDefinition.Name, "Price", x => part.Price = XmlConvert.ToDecimal(x));
		}

		protected override void Exporting(ProductPart part, ExportContentContext context) {
			context.Element(part.PartDefinition.Name).SetAttributeValue("Price", part.Price);
		}
	}
}