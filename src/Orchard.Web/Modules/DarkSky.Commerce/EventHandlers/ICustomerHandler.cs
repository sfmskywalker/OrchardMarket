using DarkSky.Commerce.Services;
using Orchard.Events;

namespace DarkSky.Commerce.EventHandlers {
    public interface ICustomerHandler : IEventHandler {
        void GetCustomerInfo(CustomerInfoContext context);
    }

}