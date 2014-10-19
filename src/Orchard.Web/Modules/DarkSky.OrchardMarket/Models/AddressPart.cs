using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement.Utilities;

namespace DarkSky.OrchardMarket.Models {
    public class AddressPart : ContentPart<AddressPartRecord> {
        public string AddressLine1 {
            get { return Record.AddressLine1; }
            set { Record.AddressLine1 = value; }
        }

        public string AddressLine2 {
            get { return Record.AddressLine2; }
            set { Record.AddressLine2 = value; }
        }

        public string Zipcode {
            get { return Record.Zipcode; }
            set { Record.Zipcode = value; }
        }

        public string City {
            get { return Record.City; }
            set { Record.City = value; }
        }

        private readonly LazyField<Country> _country = new LazyField<Country>();
        internal LazyField<Country> CountryField { get { return _country; } } 

        public Country Country {
            get { return _country.Value; }
            set { _country.Value = value; }
        }
    }

    public class AddressPartRecord : ContentPartRecord {
        public virtual string AddressLine1 { get; set; }
        public virtual string AddressLine2 { get; set; }
        public virtual string Zipcode { get; set; }
        public virtual string City { get; set; }
        public virtual int? CountryId { get; set; }
    }
}