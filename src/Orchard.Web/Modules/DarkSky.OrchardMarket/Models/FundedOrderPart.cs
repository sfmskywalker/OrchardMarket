using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace DarkSky.OrchardMarket.Models {
    public class FundedOrderPart : ContentPart<FundedOrderPartRecord> {
        public bool Funded {
            get { return Record.Funded; }
            set { Record.Funded = value; }
        }

        public DateTime? FundedUtc {
            get { return Record.FundedUtc; }
            set { Record.FundedUtc = value; }
        }
    }

    public class FundedOrderPartRecord : ContentPartRecord {
        public virtual bool Funded { get; set; }
        public virtual DateTime? FundedUtc { get; set; }
    }
}