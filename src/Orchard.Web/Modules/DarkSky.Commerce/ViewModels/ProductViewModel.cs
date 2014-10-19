using System.ComponentModel.DataAnnotations;

namespace DarkSky.Commerce.ViewModels {
    public class ProductViewModel {
        public decimal UnitPrice { get; set; }
        public string Currency { get; set; }
        public string ImageUrl { get; set; }

        [Required]
        [UIHint("VatPicker")]
        public int? VatId { get; set; }
    }
}