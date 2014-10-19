namespace DarkSky.OrchardMarket.Models {
    public class PayoutOption {
        public virtual int Id { get; set; }
        public virtual int OrganizationId { get; set; }
        public virtual string PayoutMethodName { get; set; }
        public virtual string Data { get; set; }
        public virtual bool IsActive { get; set; }
    }
}