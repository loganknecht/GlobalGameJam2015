using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// Any UnityObject type tagged with this interface will *not* be included in a published
    /// build. This is useful if there are components that should be editor-only.
    /// </summary>
    public interface IEditorOnlyTag { }

#if UNITY_EDITOR
    internal static class EditorOnlyMonoBehaviorRemover {
        [UnityEditor.Callbacks.PostProcessScene(-1)]
        public static void ClearData() {
            // [PostProcessScene] methods are called when Unity enters play-mode, but this is a
            // situation where we don't want to destroy the EditorOnly behaviors, as we are still
            // in the editor.
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
                return;
            }

            var derivedTypes = fiRuntimeReflectionUtility.AllSimpleTypesDerivingFrom(typeof(IEditorOnlyTag));
            foreach (var type in derivedTypes) {
                var behaviors = GameObject.FindObjectsOfType(type);
                foreach (var behavior in behaviors) {
                    UnityObject.DestroyImmediate(behavior);
                }
            }
        }
    }
#endif
}