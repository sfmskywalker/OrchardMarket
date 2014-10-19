using System.Collections.Generic;
using System.Linq;
using Contrib.Navigation.Models;
using Orchard.ContentManagement;
using Orchard.UI.Navigation;

namespace Contrib.Navigation.Handlers {
    public class NavigationFilter : INavigationFilter {
        public IEnumerable<MenuItem> Filter(IEnumerable<MenuItem> menuItems) {
            foreach (var menuItem in menuItems.Where(x => x.Content.Is<ActionLinkPart>())) {
                menuItem.Href = "#";
            }
            return menuItems;
        }
    }
}