using DarkSky.Commerce.Models;
using Orchard.Events;

namespace DarkSky.Commerce.EventHandlers {
    public interface IInvoiceEventHandler : IEventHandler {
        void StatusChanged(InvoiceStatusChangedContext context);
    }

    public class InvoiceStatusChangedContext {
        public InvoicePart Invoice { get; set; }
        public InvoiceStatus PreviousStatus { get; set; }
        public InvoiceStatus NewStatus { get; set; }
    }
}