using System;

namespace DarkSky.OrchardMarket.Models {
    public class Revenue {
        public virtual int Id { get; set; }
        public virtual int OrganizationId { get; set; }
        public virtual int InvoiceDetailId { get; set; }
        public virtual float RevenuePercentage { get; set; }
        public virtual decimal SalesTotal { get; set; }
        public virtual decimal RevenueTotal { get; set; }
        public virtual bool Paid { get; set; }
        public virtual int? PayoutOptionId { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
        public virtual DateTime? PaidUtc { get; set; }
    }
}