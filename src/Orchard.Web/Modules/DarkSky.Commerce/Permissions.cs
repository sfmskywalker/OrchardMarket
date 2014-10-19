using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace DarkSky.Commerce {
    public class Permissions : IPermissionProvider {
        public static readonly Permission ManageOrders = new Permission { Description = "Manage orders", Name = "ManageOrders" };
        public static readonly Permission ManageOwnOrders = new Permission { Description = "Manage own orders", Name = "ManageOwnOrders", ImpliedBy = new[] { ManageOrders } };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            yield return ManageOrders;
            yield return ManageOwnOrders;
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ManageOrders}
                },
                new PermissionStereotype {
                    Name = "Customer",
                    Permissions = new[] {ManageOwnOrders}
                }
            };
        }

    }
}


