using System;

namespace DarkSky.OrchardMarket.Models {
    public class JoinRequest {
        public virtual int Id { get; set; }
        public virtual int OrganizationId { get; set; }
        public virtual int UserId { get; set; }
        public virtual string Token { get; set; }
        public virtual JoinRequestStatus Status { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
        public virtual DateTime? AcceptedUtc { get; set; }
    }

    public enum JoinRequestStatus {
        Pending,
        Accepted
    }
}