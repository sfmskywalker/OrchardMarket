using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Contrib.Navigation.Models {
    public class ActionLinkPart : ContentPart<ActionLinkPartRecord> {
        public string ActionName {
            get { return Record.ActionName; }
            set { Record.ActionName = value; }
        }

        public string ControllerName {
            get { return Record.ControllerName; }
            set { Record.ControllerName = value; }
        }

        public string AreaName {
            get { return Record.AreaName; }
            set { Record.AreaName = value; }
        }

        public string RouteValues {
            get { return Record.RouteValues; }
            set { Record.RouteValues = value; }
        }
    }

    public class ActionLinkPartRecord : ContentPartRecord {
        public virtual string ActionName { get; set; }
        public virtual string ControllerName { get; set; }
        public virtual string AreaName { get; set; }
        public virtual string RouteValues { get; set; }
    }
}