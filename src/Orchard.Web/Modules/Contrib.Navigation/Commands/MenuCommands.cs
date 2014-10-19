using Contrib.Navigation.Models;
using Orchard.Commands;
using Orchard.ContentManagement;
using Orchard.Core.Navigation.Models;
using Orchard.Core.Navigation.Services;

namespace Contrib.Navigation.Commands {
    public class MenuCommands : DefaultOrchardCommandHandler {
        private readonly IContentManager _contentManager;
        private readonly IMenuService _menuService;

        public MenuCommands(IContentManager contentManager, IMenuService menuService) {
            _contentManager = contentManager;
            _menuService = menuService;
        }

        [OrchardSwitch]
        public string MenuPosition { get; set; }

        [OrchardSwitch]
        public string MenuText { get; set; }

        [OrchardSwitch]
        public string ActionName { get; set; }

        [OrchardSwitch]
        public string ControllerName { get; set; }

        [OrchardSwitch]
        public string AreaName { get; set; }

        [OrchardSwitch]
        public string RouteValues { get; set; }

        [OrchardSwitch]
        public string MenuName { get; set; }

        [CommandName("menu actionlink create")]
        [CommandHelp("menu actionlink create /MenuPosition:<position> /MenuText:<text> /ActionName:<actionname> /ControllerName:<controllername> /AreaName:<areaname> /RouteValues:<routevalues> /MenuName:<name>\r\n\t" + "Creates a new action link menu item")]
        [OrchardSwitches("MenuPosition,MenuText,ActionName,ControllerName,AreaName,RouteValues,MenuName")]
        public void Create() {
            var menu = _menuService.GetMenu(MenuName);

            if(menu == null) {
                Context.Output.WriteLine(T("Menu not found.").Text);
                return;
            }

            var menuItem = _contentManager.Create("ActionLink");
            var menuPart = menuItem.As<MenuPart>();
            var actionLinkPart = menuItem.As<ActionLinkPart>();
            menuPart.MenuPosition = MenuPosition;
            menuPart.MenuText = T(MenuText).ToString();
            menuPart.Menu = menu.ContentItem;
            actionLinkPart.ActionName = ActionName;
            actionLinkPart.ControllerName = ControllerName;
            actionLinkPart.AreaName = AreaName;
            actionLinkPart.RouteValues = RouteValues;

            Context.Output.WriteLine(T("ActionLink menu item created successfully.").Text);
        }
    }
}