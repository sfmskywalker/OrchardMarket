using Orchard.ContentManagement;

namespace DarkSky.Commerce.Models {
    public interface IProductAspect : IContent {
        IShopAspect Shop { get; set; }
        decimal UnitPrice { get; set; }
        string Currency { get; set; }
        string ImageUrl { get; set; }
        VatPart Vat { get; set; }
        int Sales { get; set; }
    }
}