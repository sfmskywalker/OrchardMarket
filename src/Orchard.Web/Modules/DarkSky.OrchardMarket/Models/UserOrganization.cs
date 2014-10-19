using System;

namespace DarkSky.OrchardMarket.Models {
    public class UsersInOrganization {
        public virtual int Id { get; set; }
        public virtual int OrganizationId { get; set; }
        public virtual int UserId { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
    }
}