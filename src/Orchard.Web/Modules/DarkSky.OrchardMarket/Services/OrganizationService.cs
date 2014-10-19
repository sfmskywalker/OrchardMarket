using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using DarkSky.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.FileSystems.Media;
using DarkSky.OrchardMarket.Models;
using Orchard.Security;
using Orchard.Services;
using Orchard.UI.Notify;

namespace DarkSky.OrchardMarket.Services {
    public interface IOrganizationService : IDependency {
        void UpdateLogo(HttpPostedFileBase file, OrganizationPart organization);
        string GetLogoUrl(OrganizationPart organization);
        IEnumerable<OrganizationPart> GetOrganizationsByUser(int userId);
        IEnumerable<UserInOrganization> GetUsersInOrganization(int organizationId);
        OrganizationPart GetOrganization(int id);
        OrganizationPart GetOrganizationByName(string name);
        IEnumerable<OrganizationPart> GetOrganizationsByManager(int userId);
        OrganizationPart CreateOrganization(CreateOrganizationArgs args);
        UsersInOrganization AddUserToOrganization(OrganizationPart organization, IUser user);
        UsersInOrganization GetUserInOrganization(int organizationId, int userId);
        Invitation InviteUser(OrganizationPart organization, IUser user);
        Invitation GetInvitationByToken(string token);
        void AcceptInvitation(Invitation invitation);
        bool IsOrganizationNameAvailable(string name, int? organizationId = null);
        JoinRequest CreateJoinRequest(OrganizationPart organization, IUser user);
        JoinRequest GetJoinRequestByToken(string token);
        void AcceptJoinRequest(JoinRequest request);
        IEnumerable<OrganizationPart> GetOrganizationsByCurrentUser();
        void RemoveUserFromOrganization(UsersInOrganization entry);
    }

    public class OrganizationService : Component, IOrganizationService {
        private readonly IStorageProvider _storageProvider;
        private readonly IRepository<UsersInOrganization> _usersInOrganizationRepository;
        private readonly IContentManager _contentManager;
        private readonly IRepository<Country> _countryRepository;
        private readonly IClock _clock;
        private readonly IRepository<Invitation> _invitationRepository;
        private readonly IEmailService _emailService;
        private readonly IRepository<JoinRequest> _joinRequestRepository;
        private readonly IOrchardServices _services;
        private dynamic New { get; set; }

        public OrganizationService(
            IStorageProvider storageProvider, 
            IRepository<UsersInOrganization> usersInOrganizationRepository, 
            IRepository<Country> countryRepository, 
            IClock clock, 
            IRepository<Invitation> invitationRepository, 
            IEmailService emailService, 
            IRepository<JoinRequest> joinRequestRepository, 
            IOrchardServices services) {
            _storageProvider = storageProvider;
            _usersInOrganizationRepository = usersInOrganizationRepository;
            _contentManager = services.ContentManager;
            _countryRepository = countryRepository;
            _clock = clock;
            _invitationRepository = invitationRepository;
            _emailService = emailService;
            New = services.New;
            _joinRequestRepository = joinRequestRepository;
            _services = services;
        }


        public void UpdateLogo(HttpPostedFileBase file, OrganizationPart organization) {

            var folder = string.Format("Organizations\\{0}", organization.Id);
            var fileName = string.Format("logo{0}", Path.GetExtension(file.FileName));
            var path = _storageProvider.Combine(folder, fileName);
            _storageProvider.TryCreateFolder(folder);

            try {
                _storageProvider.DeleteFile(path);
            }
// ReSharper disable EmptyGeneralCatchClause
            catch { }
// ReSharper restore EmptyGeneralCatchClause

            _storageProvider.SaveStream(path, file.InputStream);
            organization.LogoUrl = path;
        }

        public string GetLogoUrl(OrganizationPart organization) {
            return !string.IsNullOrWhiteSpace(organization.LogoUrl) ? string.Format(_storageProvider.GetPublicUrl(organization.LogoUrl)) : null;
        }

