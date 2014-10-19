using DarkSky.Commerce.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Drivers {
    [OrchardFeature("DarkSky.Commerce.Orders")]
    public class OrderPartDriver : ContentPartDriver<OrderPart> {
    }
}