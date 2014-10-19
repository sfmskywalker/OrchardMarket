using DarkSky.OrchardMarket.Models;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;
using Orchard.Services;
using Orchard.Users.Events;

namespace DarkSky.OrchardMarket.Handlers {
    public class UserEventHandler : IUserEventHandler {
        private readonly IClock _clock;
        private readonly IRepository<UserRolesPartRecord> _userRolesRepository;
        private readonly IRoleService _roleService;

        public UserEventHandler(IClock clock, IRoleService roleService, IRepository<UserRolesPartRecord> userRolesRepository) {
            _clock = clock;
            _roleService = roleService;
            _userRolesRepository = userRolesRepository;
        }

        public void LoggedIn(IUser user) {
            UpdateLastLogin(user);
        }

        public void Created(UserContext context) {
            var role = _roleService.GetRoleByName("Customer");
            if (role == null) {
                _roleService.CreateRole("Customer");
                role = _roleService.GetRoleByName("Customer");
            }
            _userRolesRepository.Create(new UserRolesPartRecord { UserId = context.User.Id, Role = role });
        }

        public void Creating(UserContext context) { }
        public void LoggedOut(IUser user) { }
        public void AccessDenied(IUser user) { }
        public void ChangedPassword(IUser user) { }
        public void SentChallengeEmail(IUser user) { }
        public void ConfirmedEmail(IUser user) { }
        public void Approved(IUser user) { }
        public void LoggingIn(string userNameOrEmail, string password) { }
        public void LogInFailed(string userNameOrEmail, string password) { }

        private void UpdateLastLogin(IUser user) {
            user.As<UserProfilePart>().LastLoginUtc = _clock.UtcNow;
        }
    }
}