using System.ComponentModel.DataAnnotations;

namespace DarkSky.OrchardMarket.ViewModels {
	public class SignInViewModel {
		[Required]
		public string UserNameOrEmail { get; set; }

		[Required]
		public string Password { get; set; }

		public bool CreatePersistentCookie { get; set; }
	}
}