using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DarkSky.Commerce.Helpers;
using DarkSky.Commerce.Models;
using DarkSky.Commerce.Services;
using DarkSky.OrchardMarket.Helpers;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Notify;
using Orchard.Users.Models;
using Orchard.Users.Services;
using DarkSky.OrchardMarket.Models;
using DarkSky.OrchardMarket.Services;
using DarkSky.OrchardMarket.ViewModels;

namespace DarkSky.OrchardMarket.Controllers {
    [Authorize]
    public class AccountController : ControllerBase {
        private readonly IOrchardServices _services;
        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly IUserProfileManager _userProfileManager;
        private readonly IOrderManager _orderManager;
        private readonly IPayoutService _payoutService;
        private readonly IUserProfileManager _profileManager;

        protected dynamic New { get; set; }

        int MinPasswordLength {
            get { return _membershipService.GetSettings().MinRequiredPasswordLength; }
        }

        public AccountController(IOrchardServices services, IUserService userService, IMembershipService membershipService, IUserProfileManager userProfileManager, IOrderManager orderManager, IPayoutService payoutService, IUserProfileManager profileManager) {
            _services = services;
            _userService = userService;
            _membershipService = membershipService;
            _userProfileManager = userProfileManager;
            _orderManager = orderManager;
            _payoutService = payoutService;
            _profileManager = profileManager;
            New = _services.New;
        }

        [Themed]
        public ActionResult Index() {
            var currentUser = _services.WorkContext.CurrentUser;
            var profile = currentUser.As<UserProfilePart>();
            var purchases = _orderManager.GetOrdersByUser(currentUser.Id).ToList();
            var address = profile.Address;
            //var shop = profile.Organization;
            //var sales = _orderManager.GetOrdersByShop(shop.Id).ToList();
            //var balance = sales.Select(x => x.As<FundedOrderPart>()).Where(x => x.Funded).Select(x => x.As<OrderPart>().Totals()).Sum();
            //var activePayoutOption = _payoutService.GetActivePayoutOption(shop.Id);

            var viewModel = New.ViewModel(
                Account: New.Account(
                    UserName: currentUser.UserName,
                    Email: currentUser.Email,
                    LogoUrl: _userProfileManager.GetAvatarUrl(profile),
                    LastLogin: profile.LastLoginUtc),
                Profile: _services.New.Profile(
                    FirstName: profile.FirstName,
                    LastName: profile.LastName,
                    AvatarUrl: _profileManager.GetAvatarUrl(profile),
                    AddressLine1: address.AddressLine1,
                    AddressLine2: address.AddressLine2,
                    Zipcode: address.Zipcode,
                    City: address.City,
                    Country: address.Country != null ? address.Country.Name : ""),
                Statistics: New.Statistics(
                    Purchases: purchases.Count)
            );
            return View((object)viewModel);
        }

        [Themed]
        public ActionResult Edit() {
            var currentUser = _services.WorkContext.CurrentUser;
            var viewModel = new AccountViewModel {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                LogoUrl = _userProfileManager.GetAvatarUrl(currentUser.As<UserProfilePart>())
            };
            return View(viewModel);
        }

        [Themed, HttpPost]
        public ActionResult Edit(AccountViewModel viewModel, HttpPostedFileBase logo) {

            if(ValidateAccountSettings(viewModel)) {
                var user = (UserPart)_services.WorkContext.CurrentUser;
                var userProfile = _services.WorkContext.CurrentUser.As<UserProfilePart>();
                var email = viewModel.Email.TrimSafe();

                if (user.UserName != viewModel.UserName.TrimSafe()) {
                    user.UserName = viewModel.UserName.Trim();
                    user.NormalizedUserName = user.UserName.ToLower();
                }

                if (user.Email != email)
                    user.Email = email;

                if (!string.IsNullOrWhiteSpace(viewModel.Password))
                    _membershipService.SetPassword(user, viewModel.Password.Trim());

                if (logo != null && logo.ContentLength > 0)    
                    _userProfileManager.UpdateLogo(logo, userProfile);

                _services.Notifier.Information(T("Your account settings have been updated"));
                return RedirectToAction("Index");
            }


            return View(viewModel);
        }

        private bool ValidateAccountSettings(AccountViewModel viewModel) {
            var currentUser = _services.WorkContext.CurrentUser;
            var userName = viewModel.UserName.TrimSafe();
            var email = viewModel.Email.TrimSafe();
            var password = viewModel.Password;
            var confirmPassword = viewModel.ConfirmPassword;

            if (string.IsNullOrWhiteSpace(userName))
                ModelState.AddModelError("UserName", T("You must specify a username."));

            if (string.IsNullOrWhiteSpace(email))
                ModelState.AddModelError("Email", T("You must specify an email address."));
            else if (!Regex.IsMatch(email, UserPart.EmailPattern, RegexOptions.IgnoreCase))
                ModelState.AddModelError("email", T("You must specify a valid email address."));

            if (!string.Equals(currentUser.UserName, userName, StringComparison.InvariantCultureIgnoreCase)) {
                if (!_userService.VerifyUserUnicity(currentUser.Id, userName, email))
                    ModelState.AddModelError("UserName", "The specified username and/or email address is already taken");
            }

            if (!string.IsNullOrEmpty(password) || !string.IsNullOrEmpty(confirmPassword)) {
                if (!string.Equals(password, confirmPassword, StringComparison.InvariantCulture))
                    ModelState.AddModelError("ConfirmPassword", T("The two passwords do not match").Text);
                else if (password == null || password.Length < MinPasswordLength)
                    ModelState.AddModelError("Password", T("You must specify a password of {0} or more characters", MinPasswordLength).Text);
            }

            return ModelState.IsValid;
        }
    }
}