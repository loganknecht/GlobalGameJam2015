using FullInspector.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    /// <summary>
    /// A simple wrapper tye for fiGraphMetadata so that we can have a different API signature
    /// when calling IPropertyEditor.Edit/GetHeight so that it is obvious that you need to call
    /// metadata.Enter(identifier) instead of passing in metadata.
    /// </summary>
    public struct fiGraphMetadataChild {
        public fiGraphMetadata Metadata;
    }

    /// <summary>
    /// An item that can be used as metadata inside of the graph metadata engine.
    /// </summary>
    public interface IGraphMetadataItem {
    }

    /// <summary>
    /// The graph metadata system allows for metadata storage based on the location of an item in
    /// the inspector, regardless of the actual object instance. The graph metadata system requires
    /// minor user support -- you simply have to pass an associated key when entering into
    /// child metadata items.
    /// </summary>
    public class fiGraphMetadata {
        /// <summary>
        /// A cullable dictionary is like a normal dictionary except that items inside of it can
        /// be removed if they are not used after a cull cycle.
        /// </summary>
        private class CullableDictionary<TKey, TValue, TDictionary>
            where TDictionary : IDictionary<TKey, TValue>, new() {

            /// <summary>
            /// Items that have been used and will therefore *not* be culled.
            /// </summary>
            [SerializeField]
            private TDictionary _primary;

            /// <summary>
            /// The items that we will cull if EndCullZone is called.
            /// </summary>
            [SerializeField]
            private TDictionary _culled;

            /// <summary>
            /// Are we currently culling data?
            /// </summary>
            [SerializeField]
            private bool _isCulling;

            public CullableDictionary() {
                _primary = new TDictionary();
                _culled = new TDictionary();
            }

            public TValue this[TKey key] {
                get {
                    TValue value;
                    if (TryGetValue(key, out value) == false) {
                        throw new KeyNotFoundException("" + key);
                    }
                    return value;
                }
                set {
                    _culled.Remove(key);
                    _primary[key] = value;
                }
            }

            /// <summary>
            /// Add an item to the dictionary.
            /// </summary>
            public void Add(TKey key, TValue value) {
                _primary.Add(key, value);
            }

            /// <summary>
            /// Attempts to fetch a value for the given key. If a value is found, then it will
            /// not be culled on the next cull-cycle.
            /// </summary>
            public bool TryGetValue(TKey key, out TValue value) {
                if (_culled.TryGetValue(key, out value)) {

                    // If we access the value in the culled set, then we want to make sure to move
                    // it into the primary set so we don't cull it.
                    _culled.Remove(key);
                    _primary.Add(key, value);

                    return true;
                }

                return _primary.TryGetValue(key, out value);
            }

            /// <summary>
            /// Begin a call zone. This is fine to call multiple times.
            /// </summary>
            public void BeginCullZone() {
                if (_isCulling == false) {
                    fiUtility.Swap(ref _primary, ref _culled);
                    _isCulling = true;
                }
            }

            /// <summary>
            /// Clears out all unused items. This method is harmless if called outside of
            /// BeginCullZone().
            /// </summary>
            public void EndCullZone() {
                if (_isCulling) _isCulling = false;

                if (fiSettings.EmitGraphMetadataCulls) {
                    // sigh: Only run the foreach loop if we actually have content to emit,
                    // otherwise we will allocate an iterator needlessly.
                    if (_culled.Count > 0) {
                        foreach (var item in _culled) {
                            Debug.Log("fiGraphMetadata culling \"" + item.Key + "\"");
                        }
                    }
                }
                _culled.Clear();
            }
        }

        #region Global Graph Access
#if UNITY_EDITOR
        /// <summary>
        /// Returns the graph metadata for the given UnityObject target.
        /// </summary>
        public static fiGraphMetadata GetGlobal(UnityObject obj) {
            fiGraphMetadata metadata;

            if (fiGraphMetadataPersistentStorage.Instance.SavedGraphs.TryGetValue(obj, out metadata) == false) {
                metadata = new fiGraphMetadata();
                fiGraphMetadataPersistentStorage.Instance.SavedGraphs[obj] = metadata;
            }

            return metadata;
        }
#endif
        #endregion

        #region Culling
        /// <summary>
        /// In order to avoid accumulating large amounts of metadata that is no longer useful or
        /// used, the graph metadata system supports automatic culling of unused metadata. If you
        /// wrap a set of code in a Begin/EndCullZone function calls, then any metadata that is
        /// not used between the Begin/End calls will be automatically released. This includes
        /// child metadata items.
        /// </summary>
        /// <remarks>
        /// Note that BeginCullZone() and EndCullZone() do *not* stack. Calling BeginCullZone()
        /// multiple times is like calling it only once. Calling EndCullZone() without having first
        /// called BeginCullZone() will do nothing.
        /// </remarks>
        /// <remarks>
        /// You almost certainly will not need to use this function. The IPropertyEditorExtensions
        /// engine handles it automatically.
        /// </remarks>
        public void BeginCullZone() {
            _childrenInt.BeginCullZone();
            _childrenString.BeginCullZone();
            _metadata.BeginCullZone();
        }

        /// <summary>
        /// This ends a culling zone. See docs on BeginCullZone.
        /// </summary>
        /// <remarks>
        /// You should not need to use this function -- it is for internal purposes. The
        /// IPropertyEditorExtensions engine handles it automatically.
        /// </remarks>
        public void EndCullZone() {
            _childrenInt.EndCullZone();
            _childrenString.EndCullZone();
            _metadata.EndCullZone();
        }
        #endregion

        /// <summary>
        /// The child metadata objects (at construction time).
        /// </summary>
        /// <remarks>
        /// This can go out of date if the metadata graph is adjusted by property editors! It's
        /// useful for debugging purposes, but don't rely on it for the actual edit graph.
        /// </remarks>
        /// <remarks>
        /// We use two dictionaries instead of just one (that takes an object key) to avoid boxing
        /// ints.
        /// </remarks>
        [ShowInInspector, SerializeField]
        private CullableDictionary<int, fiGraphMetadata, IntDictionary<fiGraphMetadata>> _childrenInt;
        [ShowInInspector, SerializeField]
        private CullableDictionary<string, fiGraphMetadata, Dictionary<string, fiGraphMetadata>> _childrenString;

        /// <summary>
        /// The actual metadata objects.
        /// </summary>
        [ShowInInspector, SerializeField]
        private CullableDictionary<Type, IGraphMetadataItem, Dictionary<Type, IGraphMetadataItem>> _metadata;

        public fiGraphMetadata() {
            _childrenInt = new CullableDictionary<int, fiGraphMetadata, IntDictionary<fiGraphMetadata>>();
            _childrenString = new CullableDictionary<string, fiGraphMetadata, Dictionary<string, fiGraphMetadata>>();
            _metadata = new CullableDictionary<Type, IGraphMetadataItem, Dictionary<Type, IGraphMetadataItem>>();

            //UnityEngine.Debug.Log("Constructed " + this);
        }

        #region Metadata Migration APIs
        /// <summary>
        /// Forcibly change the metadata that the given identifier points to to the specified
        /// instance. This is extremely useful if the inspector graph has been modified and the
        /// editor needs to make an adjustment to the metadata so that the metadata graph remains
        /// consistent with the actual inspector graph.
        /// </summary>
        /// <remarks>
        /// You do not need to worry about removing child metadata -- they will be automatically
        /// removed.
        /// </remarks>
        public void SetChild(int identifier, fiGraphMetadata metadata) {
            _childrenInt[identifier] = metadata;
        }
        /// <summary>
        /// Forcibly change the metadata that the given identifier points to to the specified
        /// instance. This is extremely useful if the inspector graph has been modified and the
        /// editor needs to make an adjustment to the metadata so that the metadata graph remains
        /// consistent with the actual inspector graph.
        /// </summary>
        /// <remarks>
        /// You do not need to worry about removing child metadata -- they will be automatically
        /// removed.
        /// </remarks>
        public void SetChild(string identifier, fiGraphMetadata metadata) {
            _childrenString[identifier] = metadata;
        }

        public struct MetadataMigration {
            public int NewIndex, OldIndex;
        }

        /// <summary>
        /// Helper method that automates metadata migration for array based graph reorders.
        /// </summary>
        public static void MigrateMetadata<T>(fiGraphMetadata metadata, T[] previous, T[] updated) {
            List<MetadataMigration> migrations;
            MigrateMetadata<T>(metadata, previous, updated, out migrations);

            List<fiGraphMetadata> copiedGraphs = new List<fiGraphMetadata>(migrations.Count);
            for (int i = 0; i < migrations.Count; ++i) {
                copiedGraphs.Add(metadata.Enter(i).Metadata);
            }

            for (int i = 0; i < migrations.Count; ++i) {
                MetadataMigration migration = migrations[i];
                metadata.SetChild(migration.NewIndex, copiedGraphs[i]);
            }
        }

        /// <summary>
        /// Helper method that automates metadata migration for array based graph reorders.
        /// </summary>
        public static void MigrateMetadata<T>(fiGraphMetadata metadata, T[] previous, T[] updated, out List<MetadataMigration> migrations) {
            migrations = new List<MetadataMigration>();

            for (int newIndex = 0; newIndex < updated.Length; ++newIndex) {
                int prevIndex = Array.IndexOf(previous, updated[newIndex]);

                if (prevIndex != -1 && prevIndex != newIndex) {
                    migrations.Add(new MetadataMigration {
                        NewIndex = newIndex,
                        OldIndex = prevIndex
                    });
                }
            }

        }
        #endregion
        /// <summary>
        /// Get a child metadata instance for the given identifier. This is useful for collections
        /// where each item maps to a unique index.
        /// </summary>
        public fiGraphMetadataChild Enter(int childIdentifier) {
            fiGraphMetadata metadata;


            if (_childrenInt.TryGetValue(childIdentifier, out metadata) == false) {
                metadata = new fiGraphMetadata();
                _childrenInt[childIdentifier] = metadata;
            }

            return new fiGraphMetadataChild { Metadata = metadata };
        }

        /// <summary>
        /// Get a child metadata instance for the given identifier. This is useful for general
        /// classes and structs where an object has a set of discrete named fields or properties.
        /// </summary>
        public fiGraphMetadataChild Enter(string childIdentifier) {
            fiGraphMetadata metadata;

            if (_childrenString.TryGetValue(childIdentifier, out metadata) == false) {
                metadata = new fiGraphMetadata();
                _childrenString[childIdentifier] = metadata;
            }

            return new fiGraphMetadataChild { Metadata = metadata };
        }

        /// <summary>
        /// Get a metadata instance for an object.
        /// </summary>
        /// <typeparam name="T">The type of metadata instance.</typeparam>
        public T GetMetadata<T>() where T : IGraphMetadataItem, new() {
            IGraphMetadataItem val;
            if (_metadata.TryGetValue(typeof(T), out val) == false) {
                val = new T();
                _metadata[typeof(T)] = val;
            }
            return (T)val;
        }

        /// <summary>
        /// Attempts to fetch a pre-existing metadata instance for an object.
        /// </summary>
        /// <typeparam name="T">The type of metadata instance.</typeparam>
        /// <param name="metadata">The found metadata instance, or null.</param>
        /// <returns>True if a metadata instance was found, false otherwise.</returns>
        public bool TryGetMetadata<T>(out T metadata) where T : IGraphMetadataItem, new() {
            IGraphMetadataItem item;
            bool result = _metadata.TryGetValue(typeof(T), out item);

            metadata = (T)item;
            return result;
        }
    }

    /// <summary>
    /// A (partial) dictionary implementation that has been optimized for fast >= 0 int access.
    /// </summary>
    internal class IntDictionary<TValue> : IDictionary<int, TValue> {
        private List<fiOption<TValue>> _positives = new List<fiOption<TValue>>();
        private Dictionary<int, TValue> _negatives = new Dictionary<int, TValue>();

        public void Add(int key, TValue value) {
            if (key < 0) {
                _negatives.Add(key, value);
            }
            else {
                while (key >= _positives.Count) {
                    _positives.Add(fiOption<TValue>.Empty);
                }
                if (_positives[key].HasValue) throw new Exception("Already have a key for " + key);
                _positives[key] = fiOption.Just(value);
            }
        }

        public bool ContainsKey(int key) {
            if (key < 0) return _negatives.ContainsKey(key);
            else return key < _positives.Count && _positives[key].HasValue;
        }

        public ICollection<int> Keys {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(int key) {
            if (key < 0) return _negatives.Remove(key);
            else {
                if (key >= _positives.Count) return false;
                if (_positives[key].IsEmpty) return false;

                _positives[key] = fiOption<TValue>.Empty;
                return true;
            }
        }

        public bool TryGetValue(int key, out TValue value) {
            if (key < 0) return _negatives.TryGetValue(key, out value);

            value = default(TValue);
            if (key >= _positives.Count) return false;
            if (_positives[key].IsEmpty) return false;

            value = _positives[key].Value;
            return true;
        }

        public ICollection<TValue> Values {
            get { throw new NotImplementedException(); }
        }

        public TValue this[int key] {
            get {
                if (key < 0) return _negatives[key];
                else {
                    if (key >= _positives.Count) throw new KeyNotFoundException("" + key);
                    if (_positives[key].IsEmpty) throw new KeyNotFoundException("" + key);

                    return _positives[key].Value;
                }
            }
            set {
                if (key < 0) {
                    _negatives[key] = value;
                }
                else {
                    while (key >= _positives.Count) {
                        _positives.Add(fiOption<TValue>.Empty);
                    }
                    _positives[key] = fiOption.Just(value);
                }
            }
        }

        public void Add(KeyValuePair<int, TValue> item) {
            Add(item.Key, item.Value);
        }

        public void Clear() {
            _negatives.Clear();
            _positives.Clear();
        }

        public bool Contains(KeyValuePair<int, TValue> item) {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<int, TValue>[] array, int arrayIndex) {
            foreach (var item in this) {
                if (arrayIndex >= array.Length) break;

                array[arrayIndex++] = item;
            }
        }

        public int Count {
            get {
                int count = _negatives.Count;

                for (int i = 0; i < _positives.Count; ++i) {
                    if (_positives[i].HasValue) ++count;
                }

                return count;
            }
        }

        public bool IsReadOnly {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(KeyValuePair<int, TValue> item) {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator() {
            foreach (KeyValuePair<int, TValue> value in _negatives) {
                yield return value;
            }

            for (int i = 0; i < _positives.Count; ++i) {
                if (_positives[i].HasValue) {
                    yield return new KeyValuePair<int, TValue>(i, _positives[i].Value);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}