        public IEnumerable<UserInOrganization> GetUsersInOrganization(int organizationId) {
            var usersInOrganizations = _usersInOrganizationRepository.Fetch(x => x.OrganizationId == organizationId).ToList();
            var userIds = usersInOrganizations.Select(x => x.UserId).ToList();
            var users = _contentManager.GetMany<IUser>(userIds, VersionOptions.Published, QueryHints.Empty.ExpandParts<UserProfilePart>()).ToList();

            return from entry in usersInOrganizations
                   let user = users.First(x => x.Id == entry.UserId)
                   select new UserInOrganization {
                        CreatedUtc = entry.CreatedUtc,
                        OrganizationId = organizationId,
                        User = user
                    };
        }

        public IEnumerable<OrganizationPart> GetOrganizationsByUser(int userId) {
            var usersInOrganizations = _usersInOrganizationRepository.Fetch(x => x.UserId == userId);
            var organizationIds = usersInOrganizations.Select(x => x.OrganizationId).ToList();
            return _contentManager.GetMany<OrganizationPart>(organizationIds, VersionOptions.Published, QueryHints.Empty.ExpandParts<ShopPart>()).ToList();
        }

        public IEnumerable<OrganizationPart> GetOrganizationsByCurrentUser() {
            return GetOrganizationsByUser(_services.WorkContext.CurrentUser.Id);
        }

        public void RemoveUserFromOrganization(UsersInOrganization entry) {
            _usersInOrganizationRepository.Delete(entry);
        }

        public IEnumerable<OrganizationPart> GetOrganizationsByManager(int userId) {
            return _contentManager.Query<OrganizationPart>().Join<CommonPartRecord>().Where(x => x.OwnerId == userId).WithQueryHints(QueryHints.Empty.ExpandParts<ShopPart>()).List().ToList();
        }

        public OrganizationPart CreateOrganization(CreateOrganizationArgs args) {
            var organization = _contentManager.New<OrganizationPart>("Organization");
            var commonPart = organization.As<CommonPart>();

            organization.Name = args.Name;
            organization.Description = args.Description;
            organization.IndustryBranch = args.IndustryBranch;
            commonPart.Owner = args.Manager;

            _contentManager.Create(organization);

            var address = organization.Address;
            address.AddressLine1 = args.Address.AddressLine1;
            address.AddressLine2 = args.Address.AddressLine2;
            address.City = args.Address.City;
            address.Zipcode = args.Address.Zipcode;
            address.Country = args.Address.CountryId != null ? _countryRepository.Get(args.Address.CountryId.Value) : default(Country);

            AddUserToOrganization(organization, args.Manager);
            return organization;
        }

        public UsersInOrganization AddUserToOrganization(OrganizationPart organization, IUser user) {
            var userInOrganization = GetUserInOrganization(organization.Id, user.Id);

            if (userInOrganization != null)
                throw new InvalidOperationException("The specified user is already part of the specified organization");

            var entry = new UsersInOrganization {
                OrganizationId = organization.Id,
                UserId = user.Id,
                CreatedUtc = _clock.UtcNow
            };

            _usersInOrganizationRepository.Create(entry);

            var message = new MailMessage { Subject = T("You have been added to organization {0}", organization.Name).Text };
            var profile = user.As<UserProfilePart>();
            var currentUser = _services.WorkContext.CurrentUser;

            if (currentUser == null || currentUser.Id != user.Id) {
                if (!string.IsNullOrWhiteSpace(user.Email)) {
                    message.To.Add(new MailAddress(user.Email, profile.DisplayName));
                    _emailService.Send(message, MessageTemplates.JoinConfirmation, new {
                        Organization = organization,
                        User = user,
                        profile.DisplayName,
                        OrganizationUrl = _emailService.Action("Display", "Item", new { id = organization.Id, area = "Content" })
                    });
                }
                else {
                    _services.Notifier.Warning(T("The user {0} has been added to {1}, but that user doesn't have an email address specified so we could not notify him.", user.UserName, organization.Name));
                }
            }
            return entry;
        }

        public UsersInOrganization GetUserInOrganization(int organizationId, int userId) {
            return _usersInOrganizationRepository.Get(x => x.OrganizationId == organizationId && x.UserId == userId);
        }

