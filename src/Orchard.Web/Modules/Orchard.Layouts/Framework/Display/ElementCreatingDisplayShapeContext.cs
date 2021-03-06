﻿using Orchard.ContentManagement;
using Orchard.Layouts.Framework.Elements;

namespace Orchard.Layouts.Framework.Display {
    public class ElementCreatingDisplayShapeContext {
        public IContent Content { get; set; }
        public IElement Element { get; set; }
        public string DisplayType { get; set; }
    }
}