namespace DarkSky.Commerce.Models {
    public class ShoppingCartItem {
        public virtual int Id { get; set; }
        public virtual ShoppingCartRecord ShoppingCartRecord { get; set; }
        public virtual int ProductId { get; set; }
        public virtual int Quantity { get; set; }
    }
}