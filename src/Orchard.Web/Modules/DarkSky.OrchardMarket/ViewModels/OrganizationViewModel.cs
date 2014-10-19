namespace DarkSky.OrchardMarket.ViewModels {
    public class OrganizationViewModel {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IndustryBranch { get; set; }
        public string LogoUrl { get; set; }
        public AddressViewModel Address { get; set; }
    }
}