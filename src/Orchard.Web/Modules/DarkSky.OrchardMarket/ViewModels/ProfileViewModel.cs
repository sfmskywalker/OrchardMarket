namespace DarkSky.OrchardMarket.ViewModels {
    public class ProfileViewModel {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AddressViewModel Address { get; set; }
        public string ReturnUrl { get; set; }
    }
}