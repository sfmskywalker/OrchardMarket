using System.ComponentModel.DataAnnotations;

namespace DarkSky.OrchardMarket.ViewModels {
	public class SignUpViewModel {
		[Required]
		public string UserName { get; set; }

		[Required, DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[Required, Compare("Email"), DataType(DataType.EmailAddress)]
		public string RepeatEmail { get; set; }

		[Required, DataType(DataType.Password)]
		public string Password { get; set; }

		[Required, Compare("Password"), DataType(DataType.Password)]
		public string RepeatPassword { get; set; }
	}
}