using System.ComponentModel.DataAnnotations;

namespace DarkSky.OrchardMarket.ViewModels {
    public class AddPayoutOptionViewModel {
        public int OrganizationId { get; set; }

        [UIHint("PayoutMethodPicker")]
        [Required]
        public string PayoutMethodName { get; set; }
    }
}