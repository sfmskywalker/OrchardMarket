using DarkSky.Commerce.EventHandlers;
using DarkSky.Commerce.Models;
using DarkSky.OrchardMarket.Services;

namespace DarkSky.OrchardMarket.Handlers {
    public class InvoiceEventHandler : IInvoiceEventHandler {
        private readonly IRevenueService _revenueService;

        public InvoiceEventHandler(IRevenueService revenueService) {
            _revenueService = revenueService;
        }

        public void StatusChanged(InvoiceStatusChangedContext context) {
            if (context.NewStatus != InvoiceStatus.Paid) return;
            _revenueService.CreateRevenues(context.Invoice);
        }
    }
}