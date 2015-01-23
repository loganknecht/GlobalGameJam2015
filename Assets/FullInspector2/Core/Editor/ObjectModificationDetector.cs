using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// A helper class that identifies when an ISerializedObject has been modified for some reason.
    /// </summary>
    /// <remarks>
    /// To determine if the object state has changed, this hashes the serialized state of the
    /// object. If the hash of the serialized state has changed, then the object has been modified.
    /// </remarks>
    internal static class ObjectModificationDetector {
        /// <summary>
        /// The saved state of an object.
        /// </summary>
        private class SavedObject {
            public Dictionary<string, string> States = new Dictionary<string, string>();
            public List<UnityObject> UnityObjects = new List<UnityObject>();
        }
        private static Dictionary<ISerializedObject, SavedObject> _savedObjects = new Dictionary<ISerializedObject, SavedObject>();

        /// <summary>
        /// Returns true if the given object has been modified since its last update.
        /// </summary>
        public static bool WasModified(ISerializedObject obj) {
            SavedObject savedObj;
            if (_savedObjects.TryGetValue(obj, out savedObj) == false) {
                // We want to reset if we don't have any data already stored, because the object may
                // have already been modified from it's initial prefab state.
                return true;
            }

            bool modified = false;

            if (!modified) {
                modified = obj.SerializedStateKeys.Count != savedObj.States.Count;
            }

            if (!modified) {
                for (int i = 0; i < obj.SerializedStateKeys.Count; ++i) {
                    string key = obj.SerializedStateKeys[i];
                    string value = obj.SerializedStateValues[i];

                    string savedValue;
                    if (savedObj.States.TryGetValue(key, out savedValue) == false) {
                        modified = true;
                        break;
                    }

                    if (value != savedValue) {
                        modified = true;
                        break;
                    }
                }
            }

            if (!modified) {
                modified = obj.SerializedObjectReferences.Count != savedObj.UnityObjects.Count;
            }

            if (!modified) {
                for (int i = 0; i < obj.SerializedObjectReferences.Count; ++i) {
                    if (obj.SerializedObjectReferences[i] != savedObj.UnityObjects[i]) {
                        modified = true;
                        break;
                    }
                }
            }

            return modified;
        }

        /// <summary>
        /// Update the stored serialized state of the given object.
        /// </summary>
        public static void Update(ISerializedObject obj) {
            SavedObject savedObj;
            if (_savedObjects.TryGetValue(obj, out savedObj) == false) {
                savedObj = new SavedObject();
                _savedObjects[obj] = savedObj;
            }

            // update the saved states
            savedObj.States.Clear();
            for (int i = 0; i < obj.SerializedStateKeys.Count; ++i) {
                string key = obj.SerializedStateKeys[i];
                string value = obj.SerializedStateValues[i];

                savedObj.States[key] = value;
            }

            // update the saved unity object references
            savedObj.UnityObjects.Clear();
            savedObj.UnityObjects.AddRange(obj.SerializedObjectReferences);
        }
    }
}