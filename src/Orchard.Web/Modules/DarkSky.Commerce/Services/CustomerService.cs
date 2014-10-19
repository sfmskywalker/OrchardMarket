using DarkSky.Commerce.EventHandlers;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Security;

namespace DarkSky.Commerce.Services {
    public interface ICustomerService : IDependency {
        CustomerInfo GetCustomerInfo(IUser user);
    }

    [OrchardFeature("DarkSky.Commerce.Customers")]
    public class CustomerService : ICustomerService {
        private readonly ICustomerHandler _customerHandler;

        public CustomerService(ICustomerHandler customerHandler) {
            _customerHandler = customerHandler;
        }

        public CustomerInfo GetCustomerInfo(IUser user) {
            var context = new CustomerInfoContext {
                CustomerInfo = new CustomerInfo {
                    Address = new AddressInfo(),
                    BillingAddress = new AddressInfo(),
                    EmailAddress = user.Email
                },
                User = user
            };
            _customerHandler.GetCustomerInfo(context);
            return context.CustomerInfo;
        }
    }

    public class CustomerInfoContext {
        public IUser User { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
    }

    public class CustomerInfo {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public AddressInfo Address { get; set; }
        public AddressInfo BillingAddress { get; set; }

        public string FullName {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        public CustomerInfo() {
            Address = new AddressInfo();
            BillingAddress = new AddressInfo();
        }
    }

    public class AddressInfo {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string StateOrProvince { get; set; }
        public CountryInfo Country { get; set; }

        public AddressInfo() {
            Country = new CountryInfo();
        }
    }

    public class CountryInfo {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}