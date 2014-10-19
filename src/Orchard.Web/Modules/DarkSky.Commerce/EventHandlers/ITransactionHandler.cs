using DarkSky.Commerce.Models;
using Orchard.Events;

namespace DarkSky.Commerce.EventHandlers {
    public interface ITransactionHandler : IEventHandler {
        void PaymentCreated(PaymentContext context);
    }

    public class PaymentContext {
        public Payment Payment { get; set; }
    }
}