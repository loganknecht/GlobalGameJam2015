using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// Stores graph metadata so that it will persist through play-mode, assembly reloads, and
    /// through editor sessions.
    /// </summary>
    [AddComponentMenu("")]
    public class fiGraphMetadataPersistentStorage : BaseBehavior<FullSerializerSerializer>,
        IEditorOnlyTag, ISerializationAlwaysDirtyTag, ISerializationCallbacks {

        #region Serialization
        [SerializeField]
        private UnityObject[] _keys;
        [SerializeField]
        private fiGraphMetadata[] _values;

        void ISerializationCallbacks.OnBeforeSerialize() {
            _keys = null;
            _values = null;

            if (SavedGraphs != null) {
                _keys = SavedGraphs.Keys.ToArray();
                _values = SavedGraphs.Values.ToArray();
            }
        }

        void ISerializationCallbacks.OnAfterDeserialize() {
            SavedGraphs = fiUtility.CreateDictionary(_keys, _values);
        }
        #endregion



        [NotSerialized, HideInInspector]
        public Dictionary<UnityObject, fiGraphMetadata> SavedGraphs = new Dictionary<UnityObject, fiGraphMetadata>();

        [NotSerialized, ShowInInspector]
        public Dictionary<UnityObject, fiGraphMetadata> ViewableMetadata {
            get {
                var dict = new Dictionary<UnityObject, fiGraphMetadata>(SavedGraphs);
                dict.Remove(this);
                dict.Remove(gameObject);
                return dict;
            }
            set { }
        }

#if UNITY_EDITOR
        private static fiGraphMetadataPersistentStorage _instance;
        public static fiGraphMetadataPersistentStorage Instance {
            get {
                if (_instance == null) {
                    _instance = GameObject.FindObjectOfType<fiGraphMetadataPersistentStorage>();

                    if (_instance == null) {
                        var container = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags("fiSceneMetadata", HideFlags.HideInHierarchy);
                        _instance = container.AddComponent<fiGraphMetadataPersistentStorage>();
                    }

                    if (fiSettings.EnableMetadataPersistence == false) {
                        _instance.gameObject.hideFlags |= HideFlags.DontSave;
                    }
                }

                return _instance;
            }
        }
#endif
    }
}
