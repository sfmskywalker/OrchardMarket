using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DarkSky.Commerce.Helpers;
using DarkSky.Commerce.Models;
using DarkSky.Commerce.Services;
using DarkSky.OrchardMarket.Helpers;
using DarkSky.OrchardMarket.Models;
using DarkSky.OrchardMarket.Services;
using DarkSky.OrchardMarket.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Notify;
using Orchard.Users.Models;
using Orchard.Users.Services;

namespace DarkSky.OrchardMarket.Controllers {
    [Authorize]
	public class OrganizationController : ControllerBase {
        private readonly IOrchardServices _services;
        private readonly IOrganizationService _organizationService;
        private readonly IUserService _userService;
        private readonly IMembershipService _membershipService;
        private readonly IOrderManager _orderManager;
        private readonly IPayoutService _payoutService;
        private readonly IRevenueService _revenueService;

        public OrganizationController(IOrchardServices services, IOrganizationService organizationService, IUserService userService, IMembershipService membershipService, IOrderManager orderManager, IPayoutService payoutService, IRevenueService revenueService) {
            _services = services;
            _organizationService = organizationService;
            _userService = userService;
            _membershipService = membershipService;
            _orderManager = orderManager;
            _payoutService = payoutService;
            _revenueService = revenueService;
        }

        [Themed]
        public ActionResult Index() {
            var user = _services.WorkContext.CurrentUser;
            var organizations = _organizationService.GetOrganizationsByUser(user.Id).ToList();
            var shape = _services.New.Organizations(
                Organizations: organizations.Select(x => _services.New.Organization(
                    OrganizationId: x.Id,
                    Name: x.Name,
                    Users: _organizationService.GetUsersInOrganization(x.Id).ToList(),
                    CurrentUserEntry: _organizationService.GetUserInOrganization(x.Id, user.Id))
                ).ToList());
            return View(shape);
        }

        [Themed]
        public ActionResult Create() {
            return View(new OrganizationViewModel());
        }

        [Themed, HttpPost]
        public ActionResult Create(OrganizationViewModel model, HttpPostedFileBase logo) {

            if (ModelState.IsValid) {
                if (!_organizationService.IsOrganizationNameAvailable(model.Name.TrimSafe())) {
                    ModelState.AddModelError("Name", T("That name is already in use by another organization. Please specify a different name").ToString());
                }
            }

            if(!ModelState.IsValid)
                return View(model);

            var organization = _organizationService.CreateOrganization(new CreateOrganizationArgs {
                Manager = _services.WorkContext.CurrentUser,
                Name = model.Name.TrimSafe(),
                Description = model.Description.TrimSafe(),
                IndustryBranch = model.IndustryBranch.TrimSafe(),
                Address = new CreateAddressArgs {
                    AddressLine1 = model.Address.AddressLine1,
                    AddressLine2 = model.Address.AddressLine2,
                    City = model.Address.City.TrimSafe(),
                    Zipcode = model.Address.Zipcode,
                    CountryId = model.Address.CountryId
                }
            });

            if (logo != null && logo.ContentLength > 0)
                _organizationService.UpdateLogo(logo, organization);

            _services.Notifier.Information(T("Your Organization has been created"));
            return RedirectToAction("Index");
        }

