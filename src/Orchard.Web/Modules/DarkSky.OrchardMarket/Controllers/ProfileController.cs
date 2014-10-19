using System.Web.Mvc;
using DarkSky.OrchardMarket.Helpers;
using DarkSky.OrchardMarket.Models;
using DarkSky.OrchardMarket.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Themes;
using Orchard.UI.Notify;

namespace DarkSky.OrchardMarket.Controllers {
    [Authorize]
	public class ProfileController : ControllerBase {
        private readonly IOrchardServices _services;

        public ProfileController(IOrchardServices services) {
            _services = services;
        }

        [Themed]
        public ActionResult Edit(string returnUrl = null) {
            var user = _services.WorkContext.CurrentUser;
            var profile = user.As<UserProfilePart>();
            var address = profile.Address;
            var viewModel = new ProfileViewModel {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                ReturnUrl = AdjustReturnUrl(returnUrl),
                Address = new AddressViewModel {
                    AddressLine1 = address.AddressLine1,
                    AddressLine2 = address.AddressLine2,
                    Zipcode = address.Zipcode,
                    City = address.City,
                    CountryId = address.Record.CountryId
                }
            };
            return View(viewModel);
        }

        [Themed, HttpPost]
        public ActionResult Edit(ProfileViewModel viewModel) {
            if (!ModelState.IsValid)
                return View(viewModel);

            var user = _services.WorkContext.CurrentUser;
            var profile = user.As<UserProfilePart>();
            var address = profile.Address;

            profile.FirstName = viewModel.FirstName.TrimSafe();
            profile.LastName = viewModel.LastName.TrimSafe();
            address.AddressLine1 = viewModel.Address.AddressLine1.TrimSafe();
            address.AddressLine2 = viewModel.Address.AddressLine2.TrimSafe();
            address.Zipcode = viewModel.Address.Zipcode.TrimSafe();
            address.City = viewModel.Address.City.TrimSafe();
            address.Record.CountryId = viewModel.Address.CountryId;

            _services.Notifier.Information(T("Your profile has been updated"));
            return Redirect(AdjustReturnUrl(viewModel.ReturnUrl));
        }

        private string AdjustReturnUrl(string returnUrl) {
            return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("Index", "Account");
        }
	}
}