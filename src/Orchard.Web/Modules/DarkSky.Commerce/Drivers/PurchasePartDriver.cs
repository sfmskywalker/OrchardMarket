using DarkSky.Commerce.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Drivers {
    [OrchardFeature("DarkSky.Commerce.Products")]
    public class PurchasePartDriver : ContentPartDriver<PurchasePart> {
    }
}