using System.ComponentModel.DataAnnotations;

namespace DarkSky.OrchardMarket.ViewModels {
    public class CreatePackageViewModel {
        [UIHint("OrganizationPicker")]
        [Required]
        public int? OrganizationId { get; set; }
    }
}