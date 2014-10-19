using System.Collections.Generic;
using DarkSky.Commerce.Models;
using DarkSky.OrchardMarket.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Services;

namespace DarkSky.OrchardMarket.Services {
    public interface IRevenueService : IDependency {
        IList<Revenue> CreateRevenues(InvoicePart invoice);
        IEnumerable<Revenue> GetRevenues(int organizationId);
    }

    public class RevenueService : IRevenueService {
        private readonly IRepository<Revenue> _revenueRepository;
        private readonly IClock _clock;
        private readonly IWorkContextAccessor _wca;

        public RevenueService(IRepository<Revenue> revenueRepository, IClock clock, IWorkContextAccessor wca) {
            _revenueRepository = revenueRepository;
            _clock = clock;
            _wca = wca;
        }

        public IList<Revenue> CreateRevenues(InvoicePart invoice) {
            var context = _wca.GetContext();
            var percentage = context.CurrentSite.As<MarketSettingsPart>().PayoutPercentage;
            var revenues = new List<Revenue>();

            foreach (var detail in invoice.Details) {
                var salesTotal = detail.Total();
                var revenue = new Revenue {
                    CreatedUtc = _clock.UtcNow,
                    InvoiceDetailId = detail.Id,
                    OrganizationId = invoice.Shop.Id,
                    RevenuePercentage = percentage,
                    SalesTotal = salesTotal,
                    RevenueTotal = salesTotal * (decimal)percentage,
                };

                _revenueRepository.Create(revenue);
                revenues.Add(revenue);
            }

            return revenues;
        }

        public IEnumerable<Revenue> GetRevenues(int organizationId) {
            return _revenueRepository.Fetch(x => x.OrganizationId == organizationId);
        }
    }
}