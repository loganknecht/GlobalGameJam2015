using FullInspector.Rotorz.ReorderableList;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullInspector.Modules.Collections {
    /// <summary>
    /// Reorderable list adapter for generic list.
    /// </summary>
    /// <remarks>
    /// <para>This adapter can be subclassed to add special logic to item height calculation. You
    /// may want to implement a custom adapter class where specialized functionality is
    /// needed.</para>
    /// </remarks>
    public class GenericListAdaptorWithDynamicHeight<T> : IReorderableListAdaptor {
        public delegate float ItemHeight(T item, fiGraphMetadataChild metadata);
        public delegate T ItemDrawer(Rect position, T item, fiGraphMetadataChild metadata);

        private ItemHeight _itemHeight;
        private ItemDrawer _itemDrawer;
        private fiGraphMetadata _metadata;
        private IList<T> _list;

        private static T DefaultItemGenerator() {
            return default(T);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GenericListAdaptor{T}"/>.
        /// </summary>
        /// <param name="list">The list which can be reordered.</param>
        /// <param name="itemDrawer">Callback to draw list item.</param>
        /// <param name="itemHeight">Height of list item in pixels.</param>
        /// <param name="itemGenerator">The function used to generate new items. If null, then the
        /// default item generator is used.</param>
        public GenericListAdaptorWithDynamicHeight(IList<T> list, ItemDrawer itemDrawer,
            ItemHeight itemHeight, fiGraphMetadata metadata) {
            _metadata = metadata;
            _list = list;
            _itemDrawer = itemDrawer;
            _itemHeight = itemHeight;
        }

        public int Count {
            get { return _list.Count; }
        }
        public virtual bool CanDrag(int index) {
            return true;
        }
        public virtual bool CanRemove(int index) {
            return true;
        }
        public void Add() {
            T item = DefaultItemGenerator();
            _list.Add(item);
        }
        public void Insert(int index) {
            Add();

            // shift elements forwards
            for (int i = _list.Count - 1; i > index; --i) {
                _list[i] = _list[i - 1];
                _metadata.SetChild(i, _metadata.Enter(i - 1).Metadata);
            }

            // update the reference at index
            _list[index] = default(T);
        }
        public void Duplicate(int index) {
            T current = _list[index];
            Insert(index);
            _list[index] = current;
        }
        public void Remove(int index) {
            // shift elements back
            for (int i = index; i < _list.Count - 1; ++i) {
                _list[i] = _list[i + 1];
                _metadata.SetChild(i, _metadata.Enter(i + 1).Metadata);
            }

            // pop the last element
            _list.RemoveAt(_list.Count - 1);
        }
        public void Move(int sourceIndex, int destIndex) {
            if (destIndex > sourceIndex)
                --destIndex;

            T item = _list[sourceIndex];
            fiGraphMetadata itemMetadata = _metadata.Enter(sourceIndex).Metadata;

            Remove(sourceIndex);
            Insert(destIndex);

            _list[destIndex] = item;
            _metadata.SetChild(destIndex, itemMetadata);
        }
        
        public void Clear() {
            _list.Clear();
        }

        public virtual void DrawItem(Rect position, int index) {
            // Rotorz seems to sometimes give an index of -1, not sure why.
            if (index < 0) {
                return;
            }

            _list[index] = _itemDrawer(position, _list[index], _metadata.Enter(index));
        }
        public virtual float GetItemHeight(int index) {
            return _itemHeight(_list[index], _metadata.Enter(index));
        }
    }
}