using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace DarkSky.OrchardMarket {
    public class Permissions : IPermissionProvider {
        public static readonly Permission ManagePackages = new Permission { Description = "Manage packages for others", Name = "ManagePackages" };
        public static readonly Permission ManageOrganizationPackages = new Permission { Description = "Manage your organization's packages", Name = "ManageOrganizationPackages", ImpliedBy = new[] { ManagePackages } };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            yield return ManagePackages;
            yield return ManageOrganizationPackages;
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ManagePackages}
                },
                new PermissionStereotype {
                    Name = "BusinessOwner",
                    Permissions = new[] {ManageOrganizationPackages}
                },
                new PermissionStereotype {
                    Name = "Developer",
                    Permissions = new[] {ManageOrganizationPackages}
                },
            };
        }

    }
}


