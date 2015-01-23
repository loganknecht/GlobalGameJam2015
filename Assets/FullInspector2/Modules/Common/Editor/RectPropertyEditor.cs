using FullInspector.LayoutToolkit;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules.Common {
    [CustomPropertyEditor(typeof(Rect))]
    public class RectPropertyEditor : PropertyEditor<Rect> {
        private static fiLayout Layout;

        static RectPropertyEditor() {
            float vecHeight = EditorStyles.label.CalcHeight(GUIContent.none, 0);
            Layout = new fiVerticalLayout {
                { "Label", vecHeight },

                new fiHorizontalLayout {
                    15,

                    new fiVerticalLayout {
                        { "Position", vecHeight },
                        2,
                        { "Size", vecHeight },
                    }
                }
            };
        }

        public override Rect Edit(Rect region, GUIContent label, Rect element, fiGraphMetadata metadata) {
            EditorGUI.LabelField(Layout.GetSectionRect("Label", region), label);
            element.position = EditorGUI.Vector2Field(Layout.GetSectionRect("Position", region), "Position", element.position);
            element.size = EditorGUI.Vector2Field(Layout.GetSectionRect("Size", region), "Size", element.size);
            return element;
        }

        public override float GetElementHeight(GUIContent label, Rect element, fiGraphMetadata metadata) {
            return Layout.Height;
        }
    }
}