        [Themed]
        public ActionResult Edit(int id) {
            var organization = _organizationService.GetOrganization(id);
            var address = organization.Address;
            var viewModel = new OrganizationViewModel {
                Name = organization.Name,
                Description = organization.Description,
                IndustryBranch = organization.IndustryBranch,
                LogoUrl = _organizationService.GetLogoUrl(organization),
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
        public ActionResult Edit(OrganizationViewModel viewModel, HttpPostedFileBase logo) {
            if (ModelState.IsValid) {
                if (!_organizationService.IsOrganizationNameAvailable(viewModel.Name.TrimSafe(), viewModel.Id)) {
                    ModelState.AddModelError("Name", T("That name is already in use by another organization. Please specify a different name").ToString());
                }
            }

            if (!ModelState.IsValid)
                return View(viewModel);

            var organization = _organizationService.GetOrganization(viewModel.Id);
            var address = organization.Address;
            var file = Request.Files["logo"];

            organization.Name = viewModel.Name.TrimSafe();
            organization.Description = viewModel.Description.TrimSafe();
            address.AddressLine1 = viewModel.Address.AddressLine1.TrimSafe();
            address.AddressLine2 = viewModel.Address.AddressLine2.TrimSafe();
            address.Zipcode = viewModel.Address.Zipcode.TrimSafe();
            address.City = viewModel.Address.City.TrimSafe();
            address.Record.CountryId = viewModel.Address.CountryId;

            if (file != null && file.ContentLength > 0)
                _organizationService.UpdateLogo(file, organization);

            _services.Notifier.Information(T("Your organization has been updated"));
            return RedirectToAction("Details", "Organization", new { id = organization.Id });
        }

        [Themed]
        public ActionResult Details(int id) {
            var organization = _organizationService.GetOrganization(id);
            var users = _organizationService.GetUsersInOrganization(id).ToList();
            var sales = _orderManager.GetOrdersByShop(organization.Id).ToList();
            var revenues = _revenueService.GetRevenues(id).ToList();
            var lastPaidOutRevenue = revenues.Where(x => x.Paid).OrderBy(x => x.PaidUtc).LastOrDefault();
            var revenueRate = _services.WorkContext.CurrentSite.As<MarketSettingsPart>().PayoutPercentage;
            var activePayoutOption = _payoutService.GetActivePayoutOption(organization.Id);
            var model = _services.New.ViewModel(
                Organization: organization,
                LogoUrl: _organizationService.GetLogoUrl(organization),
                Users: users,
                Sales: sales,
                TotalRevenues: revenues.Sum(x => x.RevenueTotal),
                PaidOutRevenues: revenues.Where(x => x.Paid).Sum(x => x.RevenueTotal),
                LastPaidOut: lastPaidOutRevenue != null ? lastPaidOutRevenue.PaidUtc : default(DateTime?),
                RevenueRate: revenueRate,
                ActivePayoutOption: activePayoutOption);
            return View(model);
        }

        [Themed]
        public ActionResult Users(int id) {
            var organization = _organizationService.GetOrganization(id);
            var users = _organizationService.GetUsersInOrganization(id).ToList();
            var shape = _services.New.ViewModel(Organization: organization, Users: users);
            return View(shape);
        }

        [Themed]
        public ActionResult AddUser(int id) {
            var organization = _organizationService.GetOrganization(id);
            var shape = _services.New.ViewModel(Organization: organization, Model: new AddUserToOrganizationViewModel { Id = id });
            return View(shape);
        }

        [Themed, HttpPost]
        public ActionResult AddUser(AddUserToOrganizationViewModel model) {
            var user = default(IUser);
            var organization = _organizationService.GetOrganization(model.Id);

            if (ModelState.IsValid) {
                user = _membershipService.GetUser(model.UserName);
                if(user == null)
                    ModelState.AddModelError("UserName", T("The specified user could not be found. Please check the user name and try again.").ToString());
                else if(_organizationService.GetUserInOrganization(model.Id, user.Id) != null)
                    ModelState.AddModelError("UserName", T("The specified user is already part of the organization.").ToString());
                else if(string.IsNullOrWhiteSpace(user.Email) && _services.WorkContext.CurrentUser.Id != user.Id)
                    ModelState.AddModelError("UserName", T("The specified user does not have a valid email address specified, so an invitation could not be created.").ToString());
            }

            if (!ModelState.IsValid) {
                var shape = _services.New.ViewModel(Organization: organization, Model: model);
                return View(shape);
            }

            if (_services.WorkContext.CurrentUser.Id == user.Id) {
                _organizationService.AddUserToOrganization(organization, user);
                _services.Notifier.Information(T("You have been added to {0}.", organization.Name));
            }
            else {
                _organizationService.InviteUser(organization, user);
                _services.Notifier.Information(T("An invitation has been sent to {0}.", model.UserName));
            }
            
            return RedirectToAction("Details", new { id = model.Id });
        }

        public ActionResult RemoveUser(int organizationId, int userId) {
            var userInOrganization = _organizationService.GetUserInOrganization(organizationId, userId);

            if (userInOrganization == null)
                return HttpNotFound();

            var user = _services.ContentManager.Get<IUser>(userId);
            var organization = _organizationService.GetOrganization(organizationId);

            _organizationService.RemoveUserFromOrganization(userInOrganization);
            _services.Notifier.Information(T("User {0} has been removed from {1}", user.UserName, organization.Name));

            return RedirectToAction("Details", new {id = organizationId});
        }

        [Themed, AllowAnonymous]
        public ActionResult AcceptInvitation(string token) {
            var invitation = _organizationService.GetInvitationByToken(token);

            if (invitation == null)
                return HttpNotFound();

            if (invitation.Status == InvitationStatus.Accepted)
                return View("InvitationAlreadyAccepted");

            _organizationService.AcceptInvitation(invitation);
            var user = _services.ContentManager.Get<UserProfilePart>(invitation.UserId);
            var organization = _organizationService.GetOrganization(invitation.OrganizationId);
            var shape = _services.New.ViewModel(User: user, Organization: organization);
            return View(shape);
        }

        [Themed]
        public ActionResult Join() {
            return View(new JoinOrganizationViewModel());
        }

        [Themed, HttpPost]
        public ActionResult Join(JoinOrganizationViewModel model) {
            OrganizationPart organization = null;

            if (ModelState.IsValid) {
                organization = _organizationService.GetOrganizationByName(model.Name.TrimSafe());
                if (organization == null) {
                    ModelState.AddModelError("Name", T("No organization with that name exists.").ToString());
                }

                if(_organizationService.GetUserInOrganization(organization.Id, _services.WorkContext.CurrentUser.Id) != null)
                    ModelState.AddModelError("Name", T("You are already a member of {0}", organization.Name).ToString());
            }

            if (!ModelState.IsValid)
                return View(model);

            _organizationService.CreateJoinRequest(organization, _services.WorkContext.CurrentUser);
            _services.Notifier.Information(T("A join request has been sent to {0}", organization.Name));
            return RedirectToAction("Index");
        }

        [Themed, AllowAnonymous]
        public ActionResult AcceptRequest(string token) {
            var request = _organizationService.GetJoinRequestByToken(token);

            if (request == null)
                return HttpNotFound();

            if (request.Status == JoinRequestStatus.Accepted)
                return View("RequestAlreadyAccepted");

            _organizationService.AcceptJoinRequest(request);
            var user = _services.ContentManager.Get<UserProfilePart>(request.UserId);
            var organization = _organizationService.GetOrganization(request.OrganizationId);
            var shape = _services.New.ViewModel(User: user, Organization: organization);
            return View(shape);
        }

        public ActionResult UserNames(string term) {
            var matches = _services.ContentManager
                            .Query()
                            .ForPart<UserPart>()
                            .Where<UserPartRecord>(x => x.UserName.Contains(term))
                            .List()
                            .Select(x => x.UserName)
                            .ToList();

            return Json(matches, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrganizationNames(string term) {
            var matches = _services.ContentManager
                            .HqlQuery<OrganizationPart>()
                            .Where(x => x.ContentPartRecord<OrganizationPartRecord>(), x => x.Like("Name", term, HqlMatchMode.Anywhere))
                            .List()
                            .Select(x => x.Name)
                            .ToList();

            return Json(matches, JsonRequestBehavior.AllowGet);
        }
    }
}