        public Invitation InviteUser(OrganizationPart organization, IUser user) {
            var userInOrganization = GetUserInOrganization(organization.Id, user.Id);

            if(userInOrganization != null)
                throw new InvalidOperationException("The specified user is already part of the specified organization");

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new InvalidOperationException("The specified user does not have a valid email address specified, so an invitation could not be created.");
            
            var invitation = new Invitation {
                OrganizationId = organization.Id,
                UserId = user.Id,
                CreatedUtc = _clock.UtcNow,
                Token = Guid.NewGuid().ToString(),
                Status = InvitationStatus.Pending
            };

            var profile = user.As<UserProfilePart>();

            _invitationRepository.Create(invitation);

            var message = new MailMessage { Subject = T("{0} invites you to join their organization", organization.Name).Text };
            message.To.Add(new MailAddress(user.Email, profile.DisplayName));
            _emailService.Send(message,  MessageTemplates.Invitation, new {
                Organization = organization,
                User = user,
                Invitation = invitation,
                profile.DisplayName,
                AcceptUrl = _emailService.Action("AcceptInvitation", "Organization", new { token = invitation.Token, area = "DarkSky.OrchardMarket" })
            });

            return invitation;
        }

        public Invitation GetInvitationByToken(string token) {
            return _invitationRepository.Get(x => x.Token == token);
        }

        public void AcceptInvitation(Invitation invitation) {
            if(invitation.Status != InvitationStatus.Pending)
                throw new InvalidOperationException("The specified invitation has already been accepted");

            var organization = GetOrganization(invitation.OrganizationId);
            var user = _contentManager.Get<IUser>(invitation.UserId);
            AddUserToOrganization(organization, user);
            invitation.Status = InvitationStatus.Accepted;
            invitation.AcceptedUtc = _clock.UtcNow;
        }

        public OrganizationPart GetOrganizationByName(string name) {
            return _contentManager
                        .HqlQuery<OrganizationPart>()
                        .Where(x => x.ContentPartRecord<OrganizationPartRecord>(), x => x.Like("Name", name, HqlMatchMode.Anywhere))
                        .List()
                        .FirstOrDefault();
        }

        public bool IsOrganizationNameAvailable(string name, int? organizationId = null) {
            var organization = GetOrganizationByName(name);
            return organization == null || organization.Id == organizationId;
        }

        public OrganizationPart GetOrganization(int id) {
            return _contentManager.Get<OrganizationPart>(id);
        }

        public JoinRequest CreateJoinRequest(OrganizationPart organization, IUser user) {
            var request = new JoinRequest {
                OrganizationId = organization.Id,
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                CreatedUtc = _clock.UtcNow,
                Status = JoinRequestStatus.Pending
            };

            _joinRequestRepository.Create(request);

            var profile = user.As<UserProfilePart>();
            var managerProfile = organization.Manager.As<UserProfilePart>();
            var message = new MailMessage { Subject = T("{0} requests to join your organization {1}", profile.DisplayName, organization.Name).Text };
            message.To.Add(new MailAddress(user.Email, profile.DisplayName));
            _emailService.Send(message, MessageTemplates.JoinRequest, new {
                Organization = organization,
                ManagerDisplayName = managerProfile.DisplayName,
                User = user,
                UserDisplayName = profile.DisplayName,
                UserProfileUrl = _emailService.Action("Display", "Item", new { id = user.Id, area = "Contents" }),
                Request = request,
                profile.DisplayName,
                AcceptUrl = _emailService.Action("AcceptRequest", "Organization", new { token = request.Token, area = "DarkSky.OrchardMarket" })
            });

            return request;
        }

        public JoinRequest GetJoinRequestByToken(string token) {
            return _joinRequestRepository.Get(x => x.Token == token);
        }

        public void AcceptJoinRequest(JoinRequest request) {
            var organization = GetOrganization(request.OrganizationId);
            var user = _contentManager.Get<IUser>(request.UserId);
            AddUserToOrganization(organization, user);
            request.Status = JoinRequestStatus.Accepted;
            request.AcceptedUtc = _clock.UtcNow;
        }
    }

    public class UserInOrganization {
        public int OrganizationId { get; set; }
        public IUser User { get; set; }
        public DateTime CreatedUtc { get; set; }
    }

    public class CreateOrganizationArgs {
        public IUser Manager { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IndustryBranch { get; set; }
        public string LogoUrl { get; set; }
        public CreateAddressArgs Address { get; set; }
    }

    public class CreateAddressArgs {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public int? CountryId { get; set; }
    }
}