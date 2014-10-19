using DarkSky.Commerce.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Drivers {
    [OrchardFeature("DarkSky.Commerce")]
    public class ShopPartDriver : ContentPartDriver<ShopPart> {
    }
}