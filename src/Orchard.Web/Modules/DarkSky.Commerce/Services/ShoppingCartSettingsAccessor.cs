using Orchard;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Services {
    public interface IShoppingCartSettingsAccessor : IDependency {
        int MaxQuantityPerItem { get; }
    }

    [OrchardFeature("DarkSky.Commerce.ShoppingCart")]
    public class ShoppingCartSettingsAccessor : IShoppingCartSettingsAccessor {
        public int MaxQuantityPerItem { get { return 99; } }
    }
}