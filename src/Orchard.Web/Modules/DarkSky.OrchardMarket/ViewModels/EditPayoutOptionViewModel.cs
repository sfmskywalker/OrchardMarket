using System.ComponentModel.DataAnnotations;

namespace DarkSky.OrchardMarket.ViewModels {
    public class EditPayoutOptionViewModel {
        public int Id { get; set; }

        [UIHint("YesNo")]
        public bool IsActive { get; set; }
    }
}