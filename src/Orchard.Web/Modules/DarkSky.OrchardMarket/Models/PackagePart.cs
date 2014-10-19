using System.Collections.Generic;
using System.Collections.ObjectModel;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement.Utilities;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Security;
using Orchard.Taxonomies.Models;

namespace DarkSky.OrchardMarket.Models {
    public class PackagePart : ContentPart<PackagePartRecord> {
        internal LazyField<IList<PackageReleasePart>> ReleasesField = new LazyField<IList<PackageReleasePart>>();
        internal LazyField<PackageReleasePart> LatestReleaseField = new LazyField<PackageReleasePart>();
        internal LazyField<string> LogoUrlField = new LazyField<string>();
        internal LazyField<ReadOnlyCollection<TermPart>> TagsField = new LazyField<ReadOnlyCollection<TermPart>>();
        internal LazyField<ReadOnlyCollection<TermPart>> CategoriesField = new LazyField<ReadOnlyCollection<TermPart>>();

        public IList<PackageReleasePart> Releases {
            get { return ReleasesField.Value; }
        }

        public PackageReleasePart LatestRelease {
            get { return LatestReleaseField.Value; }
        }

        public string LogoUrl {
            get { return LogoUrlField.Value; }
        }

        public OrganizationPart Organization {
            get { return this.As<ICommonPart>().Container.As<OrganizationPart>(); }
            set { this.As<ICommonPart>().Container = value; }
        }

        public IUser User {
            get { return this.As<ICommonPart>().Owner; }
            set { this.As<ICommonPart>().Owner = value; }
        }

        public ExtensionType ExtensionType {
            get { return Record.ExtensionType; }
            set { Record.ExtensionType = value; }
        }

        public int Downloads {
            get { return Record.Downloads; }
            set { Record.Downloads = value; }
        }

        public string Name {
            get { return this.As<TitlePart>().Title; }
            set { this.As<TitlePart>().Title = value; }
        }

        public string DisplayName {
            get { return string.Format("{0} - {1}", Name, LatestRelease.Version); }
        }

        public string Description {
            get { return this.As<BodyPart>().Text; }
            set { this.As<BodyPart>().Text = value; }
        }

        public string SearchIndex {
            get { return Record.SearchIndex; }
            set { Record.SearchIndex = value; }
        }

        public ReadOnlyCollection<TermPart> Tags {
            get { return TagsField.Value; }
        }

        public ReadOnlyCollection<TermPart> Categories {
            get { return CategoriesField.Value; }
        }
    }

    public class PackagePartRecord : ContentPartRecord {
        public virtual ExtensionType ExtensionType { get; set; }
        public virtual int Downloads { get; set; }
        public virtual string SearchIndex { get; set; }
    }

    public enum ExtensionType {
        Module,
        Theme
    }
}