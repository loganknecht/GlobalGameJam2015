using FullInspector.Rotorz.ReorderableList;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FullInspector.Modules.Collections {
    /// <summary>
    /// Reorderable list adapter for generic collections.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class DictionaryAdapter<TKey, TValue> : IReorderableListAdaptor {
        public delegate float ItemHeight<T>(T item, fiGraphMetadataChild metadata);
        public delegate T ItemDrawer<T>(Rect position, T item, fiGraphMetadataChild metadata);

        private IDictionary<TKey, TValue> _dictionary;
        private KeyValuePair<TKey, TValue>[] _dictionaryCache;

        private ItemHeight<TKey> _keyHeight;
        private ItemHeight<TValue> _valueHeight;
        private ItemDrawer<TKey> _keyDrawer;
        private ItemDrawer<TValue> _valueDrawer;
        private fiGraphMetadata _metadata;

        private static KeyValuePair<TKey, TValue> DefaultItemGenerator() {
            return new KeyValuePair<TKey, TValue>();
        }

        /// <summary>
        /// For performance reasons, the DictionaryAdapter stores an array version of the
        /// dictionary. If the adapted dictionary has been structurally modified, for example, an
        /// item has been added, then the local cache is invalid. Calling this method updates the
        /// cache, which will restore proper adapter semantics.
        /// </summary>
        public void InvalidateCache(bool migrateMetadata) {
            KeyValuePair<TKey, TValue>[] oldCache = _dictionaryCache;
            KeyValuePair<TKey, TValue>[] newCache = _dictionary.ToArray();
            _dictionaryCache = newCache;

            // migrate metadata
            if (oldCache != null && migrateMetadata) {
                List<fiGraphMetadata.MetadataMigration> migrations;
                fiGraphMetadata.MigrateMetadata(_metadata, oldCache, newCache, out migrations);

                // get references to the graphs we are going to move before we delete them from _metadata
                List<fiGraphMetadata> copiedGraphKeys = new List<fiGraphMetadata>(migrations.Count);
                List<fiGraphMetadata> copiedGraphValues = new List<fiGraphMetadata>(migrations.Count);
                for (int i = 0; i < migrations.Count; ++i) {
                    fiGraphMetadata.MetadataMigration migration = migrations[i];
                    copiedGraphKeys.Add(_metadata.Enter(KeyMetadataIndex(migration.OldIndex)).Metadata);
                    copiedGraphValues.Add(_metadata.Enter(ValueMetadataIndex(migration.OldIndex)).Metadata);
                }

                // run the actual migrations
                for (int i = 0; i < migrations.Count; ++i) {
                    fiGraphMetadata.MetadataMigration migration = migrations[i];
                    _metadata.SetChild(KeyMetadataIndex(migration.NewIndex), copiedGraphKeys[i]);
                    _metadata.SetChild(ValueMetadataIndex(migration.NewIndex), copiedGraphValues[i]);
                }

            }
        }

        /// <summary>
        /// Initializes a new instance of DictionaryAdapter.
        /// </summary>
        /// <param name="dictionary">The dictionary that will be adapter for Rotorz</param>
        /// <param name="keyDrawer">The function that will edit keys in the dictionary</param>
        /// <param name="valueDrawer">The function that will edit values in the dictionary</param>
        /// <param name="itemHeight">The function that computes the height of dictionary
        /// items</param>
        /// <param name="itemGenerator">The function used to generate new items. If null, then the
        /// default item generator is used</param>
        public DictionaryAdapter(IDictionary<TKey, TValue> dictionary,
            ItemDrawer<TKey> keyDrawer,
            ItemDrawer<TValue> valueDrawer,
            ItemHeight<TKey> keyHeight,
            ItemHeight<TValue> valueHeight,
            fiGraphMetadata metadata) {

            _dictionary = dictionary;
            _keyDrawer = keyDrawer;
            _valueDrawer = valueDrawer;
            _keyHeight = keyHeight;
            _valueHeight = valueHeight;
            _metadata = metadata;

            InvalidateCache(migrateMetadata: false);
        }

        public int Count {
            get {
                return _dictionary.Count;
            }
        }

        public virtual bool CanDrag(int index) {
            return false;
        }

        public virtual bool CanRemove(int index) {
            return true;
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            if (_dictionary.ContainsKey(item.Key)) {
                if (fiSettings.EmitWarnings) {
                    Debug.LogWarning("Dictionary already contains an item for key=" + item.Key);
                }
                return;
            }

            _dictionary.Add(item);

            InvalidateCache(migrateMetadata: true);
        }

        public void Add() {
            Add(DefaultItemGenerator());
        }

        public void Insert(int index) {
            // oh well, can't insert into a collection
            Debug.LogWarning("Cannot insert into a specific index into a collection; adding to the end");
            Add();
            InvalidateCache(migrateMetadata: true);
        }

        public void Duplicate(int index) {
            Debug.LogError("Cannot duplicate a Dictionary element");
        }

        public void Remove(int index) {
            var item = _dictionaryCache[index];
            _dictionary.Remove(item);
            InvalidateCache(migrateMetadata: true);
        }

        public void Move(int sourceIndex, int destIndex) {
            Debug.LogError("Cannot move elements in a dictionary");
        }

        public void Clear() {
            _dictionary.Clear();
        }


        private static int KeyMetadataIndex(int index) {
            return index * 2;
        }
        private static int ValueMetadataIndex(int index) {
            return (index * 2) + 1;
        }

        public virtual void DrawItem(Rect position, int index) {
            // Rotorz seems to sometimes give an index of -1, not sure why.
            if (index < 0) {
                return;
            }

            KeyValuePair<TKey, TValue> item = _dictionaryCache[index];

            TKey key = _keyDrawer(position, item.Key, _metadata.Enter(KeyMetadataIndex(index)));
            TValue value = _valueDrawer(position, item.Value, _metadata.Enter(ValueMetadataIndex(index)));

            // has the key been edited?
            if (EqualityComparer<TKey>.Default.Equals(key, item.Key) == false) {
                // we already contain an element for the new key; lets just update the old key
                // in-case the value also changed
                if (_dictionary.ContainsKey(key)) {
                    _dictionary[item.Key] = value;
                }

                // we don't have a slot for the new key, so lets swap the values over from the old
                // key to the new key
                else {
                    _dictionary.Remove(item.Key);
                    _dictionary[key] = value;
                }

                InvalidateCache(migrateMetadata: false);
            }

            // The key was not changed, so we can just update the value in the dictionary. However,
            // we only need to do this if the value has actually been modified and the modification
            // is not reflected inside of the dictionary.
            else if (EqualityComparer<TValue>.Default.Equals(_dictionary[key], value) == false) {
                _dictionary[key] = value;
                InvalidateCache(migrateMetadata: false);
            }
        }

        public virtual float GetItemHeight(int index) {
            return Math.Max(
                _keyHeight(_dictionaryCache[index].Key, _metadata.Enter(KeyMetadataIndex(index))),
                _valueHeight(_dictionaryCache[index].Value, _metadata.Enter(ValueMetadataIndex(index)))
            );
        }
    }
}