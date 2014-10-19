using DarkSky.Commerce.Models;
using DarkSky.Commerce.Services;
using Orchard.Events;

namespace DarkSky.Commerce.EventHandlers {
    public interface IShoppingCartHandler : IEventHandler {
        void Display(CartItemDisplayContext context);
        void Adding(AddingShoppingCartItemContext context);
        void Added(AddedShoppingCartItemContext context);
        void Removing(RemovingShoppingCartItemContext context);
        void Removed(RemovedShoppingCartItemContext context);
    }

    public class CartItemDisplayContext {
        public ProductQuantity Product { get; set; }
        public dynamic Shape { get; set; }
    }

    public class AddingShoppingCartItemContext {
        public int ProductId { get; set; }
        public IProductAspect Product { get; set; }
        public int Quantity { get; set; }
        public bool IsQuantityValid { get; set; }
        public IShoppingCartSettingsAccessor Settings { get; set; }
    }

    public class AddedShoppingCartItemContext {
        public IProductAspect Product { get; set; }
        public ShoppingCartItem Item { get; set; }
    }

    public class RemovingShoppingCartItemContext {
        public ShoppingCartItem Item { get; set; }
        public IProductAspect Product { get; set; }
    }

    public class RemovedShoppingCartItemContext {
        public ShoppingCartItem Item { get; set; }
        public IProductAspect Product { get; set; }
    }
}