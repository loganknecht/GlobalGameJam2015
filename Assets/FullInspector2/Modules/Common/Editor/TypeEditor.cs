using FullInspector.Internal;
using System;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules.Common {
    [CustomPropertyEditor(typeof(Type))]
    public class TypePropertyEditor : PropertyEditor<Type> {
        public class StateObject {
            public TypeSelectionPopupWindow Window;
        }

        public override Type Edit(Rect region, GUIContent label, Type element, fiGraphMetadata metadata) {
            Rect labelRect, buttonRect = region;

            if (string.IsNullOrEmpty(label.text) == false) {
                RectTools.SplitHorizontalPercentage(region, .3f, 2, out labelRect, out buttonRect);
                GUI.Label(labelRect, label);
            }

            string displayed = "<no type>";
            if (element != null) {
                displayed = element.CSharpName();
            }

            int stateId = EditorGUIUtility.GetControlID(FocusType.Passive);
            StateObject stateObj = (StateObject)GUIUtility.GetStateObject(typeof(StateObject), stateId);

            if (GUI.Button(buttonRect, displayed)) {
                if (stateObj.Window == null) {
                    stateObj.Window = TypeSelectionPopupWindow.CreateSelectionWindow(element);
                }
            }

            if (stateObj.Window == null && ReferenceEquals(stateObj.Window, null) == false) {
                var window = stateObj.Window;
                stateObj.Window = null;

                if (window.SelectedType.HasValue) {
                    return window.SelectedType.Value;
                }
            }

            return element;
        }

        public override float GetElementHeight(GUIContent label, Type element, fiGraphMetadata metadata) {
            return EditorStyles.toolbarButton.CalcHeight(label, Screen.width);
        }
    }
}
