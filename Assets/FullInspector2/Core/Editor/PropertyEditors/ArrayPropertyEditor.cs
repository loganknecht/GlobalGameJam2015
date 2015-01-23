using FullInspector.Rotorz.ReorderableList;
using System;
using System.Reflection;
using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// Provides a property editor for arrays, or a type T[].
    /// </summary>
    public class ArrayPropertyEditor<T> : PropertyEditor<T[]> {
        public override bool CanIndentLabelForDropdown {
            get { return false; }
        }

        private PropertyEditorChain _propertyEditor = PropertyEditor.Get(typeof(T), typeof(T));
        private bool _overrideDropdownDisable;

        public ArrayPropertyEditor(ICustomAttributeProvider attributes) {
            _overrideDropdownDisable = attributes == null || attributes.IsDefined(typeof(InspectorCollectionShowItemDropdownAttribute), true) == false;
        }

        public override T[] Edit(Rect region, GUIContent label, T[] elements, fiGraphMetadata metadata) {
            metadata.GetMetadata<DropdownMetadata>().ExtraIndent = true;

            if (elements == null) {
                elements = new T[0];
            }

            ArrayListAdaptor<T> adapter = GetAdapter(elements, metadata);

            Rect titleRect = new Rect(region);
            titleRect.height = GetTitleHeight();
            ReorderableListGUI.Title(titleRect, label);

            Rect bodyRect = new Rect(region);
            bodyRect.y += GetTitleHeight();
            bodyRect.height -= GetTitleHeight();
            ReorderableListGUI.ListFieldAbsolute(bodyRect, adapter, DrawEmpty);

            return adapter.StoredArray;
        }

        private ArrayListAdaptor<T> GetAdapter(T[] elements, fiGraphMetadata metadata) {
            return new ArrayListAdaptor<T>(elements, DrawItem, GetItemHeight, metadata);
        }

        private T DrawItem(Rect rect, T item, fiGraphMetadataChild metadata) {
            metadata.Metadata.GetMetadata<DropdownMetadata>().OverrideDisable = _overrideDropdownDisable;

            return (T)_propertyEditor.FirstEditor.Edit(rect, GUIContent.none, item, metadata);
        }

        private void DrawEmpty(Rect rect) {
        }

        private float GetItemHeight(T element, fiGraphMetadataChild metadata) {
            float height = _propertyEditor.FirstEditor.GetElementHeight(GUIContent.none, element, metadata);
            if (height < ReorderableListGUI.DefaultItemHeight) {
                return ReorderableListGUI.DefaultItemHeight;
            }
            return height;
        }

        private float GetTitleHeight() {
            return ReorderableListGUI.CalculateTitleHeight();
        }

        public override float GetElementHeight(GUIContent label, T[] elements, fiGraphMetadata metadata) {
            if (elements == null || elements.Length == 0) {
                const int heightOfEmpty = 20;
                return GetTitleHeight() + heightOfEmpty;
            }

            return GetTitleHeight() + ReorderableListGUI.CalculateListFieldHeight(GetAdapter(elements, metadata));
        }

        public override GUIContent GetFoldoutHeader(GUIContent label, object element) {
            if (element == null) {
                return label;
            }

            return new GUIContent(label.text + " (" + ((T[])element).Length + " elements)",
                label.tooltip);
        }
    }

    public static class ArrayPropertyEditor {
        public static IPropertyEditor TryCreate(Type dataType, ICustomAttributeProvider attributes) {
            if (dataType.IsArray == false) {
                return null;
            }

            Type elementType = dataType.GetElementType();

            Type editorType = typeof(ArrayPropertyEditor<>).MakeGenericType(elementType);
            return (IPropertyEditor)Activator.CreateInstance(editorType, new object[] { attributes });
        }
    }
}