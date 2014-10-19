using Orchard;
using Orchard.DisplayManagement.Implementation;

namespace DarkSky.OrchardMarket.Shapes {
    public class UserNavigation : ShapeDisplayEvents {
        private readonly WorkContext _workContext;

        public UserNavigation(IWorkContextAccessor workContext) {
            _workContext = workContext.GetContext();
        }

        public override void Displaying(ShapeDisplayingContext context) {
            if (!context.ShapeMetadata.Type.Contains("UserNavigation_")) return;

            var currentUser = _workContext.CurrentUser;
            context.Shape.CurrentUser = currentUser;
            context.Shape.IsAuthenticated = _workContext.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}