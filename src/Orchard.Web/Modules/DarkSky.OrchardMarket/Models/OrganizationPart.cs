using System.Collections.ObjectModel;
using DarkSky.Commerce.Models;
using DarkSky.OrchardMarket.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement.Utilities;
using Orchard.Security;

namespace DarkSky.OrchardMarket.Models {
    public class OrganizationPart : ContentPart<OrganizationPartRecord>, ITitleAspect, IShopAspect {

        internal readonly LazyField<AddressPart> AddressField = new LazyField<AddressPart>();
        internal readonly LazyField<ReadOnlyCollection<UserInOrganization>> UsersField = new LazyField<ReadOnlyCollection<UserInOrganization>>();

        public string Name {
            get { return Record.Name; }
            set { Record.Name = value; }
        }

        public string IndustryBranch {
            get { return Record.IndustryBranch; }
            set { Record.IndustryBranch = value; }
        }

        public string Description {
            get { return Record.Description; }
            set { Record.Description = value; }
        }

		public string LogoUrl {
			get { return Record.LogoUrl; }
            set { Record.LogoUrl = value; }
		}

        public decimal Balance {
            get { return Record.Balance; }
            set { Record.Balance = value; }
        }

        public string Title {
            get { return Name; }
        }

        public IUser Manager {
            get { return this.As<ICommonPart>().Owner; }
            set { this.As<ICommonPart>().Owner = value; }
        }

        public AddressPart Address {
            get { return AddressField.Value; }
            set { AddressField.Value = value; }
        }

        public ReadOnlyCollection<UserInOrganization> Users {
            get { return UsersField.Value; }
        }
    }

    public class OrganizationPartRecord : ContentPartRecord {
        public virtual string Name { get; set; }
        public virtual string IndustryBranch { get; set; }
        public virtual string Description { get; set; }
        public virtual string LogoUrl { get; set; }
        public virtual int? AddressId { get; set; }
        public virtual decimal Balance { get; set; }
    }
}