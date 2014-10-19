using System;
using System.Collections.ObjectModel;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement.Utilities;
using Orchard.Security;

namespace DarkSky.OrchardMarket.Models {
    public class UserProfilePart : ContentPart<UserProfilePartRecord> {

        internal readonly LazyField<ReadOnlyCollection<OrganizationPart>> OrganizationsField = new LazyField<ReadOnlyCollection<OrganizationPart>>();
        internal readonly LazyField<AddressPart> AddressField = new LazyField<AddressPart>();

        public ReadOnlyCollection<OrganizationPart> Organizations {
            get { return OrganizationsField.Value; }
            set { OrganizationsField.Value = value; }
        }

        public string FirstName {
            get { return Record.FirstName; }
            set { Record.FirstName = value; }
        }

        public string LastName {
            get { return Record.LastName; }
            set { Record.LastName = value; }
        }

		public string AvatarUrl {
			get { return Record.AvatarUrl; }
            set { Record.AvatarUrl = value; }
		}

        public DateTime? LastLoginUtc {
            get { return Record.LastLoginUtc; }
            set { Record.LastLoginUtc = value; }
        }

        public AddressPart Address {
            get { return AddressField.Value; }
            set { AddressField.Value = value; }
        }

        public string FullName() {
            return string.Format("{0} {1}", FirstName, LastName);
        }

        public string DisplayName {
            get { return string.IsNullOrWhiteSpace(FirstName) ? this.As<IUser>().UserName : FirstName; }
        }
	}

    public class UserProfilePartRecord : ContentPartRecord {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string AvatarUrl { get; set; }
        public virtual int? AddressId { get; set; }
        public virtual DateTime? LastLoginUtc { get; set; }
    }
}