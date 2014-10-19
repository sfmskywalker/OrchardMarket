namespace DarkSky.Commerce.Models {
    public class ProductQuantity {
        public IProductAspect Product { get; set; }
        public int Quantity { get; set; }

        public decimal SubTotal {
            get { return Product.UnitPrice * Quantity; }
        }
    }
}