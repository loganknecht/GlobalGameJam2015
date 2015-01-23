using FullInspector.Internal;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    /// <summary>
    /// The default behavior editor is used whenever there is not a user-defined one that should be
    /// used instead.
    /// </summary>
    public class DefaultBehaviorEditor : BehaviorEditor<UnityObject> {
        protected override void OnSceneGUI(UnityObject behavior) {
        }

        protected override void OnEdit(Rect rect, UnityObject behavior, fiGraphMetadata metadata) {
            fiGraphMetadataChild childMetadata = metadata.Enter("DefaultBehaviorEditor");
            childMetadata.Metadata.GetMetadata<DropdownMetadata>().OverrideDisable = true;

            // We don't want to get the IObjectPropertyEditor for the given target, which extends
            // UnityObject, so that we can actually edit the property instead of getting a Unity
            // reference field. We also don't want the AbstractTypePropertyEditor, which we will get
            // if the behavior has any derived types.
            PropertyEditorChain editorChain = PropertyEditor.Get(behavior.GetType(), null);
            IPropertyEditor editor = editorChain.SkipUntilNot(
                typeof(FullInspector.Modules.Common.IObjectPropertyEditor),
                typeof(AbstractTypePropertyEditor));

            // Run the editor
            editor.Edit(rect, GUIContent.none, behavior, childMetadata);
        }

        protected override float OnGetHeight(UnityObject behavior, fiGraphMetadata metadata) {
            fiGraphMetadataChild childMetadata = metadata.Enter("DefaultBehaviorEditor");
            childMetadata.Metadata.GetMetadata<DropdownMetadata>().OverrideDisable = true; 

            float height = 0;

            // We don't want to get the IObjectPropertyEditor for the given target, which extends
            // UnityObject, so that we can actually edit the property instead of getting a Unity
            // reference field. We also don't want the AbstractTypePropertyEditor, which we will get
            // if the behavior has any derived types.
            PropertyEditorChain editorChain = PropertyEditor.Get(behavior.GetType(), null);
            IPropertyEditor editor = editorChain.SkipUntilNot(
                typeof(FullInspector.Modules.Common.IObjectPropertyEditor),
                typeof(AbstractTypePropertyEditor));

            height += editor.GetElementHeight(GUIContent.none, behavior, childMetadata);

            return height;
        }

        private DefaultBehaviorEditor() {
        }
        public static DefaultBehaviorEditor Instance = new DefaultBehaviorEditor();
    }
}