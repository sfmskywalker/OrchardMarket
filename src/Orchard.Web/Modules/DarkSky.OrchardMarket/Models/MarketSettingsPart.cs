using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace DarkSky.OrchardMarket.Models {
    public class MarketSettingsPart : ContentPart<MarketSettingsPartRecord> {
        public float PayoutPercentage {
            get { return Record.PayoutPercentage; }
            set { Record.PayoutPercentage = value; }
        }
    }

    public class MarketSettingsPartRecord : ContentPartRecord {
        public virtual float PayoutPercentage { get; set; }
    }
}