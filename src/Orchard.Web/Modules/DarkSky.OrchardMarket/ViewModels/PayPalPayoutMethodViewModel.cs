using System.ComponentModel.DataAnnotations;

namespace DarkSky.OrchardMarket.ViewModels {
    public class PayPalPayoutMethodViewModel {
        [Required]
        public string Email { get; set; }
    }
}