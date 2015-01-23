using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// Manages the options that are displayed to the user in the instance selection drop-down.
    /// </summary>
    internal class AbstractTypeInstanceOptionManager {
        private Type[] _options;
        private string[] _comments;
        private List<GUIContent> _displayedOptions;

        /// <summary>
        /// Setup the instance option manager for the given type.
        /// </summary>
        public AbstractTypeInstanceOptionManager(Type baseType) {
            _options = fiReflectionUtilitity.GetCreatableTypesDeriving(baseType).ToArray();

            _comments = (from option in _options
                         let comment = option.GetAttribute<CommentAttribute>()
                         select comment != null ? comment.Comment : "").ToArray();

            _displayedOptions = new List<GUIContent>();
            _displayedOptions.Add(new GUIContent("null (" + baseType.CSharpName() + ")"));
            _displayedOptions.AddRange(from option in _options
                                       let optionName = GetOptionName(option)
                                       select new GUIContent(optionName));
        }

        private static string GetOptionName(Type type) {
            string baseName = type.CSharpName();

            if (type.IsValueType == false &&
                type.GetConstructor(fsPortableReflection.EmptyTypes) == null) {

                baseName += " (skips ctor)";
            }

            return baseName;
        }

        /// <summary>
        /// Returns an array of options that should be displayed.
        /// </summary>
        public GUIContent[] GetDisplayOptions() {
            return _displayedOptions.ToArray();
        }

        /// <summary>
        /// Remove any options from the set of display options that are not permanently visible.
        /// </summary>
        public void RemoveExtraneousOptions() {
            while (_displayedOptions.Count > (_options.Length + 1)) {
                _displayedOptions.RemoveAt(_displayedOptions.Count - 1);
            }
        }

        /// <summary>
        /// Returns the index of the option that should be displayed (from GetDisplayOptions())
        /// based on the current object instance.
        /// </summary>
        public int GetDisplayOptionIndex(object instance) {
            if (instance == null) {
                return 0;
            }

            Type instanceType = instance.GetType();
            for (int i = 0; i < _options.Length; ++i) {
                Type option = _options[i];
                if (instanceType == option) {
                    return i + 1;
                }
            }

            // we need a new display option
            _displayedOptions.Add(new GUIContent(instance.GetType() + " (cannot reconstruct)"));
            return _displayedOptions.Count - 1;
        }

        /// <summary>
        /// Returns a comment that should be displayed above the given option.
        /// </summary>
        public string GetComment(object instance) {
            if (instance == null) {
                return "";
            }

            Type instanceType = instance.GetType();
            for (int i = 0; i < _options.Length; ++i) {
                Type option = _options[i];
                if (instanceType == option) {
                    return _comments[i];
                }
            }

            return "";
        }

        /// <summary>
        /// Changes the instance of the given object, if necessary.
        /// </summary>
        public object UpdateObjectInstance(object current, int currentIndex, int updatedIndex) {
            // the index has not changed - there will be no change in object instance
            if (currentIndex == updatedIndex) {
                return current;
            }

            // index 0 is always null
            if (updatedIndex == 0) {
                return null;
            }

            // create an instance of the object
            Type type = _options[updatedIndex - 1];
            return InspectedType.Get(type).CreateInstance();
        }
    }
}