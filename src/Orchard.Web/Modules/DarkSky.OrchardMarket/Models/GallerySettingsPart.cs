using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace DarkSky.OrchardMarket.Models {
    public class GallerySettingsPart : ContentPart<GallerySettingsPartRecord> {
        public string PackagesPath {
            get { return Record.PackagesPath; }
            set { Record.PackagesPath = value; }
        }

        /// <summary>
        /// The Query to use when browsing Modules
        /// </summary>
        public int? ModulesQueryId {
            get { return Record.ModulesQueryId; }
            set { Record.ModulesQueryId = value; }
        }

        /// <summary>
        /// The Query to use when browsing Themes
        /// </summary>
        public int? ThemesQueryId {
            get { return Record.ThemesQueryId; }
            set { Record.ThemesQueryId = value; }
        }
    }

    public class GallerySettingsPartRecord : ContentPartRecord {
        public virtual string PackagesPath { get; set; }
        public virtual int? ModulesQueryId { get; set; }
        public virtual int? ThemesQueryId { get; set; }
    }
}