using System.Linq;
using DarkSky.Commerce.Models;
using DarkSky.OrchardMarket.Models;
using DarkSky.OrchardMarket.Services;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.Security;
using Orchard.Security.Permissions;

namespace DarkSky.OrchardMarket.Security {
    [UsedImplicitly]
    public class PackageAuthorizationEventHandler : IAuthorizationServiceEventHandler {
        private readonly IOrganizationService _organizationService;
        public PackageAuthorizationEventHandler(IOrganizationService organizationService) {
            _organizationService = organizationService;
        }

        public void Checking(CheckAccessContext context) { }
        public void Complete(CheckAccessContext context) { }

        public void Adjust(CheckAccessContext context) {
            if (!context.Granted && context.Content.Is<PackagePart>()) {
                if (OwnerVariationExists(context.Permission) &&
                    HasOwnership(context.User, context.Content)) {
                    context.Adjusted = true;
                    context.Permission = GetOwnerVariation(context.Permission);
                }
            }
        }

        private bool HasOwnership(IUser user, IContent content) {
            var userProfile = user.As<UserProfilePart>();
            var package = content.As<PackagePart>();

            if (userProfile == null || package == null)
                return false;

            return IsBusinessOwner(userProfile, package);
        }

        private bool IsBusinessOwner(UserProfilePart userProfile, PackagePart package) {
            var organizationIds = _organizationService.GetOrganizationsByManager(userProfile.Id).Select(x => x.Id);
            var product = package.As<IProductAspect>();

            return organizationIds.Contains(product.Shop.Id);
        }

        private static bool OwnerVariationExists(Permission permission) {
            return GetOwnerVariation(permission) != null;
        }

        private static Permission GetOwnerVariation(Permission permission) {
            if (permission.Name == Permissions.ManagePackages.Name)
                return Permissions.ManageOrganizationPackages;
            return null;
        }
    }
}