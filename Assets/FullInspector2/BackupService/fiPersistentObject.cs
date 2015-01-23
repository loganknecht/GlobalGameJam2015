using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.BackupService {
    /// <summary>
    /// A reference to a Component that tries really hard to not go away, even if it's stored
    /// inside of a ScriptableObject.
    /// </summary>
    [Serializable]
    public class fiPersistentObject {
        /// <summary>
        /// Our referenced object. Sometimes set to null by Unity -- if that happens, we use the
        /// instance id to refetch the non-null instance.
        /// </summary>
        [SerializeField]
        private UnityObject _target;

        public fiPersistentObject() {
        }

        public fiPersistentObject(UnityObject target) {
            Target = target;
        }

        /// <summary>
        /// The actual component reference.
        /// </summary>
        public UnityObject Target {
            get {
                if (_target == null) {
                    RestoreFromInstanceId();
                }

                return _target;
            }

            set {
                _target = value;
            }
        }

        /// <summary>
        /// Restores the object (if fake null) to an actual object instance via its instance id.
        /// </summary>
        private void RestoreFromInstanceId() {
#if UNITY_EDITOR
            if (ReferenceEquals(_target, null) == false) {
                int instanceId = _target.GetInstanceID();
                _target = UnityEditor.EditorUtility.InstanceIDToObject(instanceId);
            }
#endif
        }
    }
}