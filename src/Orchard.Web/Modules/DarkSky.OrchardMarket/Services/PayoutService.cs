using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkSky.OrchardMarket.Models;
using Orchard;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Data;

namespace DarkSky.OrchardMarket.Services {
    public interface IPayoutService : IDependency {
        IEnumerable<PayoutOption> GetPayoutOptions(int companyId);
        PayoutOption CreatePayoutOption(int companyId, string payoutMethodName);
        PayoutOption GetPayoutOption(int id);
        IPayoutMethod GetPayoutMethod(string payoutMethodName);
        SettingsDictionary DeserializePayoutData(string data);
        string SerializePayoutData(SettingsDictionary dictionary);
        void SetActivePayoutOption(PayoutOption payoutOption);
        void Delete(PayoutOption payoutOption);
        PayoutOption GetActivePayoutOption(int companyId);
    }

    public class PayoutService : IPayoutService {
        private readonly IRepository<PayoutOption> _payoutOptionRepository;
        private readonly IEnumerable<IPayoutMethod> _payoutMethods;

        public PayoutService(IRepository<PayoutOption> payoutOptionRepository, IEnumerable<IPayoutMethod> payoutMethods) {
            _payoutOptionRepository = payoutOptionRepository;
            _payoutMethods = payoutMethods;
        }

        public IEnumerable<PayoutOption> GetPayoutOptions(int companyId) {
            return _payoutOptionRepository.Fetch(x => x.OrganizationId == companyId);
        }

        public PayoutOption CreatePayoutOption(int companyId, string payoutMethodName) {
            var payoutOption = new PayoutOption {
                OrganizationId = companyId,
                PayoutMethodName = payoutMethodName,
                IsActive = false
            };

            _payoutOptionRepository.Create(payoutOption);
            return payoutOption;
        }

        public PayoutOption GetPayoutOption(int id) {
            return _payoutOptionRepository.Get(id);
        }

        public IPayoutMethod GetPayoutMethod(string payoutMethodName) {
            return _payoutMethods.FirstOrDefault(x => x.Name == payoutMethodName);
        }

        public SettingsDictionary DeserializePayoutData(string data) {
            var dictionary = new SettingsDictionary();
            if (!string.IsNullOrWhiteSpace(data)) {
                try {
                    var items = data.Split(new[] {"&&&"}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in items) {
                        var values = item.Split(new[] {"==="}, StringSplitOptions.RemoveEmptyEntries);
                        var key = values[0];
                        var value = values[1];
                        dictionary[key] = value;
                    }
                }
                catch {}
            }
            return dictionary;
        }

        public string SerializePayoutData(SettingsDictionary dictionary) {
            var sb = new StringBuilder();

            foreach (var item in dictionary) {
                sb.Append(string.Format("{0}==={1}&&&", item.Key, item.Value));
            }

            return sb.ToString();
        }

        public void SetActivePayoutOption(PayoutOption payoutOption) {
            var otherPayoutOptions = GetPayoutOptions(payoutOption.OrganizationId).Where(x => x.Id != payoutOption.Id).ToList();

            foreach (var option in otherPayoutOptions)
                option.IsActive = false;

            payoutOption.IsActive = true;
        }

        public PayoutOption GetActivePayoutOption(int companyId) {
            return GetPayoutOptions(companyId).FirstOrDefault(x => x.IsActive);
        }

        public void Delete(PayoutOption payoutOption) {
            _payoutOptionRepository.Delete(payoutOption);
        }

        
    }
}