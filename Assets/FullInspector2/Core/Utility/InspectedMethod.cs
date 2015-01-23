using FullInspector.Internal;
using System;
using System.Reflection;
using UnityEngine;

namespace FullInspector {
    /// <summary>
    /// A method that is being inspected, typically for the purpose of a button.
    /// </summary>
    public class InspectedMethod {
        public InspectedMethod(MethodInfo method) {
            Method = method;
            HasArguments = method.GetParameters().Length > 0;

            var attr = method.GetAttribute<InspectorNameAttribute>();
            if (attr != null) {
                DisplayName = attr.DisplayName;
            }

            if (string.IsNullOrEmpty(DisplayName)) {
                DisplayName = DisplayNameMapper.Map(method.Name);
            }
        }

        /// <summary>
        /// The wrapped method.
        /// </summary>
        public MethodInfo Method {
            get;
            private set;
        }

        /// <summary>
        /// The name that should be used when displaying the method. This value defaults to
        /// Method.Name but can be overridden with a InspectorButtonAttribute.
        /// </summary>
        public string DisplayName {
            get;
            private set;
        }

        /// <summary>
        /// True if the method has arguments (besides an implicit this).
        /// </summary>
        public bool HasArguments {
            get;
            private set;
        }

        /// <summary>
        /// Invoke the method. This function will never fail.
        /// </summary>
        public void Invoke(object instance) {
            try {
                Method.Invoke(instance, null);
            }
            catch (Exception e) {
                Debug.LogWarning("When invoking method " + Method + ", caught " +
                    "exception:\n" + e);
            }
        }
    }
}