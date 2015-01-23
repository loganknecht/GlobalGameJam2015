using FullInspector.Internal;
using FullInspector.LayoutToolkit;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules.Collections {
    /// <summary>
    /// Provides a relatively simple editor for IList{T} types that only views one element at a
    /// time. This is useful if the list is massive, or perhaps to just reduce information overload
    /// when editing.
    /// </summary>
    [CustomAttributePropertyEditor(typeof(InspectorDatabaseEditorAttribute), ReplaceOthers = true)]
    public class InspectorDatabaseEditorAttributeEditor<TDerived, T> :
        AttributePropertyEditor<IList<T>, InspectorDatabaseEditorAttribute>
        where TDerived : IList<T>, new() {

        public override bool CanIndentLabelForDropdown {
            get { return false; }
        }

        private fiLayout Header = new fiVerticalLayout {
            2,

            {
                "BoxedArea", fiLayoutUtility.Margin(10, new fiVerticalLayout {
                    3,

                    { "Label", 18 },

                    2,

                    new fiHorizontalLayout(new fiLayoutHeight(18)) {
                        2,
                        "Back",
                        2,
                        "Forward",
                        2,
                        { "Delete", 25 }
                    }
                })
            },

            5
        };

        /// <summary>
        /// Property editor that is used for the list items.
        /// </summary>
        private PropertyEditorChain _itemEditor = PropertyEditor.Get(typeof(T), null);

        /// <summary>
        /// Ensures that the given element points to a valid instance.
        /// </summary>
        private static void EnsureInstance(ref IList<T> element) {
            if (element == null) {
                element = new TDerived();
            }
        }

        /// <summary>
        /// Attempts to fetch an item to edit.
        /// </summary>
        /// <param name="list">The list that is being edited.</param>
        /// <param name="edited">The item we are currently empty. Only contains a value if this
        /// function returns true.</param>
        /// <returns>True if there is an item being edited, otherwise false.</returns>
        private bool TryGetEditedElement(IList<T> list, out T edited) {
            if (HasEditableItem(list) == false) {
                edited = default(T);
                return false;
            }

            TryEnsureValidIndex(list);
            edited = list[CurrentIndex(list)];
            return true;
        }

        /// <summary>
        /// Updates the stored value for the given edited item.
        /// </summary>
        private void UpdateEditedElement(IList<T> list, T edited) {
            if (HasEditableItem(list)) {
                list[CurrentIndex(list)] = edited;
            }
        }

        /// <summary>
        /// Returns true if there is currently an item that is being edited.
        /// </summary>
        private static bool HasEditableItem(IList<T> list) {
            int index = CurrentIndex(list);
            return index >= 0 && index < list.Count;
        }

        /// <summary>
        /// Attempts to ensure that the current editing index is not out of range. However, if the
        /// edited list is empty, then the index will always be out of range.
        /// </summary>
        private static void TryEnsureValidIndex(IList<T> list) {
            var metadata = fiGlobalMetadata.Get<InspectorDatabaseEditorMetadata>(list);

            if (list.Count == 0) {
                metadata.CurrentIndex = -1;
            }
            else if (metadata.CurrentIndex < 0) {
                metadata.CurrentIndex = 0;
            }
            else if (metadata.CurrentIndex >= list.Count) {
                metadata.CurrentIndex = list.Count - 1;
            }
        }

        /// <summary>
        /// Returns true if the editor can delete the item currently being edited.
        /// </summary>
        private static bool CanDelete(IList<T> list) {
            return HasEditableItem(list);
        }

        /// <summary>
        /// Returns true if the editor can go "back" to the previous item in the list.
        /// </summary>
        private static bool CanGoBack(IList<T> list) {
            return CurrentIndex(list) > 0;
        }

        /// <summary>
        /// Returns the index of the item being edited. The index is *not* necessarily valid.
        /// </summary>
        private static int CurrentIndex(IList<T> list) {
            var metadata = fiGlobalMetadata.Get<InspectorDatabaseEditorMetadata>(list);
            return metadata.CurrentIndex;
        }

        /// <summary>
        /// Returns the index of the item being edited. The index is *not* necessarily valid.
        /// </summary>
        private static void SetIndex(IList<T> list, int index) {
            var metadata = fiGlobalMetadata.Get<InspectorDatabaseEditorMetadata>(list);
            metadata.CurrentIndex = index;
            TryEnsureValidIndex(list);
        }

        /// <summary>
        /// Changes the item being edited by the given offset. If necessary, this adds items to the
        /// list.
        /// </summary>
        private static void ChangeEditedElement(IList<T> list, int offset) {
            var metadata = fiGlobalMetadata.Get<InspectorDatabaseEditorMetadata>(list);
            metadata.CurrentIndex += offset;

            while (metadata.CurrentIndex >= list.Count) {
                list.Add(default(T));
            }
        }

        /// <summary>
        /// Removes the item currently being edited.
        /// </summary>
        private static void RemoveEditedElement(IList<T> list) {
            if (CanDelete(list)) {
                list.RemoveAt(CurrentIndex(list));
            }
        }

        /// <summary>
        /// Adds extra information about the current item that is being edited to the label.
        /// </summary>
        private static GUIContent AddEditingInformation(GUIContent content, IList<T> list) {
            string updatedText;

            if (list.Count == 0) {
                updatedText = string.Format("{0} (empty)", content.text);
            }
            else {
                var metadata = fiGlobalMetadata.Get<InspectorDatabaseEditorMetadata>(list);
                updatedText = string.Format("{0} (element {1} of {2})", content.text,
                    metadata.CurrentIndex + 1, list.Count);

            }

            return new GUIContent(updatedText, content.image, content.tooltip);
        }

        private enum NextButtonOperation {
            MoveNext, Add
        }
        private static NextButtonOperation GetNextButtonOperation(IList<T> list) {
            if (list.Count == 0 || CurrentIndex(list) == list.Count - 1) {
                return NextButtonOperation.Add;
            }
            return NextButtonOperation.MoveNext;
        }

        private Rect DrawHeader(Rect region, GUIContent label, IList<T> list) {
            var labelRect = Header.GetSectionRect("Label", region);
            var backRect = Header.GetSectionRect("Back", region);
            var forwardRect = Header.GetSectionRect("Forward", region);
            var delRect = Header.GetSectionRect("Delete", region);
            var boxedRect = Header.GetSectionRect("BoxedArea", region);
            region = RectTools.MoveDown(region, Header.Height);

            GUI.Box(boxedRect, GUIContent.none);

            label = AddEditingInformation(label, list);
            if (list.Count == 0) {
                EditorGUI.LabelField(labelRect, label);
            }
            else {
                SetIndex(list, EditorGUI.IntSlider(labelRect, label, CurrentIndex(list), 0,
                    list.Count - 1));
            }

            GUI.color = new Color(150, 0, 0);
            EditorGUI.BeginDisabledGroup(!CanDelete(list));
            if (GUI.Button(delRect, "X")) {
                RemoveEditedElement(list);
            }
            EditorGUI.EndDisabledGroup();
            GUI.color = Color.white;

            EditorGUI.BeginDisabledGroup(!CanGoBack(list));
            if (GUI.Button(backRect, "<<")) {
                ChangeEditedElement(list, -1);
            }
            EditorGUI.EndDisabledGroup();

            string nextLabel = ">>";
            if (GetNextButtonOperation(list) == NextButtonOperation.Add) {
                GUI.color = Color.green;
                nextLabel = "Add";
            }
            if (GUI.Button(forwardRect, nextLabel)) {
                ChangeEditedElement(list, 1);
            }
            GUI.color = Color.white;

            return region;
        }

        protected override IList<T> Edit(Rect region, GUIContent label, IList<T> list, InspectorDatabaseEditorAttribute attribute, fiGraphMetadata metadata) {
            // Set the global metadata to the graph metadata, as the graph metadata is persistent
            // but users still may want to access the global metadata.
            fiGlobalMetadata.Set(list, metadata.GetMetadata<InspectorDatabaseEditorMetadata>());

            EnsureInstance(ref list);
            TryEnsureValidIndex(list);

            region = DrawHeader(region, label, list);

            T edited;
            if (TryGetEditedElement(list, out edited)) {
                edited = (T)_itemEditor.FirstEditor.Edit(region, GUIContent.none, edited, metadata.Enter("SingleItemListEditor"));
                UpdateEditedElement(list, edited);
            }

            return list;
        }

        protected override float GetElementHeight(GUIContent label, IList<T> list, InspectorDatabaseEditorAttribute attribute, fiGraphMetadata metadata) {
            // Set the global metadata to the graph metadata, as the graph metadata is persistent
            // but users still may want to access the global metadata.
            fiGlobalMetadata.Set(list, metadata.GetMetadata<InspectorDatabaseEditorMetadata>());

            EnsureInstance(ref list);
            TryEnsureValidIndex(list);

            float height = Header.Height;

            T edited;
            if (TryGetEditedElement(list, out edited)) {
                height += _itemEditor.FirstEditor.GetElementHeight(GUIContent.none, edited, metadata.Enter("SingleItemListEditor"));
            }

            return height;
        }

        /// <summary>
        /// The metadata we store on each item that we edit so that we know what the active editing
        /// item is.
        /// </summary>
        [Obsolete("Please use InspectorDatabaseEditorMetadata instead")]
        public class ItemMetadata {
            public int CurrentIndex;
        }

        /// <summary>
        /// The metadata we store on each item that we edit so that we know what the active editing
        /// item is.
        /// </summary>
        public class InspectorDatabaseEditorMetadata : IGraphMetadataItem {
            public int CurrentIndex;
        }
    }
}