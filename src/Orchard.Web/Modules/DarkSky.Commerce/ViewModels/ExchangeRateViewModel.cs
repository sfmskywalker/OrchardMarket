using System.ComponentModel.DataAnnotations;

namespace DarkSky.Commerce.ViewModels {
	public class ExchangeRateViewModel {
		[Required]
		public string Currency { get; set; }

		[Required]
		public float? Rate { get; set; }
		public string BaseCurrency { get; set; }
	}
}