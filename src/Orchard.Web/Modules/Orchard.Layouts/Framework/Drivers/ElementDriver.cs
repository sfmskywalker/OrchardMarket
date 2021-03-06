using System;
using System.Linq;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Framework.Elements;

namespace Orchard.Layouts.Framework.Drivers {
    public abstract class ElementDriver<TElement> : Component, IElementDriver where TElement: IElement {
        public virtual int Priority {
            get { return 0; }
        }

        public EditorResult BuildEditor(ElementEditorContext context) {
            return OnBuildEditor((TElement) context.Element, context);
        }

        public EditorResult UpdateEditor(ElementEditorContext context) {
            return OnUpdateEditor((TElement)context.Element, context);
        }

        public void Displaying(ElementDisplayContext context) {
            OnDisplaying((TElement) context.Element, context);
        }

        public void LayoutSaving(ElementSavingContext context) {
            OnLayoutSaving((TElement) context.Element, context);
        }

        public void ElementRemoving(ElementRemovingContext context) {
            OnElementRemoving((TElement) context.Element, context);
        }

        protected virtual EditorResult OnBuildEditor(TElement element, ElementEditorContext context) {
            return null;
        }

        protected virtual EditorResult OnUpdateEditor(TElement element, ElementEditorContext context) {
            return OnBuildEditor(element, context);
        }

        protected virtual void OnDisplaying(TElement element, ElementDisplayContext context) {
        }

        protected virtual void OnLayoutSaving(TElement element, ElementSavingContext context) {
        }

        protected virtual void OnElementRemoving(TElement element, ElementRemovingContext context) {
        }

        protected EditorResult Editor(ElementEditorContext context, params dynamic[] editorShapes) {
            foreach (var editorShape in editorShapes) {
                if (String.IsNullOrWhiteSpace(editorShape.Metadata.Position)) {
                    editorShape.Metadata.Position = "Properties:0";
                }
            }

            var result = new EditorResult {
                Editors = editorShapes.ToList()
            };

            return result;
        }
    }
}