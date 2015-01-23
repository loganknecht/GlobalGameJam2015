using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// If a type extends this interface, then it will always be serialized when Unity sends a
    /// serialization request.
    /// </summary>
    public interface ISerializationAlwaysDirtyTag { }

    /// <summary>
    /// Manages the serialization of ISerializedObject instances. Unity provides a nice
    /// serialization callback, ISerializationCallbackReceiver, however, it often executes the
    /// callback methods on auxiliary threads. Some of the serializers have issues with this
    /// because they invoke unityObj == null, which is only available on the primary thread. To
    /// deal with this, this class defers deserialization so that it always executes on the
    /// primary thread.
    /// </summary>
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public static class fiEditorSerializationManager {
#if UNITY_EDITOR
        static fiEditorSerializationManager() {
            UnityEditor.EditorApplication.update += RunDeserializations;
        }

        /// <summary>
        /// Items to deserialize. We use a queue because deserialization of one item may
        /// technically cause another item to be added to the queue.
        /// </summary>
        private static Queue<ISerializedObject> _toDeserialize = new Queue<ISerializedObject>();

        /// <summary>
        /// Items that have been modified and should be saved again. Unity will happily send
        /// lots and lots of serialization requests, but most of them are unnecessary.
        /// </summary>
        private static HashSet<ISerializedObject> _dirty = new HashSet<ISerializedObject>();

        /// <summary>
        /// Attempts to serialize the given object. Serialization will only occur if the object is
        /// dirty. After being serialized, the object is no longer dirty.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SubmitSerializeRequest(ISerializedObject obj) {
            if (_dirty.Contains(obj) || obj is ISerializationAlwaysDirtyTag) {
                obj.SaveState();
                _dirty.Remove(obj);
            }
        }

        /// <summary>
        /// Attempt to deserialize the given object. The deserialization will occur on the next
        /// call to RunDeserializations().
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SubmitDeserializeRequest(ISerializedObject obj) {
            _toDeserialize.Enqueue(obj);
        }

        public static void RunDeserializations() {
            while (_toDeserialize.Count > 0) {
                ISerializedObject item = _toDeserialize.Dequeue();

#if UNITY_EDITOR
                // If we're in play-mode, then we don't want to deserialize anything as that can wipe
                // user-data. We cannot do this in SubmitDeserializeRequest because
                // EditorApplication.isPlaying can only be called from the main thread. However,
                // we *do* want to restore prefabs and general disk-based resources which will not have
                // Awake called.
                if (UnityEditor.EditorApplication.isPlaying) {
                    // note: We do a null check against unityObj to also check for destroyed objects,
                    //       which we don't need to bother restoring. Doing a null check against an
                    //       ISerializedObject instance will *not* call the correct == method, so
                    //       we need to be explicit about calling it against UnityObject.
                    var unityObj = item as UnityObject;

                    if (unityObj == null ||
                        UnityEditor.PrefabUtility.GetPrefabType(unityObj) != UnityEditor.PrefabType.Prefab) continue;
                }
#endif

                item.RestoreState();
            }
        }
#endif

        public static void SetDirty(ISerializedObject obj) {
#if UNITY_EDITOR
            _dirty.Add(obj);
#endif
        }
    }
}