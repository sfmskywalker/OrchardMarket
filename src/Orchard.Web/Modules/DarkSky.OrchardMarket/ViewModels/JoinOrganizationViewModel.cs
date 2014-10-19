using System.ComponentModel.DataAnnotations;

namespace DarkSky.OrchardMarket.ViewModels {
    public class JoinOrganizationViewModel {
        [Required]
        public string Name { get; set; }
    }
}