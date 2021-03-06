using Orchard.ContentManagement;
using Orchard.Layouts.Framework.Elements;

namespace Orchard.Layouts.Framework.Display {
    public class ElementDisplayContext {
        public IContent Content { get; set; }
        public IElement Element { get; set; }
        public string DisplayType { get; set; }
        public dynamic ElementShape { get; set; }
        public IUpdateModel Updater { get; set; }
        public string RenderEventName { get; set; }
        public string RenderEventArgs { get; set; }
    }
}