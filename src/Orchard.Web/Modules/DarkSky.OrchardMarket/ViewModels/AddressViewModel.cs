using System.ComponentModel.DataAnnotations;

namespace DarkSky.OrchardMarket.ViewModels {
    public class AddressViewModel {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }

        [DataType("Country")]
        public int? CountryId { get; set; }
    }
}