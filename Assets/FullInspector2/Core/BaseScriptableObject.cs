using FullInspector.Internal;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// A common class that derives from ScriptableObject so that we can provide a custom editor for
    /// BaseScriptableObject{TSerializer}
    /// </summary>
    public class CommonBaseScriptableObject : ScriptableObject { }
}

namespace FullInspector {
    public abstract class BaseScriptableObject<TSerializer> :
        CommonBaseScriptableObject, ISerializedObject
#if UNITY_EDITOR
        , ISerializationCallbackReceiver
#endif
        where TSerializer : BaseSerializer {

        static BaseScriptableObject() {
            BehaviorTypeToSerializerTypeMap.Register(typeof(BaseBehavior<TSerializer>), typeof(TSerializer));
        }

        /// <summary>
        /// This awake base method calls RestoreState() by default. If you override this method, it
        /// is *critically* important that this be the first call made in your Awake method. If it
        /// is not, then your component may not be deserialized when you go to access values.
        /// </summary>
        protected virtual void OnEnable() {
            RestoreState();
        }

        /// <summary>
        /// Save the state of component so that it can go through Unity serialization correctly.
        /// </summary>
        [ContextMenu("Save Current State")]
        public void SaveState() {
            BehaviorSerializationHelpers.SaveState<TSerializer>(this);
        }

        /// <summary>
        /// Restore the state of the component after it has gone through Unity serialization. If the
        /// component has already been restored, it will be reset to its last saved state.
        /// </summary>
        [ContextMenu("Restore Saved State")]
        public void RestoreState() {
            BehaviorSerializationHelpers.RestoreState<TSerializer>(this);
        }

        /// <summary>
        /// Serializing references derived from UnityObject is tricky for a number of reasons, so we
        /// just let Unity handle it. The object can be modified in the inspector and be deleted, or
        /// it can become a prefab. Further, there is no good way to uniquely identify components
        /// and game objects that handle prefabs and instantiation well. We therefore just let Unity
        /// serialize our references for us.
        /// </summary>
        /// <remarks>
        /// We add a NotSerialized annotation to this item so that FI will not serialize it
        /// </remarks>
        [SerializeField, NotSerialized, HideInInspector]
        private List<UnityObject> _objectReferences;

        List<UnityObject> ISerializedObject.SerializedObjectReferences {
            get {
                return _objectReferences;
            }
            set {
                _objectReferences = value;
            }
        }

        /// <remarks>
        /// We add a NotSerialized annotation to this item so that FI will not serialize it
        /// </remarks>
        [SerializeField, NotSerialized, HideInInspector]
        private List<string> _serializedStateKeys;

        List<string> ISerializedObject.SerializedStateKeys {
            get {
                return _serializedStateKeys;
            }
            set {
                _serializedStateKeys = value;
            }
        }

        /// <remarks>
        /// We add a NotSerialized annotation to this item so that FI will not serialize it
        /// </remarks>
        [SerializeField, NotSerialized, HideInInspector]
        private List<string> _serializedStateValues;

        List<string> ISerializedObject.SerializedStateValues {
            get {
                return _serializedStateValues;
            }
            set {
                _serializedStateValues = value;
            }
        }

#if UNITY_EDITOR
        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            fiEditorSerializationManager.SubmitDeserializeRequest(this);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            fiEditorSerializationManager.SubmitSerializeRequest(this);
        }
#endif
    }
}