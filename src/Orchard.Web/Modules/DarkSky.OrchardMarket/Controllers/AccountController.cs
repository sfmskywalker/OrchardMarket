using System.Web.Mvc;
using DarkSky.OrchardMarket.ViewModels;
using Orchard;
using Orchard.Themes;

namespace DarkSky.OrchardMarket.Controllers {
	public class AccountController : Controller {
		private readonly IOrchardServices _services;
		public AccountController(IOrchardServices services) {
			_services = services;
		}

		[Themed]
		public ActionResult SignUp() {
			return View(new SignUpViewModel());
		}

		[Themed, HttpPost]
		public ActionResult SignUp(SignUpViewModel signUp) {
			if (!ModelState.IsValid)
				return View(signUp);

			return RedirectToAction("Index");
		}

		[Themed]
		public ActionResult SignIn() {
			return View(new SignInViewModel());
		}

		[Themed, HttpPost]
		public ActionResult SignIn(SignInViewModel signIn) {
			if (!ModelState.IsValid)
				return View(signIn);

			return RedirectToAction("Index");
		}

		[Themed]
		public ActionResult Index() {
			return View();
		}
	}
}