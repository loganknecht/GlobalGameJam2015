using FullInspector.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    public interface IBehaviorEditor {
        void Edit(Rect rect, UnityObject behavior);
        float GetHeight(UnityObject behavior);
        void SceneGUI(UnityObject behavior);
    }

    public abstract class BehaviorEditor<TBehavior> : IBehaviorEditor
        where TBehavior : UnityObject {

        protected abstract void OnEdit(Rect rect, TBehavior behavior, fiGraphMetadata metadata);
        protected abstract float OnGetHeight(TBehavior behavior, fiGraphMetadata metadata);
        protected abstract void OnSceneGUI(TBehavior behavior);

        public void SceneGUI(UnityObject behavior) {
            CheckForPrefabChanges(behavior);

            EditorGUI.BeginChangeCheck();

            // we don't want to get the IObjectPropertyEditor for the given target, which extends
            // UnityObject, so that we can actually edit the property instead of getting a Unity
            // reference field
            OnSceneGUI((TBehavior)behavior);

            // If the GUI has been changed, then we want to reserialize the current object state.
            // However, we don't bother doing this if we're currently in play mode, as the
            // serialized state changes will be lost regardless.
            if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying == false) {
                fiRuntimeReflectionUtility.InvokeMethod(behavior.GetType(), "OnValidate", behavior, null);

                Undo.RecordObject(behavior, "Scene GUI Modification");
                EditorUtility.SetDirty(behavior);

                var serializedObj = behavior as ISerializedObject;
                if (serializedObj != null) {
                    fiEditorSerializationManager.SetDirty(serializedObj);
                    ObjectModificationDetector.Update(serializedObj);
                }
            }
        }

        public void Edit(Rect rect, UnityObject behavior) {
            // edit the data
            CheckForPrefabChanges(behavior);

            //-
            //-
            //-
            // Inspector based off of the property editor
            EditorGUI.BeginChangeCheck();

            // Run the editor
            OnEdit(rect, (TBehavior)behavior, fiGraphMetadata.GetGlobal(behavior));

            // If the GUI has been changed, then we want to reserialize the current object state.
            // However, we don't bother doing this if we're currently in play mode, as the
            // serialized state changes will be lost regardless.
            if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying == false) {
                fiRuntimeReflectionUtility.InvokeMethod(behavior.GetType(), "OnValidate", behavior, null);

                Undo.RecordObject(behavior, "Inspector Modification");
                EditorUtility.SetDirty(behavior);

                var serializedObj = behavior as ISerializedObject;
                if (serializedObj != null) {
                    fiEditorSerializationManager.SetDirty(serializedObj);
                    ObjectModificationDetector.Update(serializedObj);
                }
            }
        }

        public float GetHeight(UnityObject behavior) {
            CheckForPrefabChanges(behavior);
            return OnGetHeight((TBehavior)behavior, fiGraphMetadata.GetGlobal(behavior));
        }

        /// <summary>
        /// Ensures that the given behavior has been restored so that it can be edited with the
        /// proper data populated. This also verifies that the object is displaying the most recent
        /// prefab data.
        /// </summary>
        private static void CheckForPrefabChanges(UnityObject target) {
            ISerializedObject obj = target as ISerializedObject;
            if (obj == null) {
                return;
            }

            // If the object was modified, then we want to make sure that it fully reflects the most
            // recent serialized state, so we restore the state. Notably, we don't restore while the
            // game is playing, as prefab modifications are not going to be dispatched at this
            // point.
            if (ObjectModificationDetector.WasModified(obj) &&
                EditorApplication.isPlaying == false) {

                // We have a bit of a hack to get proper "reset" support. When the user resets a
                // behavior, Unity will clear all of the serialized data. The object modification
                // detector (as expected) picks up on this, but the object restoration engine only
                // restores *saved* properties, so none of the references are set to their default
                // values. To get our serialized keys back to their original values, we just resave
                // the object state and then set the serialized values to null.
                if (obj.SerializedStateKeys == null) obj.SerializedStateKeys = new List<string>();
                if (obj.SerializedStateValues == null) obj.SerializedStateValues = new List<string>();
                if (obj.SerializedStateKeys.Count == 0 && obj.SerializedStateValues.Count == 0) {
                    obj.SaveState();

                    obj.SerializedStateValues.Clear();
                    for (int i = 0; i < obj.SerializedStateKeys.Count; ++i) {
                        obj.SerializedStateValues.Add(null);
                    }
                }

                obj.RestoreState();
                ObjectModificationDetector.Update(obj);
            }

        }
    }
}