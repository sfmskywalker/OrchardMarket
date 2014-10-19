using System.ComponentModel.DataAnnotations;

namespace DarkSky.Commerce.ViewModels {
    public class CheckoutViewModel {
        [Required(ErrorMessage = "Please select a payment method")]
        [DataType("PaymentProvider")]
        public string CheckoutProvider { get; set; }
    }
}