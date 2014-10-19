using DarkSky.Commerce.Models;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.Security;
using Orchard.Security.Permissions;

namespace DarkSky.Commerce.Security {
    [UsedImplicitly]
    public class OrderAuthorizationEventHandler : IAuthorizationServiceEventHandler {
        
        public void Checking(CheckAccessContext context) { }
        public void Complete(CheckAccessContext context) { }

        public void Adjust(CheckAccessContext context) {
            if (!context.Granted && context.Content.Is<OrderPart>()) {
                if (OwnerVariationExists(context.Permission) &&
                    HasOwnership(context.User, context.Content)) {
                    context.Adjusted = true;
                    context.Permission = GetOwnerVariation(context.Permission);
                }
            }
        }

        private bool HasOwnership(IUser user, IContent content) {
            var orderPart = content.As<OrderPart>();
            return orderPart.User != null && orderPart.User.Id == user.Id;
        }

        private static bool OwnerVariationExists(Permission permission) {
            return GetOwnerVariation(permission) != null;
        }

        private static Permission GetOwnerVariation(Permission permission) {
            if (permission.Name == Permissions.ManageOrders.Name)
                return Permissions.ManageOwnOrders;
            return null;
        }
    }
}