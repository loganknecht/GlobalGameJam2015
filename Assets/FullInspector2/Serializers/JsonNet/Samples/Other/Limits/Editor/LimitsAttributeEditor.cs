using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Samples.Other.Limits {
    [CustomAttributePropertyEditor(typeof(LimitsAttribute), ReplaceOthers = false)]
    public class LimitsAttributeEditor<TElement> : AttributePropertyEditor<TElement, LimitsAttribute> {
        private static T Cast<T>(object o) {
            return (T)Convert.ChangeType(o, typeof(T));
        }

        protected override TElement Edit(Rect region, GUIContent label, TElement element, LimitsAttribute attribute, fiGraphMetadata metadata) {
            return Cast<TElement>(GUI.HorizontalSlider(region, Cast<float>(element), attribute.Min, attribute.Max));
        }

        protected override float GetElementHeight(GUIContent label, TElement element, LimitsAttribute attribute, fiGraphMetadata metadata) {
            return EditorStyles.label.CalcHeight(label, 100);
        }

        public override bool CanEdit(Type type) {
            return type == typeof(int) || type == typeof(double) || type == typeof(float);
        }
    }
}