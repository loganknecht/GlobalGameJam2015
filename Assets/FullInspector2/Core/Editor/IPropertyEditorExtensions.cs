using FullInspector.Internal;
using System;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace FullInspector {
    /// <summary>
    /// The cached height metadata is used to optimize CPU/execution time for some memory so that
    /// we don't have to recompute heights multiple times over. In essence, if we call
    /// GetElementHeight from Edit, then GetElementHeight will short-circuit and just return a
    /// cached height.
    /// </summary>
    public class CachedHeightMedatadata : IGraphMetadataItem {
        public float CachedHeight;
    }

    public static class PropertyEditorExtensions {
        /// <summary>
        /// We store the current
        /// </summary>
        private enum BaseMethodCall {
            None,
            Edit,
            GetElementHeight,
            OnSceneGUI
        }
        /// <summary>
        /// The current method that we are invoking.
        /// </summary>
        private static BaseMethodCall BaseMethod;


        private static void BeginMethodSet(BaseMethodCall method, out bool set) {
            set = BaseMethod == BaseMethodCall.None;

            if (!set && BaseMethod != method) {
                throw new InvalidOperationException(method + " cannot be called from " + BaseMethod);
            }

            if (set) {
                BaseMethod = method;
            }
        }
        private static void EndMethodSet(bool set) {
            if (set) {
                BaseMethod = BaseMethodCall.None;
            }
        }

        private static float FoldoutHeight = EditorStyles.foldout.CalcHeight(GUIContent.none, 100);

        /// <summary>
        /// Helper method to fetch the editing API for an IPropertyEditor.
        /// </summary>
        private static IPropertyEditorEditAPI GetEditingAPI(IPropertyEditor editor) {
            var api = editor as IPropertyEditorEditAPI;

            if (api == null) {
                throw new InvalidOperationException(string.Format("Type {0} needs to extend " +
                    "IPropertyEditorEditAPI", editor.GetType()));
            }

            return api;
        }

        /// <summary>
        /// Display a Unity inspector GUI that provides an editing interface for the given object.
        /// </summary>
        /// <param name="region">The rect on the screen to draw the GUI controls.</param>
        /// <param name="label">The label to label the controls with.</param>
        /// <param name="element">The element itself to edit. This can be mutated directly. For
        /// values which cannot be mutated, such as structs, the return value is used to update the
        /// stored value.</param>
        /// <returns>An updated instance of the element.</returns>
        public static T Edit<T>(this IPropertyEditor editor, Rect region, GUIContent label, T element, fiGraphMetadataChild metadata) {
            var api = GetEditingAPI(editor);
            return DoEdit(api, region, label, element, metadata.Metadata);
        }

        private static T DoEdit<T>(IPropertyEditorEditAPI api, Rect region, GUIContent label, T element, fiGraphMetadata metadata) {
            bool setBaseMethod;
            BeginMethodSet(BaseMethodCall.Edit, out setBaseMethod);

            try {

                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = fiEditorUtility.GetLabelWidth(region.width);

                var result = DoEdit2(api, region, label, element, metadata);

                EditorGUIUtility.labelWidth = labelWidth;

                return result;
            }
            finally {
                EndMethodSet(setBaseMethod);
            }
        }


        private static T DoEdit2<T>(IPropertyEditorEditAPI api, Rect region, GUIContent label, T element, fiGraphMetadata metadata) {
            var dropdown = metadata.GetMetadata<DropdownMetadata>();

            // Activate the foldout if we're above the minimum foldout height
            dropdown.ShowDropdown =
                dropdown.ShowDropdown ||
                region.height > fiSettings.MinimumFoldoutHeight;

            // Has the foldout been activated? Should we skip rendering the entire item?
            if (dropdown.ShowDropdown && dropdown.IsActive == false) {
                // Some editors don't always supply a label (for example, the collection editors),
                // so instead of showing nothing we'll show a handy "Collapsed value" instead
                if (string.IsNullOrEmpty(label.text)) {
                    label = new GUIContent("Collapsed value");
                }
                label = api.GetFoldoutHeader(label, element);

                region.x += 8;
                region.width -= 8;
                region.height = FoldoutHeight;

                // note: we don't use dropdown.IsActive = ... because we can be animating, and doing
                // that direct-assign toggle will cause the animation to flip-flop infinitely
                if (EditorGUI.Foldout(region, false, label, toggleOnLabelClick: false) == true) {
                    dropdown.IsActive = true;
                }

                return element;
            }

            // Should we show the foldout arrow?
            if (dropdown.ShowDropdown) {
                Rect foldoutRegion = region;
                foldoutRegion.height = FoldoutHeight;
                foldoutRegion.width = 4;
                foldoutRegion.x += 10;

                // note: We don't use dropdown.IsActive = ... because we can be animating, and doing
                // that direct-assign toggle will cause the animation to flip-flop infinitely
                if (EditorGUI.Foldout(foldoutRegion, true, GUIContent.none, toggleOnLabelClick: false) == false) {
                    dropdown.IsActive = false;
                }

                // Indent the actual edited content so that it doesn't overlap with the arrow
                // TODO: This *could* change the height of the edited content, but I don't think it
                //       will be a big deal for now... Besides, fixing it would require quite a bit
                //       of re-architecturing (GetElementHeight needs to take a width).
                if (CanShowPrettyDropdown(label, api)) {
                    label.text = "  " + label.text;
                }

                else {
                    float regionIndent = 10;
                    if (string.IsNullOrEmpty(label.text) || dropdown.ExtraIndent) regionIndent += 3;
                    region.x += regionIndent;
                    region.width -= regionIndent;
                }
            }

            // Draw the actual edited content
            if (dropdown.IsAnimating) fiEditorGUI.BeginFadeGroup(FoldoutHeight, ref region, dropdown.AnimPercentage);
            var result = (T)api.Edit(region, label, element, metadata);
            if (dropdown.IsAnimating) fiEditorGUI.EndFadeGroup();

            // End the cull zone. This should have been started inside of GetElementHeight, but if
            // for some reason GetElementHeight was never called, this will be harmless if not
            // currently in a cull-zone.
            metadata.EndCullZone();

            return result;
        }

        private static bool CanShowPrettyDropdown(GUIContent content, IPropertyEditorEditAPI api) {
            return api.CanIndentLabelForDropdown && string.IsNullOrEmpty(content.text) == false;
        }

        /// <summary>
        /// Returns the height of the region that needs editing.
        /// </summary>
        /// <param name="label">The label that will be used when editing.</param>
        /// <param name="element">The element that will be edited.</param>
        /// <returns>The height of the region that needs editing.</returns>
        public static float GetElementHeight<T>(this IPropertyEditor editor, GUIContent label, T element, fiGraphMetadataChild metadata) {
            if (BaseMethod == BaseMethodCall.Edit) {
                CachedHeightMedatadata cachedHeight;
                if (metadata.Metadata.TryGetMetadata(out cachedHeight) == false) {
                    cachedHeight = metadata.Metadata.GetMetadata<CachedHeightMedatadata>();
                }
                return cachedHeight.CachedHeight;
            }

            var dropdown = metadata.Metadata.GetMetadata<DropdownMetadata>();
            if (dropdown.ShowDropdown && dropdown.IsAnimating == false && dropdown.IsActive == false) {
                return FoldoutHeight;
            }

            bool setBaseMethod;
            BeginMethodSet(BaseMethodCall.GetElementHeight, out setBaseMethod);

            try {
                // We begin (but do not end) the cull zone here. The cull zone is terminated inside
                // of Edit(). It is safe to call BeginCullZone() multiple times -- it has no
                // effect past the first call.
                metadata.Metadata.BeginCullZone();

                var api = GetEditingAPI(editor);
                float height = api.GetElementHeight(label, element, metadata.Metadata);

                if (dropdown.IsAnimating) {
                    fiEditorUtility.Repaint = true;
                    fiEditorGUI.UpdateFadeGroupHeight(ref height, FoldoutHeight, dropdown.AnimPercentage);
                }

                metadata.Metadata.GetMetadata<CachedHeightMedatadata>().CachedHeight = height;


                return height;
            }
            finally {
                EndMethodSet(setBaseMethod);
            }
        }

        /// <summary>
        /// Returns a header that should be used for the foldout. An item is displayed within a
        /// foldout when this property editor reaches a certain height.
        /// </summary>
        /// <param name="label">The current foldout label.</param>
        /// <param name="element">The current object element.</param>
        /// <returns>An updated label.</returns>
        public static GUIContent GetFoldoutHeader<T>(this IPropertyEditor editor, GUIContent label,
            T element) {

            var api = GetEditingAPI(editor);

            return api.GetFoldoutHeader(label, element);
        }

        /// <summary>
        /// Draw an optional scene GUI.
        /// </summary>
        /// <param name="element">The object instance to edit using the scene GUI.</param>
        /// <returns>An updated object instance.</returns>
        public static T OnSceneGUI<T>(this IPropertyEditor editor, T element) {
            var api = GetEditingAPI(editor);

            return (T)api.OnSceneGUI(element);
        }

        /// <summary>
        /// This method makes it easy to use a typical property editor as a GUILayout style method,
        /// where the rect is taken care of.
        /// </summary>
        /// <param name="editor">The editor that is being used.</param>
        /// <param name="label">The label to edit the region with.</param>
        /// <param name="element">The element that is being edited.</param>
        public static T EditWithGUILayout<T>(this IPropertyEditor editor, GUIContent label,
            T element, fiGraphMetadataChild metadata) {

            float height = editor.GetElementHeight(label, element, metadata);
            Rect region = EditorGUILayout.GetControlRect(false, height);
            if (Event.current.type != EventType.Layout) {
                return editor.Edit(region, label, element, metadata);
            }
            return element;
        }
    }
}